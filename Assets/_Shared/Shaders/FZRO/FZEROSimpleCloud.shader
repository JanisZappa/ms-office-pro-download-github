Shader "FZERO/SimpleCloud"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True"}
		LOD 100
		Cull Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"
			#include "Fog.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float4 color  : COLOR;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 pos    : TEXCOORD0;
			};

			float4 _Color;
			float  TrackV;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.pos    = mul(unity_ObjectToWorld, v.vertex);

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return FogResult(_Color.xyz, GetDist(i.pos));
			}
			ENDCG
		}
	}
}
