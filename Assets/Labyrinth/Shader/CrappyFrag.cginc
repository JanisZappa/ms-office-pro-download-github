   struct v2f
    {
        fixed3 normal  : TEXCOORD0;
        fixed4 color   : TEXCOORD1;
        fixed4 uv      : TEXCOORD2;
        fixed3 wP      : TEXCOORD3;
        fixed3 tangent : TEXCOORD4;
        
        float4 vertex : SV_POSITION;
    };
    
    sampler2D _MainTex;
    float FlashLight;
    
    #define lightVOff .375
    #define lightVOn  .925
    
    
    float4 frag (v2f i) : SV_Target
    {
        fixed glow = (1.0 - i.color.a);
    
        float3 camDir = _WorldSpaceCameraPos - i.wP;
        float  camDist = camDir.x * camDir.x + camDir.y * camDir.y + camDir.z * camDir.z;
        #if FOG_ON
        
        fixed fog = camDist * .0001;
        fixed fa  = pow(1.0 - saturate(fog * (.15   - glow * .25 * 0.56 )), 10000) * .96 + .04;
        fixed fb  = pow(1.0 - saturate(fog * (.0375 - glow * .0625 * 0.56)), 10000);
              fog = fa * .7 + fb * .3;
              
        #else
        fixed fog = 1;
        #endif
        
        fixed flashLight = lightVOff + (lightVOn - lightVOff) * FlashLight;//saturate(FlashLight * (1.0 - glow * .75));
        
        fog *= flashLight;
        
        glow *= fog * 1.5 * (1 - FlashLight * .75);
        //return glow;
        
        
        
        fixed pureChecker = 0;
        #if CHECKER_ON
        pureChecker = tex2D(_MainTex, i.uv.xy * 45).x;
        fixed checker = pureChecker * .075 + .925;    
        #else
        fixed checker = 1;
        #endif
        
        #if NORMAL_OFF
        fixed bumpAmount = 0;
        #else
        fixed bumpAmount = (1 + i.uv.w);// * (sin(_Time.y) * .5 + .5);
        #endif
        //return bumpAmount;
        fixed4 bump   = BumpMap(i.uv);
        fixed3 normal = GetNormal(i.normal, i.tangent, bump.xy, bumpAmount);
        //return fixed4(normal * .5 + .5, 1);
              
        //return fixed4(normal * .5 + .5, 0);     
        fixed m = .075 + checker * .075;
        fixed light = RefMulti(normal) * m + (1.0 - m);
        fixed topLight = saturate(pow(normal.y * .5 + .5, 4));
              light *= topLight * .2 + .8;
              
              
        #if SHADOW_ON
        fixed pShadow = lerp(PlayerTint(i.wP), 1, glow * .75);
        #else
        fixed pShadow = 1;
        #endif
        
        fixed rValue = max(0, i.uv.z - .0001);
        fixed specAmount = saturate(1.0 - floor(rValue) * .01);
        fixed roughness  = saturate(1.0 - fmod(rValue, 1));
        //return roughness;
              //roughness = saturate(roughness + bump.z * -.125 * (1 - roughness * .5) * .5);// + (1 - bump.z) * .95 * bumpAmount);
      
        
        fixed2 spec = Specular(i.wP, normal, saturate(specAmount * saturate((.5 + fog * .5) * pShadow) * (.4 + .6 * flashLight * (.75 + .25 * fog))), saturate(roughness));
        
       
        fixed tint = fog * checker * light * pShadow;   
        //return tint;
        
        //fixed3 color = .5 * (1 - (1.0 - bump.z) * .15 * bumpAmount); //
        fixed3 color = saturate(i.color.xyz) * (1 - (1.0 - bump.z) * .35 * bumpAmount * roughness);
        //return fog;
        
        //return spec.y;
        glow *= 1 + pow(1.0 - spec.y, 1) * 2 * glow;
        return float4(saturate((color + spec.x) * tint + color * glow), saturate(camDist * .0175));
        //return float4(saturate((color + spec.x) * tint + color * glow * (1.0 - fog) * .75 + (pow(glow, 1.5) * 1 * (.25 + 4.75 * (pow(1.0 - spec.y, 10) * .75)))), saturate(camDist * .0175));
    }