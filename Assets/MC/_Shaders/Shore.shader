Shader "MC/Shore"
{
    SubShader
    {
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
        //Blend SrcAlpha One
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
                float4 color : COLOR;
            };

            struct v2f
            {
                float  color : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 posW   : TEXCOORD1;
            };
            
            float CaveFog;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color  = v.color.x;
                o.posW   = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float c = i.color + sin(_Time.y * 9) * .007;
                float s = sin(_Time.y * 7) * .015 + .985;
                //clip((1.0 - c) -.15 * s);
                clip(c + -.75 * s);
                //clip(min(c + -.75 * s, (1.0 - c) -.15 * s));
                
                
                float dist = 1.0 - saturate(length(i.posW - _WorldSpaceCameraPos) * CaveFog);
                      dist = pow(dist, 6);
            
                return fixed4((.6).xxx * dist, .2);
            }
            ENDCG
        }
    }
}
