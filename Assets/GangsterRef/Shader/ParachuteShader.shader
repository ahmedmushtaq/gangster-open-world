Shader "ParachuteShader" {
	Properties {
		_Color ("Color", Vector) = (1,0,0,0)
		_Albedo ("Albedo", 2D) = "white" {}
		_Metallic ("Metallic", 2D) = "white" {}
		_MetallicPower ("MetallicPower", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0
		_Normal ("Normal", 2D) = "bump" {}
		_NormalScale ("NormalScale", Float) = 0
		_NoiseMaskA ("NoiseMask(A)", 2D) = "white" {}
		_NoiseIntensity ("NoiseIntensity", Float) = 0
		_NoiseScale ("NoiseScale", Float) = 0
		_NoiseSpeed ("NoiseSpeed", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
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

			struct Vertex_Stage_Input
			{
				float4 pos : POSITION;
			};

			struct Vertex_Stage_Output
			{
				float4 pos : SV_POSITION;
			};

			Vertex_Stage_Output vert(Vertex_Stage_Input input)
			{
				Vertex_Stage_Output output;
				output.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, input.pos));
				return output;
			}

			float4 _Color;

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return _Color; // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
}