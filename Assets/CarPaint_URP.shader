Shader "Custom/CarPaint_URP"
{
    Properties
    {
        _MainTex ("MainTex", 2D) = "white" {}
        _MainColor1 ("Main Color 1 (Face)", Color) = (1,1,1,1)
        _MainColor2 ("Main Color 2 (Edge)", Color) = (0,0,0,1)
        _SkinColor ("Skin Color (Highlight Tint)", Color) = (0,0,0,1)
        
        _Fresnel_Power ("Fresnel Power", Float) = 2.5
        _Fresnel_Strength ("Fresnel Strength", Float) = 2.0
        
        _Metalic ("Metallic", Range(0,1)) = 0.5
        _Smoothness_Frensel_1 ("Smoothness (Grazing)", Float) = 1.0
        _Smoothness_Frensel_2 ("Smoothness (Center)", Float) = 0.6
        _Smoothness_Strength ("Smoothness Strength", Float) = 0.9
        
        _AO ("Ambient Occlusion", Range(0,1)) = 1.0
        
        [HideInInspector] _BUILTIN_QueueOffset ("Float", Float) = 0
        [HideInInspector] _BUILTIN_QueueControl ("Float", Float) = -1
    }
    
    SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
        }
        
        LOD 200
        
        // ------------------------------------------------------------------
        // Forward Lit Pass
        Pass
        {
            Name "UniversalForward"
            Tags { "LightMode" = "UniversalForward" }
            
            HLSLPROGRAM
            #pragma target 3.5
            
            #pragma vertex vert
            #pragma fragment frag
            
            // -------------------------------------
            // URP keywords
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_fog
            
            // -------------------------------------
            // Includes
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            // -------------------------------------
            // Properties
            CBUFFER_START(UnityPerMaterial)
                float4 _MainTex_ST;
                float4 _MainColor1;
                float4 _MainColor2;
                float4 _SkinColor;
                float _Fresnel_Power;
                float _Fresnel_Strength;
                float _Metalic;
                float _Smoothness_Frensel_1;
                float _Smoothness_Frensel_2;
                float _Smoothness_Strength;
                float _AO;
            CBUFFER_END
            
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            
            // -------------------------------------
            // Vertex Input
            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv           : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            // Vertex to Fragment
            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
                float3 positionWS   : TEXCOORD1;
                float3 normalWS     : TEXCOORD2;
                float fogFactor     : TEXCOORD3;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            // -------------------------------------
            // Vertex Shader
            Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionWS = TransformObjectToWorld(input.positionOS.xyz);
                output.positionCS = TransformWorldToHClip(output.positionWS);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.uv = TRANSFORM_TEX(input.uv, _MainTex);
                
                // Fog
                output.fogFactor = ComputeFogFactor(output.positionCS.z);
                
                return output;
            }
            
            // -------------------------------------
            // Fragment Shader
            half4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                // Sample main texture
                half4 mainTexColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
                
                // Normalize world normal and view direction
                float3 normalWS = normalize(input.normalWS);
                float3 viewDirWS = normalize(_WorldSpaceCameraPos - input.positionWS);
                
                // Fresnel calculation: 1 - (N·V) raised to power, then multiplied by strength
                float fresnelDot = saturate(dot(normalWS, viewDirWS));
                float fresnelIntensity = pow(1.0 - fresnelDot, _Fresnel_Power);
                fresnelIntensity = saturate(fresnelIntensity * _Fresnel_Strength);
                
                // Combine base colors via Fresnel: MainColor1 at glancing angles, MainColor2 at center
                float3 albedoBase = lerp(_MainColor2.rgb, _MainColor1.rgb, fresnelIntensity);
                // Add SkinColor as a tint primarily on the Fresnel highlight
                albedoBase += _SkinColor.rgb * fresnelIntensity;
                // Multiply by main texture
                float3 albedo = albedoBase * mainTexColor.rgb;
                
                // Smoothness blends between two values based on Fresnel (grazing = _Smoothness_Frensel_1)
                float smoothness = lerp(_Smoothness_Frensel_2, _Smoothness_Frensel_1, fresnelIntensity);
                smoothness = saturate(smoothness * _Smoothness_Strength);
                
                // Metallic and occlusion
                float metallic = _Metalic;
                float occlusion = _AO;
                
                // Prepare SurfaceData for PBR lighting
                SurfaceData surfaceData;
                surfaceData.albedo = albedo;
                surfaceData.metallic = metallic;
                surfaceData.specular = half3(0.0, 0.0, 0.0); // not used in metallic workflow
                surfaceData.smoothness = smoothness;
                surfaceData.normalTS = half3(0.0, 0.0, 1.0); // no normal map
                surfaceData.occlusion = occlusion;
                surfaceData.emission = 0.0;
                surfaceData.alpha = 1.0;
                surfaceData.clearCoatMask = 0.0;
                surfaceData.clearCoatSmoothness = 0.0;
                
                // Input data for lighting functions
                InputData inputData;
                inputData.positionWS = input.positionWS;
                inputData.normalWS = normalWS;
                inputData.viewDirectionWS = viewDirWS;
                inputData.shadowCoord = TransformWorldToShadowCoord(input.positionWS);
                inputData.fogCoord = input.fogFactor;
                inputData.vertexLighting = half3(0.0, 0.0, 0.0);
                inputData.bakedGI = SampleSH(normalWS);
                inputData.normalizedScreenSpaceUV = float2(0.0, 0.0);
                inputData.shadowMask = 1.0;
                
                // Apply PBR lighting
                half4 color = UniversalFragmentPBR(inputData, surfaceData);
                
                // Apply fog
                color.rgb = MixFog(color.rgb, input.fogFactor);
                
                return color;
            }
            ENDHLSL
        }
        
        // ------------------------------------------------------------------
        // ShadowCaster Pass
        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }
            
            ZWrite On
            ZTest LEqual
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
            };
            
            Varyings ShadowPassVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                float3 positionWS = TransformObjectToWorld(input.positionOS.xyz);
                float3 normalWS = TransformObjectToWorldNormal(input.normalOS);
                output.positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, 1.0));
                #if UNITY_REVERSED_Z
                    output.positionCS.z = min(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #else
                    output.positionCS.z = max(output.positionCS.z, UNITY_NEAR_CLIP_VALUE);
                #endif
                return output;
            }
            
            half4 ShadowPassFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
        
        // ------------------------------------------------------------------
        // DepthOnly Pass
        Pass
        {
            Name "DepthOnly"
            Tags { "LightMode" = "DepthOnly" }
            
            ZWrite On
            ColorMask 0
            Cull Back
            
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex DepthOnlyVertex
            #pragma fragment DepthOnlyFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            Varyings DepthOnlyVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }
            
            half4 DepthOnlyFragment(Varyings input) : SV_TARGET
            {
                return 0;
            }
            ENDHLSL
        }
        
        // ------------------------------------------------------------------
        // DepthNormals Pass
        Pass
        {
            Name "DepthNormals"
            Tags { "LightMode" = "DepthNormals" }
            
            ZWrite On
            Cull Back
            
            HLSLPROGRAM
            #pragma target 3.5
            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };
            
            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 normalWS   : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };
            
            Varyings DepthNormalsVertex(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }
            
            half4 DepthNormalsFragment(Varyings input) : SV_TARGET
            {
                half3 normalWS = normalize(input.normalWS);
                return half4(normalWS, 0.0);
            }
            ENDHLSL
        }
    }
    
    Fallback "Universal Render Pipeline/Lit"
}