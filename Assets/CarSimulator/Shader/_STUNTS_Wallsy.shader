Shader "_STUNTS/Wallsy" {
	Properties {
		[Header(Ambient and Shadows)] _LightmapInt ("Lightmap-Ambient Intesity", Range(0, 4)) = 1
		_ShadowCutt ("Shadow Cuttoff", Range(0, 1)) = 0.7
		_ShadowAmbientInt ("Shadow Ambient int", Range(0, 1)) = 0
		_ShadowColor ("Shadow Color", Vector) = (0,0,0,1)
		[Header(Textures)] _MainTex ("Texture Map (RGB Albedo) A specular", 2D) = "white" {}
		_MainColor ("Main Color", Vector) = (1,1,1,1)
		[Toggle(USE_NORMALS)] _UseNormals ("Use Normals", Float) = 1
		[NoScaleOffset] _BumpTex ("Normal Map", 2D) = "bump" {}
		_Bumpinness ("Bumpiness", Range(0, 10)) = 1
		[Header(Specular)] _Shininess ("SpecShinnes", Range(0.1, 700)) = 20
		_SpecInt ("SpecIntensity Mat", Range(0, 10)) = 1
		[Header(Reflection)] [Toggle(USE_REFLECTIONS)] _UseReflections ("Use Reflections", Float) = 1
		_ReflectionCube ("Reflection cube", Cube) = "white" {}
		_ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 1
		_ReflectionSmoothness ("Reflection smoothness", Range(0, 6)) = 1
		[Toggle(UNDER_WATER)] _UnderWater ("Under Water", Float) = 0
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