Shader "FZERO/Car"
{
	Properties
	{
		_Map ("Map", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Light ("Light", float ) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" "IgnoreProjector"="True" }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			#include "Fog.cginc"

			uniform sampler2D _Map;
			float4 _Color;
			float _Light;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
                float3 worldPos  : TEXCOORD1;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.worldPos    = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                fixed2 matCapUV = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).rg * 0.44 + 0.5);

                float3 finalColor = tex2D(_Map, matCapUV) * _Color.xyz * 2 * _Light;
                return FogResult(finalColor, GetDist(i.worldPos));
			}
			ENDCG
		}
	}
}
