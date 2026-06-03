Shader "Shader Graphs/Car Paint - Custom Thanh" {
	Properties {
		_Fresnel_Power ("Fresnel Power", Float) = 2.5
		_Fresnel_Strength ("Fresnel Strength", Float) = 2
		_MainTex ("MainTex", 2D) = "white" {}
		_MainColor1 ("MainColor1", Vector) = (1,1,1,0)
		_MainColor2 ("MainColor2", Vector) = (0,0,0,0)
		_SkinColor ("SkinColor", Vector) = (0,0,0,0)
		_Metalic ("Metalic", Range(0, 1)) = 0.5
		_Smoothness_Frensel_1 ("Smoothness Frensel 1", Float) = 1
		_Smoothness_Frensel_2 ("Smoothness Frensel 2", Float) = 0.6
		_Smoothness_Strength ("Smoothness Strength", Float) = 0.9
		_AO ("AO", Float) = 1
		[HideInInspector] _BUILTIN_QueueOffset ("Float", Float) = 0
		[HideInInspector] _BUILTIN_QueueControl ("Float", Float) = -1
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
	Fallback "Hidden/Shader Graph/FallbackError"
	//CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
}