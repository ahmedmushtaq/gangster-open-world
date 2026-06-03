Shader "OceanShader/OceanShoreLine" {
	Properties {
		_WaterTex ("Normal Map (RGB), Foam (A)", 2D) = "white" {}
		_WaterTex2 ("Normal Map (RGB), Foam (B)", 2D) = "white" {}
		_ShoreLineTex ("ShoreLine Foam", 2D) = "white" {}
		_ShoreGray ("Shore Gray(gray)", 2D) = "white" {}
		_ShoreLineIntensity ("ShoreLine Intensity", Float) = 2
		_Tiling ("Wave Scale", Range(0.00025, 1)) = 0.25
		_WaveSpeed ("Wave Speed", Float) = 0.4
		_SpecularRatio ("Specular Ratio", Range(10, 500)) = 200
		_BottomColor ("Bottom Color", Vector) = (0,0,0,0)
		_TopColor ("Top Color", Vector) = (0,0,0,0)
		_Alpha ("Alpha", Range(0, 1)) = 1
		_ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 0.1
		_OceanWidth ("Ocean Width", Float) = 10240
		_OceanHeight ("Ocean Height", Float) = 10240
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