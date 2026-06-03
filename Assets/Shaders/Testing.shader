Shader "URP/Mobile/CarBody_Pearl_Decal"
{
    Properties
    {
        _BaseColor ("Vehicle Color", Color) = (1,1,1,1)
        _BaseMap ("Diffuse", 2D) = "white" {}
        _Decal ("Decal", 2D) = "white" {}
        _DecalColor ("Decal Color", Color) = (1,1,1,1)
        _DecalTransparency ("Decal Transparency", Range(0,1)) = 1

        _PearlMap ("Pearl Texture", 2D) = "white" {}
        _PearlColor ("Pearl Color", Color) = (1,1,1,1)
        _PearlIntensity ("Pearl Intensity", Range(0,4)) = 0
        _PearlScale ("Pearl Scale", Range(1,50)) = 10

        _NormalMap ("Normal Map", 2D) = "bump" {}

        _Cube ("Reflection Cubemap", Cube) = "" {}
        _ReflectionIntensity ("Reflection Intensity", Range(0,2)) = 0.5
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
                float2 uv          : TEXCOORD2;
            };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_Decal); SAMPLER(sampler_Decal);
            TEXTURE2D(_PearlMap); SAMPLER(sampler_PearlMap);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURECUBE(_Cube); SAMPLER(sampler_Cube);

            float4 _BaseColor;
            float4 _DecalColor;
            float4 _PearlColor;
            float _DecalTransparency;
            float _PearlIntensity;
            float _PearlScale;
            float _ReflectionIntensity;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = GetWorldSpaceViewDir(worldPos);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                half4 baseTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);
                half4 decalTex = SAMPLE_TEXTURE2D(_Decal, sampler_Decal, IN.uv);
                half4 pearlTex = SAMPLE_TEXTURE2D(_PearlMap, sampler_PearlMap, IN.uv);

                half3 baseColor = baseTex.rgb * _BaseColor.rgb;
                half3 decal = decalTex.rgb * _DecalColor.rgb * decalTex.a * _DecalTransparency;

                // Pearl effect (view-based)
                half pearl = pow(saturate(dot(N, V)), _PearlScale) * _PearlIntensity;
                half3 pearlColor = pearlTex.rgb * pearl * _PearlColor.rgb;

                // Reflection
                float3 R = reflect(-V, N);
                half3 reflection = SAMPLE_TEXTURECUBE(_Cube, sampler_Cube, R).rgb * _ReflectionIntensity;

                half3 finalColor =
                    baseColor +
                    decal +
                    pearlColor +
                    reflection;

                return half4(finalColor, 1);
            }
            ENDHLSL
        }
    }
}
