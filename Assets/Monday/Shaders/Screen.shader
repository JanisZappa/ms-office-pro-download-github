Shader "GBA/Screen"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Pixel ("Pixel", 2D) = "white" {}
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _Pixel;
            #define XRes 640
            #define YRes 288

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                uv -= .5;
                uv *= 2;
                uv.x *= 16.0 / 9;
                
                float l = length(uv);
                uv *= 2 - cos(l * .125);
                uv.x *= 9.0 / 16;
                uv *= .5 * .965;
                uv += .5;
                
                float vignette = (pow(saturate(1 - l * .01), 100));
                float cShift = (1 - vignette) * 2.5;
                
                fixed4 col = tex2D(_MainTex, uv);
                //return col;
                fixed4 p = tex2D(_Pixel, float2(uv.x * XRes, uv.y * YRes)) * 2 - .5;
                
                float t = .11666;
                float shift = floor(_Time.y * 20) * .5;
                float scan = (1 - abs(fmod(uv.y * YRes + shift, 1) - .5) * 2) * t + 1 - t;
                
                float4 result = (col + p * .0949) * scan;
                
                result.x *= 1 - cShift * .03;
                result.y *= 1 + cShift * .02;
                result.z *= 1 + cShift * .05;
                
                result.xyz = 1.0 - result.xyz;
                
                return result * (.65 + .35 * vignette) * 1.2;
            }
            ENDCG
        }
    }
}
