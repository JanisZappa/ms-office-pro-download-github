Shader "Unlit/Patter"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainTex2 ("Texture", 2D) = "white" {}
        _MainTex3 ("Texture", 2D) = "white" {}
        _Sky("Sky", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}
        LOD 100
        ZWRITE OFF
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
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 wPos   : TEXCOORD2;
            };

            sampler2D _MainTex, _MainTex2, _MainTex3;
            float4 _Sky;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.normal = mul(unity_ObjectToWorld, v.normal);
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            
            float3 GetNormal(sampler2D tex, float2 uv, float3 f, float3 r, float3 n)
            {
                float4 col = tex2D(tex, uv) * 2 - 1;
                return r * col.x + f * col.y + n * col.z;
            } 
              

            fixed4 frag (v2f i) : SV_Target
            {
                float3 light = normalize(float3(1, .84, .3));
            
                
                float3 n = normalize(i.normal);
                float d = saturate(dot(n, light));
                //return d;
                float3 r = normalize(cross(float3(0, 0, 1), n));
                float3 f = normalize(cross(r, n));
                
                n = normalize(GetNormal(_MainTex2, i.uv  * 80, f, r, n));
                r = normalize(cross(float3(0, 0, 1), n));
                f = normalize(cross(r, n));
                
                d = saturate(dot(n, light) * .5 + .5);
                //return d;
                float3 camDir = i.wPos - _WorldSpaceCameraPos;
                float dist = length(camDir);
                camDir /= dist;
                
                float l = saturate(dot(reflect(camDir, n), light));
                float s = (pow(l, 4) * .4 + pow(l, 400) * .4) * .95;
                float b = pow(1 - saturate(dot(-camDir, n)), 8)* .15 * 0;
                
                //return s + b;
                
                float4 result = d + s + b  + (1.0 - pow(1.0 - saturate(dist * .000275), 2)) * .35;
                float4 col = tex2D(_MainTex, i.uv * 80);
                result *= col;
                //result.xyz *= lerp(float3(.27, .35, .45), float3(.9, .65, .5), pow(n.y, 2)) * 1.35;
                result.xyz = pow(result.xyz + .075, 1.75) * 1.15;
                result = lerp(result, _Sky, pow(saturate(dist * .00085), 2) * .8);
                return float4(result.xyz, col.a);
            }
            ENDCG
        }
    }
}
