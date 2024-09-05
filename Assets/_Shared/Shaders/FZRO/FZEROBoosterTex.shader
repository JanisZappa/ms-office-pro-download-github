Shader "FZERO/BoosterTex"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LowTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
            #include "MipMap.cginc"
            #include "Fog.cginc"
            
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv     : TEXCOORD0;
				float4 color  : COLOR;
			};

			struct v2f
			{
				float2 uv     : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 pos    : TEXCOORD1;
				float4 color  : TEXCOORD2;
			};

			

			float TrackV;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv     = v.uv;
				o.pos    = mul(unity_ObjectToWorld, v.vertex);
				o.color  = v.color;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float tint = 1; //i.color.x * i.color.x;
				
				float dist = GetDist(i.pos);

				float3 colorMix = MainTexLerp(i.uv, dist) * tint;

				return FogResult(colorMix, dist);
			}
			ENDCG
		}
	}
}
