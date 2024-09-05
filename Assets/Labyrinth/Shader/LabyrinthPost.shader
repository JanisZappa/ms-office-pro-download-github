Shader "Labyrinth/Post"
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
            #pragma vertex vert_img
            #pragma fragment frag
            #include "Crappy.cginc"
            
            
            #pragma multi_compile VIGNETTE_OFF  VIGNETTE_ON
            #pragma multi_compile NOISE_OFF     NOISE_ON
            #pragma multi_compile COLOR_OFF     COLOR_ON
            #pragma multi_compile BLUR_OFF      BLUR_ON
            
            uniform sampler2D _MainTex;
            float2 NoiseOffset;
            float2 XYAmount;
            
            #define LOOPS 6
            #define DIAGONAL .70710678
            #define DEPTH 10
            
            fixed4 frag(v2f_img i) : COLOR 
            {
                fixed3 screentex = ScreenTex(i.uv);
                
                fixed na = animnoise(screentex.x,  5);
                fixed nb = animnoise(screentex.y, -2);
                
                fixed4 c   = tex2D(_MainTex, i.uv);
                fixed noise = (na + nb) * .0325;
                
                fixed4 result = saturate(pow(saturate(c + noise), 1.4) * 1.5 + .001);
                // result = (result.r *.3 + result.g *.59 + result.b *.11 + noise) * float4(.3, .86, 1.39, 1) * 1.1;
                result = c + noise;
                return result;
                return result.r *.3 + result.g *.59 + result.b *.11 + noise;
            }
            ENDCG
        }
    }
}
