Shader "URP/Mobile/CarGlass_Simple"
{
    Properties
    {
        _Color ("Glass Color", Color) = (1,1,1,1)
        _Transparency ("Glass Transparency", Range(0.05, 0.9)) = 0.5

        _Cube ("Reflection Cubemap", Cube) = "" {}
        _ReflectionIntensity ("Reflection Intensity", Range(0,2)) = 0.5

        _RenderedTexture ("Rendered Texture", 2D) = "white" {}
        _UseRenderedTex ("Use Rendered Texture (0/1)", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }

        Pass
        {
            Name "ForwardTransparent"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
            };

            float4 _Color;
            float _Transparency;
            float _ReflectionIntensity;
            float _UseRenderedTex;

            TEXTURECUBE(_Cube);
            SAMPLER(sampler_Cube);

            TEXTURE2D(_RenderedTexture);
            SAMPLER(sampler_RenderedTexture);

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.viewDirWS = GetWorldSpaceViewDir(worldPos);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
{
    float3 N = normalize(IN.normalWS);
    float3 V = normalize(IN.viewDirWS);

    // Reflection vector
    float3 R = reflect(-V, N);

    half3 cubeRefl = SAMPLE_TEXTURECUBE(_Cube, sampler_Cube, R).rgb;
    half3 renderedRefl = SAMPLE_TEXTURE2D(
        _RenderedTexture,
        sampler_RenderedTexture,
        R.xy * 0.5 + 0.5
    ).rgb;

    half3 reflection = lerp(cubeRefl, renderedRefl, saturate(_UseRenderedTex));
    reflection *= _ReflectionIntensity;

    // Final color (DECLARE ONCE)
    half3 finalColor = _Color.rgb + reflection;

    // Alpha
    half reflectionLum = dot(reflection, half3(0.2126, 0.7152, 0.0722));
    half alpha = _Color.a * _Transparency + reflectionLum;
    alpha = saturate(alpha);

    return half4(finalColor, alpha);
}

            ENDHLSL
        }
    }
}
