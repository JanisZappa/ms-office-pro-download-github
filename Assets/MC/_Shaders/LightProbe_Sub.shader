Shader "MC/LightProbe_Sub"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
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
            };

            struct v2f
            {
                fixed3 posW : TEXCOORD0;
                fixed4 vertex : SV_POSITION;
            };

            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.posW    = mul(unity_ObjectToWorld, fixed4(0, 0, 0, 1));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                LightSample sample = Sample(i.posW);
                fixed4 c  = sample.bake * 1.15;
                fixed4 c2 = sample.dyn  * 1.35;
                
                return Adjusted(Water(fixed4(_Color.xyz * lerp(fixed3(.75, .75, .75), c.xyz, .825) * (1.0 - CaveDarkness) + c2.xyz, 1), i.posW, Dist(i.posW)));
            }
            ENDCG
        }
    }
}
