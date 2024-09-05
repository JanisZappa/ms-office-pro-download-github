Shader "Unlit/CenterUV"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
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
                float2 uv2 : TEXCOORD1;
                float2 uv3 : TEXCOORD2;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float4 screenPos       : TEXCOORD0;
                float4 centerScreenPos : TEXCOORD1;
                float radius : TEXCOORD2;
                float dist : TEXCOORD3;
                float pick : TEXCOORD4;
                float3 wPos : TEXCOORD5;
                float3 normal : TEXCOORD6;
            };

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                
                float4 vert = UnityObjectToClipPos(v.vertex);
                float3 c = float3(v.uv, v.uv2.x);
                float4 cent = UnityObjectToClipPos(float4(c, 1));
                 
                
                o.vertex = vert;
                
                o.screenPos       = ComputeScreenPos(vert);
                o.centerScreenPos = ComputeScreenPos(cent);
                o.radius = length(mul(unity_ObjectToWorld, float4(normalize(float3(1, 1, 1)) * v.uv2.y, 0)));
                o.dist = length(mul(unity_ObjectToWorld, float4(c, 1)).xyz - _WorldSpaceCameraPos);
                o.pick = v.uv3.x;
                
                o.normal = mul(unity_ObjectToWorld, float4(v.normal, 0));
                o.wPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }
            
            #define PI 3.1415926535

            fixed4 frag (v2f i) : SV_Target
            {
                float aspect = _ScreenParams.x / _ScreenParams.y;
                
                float2 uv       =  i.screenPos.xy / i.screenPos.w;
                float2 uvCenter =  i.centerScreenPos.xy / i.centerScreenPos.w;
                
                uv = uv - uvCenter;
                uv.x *= aspect;
             
                uv *= i.dist / i.radius;
                float l = length(uv);
               
                uv *= .5;
                uv += .5;
                uv /= 8;
                uv.y += i.pick * .125;
                 
                float4 result = tex2D(_MainTex, uv);
                float3 camDir = normalize(i.wPos - _WorldSpaceCameraPos);
                
                float d = saturate(dot(-camDir, normalize(i.normal)));
                float b = pow(1 - d, 4);
                uv.y += .125 * 4;
                
                result = lerp(result, tex2D(_MainTex, uv), saturate(b * 3 ) * .5);
                //result *= 1 + saturate(pow(d, 120) * 18) * 3 * result;
                
                return result;
                
                
            }
            ENDCG
        }
    }
}
