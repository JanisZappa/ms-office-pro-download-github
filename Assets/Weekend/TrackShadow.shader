Shader "Unlit/TrackShadow"
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4x4 toLight, toWorld;
            float3 lightDir;
            v2f vert (appdata v)
            {
                v2f o;
                
                float4 vert = v.vertex;
                //vert = mul(toLight, vert);
                //vert.z = 0;
                //vert = mul(toWorld, vert);
                vert -= float4(lightDir * ( vert.y / lightDir.y ), 0);
                o.vertex = UnityObjectToClipPos(vert);
                
                
                
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return .03;
            }
            ENDCG
        }
    }
}
