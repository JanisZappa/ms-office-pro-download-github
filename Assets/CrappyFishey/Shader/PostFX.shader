Shader "Crappy/PostFX"
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
                #if VIGNETTE_ON
                fixed ring = screentex.z;
                #else
                fixed ring = 1;
                #endif
                
                
                fixed na = animnoise(screentex.x,  5);
                fixed nb = animnoise(screentex.y, -2);
                
                
                fixed2 uv = i.uv;
                
                fixed4 c   = tex2D(_MainTex, uv);
                fixed tint = c.r *.3 + c.g *.59 + c.b *.11;
                
                #if BLUR_ON
                fixed weight = c.w;
                fixed amount = 1.15 - pow(1.0 - weight, 2);
                fixed w = 1;
                fixed r = (pow(1.0 - ring * .85, 1) + na * .2) * 1.25;
             
                for(int i = 0; i < LOOPS; i++)
                {
                    fixed w2 = 0;
                    fixed4 c2 = fixed4(0, 0, 0, 0);
                    
                    fixed f1 = (i + 1) * 1.0 / LOOPS;
                    fixed f2 = (1.0 - f1);
                    fixed x  = XYAmount.x * max(r, amount * f1); //.008
                    fixed y  = x * XYAmount.y;
                    fixed ii = saturate(6 - i) * .5 * f2;
                     
                    c2 = tex2D(_MainTex, uv + fixed2(-x, -y)* DIAGONAL);
                    w2 = (saturate(1.0 - (weight - c2.w) * DEPTH) * f2 + r) + ii;
                    w += w2;
                    c += c2 * w2;
                    c2 = tex2D(_MainTex, uv + fixed2( 0, -y));
                    w2 = saturate(1.0 - (weight - c2.w) * DEPTH) * f2 + r + ii;
                    w += w2;
                    c += c2 * w2;
                    c2 = tex2D(_MainTex, uv + fixed2(x, -y)* DIAGONAL);
                    w2 = (saturate(1.0 - (weight - c2.w) * DEPTH) * f2 + r) + ii;
                    w += w2;
                    c += c2 * w2;
                    
                    c2 = tex2D(_MainTex, uv + fixed2(-x, 0));
                    w2 = saturate(1.0 - (weight - c2.w) * DEPTH) * f2 + r + ii;
                    w += w2;
                    c += c2 * w2;
                    c2 = tex2D(_MainTex, uv + fixed2(x, 0));
                    w2 = saturate(1.0 - (weight - c2.w) * DEPTH) * f2 + r + ii;
                    w += w2;
                    c += c2 * w2;
                    
                    c2 = tex2D(_MainTex, uv + fixed2(-x, y) * DIAGONAL);
                    w2 = (saturate(1.0 - (weight - c2.w) * DEPTH) * f2 + r) + ii;
                    w += w2;
                    c += c2 * w2;
                    c2 = tex2D(_MainTex, uv + fixed2( 0, y));
                    w2 = saturate(1.0 - (weight - c2.w) * DEPTH) * f2 + r + ii;
                    w += w2;
                    c += c2 * w2;
                    c2 = tex2D(_MainTex, uv + fixed2(x, y)* DIAGONAL);
                    w2 = (saturate(1.0 - (weight - c2.w) * DEPTH) * f2 + r) + ii;
                    w += w2;
                    c += c2 * w2;
                }
                c /= w;
                
                tint = c.r *.3 + c.g *.59 + c.b *.11;
                #endif
                
                c *= 1 + c.w * (1.75 + (na + nb) * -8);
                c = saturate(c);
                
                tint = c.r *.3 + c.g *.59 + c.b *.11;
                
                
                #if NOISE_ON  
                fixed ntint = pow(1.0 - pow(1.0 - tint, 25), 2);
                fixed noise = (na + nb) * .75 * ((1.0 - ring) + (1.0 - ntint) * 2 + .4);
                #else
                fixed noise = 0;
                #endif
                
                
                #if COLOR_ON
                fixed multi = lerp(.7, 1.2, tint);
                c.x *= multi;
                c.z *= 1.0 / multi;
                
                fixed total = c.x + c.y + c.z;
                fixed ctint = tint + noise * 10;
                fixed t = _Time.y;
                fixed a = .07 - ring * .05;
                c.x = pow(c.x, lerp(3.3, 6.2, ctint)) * (sin(t * 6.43   + ring * 10) * a + .1) + c.x;
                c.y = pow(c.y, lerp(2.2, 4  , ctint)) * (sin(t * 3.212  - ring * 10) * a + .1) + c.y;
                c.z = pow(c.z, lerp(6.5, 3.1, ctint)) * (sin(t * 2.2427 + ring * 10) * a + .1) + c.z;
                
                total = total / (c.x + c.y + c.z);
                
                c.x *= total;
                c.y *= total;
                c.z *= total;
                
                tint = c.r *.3 + c.g *.59 + c.b *.11;
                c = tint + (c - tint) * 1.2;
                
                c *= 1.2;
                
                fixed4 result = saturate(c * ring + noise);
                
                return (result * .55 + (1.0 - pow(1.0 - pow(result, 2), 4)) * .6 + (1.0 - pow(1.0 - pow(result, 8), 8)) * .20);
                
                #endif
                
                return saturate(c * ring + noise);
            }
            ENDCG
        }
    }
}
