Shader "Invector/EmissionWithMask" {
	Properties {
		_Color ("Color", Vector) = (1,1,1,0)
		_Difuse ("Difuse", 2D) = "white" {}
		_Metallic ("Metallic", Float) = 0
		_Smoothness ("Smoothness", Float) = 0
		_Normal ("Normal", 2D) = "bump" {}
		_NormalPower ("NormalPower", Float) = 0
		_EmissionColor ("EmissionColor", Vector) = (0,0,0,0)
		_EmissionMask ("Emission Mask", 2D) = "white" {}
		_Emission ("Emission", 2D) = "white" {}
		_Emissionpower ("Emission power", Float) = 1
		_EmissionTransition ("EmissionTransition", Range(0, 1)) = 100
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
	//CustomEditor "ASEMaterialInspector"
}