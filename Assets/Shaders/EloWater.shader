 Shader "Custom/EloWater" {
     Properties {
         _Color ("Color", Color) = (1,1,1,1)
         _MainTex ("Albedo (RGB)", 2D) = "white" {}
         _Glossiness ("Smoothness", Range(0,1)) = 0.5
         _Metallic ("Metallic", Range(0,1)) = 0.0
         _RimColor ("Rim Color", Color) = (0.26,0.19,0.16,0.0)
      	 _RimPower ("Rim Power", Range(0.5,8.0)) = 3.0
      	 
      	 _InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
     }
     SubShader {
         Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
         Blend One OneMinusSrcAlpha
         LOD 200
         
         CGPROGRAM
		 
         #pragma surface surf Standard vertex:vert fullforwardshadows 
         #pragma target 3.0

		 #include "UnityCG.cginc"
		 
         struct Input {
//			float2 uv_MainTex;
			float3 vertexColor; // Vertex color stored here by vert() method
			float3 viewDir;
			
			fixed4 color : COLOR;
			
			UNITY_FOG_COORDS(1)
//			#ifdef SOFTPARTICLES_ON
//			float4 projPos : TEXCOORD2;
//			#endif
			
//			float4 vertex : SV_POSITION;
			float2 texcoord : TEXCOORD0;
         };
         
         struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			
			
         };
         
		sampler2D _MainTex;
		half _Glossiness;
		half _Metallic;
		fixed4 _Color;
		float4 _RimColor;
		float _RimPower;
		float4 _MainTex_ST;
		
		sampler2D_float _CameraDepthTexture;
		float _InvFade;
		
         Input vert (inout appdata_full v, out Input o)
         {
			UNITY_INITIALIZE_OUTPUT(Input,o);
			o.vertexColor = v.color; // Save the Vertex Color in the Input for the surf() method
			
			Input output;
//			output.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
//			#ifdef SOFTPARTICLES_ON
//			output.projPos = ComputeScreenPos (output.vertex);
//			COMPUTE_EYEDEPTH(output.projPos.z);
//			#endif
			output.color = v.color;
			output.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
			UNITY_TRANSFER_FOG(output,output.vertex);
			return output;
				
         }
 
 
         void surf (Input IN, inout SurfaceOutputStandard o) 
         {
             // Albedo comes from a texture tinted by color
             fixed4 c = tex2D (_MainTex, IN.texcoord) * _Color;
             o.Albedo = c.rgb * IN.vertexColor; // Combine normal color with the vertex color
             // Metallic and smoothness come from slider variables
             o.Metallic = _Metallic;
             o.Smoothness = _Glossiness;
             half rim = 1.0 - saturate(dot (normalize(IN.viewDir), o.Normal));
          	 o.Emission = _RimColor.rgb * pow (rim, _RimPower);
          	 o.Alpha = (1 - c.a) * (_Color.a * IN.color.a * 2.0f);;
          	 
          	 
         }
         ENDCG
     } 
     FallBack "Diffuse"
 }