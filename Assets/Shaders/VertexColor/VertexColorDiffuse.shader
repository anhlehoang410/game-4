Shader "Vertex Colors/Solid" {
    SubShader {
        ZWrite Off
        ZTest Always
        Pass {
            BindChannels {
                Bind "Color", color
                Bind "Vertex", vertex
            }
        }
    }
}
