Shader "Unlit/Cool"
{
 Properties
{
    _Sky ("Sky", Color) = (1,1,1,1)
    _Light("Light", Color) = (1,1,1,1)
    _Wall ("Wall", Color) = (1,1,1,1)
    _Wall2 ("Wall2", Color) = (1,1,1,1)
    _Gras ("Gras", Color) = (1,1,1,1)
    _Gras2("Gras2", Color) = (1,1,1,1)

    _Amount("Amount", float) = 0
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

            float4 _Sky, _Wall, _Wall2, _Light, _Gras, _Gras2;
            float _Amount;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal: TEXCOORD0;
                float3 wPos : TEXCOORD1;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.wPos   = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 n = normalize(i.normal);
                float3 light = normalize(float3(-1, .1, -2));
                float l = max(0, dot(n, light));

                float wall = ((1 - saturate((n.y - (1 - _Amount)) * 2)) * .5 + .5 * (1 - saturate((n.y - (1 - _Amount)) * 4)));

                float4 result = lerp(_Wall2, _Wall, pow(abs(n.y), 6)) * wall + lerp(_Gras2, _Gras, pow(abs(n.y), 8)) * (1 - wall);

                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float d = max(0, dot(reflect(normalize(camDir), n), light));
                      d = pow(d, 30) * .1 * wall +  pow(d, 2) * .025 * (1 - wall);

                float fog = 1;// + 2.45 * pow( (saturate(length(camDir) * .0025)), 2);
                return pow((result * (_Light * l + _Sky * max(0, n.y * .5 + .5)) + d * 3 * _Light) * fog, 1.1) * 3.5;
            }
            ENDCG
        }
    }
}
