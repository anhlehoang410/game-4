Shader "Vertex Colors/Alpha" {
    SubShader {
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        AlphaTest Greater 0.0
        Pass {
            BindChannels {
                Bind "Color", color
                Bind "Vertex", vertex
            }
        }
    }
}