Shader "Mobify/Mobile Emissive" {
    Properties {
        _MainTex ("Main Texture", 2D) = "white" {}
        _EmissionMap ("Emission Map", 2D) = "black" {}
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EnableEmission ("Enable Emission", Range(0, 1)) = 0
    }
 
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _EmissionMap;
            fixed4 _EmissionColor;
            int _EnableEmission;

            v2f vert (appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv);
                
                if (_EnableEmission > 0) {
                    fixed4 emission = tex2D(_EmissionMap, i.uv) * _EmissionColor;
                    col.rgb += emission.rgb;
                }

                return col;
            }
            ENDCG
        }
    }
}
