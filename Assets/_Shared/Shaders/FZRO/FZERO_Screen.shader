Shader "FZERO/Screen"
{
	Properties
	{
		_Map ("Map", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        Blend SrcAlpha One
        Cull Off Lighting Off ZWrite Off Fog { Color (0,0,0,0) }
		LOD 100

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform sampler2D _Map;

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};              

			struct v2f
			{
				float4 pos : SV_POSITION;
                float3 normalDir : TEXCOORD0;
			};
			
			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.normalDir = UnityObjectToWorldNormal(v.normal);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
                fixed2 matCapUV = (mul( UNITY_MATRIX_V, float4(i.normalDir,0) ).xyz.rgb.rg * 0.44 + 0.5);


                return pow((tex2D(_Map, matCapUV).x - .5), 2) * 5;
			}
			ENDCG
		}
	}
}
