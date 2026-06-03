Shader "_STUNTS/Flooraida" {
	Properties {
		_MainTex ("Texture (A Specular :D)", 2D) = "white" {}
		[NoScaleOffset] _NormalMap ("Normal", 2D) = "bump" {}
		_Bumpiness ("Bumpiness", Range(0, 5)) = 0
		[NoScaleOffset] _FloorMask ("Floor mask", 2D) = "black" {}
		[NoScaleOffset] _FloorDecals ("Floor decals", 2D) = "black" {}
		_DecalColorR ("Decal color R", Vector) = (1,1,1,1)
		_DecalColorG ("Decal color G", Vector) = (1,1,1,1)
		_DecalColorB ("Decal color B", Vector) = (1,1,1,1)
		_DecalColorA ("Decal color A", Vector) = (1,1,1,1)
		_DecalScaleOffsetR ("Decal R tile/offset", Vector) = (1,1,0,0)
		_DecalScaleOffsetG ("Decal G tile/offset", Vector) = (1,1,0,0)
		_DecalScaleOffsetB ("Decal B tile/offset", Vector) = (1,1,0,0)
		_DecalScaleOffsetA ("Decal A tile/offset", Vector) = (1,1,0,0)
		_SpecInt ("Specular intensity", Float) = 1
		_SpecGloss ("Specular glossiness", Float) = 1
		_LightmapInt ("Lightmap intensity", Float) = 1
		_ShadowInt ("Shadow intensity", Range(0, 1)) = 1
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