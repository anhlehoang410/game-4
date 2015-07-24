Shader "Texture Only" {
    Properties {
        _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
    }
 
    SubShader {
	    Tags { "Queue" = "Transparent" }
		Blend SrcAlpha OneMinusSrcAlpha
        Pass {
            SetTexture [_MainTex]
        }
    }
}