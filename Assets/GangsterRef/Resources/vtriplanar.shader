Shader "Invector/Triplanar" {
	Properties {
		[Toggle] _TriplanarAlbedo ("TriplanarAlbedo", Float) = 0
		[Toggle] _TriplanarNormal ("TriplanarNormal", Float) = 0
		[Toggle] _TriplanarMetallicSmoothneess ("TriplanarMetallicSmoothneess", Float) = 0
		_AlbedoColor ("AlbedoColor", Vector) = (1,1,1,1)
		_Albedo ("Albedo", 2D) = "white" {}
		[Normal] _Normal ("Normal", 2D) = "bump" {}
		_NormalScale ("NormalScale", Float) = 0
		_MetallicSmoothness ("MetallicSmoothness", 2D) = "white" {}
		_Metallic ("Metallic", Range(0, 1)) = 0
		_Smoothness ("Smoothness", Range(0, 1)) = 0
		[Toggle] _TriplanarDetailAlbedo ("TriplanarDetailAlbedo", Float) = 0
		[Toggle] _TriplanarDetailNormal ("TriplanarDetailNormal", Float) = 0
		[Toggle] _TriplanarDetailMetallicSmoothneess ("TriplanarDetailMetallicSmoothneess", Float) = 0
		_DetailColor ("DetailColor", Vector) = (1,1,1,1)
		_AlbedoDetail ("AlbedoDetail", 2D) = "white" {}
		[Normal] _NormalDetail ("NormalDetail", 2D) = "bump" {}
		_NormalDetailScale ("NormalDetailScale", Float) = 0
		_MetallicSmoothnessDetail ("MetallicSmoothnessDetail", 2D) = "white" {}
		_MetallicDetail ("MetallicDetail", Range(0, 1)) = 0
		_SmoothnessDetail ("SmoothnessDetail", Range(0, 1)) = 0
		[Toggle] _TriplanarGlobal ("TriplanarGlobal", Float) = 1
		[Toggle] _ShowVertexMaskR ("ShowVertexMask(R)", Float) = 0
		[Toggle] _InvertVertexColorR ("InvertVertexColor(R)", Float) = 0
		_VertexMaskContrast ("VertexMaskContrast", Float) = 1
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType" = "Opaque" }
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

			float4 frag(Vertex_Stage_Output input) : SV_TARGET
			{
				return float4(1.0, 1.0, 1.0, 1.0); // RGBA
			}

			ENDHLSL
		}
	}
	Fallback "Diffuse"
}