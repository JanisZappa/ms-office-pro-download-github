Shader "FZERO/Device"
{
	Properties
	{
		_Map ("Map", 2D) = "white" {}
		_Colors ("Colors", 2D) = "white" {}
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
			
			#include "UnityCG.cginc"

			uniform sampler2D _Map;
			uniform sampler2D _Colors;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv     : TEXCOORD0;
				float4 color  : COLOR;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                float  color : TEXCOORD2;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				o.color = v.color.x;
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
			    float tint = .6 + (1 - pow(1 - i.color, 2)) * .4;
			 
                fixed2 matCapUV = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).rg * 0.44 + 0.5);

                float3 matCap = tex2D(_Map, matCapUV);
                float finalColor = matCap.x * 1.9 * tint;
                float3 colors = tex2D(_Colors, i.uv) * tint;
                
                return fixed4(finalColor * colors * tint, 1);
			}
			ENDCG
		}
	}
}
