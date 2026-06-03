Shader "Mobile/ Realistic Car Shaders/Body Color + Decal + Pearlescent + Diffuse" {
	Properties {
		_Color ("Vehicle Color", Vector) = (1,1,1,1)
		_MainTex ("Diffuse", 2D) = "white" {}
		_DiffuseUVScale ("Diffuse UV Scale", Range(1, 100)) = 1
		_DiffuseBumpMap ("Diffuse Bumpmap", 2D) = "bump" {}
		_RenderedTexture ("Rendered Texture", 2D) = "white" {}
		_DecalColor ("Decal Color", Vector) = (1,1,1,1)
		_Decal ("Decal", 2D) = "white" {}
		_DecalTransparency ("Decal Transparency", Range(0.1, 1)) = 1
		_DecalReflection ("Decal Reflection", Range(0, 1)) = 0.5
		_DecalUVScale ("Decal UV Scale", Range(1, 50)) = 1
		_PearlescentColor ("Pearlescent Color", Vector) = (1,1,1,1)
		_MainTexPearl ("Diffuse Pearl", 2D) = "white" {}
		_PearlBumpMap ("Diffuse Bumpmap", 2D) = "bump" {}
		_PearlUVScale ("Texture UV Scale", Range(1, 100)) = 1
		_ShininessIntensity ("Pearlescent Intensity", Range(0, 4)) = 0
		_ShininessScale ("Pearlescent Scale", Range(1, 50)) = 1
		_Cube ("Reflection Cubemap", Cube) = "white" {}
		_RefIntensity ("Reflection Intensity", Range(0, 2)) = 0
		_RefVisibility ("Reflection Visibility Scale", Range(0.1, 2)) = 0.1
		_MetalBrightnessIntensity ("Metal Brightness Intensity", Range(0.1, 2)) = 1
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
	Fallback "Standard"
	//CustomEditor "VehicleDecalPearlBump_Editor"
}