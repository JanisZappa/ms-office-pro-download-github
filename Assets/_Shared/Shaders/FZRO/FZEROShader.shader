Shader "FZERO/Main"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_LowTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="False"}
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

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

			sampler2D _MainTex;
			sampler2D _LowTex;

			float4 _Color;
			float4 FZero_Fog;
			float  FZero_FogDist;

			float TrackV;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv     = float2(v.uv.x, v.uv.y * TrackV);
				o.pos    = mul(unity_ObjectToWorld, v.vertex);
				o.color  = v.color;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				//return 

				float tint = i.color.x * i.color.x;
				float3 dir = (_WorldSpaceCameraPos - i.pos);
				
				float distValue = dot(dir, dir);
				float fogLerp = saturate(distValue / FZero_FogDist);

				float mipMapLerp = saturate(distValue / 20000);

				float3 tex  = tex2D(_MainTex, i.uv).xyz;
				float3 tex2 = tex2D(_LowTex, i.uv).xyz;

				float3 colorMix = lerp(tex, tex2, mipMapLerp) * _Color.xyz * tint;

				return float4(lerp(colorMix, FZero_Fog.xyz, fogLerp), 1);
			}
			ENDCG
		}
	}
}
