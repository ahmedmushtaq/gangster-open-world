Shader "Universal Render Pipeline/Car/Plastic"
{
    Properties
    {
        // plastic color
        [MainColor] _Color("Plastic Color", Color) = (1, 1, 1, 1)

        // texture
        [MainTexture] _MainTex("Diffuse", 2D) = "white" {}
        [Normal] _DiffuseBumpMap("Diffuse Bumpmap", 2D) = "bump" {}
        _DiffuseUVScale("Diffuse UV Scale", Range(1, 100)) = 1

        // shininess settings
        _ShininessIntensity("Plastic Shininess Intensity", Range(0.0, 4.0)) = 0
        _ShininessScale("Plastic Shininess Scale", Range(0.0, 20.0)) = 0
        
        // URP specific
        _Smoothness("Smoothness", Range(0, 1)) = 0.5
        _Specular("Specular", Range(0, 1)) = 0.5
    }
    
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
            "Queue" = "Geometry"
        }
        
        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            // URP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            
            // Custom feature
            #pragma shader_feature_local _BUMPED_DIFFUSE_ON
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalOS     : NORMAL;
                float4 tangentOS    : TANGENT;
            };
            
            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 normalWS     : TEXCOORD1;
                float3 tangentWS    : TEXCOORD2;
                float3 bitangentWS  : TEXCOORD3;
                float3 positionWS   : TEXCOORD4;
                float3 viewDirWS    : TEXCOORD5;
            };
            
            TEXTURE2D(_MainTex);        SAMPLER(sampler_MainTex);
            TEXTURE2D(_DiffuseBumpMap); SAMPLER(sampler_DiffuseBumpMap);
            
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _Color;
                float _DiffuseUVScale;
                float _ShininessIntensity;
                float _ShininessScale;
                float _Smoothness;
                float _Specular;
            CBUFFER_END
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                VertexPositionInputs vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
                VertexNormalInputs normalInput = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                
                OUT.positionHCS = vertexInput.positionCS;
                OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                OUT.normalWS = normalInput.normalWS;
                OUT.tangentWS = normalInput.tangentWS;
                OUT.bitangentWS = normalInput.bitangentWS;
                OUT.positionWS = vertexInput.positionWS;
                OUT.viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
                
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_Target
            {
                // Sample texture
                half4 bodyTexture = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv * _DiffuseUVScale);
                
                // Handle normal map
                half3 normalTS = half3(0, 0, 1);
                
                #if _BUMPED_DIFFUSE_ON
                half4 bumpSample = SAMPLE_TEXTURE2D(_DiffuseBumpMap, sampler_DiffuseBumpMap, IN.uv * _DiffuseUVScale);
                normalTS = normalize(half3(0, 0, 1) + UnpackNormal(bumpSample));
                #endif
                
                // Transform normal to world space
                half3x3 tangentToWorld = half3x3(IN.tangentWS, IN.bitangentWS, IN.normalWS);
                half3 normalWS = normalize(mul(normalTS, tangentToWorld));
                
                // Calculate view direction
                half3 viewDirWS = SafeNormalize(IN.viewDirWS);
                
                // Body specular mask
                half bodySpecularMask = bodyTexture.a;
                
                // Body diffuse (from original shader)
                half4 bodyDiffuse = (_Color * (1 - bodySpecularMask) * bodyTexture) + (_Color * bodyTexture * bodySpecularMask);
                
                // Shininess calculation (from original shader)
                half shininess = _ShininessIntensity * pow(abs(dot(normalize(viewDirWS), normalWS)), _ShininessScale);
                
                // Combine everything (from original shader)
                half3 albedo = bodyDiffuse.rgb * bodySpecularMask + shininess + _Color.rgb * (1 - bodySpecularMask);
                
                // Create SurfaceData for URP lighting
                SurfaceData surfaceData;
                surfaceData.albedo = albedo;
                surfaceData.alpha = 1.0;
                surfaceData.metallic = 0.0; // Plastic is non-metallic
                surfaceData.smoothness = _Smoothness * bodyTexture.a;
                surfaceData.occlusion = 1.0;
                surfaceData.emission = shininess; // Add shininess as emission
                surfaceData.normalTS = normalTS;
                surfaceData.specular = _Specular;
                surfaceData.clearCoatMask = 0.0;
                surfaceData.clearCoatSmoothness = 0.0;
                
                // Create InputData for URP lighting
                InputData inputData;
                inputData.positionWS = IN.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = viewDirWS;
                
                #if defined(_MAIN_LIGHT_SHADOWS) || defined(_MAIN_LIGHT_SHADOWS_CASCADE)
                inputData.shadowCoord = TransformWorldToShadowCoord(IN.positionWS);
                #else
                inputData.shadowCoord = float4(0, 0, 0, 0);
                #endif
                
                inputData.fogCoord = 0;
                inputData.vertexLighting = half3(0, 0, 0);
                inputData.bakedGI = SampleSH(normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionHCS);
                inputData.shadowMask = half4(1, 1, 1, 1);
                
                // Calculate final color with URP lighting
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                return color;
            }
            ENDHLSL
        }
        
        // Shadow caster pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionCS : SV_POSITION;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
        
        // Depth only pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ColorMask R
            ZWrite On
            ZTest LEqual
            Cull Back
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct Varyings
            {
                float2 uv : TEXCOORD0;
                float4 positionHCS : SV_POSITION;
            };
            
            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }
            
            half4 frag(Varyings IN) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
    }
    
    FallBack "Universal Render Pipeline/SimpleLit"
    
    CustomEditor "PlasticShaderURPEditor"
}