Shader "GBA/BlurH"
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
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            
            float4 Blur(float2 uv)
            {
                return float4(
                tex2D(_MainTex, uv + float2(-0.00078125,0)).rgb * 0.1993083 +
                tex2D(_MainTex, uv + float2(-0.000390625,0)).rgb * 0.2003455 +
                tex2D(_MainTex, uv + float2(0,0)).rgb * 0.2006924 +
                tex2D(_MainTex, uv + float2(0.000390625,0)).rgb * 0.2003455 +
                tex2D(_MainTex, uv + float2(0.00078125,0)).rgb * 0.1993083, 0);
            }
            

            fixed4 frag (v2f i) : SV_Target
            {
                return Blur(i.uv);
            }
            ENDCG
        }
    }
}
