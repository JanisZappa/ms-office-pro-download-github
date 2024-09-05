Shader "MC/MC"
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

            #include "Lighting.cginc"

            struct appdata
            {
                fixed4 vertex : POSITION;
                fixed2 uv : TEXCOORD0;
                fixed4 color : COLOR;
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
                //clip(tex2Dlod(_MainTex, fixed4(i.uv, 0, 0)).a - .9);
            
                fixed4 col = tex2D(_MainTex, i.uv);
                clip(col.a - .01);
                
                fixed3 caveDist = Dist(i.posW);
                fixed d = caveDist.x;
                
                fixed dist        = pow(1.0 - saturate(d * CaveFog), 6);
                fixed shortLight  = (1 - saturate(d * .015)) * .175;
                fixed shortShadow = .625 + .375 * saturate(d * .55);
                fixed maxShadow   = pow(saturate((CaveFogMax - d) / 50.0), 4);
                
                fixed4 c2 = DynSample(i.posW);
                
                fixed4 color = i.color * 1.15 * (1.0 - CaveDarkness);
                
                return Adjusted((col * max(color, shortLight.xxxx) + c2) * (dist * shortShadow * maxShadow));
            }
            ENDCG
        }
    }
}
