Shader "Mobile/ Realistic Car Shaders/Glass + Diffuse" {
	Properties {
		_Color ("Glass Color", Vector) = (1,1,1,1)
		_Transprnt ("Glass Transparency", Range(0.05, 0.9)) = 0.5
		_MainTex ("Diffuse", 2D) = "white" {}
		_DiffuseUVScale ("Diffuse UV Scale", Range(1, 100)) = 1
		_DiffuseBumpMap ("Diffuse Bumpmap", 2D) = "bump" {}
		_Cube ("Reflection Cubemap", Cube) = "white" {}
		_RefIntensity ("Reflection Intensity", Range(0, 2)) = 0
		_RenderedTexture ("Rendered Texture", 2D) = "white" {}
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
	//CustomEditor "VehicleGlass_Editor"
}