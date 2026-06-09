Shader "Zen/Metallic" {
	Properties {
		_DiffuseColor ("Diffuse Color", Vector) = (1,1,1,1)
		_DiffuseColorForce ("Diffuse Color Force", Range(0, 2)) = 1
		_MainTex ("Albedo (RGB) A Metallic", 2D) = "white" {}
		[Toggle(_OCCLUSION_ON)] _UseOcclusion ("Use Occlusion", Float) = 0
		_Occlusion ("Occlusion", Range(0, 2)) = 1
		[Toggle(_METALLIC_ON)] _UseMetallic ("Use Metallic", Float) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0.9
		_Metallic ("Metallic", Range(0, 1)) = 0.25
		[Toggle(_DETAIL_ON)] _UseDetail ("Use Detail", Float) = 0
		_DetailColor ("Detail Color", Vector) = (1,1,1,1)
		_DetailColorForce ("Detail Color Force", Range(-1, 2)) = 0
		_DetailTex ("Detail (RGB) A Metallic", 2D) = "white" {}
		[Toggle(_NORMAL_ON)] _UseNormal ("Use Normal", Float) = 0
		_NormalTex ("Normal", 2D) = "white" {}
		[Toggle(_DECAL_ON)] _UseDecal ("Use Decal", Float) = 0
		_DecalColor ("Decal Color", Vector) = (1,1,1,1)
		_DecalTex ("Albedo (RGB) A Alpha", 2D) = "white" {}
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

			struct Fragment_Stage_Input
			{
				float2 uv : TEXCOORD0;
			};

			float4 frag(Fragment_Stage_Input input) : SV_TARGET
			{
				return _MainTex.Sample(sampler_MainTex, input.uv.xy);
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
}