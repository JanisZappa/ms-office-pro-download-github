Shader "FZERO/Clouds"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }
		LOD 100
		ZWrite Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
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
			float4 _MainTex_ST;
			float4 _Color;
			float4 FZero_Fog;
			float  FZero_FogDist;
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv     = TRANSFORM_TEX(v.uv, _MainTex);
				o.pos    = mul(unity_ObjectToWorld, v.vertex);
				o.color  = v.color;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				float tint = i.color.x * i.color.x;
				float3 dir = (_WorldSpaceCameraPos - i.pos);
				
				float distLerp = saturate(dot(dir, dir) / FZero_FogDist);

				float texAlpha = tex2D(_MainTex, i.uv).x;
				float alpha = (1- pow(1 - texAlpha, 3)) * smoothstep(0, 1, i.color.r * i.color.r);

				return float4(_Color.xyz, alpha * .35);
			}
			ENDCG
		}
	}
}
