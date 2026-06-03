Shader "Beffio/Upgrade Clear Coat/Layer - Overlay" {
	Properties {
		_OcclusionStrength ("Strength", Range(0, 1)) = 1
		_OcclusionMap ("Occlusion", 2D) = "white" {}
		_OverlayColor ("Overlay Color", Vector) = (1,1,1,1)
		_OverlayMainTex ("Overlay Albedo (RGB)", 2D) = "white" {}
		_OverlayGlossiness ("Overlay Smoothness", Range(0, 1)) = 0.5
		_OverlaySpecular ("Overlay Specular", Vector) = (0.3,0.3,0.3,1)
		_OverlaySpecGlossMap ("Overlay Specular Texture", 2D) = "black" {}
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