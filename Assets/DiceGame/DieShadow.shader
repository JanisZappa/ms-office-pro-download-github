Shader "Unlit/DieShadow"
{
    Properties
    {
        _Color("C", Color) = (1,1,1,1)
    }
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
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                
                
                float3 light = normalize(float3(.6, -1, .25));
                float4 vert = mul(unity_ObjectToWorld, v.vertex);
                vert.xyz += light * vert.y;
                vert =  mul(unity_WorldToObject, vert);
                o.vertex = UnityObjectToClipPos(vert);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _Color ;
            }
            ENDCG
        }
    }
}
