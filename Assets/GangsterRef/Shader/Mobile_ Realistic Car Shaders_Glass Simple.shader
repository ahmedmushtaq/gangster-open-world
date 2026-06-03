Shader "Mobile/ Realistic Car Shaders/Glass Simple" {
	Properties {
		_Color ("Glass Color", Vector) = (1,1,1,1)
		_Transprnt ("Glass Transparency", Range(0.05, 0.9)) = 0.5
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
	Fallback "Standard"
	//CustomEditor "VehicleGlassSimple_Editor"
}