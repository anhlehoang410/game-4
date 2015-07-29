Shader "Vertex Colors/Alpha Texture" {
    Properties {
        _Alpha ("Opacity (A)", 2D) = "white" {}
    }
    SubShader {
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        AlphaTest Greater 0.0
        Pass {
            BindChannels {
                Bind "Color", color
                Bind "Vertex", vertex
                Bind "TexCoord", texcoord
            }
            SetTexture[_Alpha] {
                combine primary, primary*texture
            }
        }
    }
}