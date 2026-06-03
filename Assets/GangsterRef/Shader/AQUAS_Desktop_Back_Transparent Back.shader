Shader "AQUAS/Desktop/Back/Transparent Back" {
	Properties {
		[Header(Wave Options)] [NoScaleOffset] _NormalTexture ("Normal Texture", 2D) = "bump" {}
		_NormalTiling ("Normal Tiling", Range(0.01, 2)) = 1
		_NormalStrength ("Normal Strength", Range(0, 2)) = 0
		_WaveSpeed ("Wave Speed", Float) = 0
		_Refraction ("Refraction", Range(0, 1)) = 0.1
		_DeepWaterColor ("Deep Water Color", Vector) = (0,0,0,0)
		[Header(Distance Options)] _MediumTilingDistance ("Medium Tiling Distance", Float) = 0
		_FarTilingDistance ("Far Tiling Distance", Float) = 0
		_DistanceFade ("Distance Fade", Float) = 0
		[Header(Shoreline Waves)] _ShorelineFrequency ("Shoreline Frequency", Float) = 0
		_ShorelineSpeed ("Shoreline Speed", Range(0, 0.2)) = 0
		_ShorelineNormalStrength ("Shoreline Normal Strength", Range(0, 1)) = 0
		_ShorelineBlend ("Shoreline Blend", Range(0, 1)) = 0
		[NoScaleOffset] _ShorelineMask ("Shoreline Mask", 2D) = "white" {}
		_RandomMask1 ("Random Mask", 2D) = "white" {}
		[Header(Flowmap Options)] [NoScaleOffset] _FlowMap ("FlowMap", 2D) = "white" {}
		_FlowSpeed ("Flow Speed", Float) = 20
		[Toggle] _LinearColorSpace ("Linear Color Space", Float) = 0
		[Header(Ripple Options)] _RippleStrength ("Ripple Strength", Range(0, 1)) = 0.5
		[HideInInspector] _RippleTex0 ("RippleTex0", 2D) = "white" {}
		[HideInInspector] _Scale0 ("Scale0", Float) = 0
		[HideInInspector] _XOffset0 ("XOffset0", Float) = 0
		[HideInInspector] _ZOffset0 ("ZOffset0", Float) = 0
		[HideInInspector] _RippleTex1 ("RippleTex1", 2D) = "white" {}
		[HideInInspector] _Scale1 ("Scale1", Float) = 0
		[HideInInspector] _XOffset1 ("XOffset1", Float) = 0
		[HideInInspector] _ZOffset1 ("ZOffset1", Float) = 0
		[HideInInspector] _RippleTex2 ("RippleTex2", 2D) = "white" {}
		[HideInInspector] _Scale2 ("Scale2", Float) = 0
		[HideInInspector] _XOffset2 ("XOffset2", Float) = 0
		[HideInInspector] _ZOffset2 ("ZOffset2", Float) = 0
		[HideInInspector] _RippleTex3 ("RippleTex3", 2D) = "white" {}
		[HideInInspector] _Scale3 ("Scale3", Float) = 0
		[HideInInspector] _XOffset3 ("XOffset3", Float) = 0
		[HideInInspector] _ZOffset3 ("ZOffset3", Float) = 0
		[Toggle] [HideInInspector] _ProjectGrid ("Project Grid", Float) = 0
		[HideInInspector] _ObjectScale ("Object Scale", Vector) = (0,0,0,0)
		[HideInInspector] _waterLevel ("waterLevel", Float) = 0
		[HideInInspector] _RangeVector ("Range Vector", Vector) = (0,0,0,0)
		_PhysicalNormalStrength ("Physical Normal Strength", Range(0, 1)) = 0
		[HideInInspector] _texcoord ("", 2D) = "white" {}
		[HideInInspector] __dirty ("", Float) = 1
		[Header(Forward Rendering Options)] [ToggleOff] _GlossyReflections ("Reflections", Float) = 1
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