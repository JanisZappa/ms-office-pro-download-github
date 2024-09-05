Shader "Unlit/GridFloor"
{
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "GridInc.cginc"
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float3 wPos : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                float4 vert = v.vertex;
                vert.z = 0;
                o.wPos = mul(unity_ObjectToWorld, vert);
                o.wPos.z = o.wPos.y;
                o.wPos.y = 0;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = Gray * .75;
                float4 dtl = float4(.5, .85, 1, 0);
                
                float checker = floor(i.wPos.x + 10000) + floor(i.wPos.z + 10000);
                checker = step(.5, fmod(checker, 2.0));
                checker = .75;
                col *= checker * .1 + .9;
                dtl.y += (1 - checker) * .1;
                
                //clip((1 - ClipIt(i.wPos + float3(0, 0, -.125))) - .25);
                
                return float4(Result(col.xyz, float3(0, 1, 0), dtl, i.wPos, 0, 0), 1);
                
            }
            ENDCG
        }
    }
}
