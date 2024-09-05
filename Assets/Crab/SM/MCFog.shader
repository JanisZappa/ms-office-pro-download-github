Shader "MC/MCFog"
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
            #include "volume.cginc"
            

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv     : TEXCOORD0;
                fixed4 color  : COLOR;
            };

            struct v2f
            {
                fixed2 uv     : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
                fixed4 color  : TEXCOORD1;
                fixed3 posW   : TEXCOORD2;
            };

            sampler2D _MainTex;
            
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv     = v.uv;
                o.color  = v.color;
                o.color  = lerp(v.color, fixed4((.3 *  v.color.r + .59 *  v.color.g + .11 *  v.color.b).xxx, 1), .3) * 1.2;
                o.posW   = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                //clip(tex2Dlod(_MainTex, fixed4(i.uv, 0, 0)).a - .01);
            
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - .01);
                
                fixed4 color = i.color * 1.15;
                
                fixed4 result = col * color;
                return result;
                      // result = 0;
                      
                float3 dir = i.posW - _WorldSpaceCameraPos;
                float d = length(dir);
                float3 p = _WorldSpaceCameraPos + dir / d * min(d, 90);
                return lerp(SampleTex(p, result.xyz), fixed4(0, 0, 0, 0), step(i.posW.y, -16) * .15);
            }
            ENDCG
        }
    }
}
