Shader "Hidden/AQUAS/Underwater/Fog" {
	Properties {
		_MainTex ("Main Tex", 2D) = "white" {}
		_Fade ("Fade", Range(0.001, 10)) = 1
		_FogColor ("Fog Color", Vector) = (0.3550641,0.6285198,0.745283,0)
		_Density ("Density", Float) = 40
		_DepthMask ("DepthMask", 2D) = "white" {}
		_DistortionLens ("DistortionLens", 2D) = "white" {}
		_Distortion ("Distortion", Range(0, 0.05)) = 0.05
		_DropletMask ("Droplet Mask", 2D) = "white" {}
		_DropletNormals ("Droplet Normals", 2D) = "white" {}
		_DropletCutout ("DropletCutout", 2D) = "white" {}
		[Toggle] _EnableWetLens ("Enable Wet Lens", Float) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
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
}