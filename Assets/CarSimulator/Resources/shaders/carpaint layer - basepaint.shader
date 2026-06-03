Shader "Beffio/Upgrade Clear Coat/Layer - Base Paint" {
	Properties {
		_Color ("Base Albedo", Vector) = (1,1,1,1)
		_MainTex ("Base Albedo Texture", 2D) = "white" {}
		_Glossiness ("Base Smoothness", Range(0, 1)) = 0.5
		[Gamma] _Metallic ("Base Metallic", Range(0, 1)) = 0
		_MetallicGlossMap ("Base Metallic Texture", 2D) = "white" {}
		_BumpScale ("Scale", Float) = 1
		[Normal] _BumpMap ("Normal Map", 2D) = "bump" {}
		_OcclusionStrength ("Strength", Range(0, 1)) = 1
		_OcclusionMap ("Occlusion", 2D) = "white" {}
		_EmissionColor ("Color", Vector) = (0,0,0,1)
		_EmissionMap ("Emission", 2D) = "white" {}
		[NoScaleOffset] [Normal] _FlakesBumpMap ("Base Bump Flakes (normal)", 2D) = "bump" {}
		_FlakesBumpMapScale ("Base Bump Flakes Scale", Float) = 1
		_FlakesBumpStrength ("Base Bump Flakes Strength", Range(0.001, 8)) = 1
		_FlakeColor ("Base Flakes Albedo", Vector) = (1,1,1,1)
		_FlakesColorMap ("Base Flakes Albedo Texture", 2D) = "black" {}
		_FlakesColorMapScale ("Base Flakes Color Scale", Float) = 1
		_FlakesColorStrength ("Base Flakes Color Strength", Range(0, 10)) = 1
		_FlakesColorCutoff ("Base Flakes Color Cutoff", Range(0, 0.95)) = 0.5
		_FresnelColor ("Fresnel Color", Vector) = (1,1,1,1)
		_FresnelPower ("Fresnel Power", Range(0, 10)) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			float4x4 unity_ObjectToWorld;
			float4x4 unity_MatrixVP;
			float4 _MainTex_ST;

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct Vertex_Stage_Output
			{
				float2 uv : TEXCOORD0;
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.uv = (input.uv.xy * _MainTex_ST.xy) + _MainTex_ST.zw;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			Texture2D<float4> _MainTex;
			SamplerState sampler_MainTex;
			float4 _Color;

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy) * _Color;
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
}