 Shader "Custom/StandardVertex" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
         _Glossiness ("Smoothness", Range(0,1)) = 0.5
         _Metallic ("Metallic", Range(0,1)) = 0.0
         _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      	 _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
     }
     SubShader {
         Tags { "RenderType"="Opaque" }
//         Blend One OneMinusSrcAlpha
         LOD 200
         
         CGPROGRAM
         #pragma surface surf Standard vertex:vert fullforwardshadows
         #pragma target 3.0
         struct Input {
             float2 uv_MainTex;
             float3 vertexColor; // Vertex color stored here by vert() method
             float3 viewDir;
         };
         
         struct v2f {
           float4 pos : SV_POSITION;
           fixed4 color : COLOR;
         };
 
         void vert (inout appdata_full v, out Input o)
         {
             UNITY_INITIALIZE_OUTPUT(Input,o);
             o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
         }
 
         sampler2D _MainTex;
 
         half _Glossiness;
         half _Metallic;
         fixed4 _Color;
         float4 _RimColor;
      	 float _RimPower;
 
         void surf (Input IN, inout SurfaceOutputStandard o) 
         {
             // Albedo comes from a texture tinted by color
             fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
             o.Albedo = c.rgb * IN.vertexColor; // Combine normal color with the vertex color
             // Metallic and smoothness come from slider variables
             o.Metallic = _Metallic;
             o.Smoothness = _Glossiness;
             half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          	 o.Emission = _RimColor.rgb * pow (rim, _RimPower);
          	 o.Alpha = c.a;
         }
         ENDCG
     } 
     FallBack "Diffuse"
 }