#include "UnityCG.cginc"

fixed3 BakeLightsMin;
fixed BakeLightsSize;
sampler3D BakedLights;
sampler3D DynamicLights;
fixed DynamicMulti;
fixed CaveFog, CaveFogMax;

fixed  WaterLevel;
fixed4 WaterColor;
fixed WaterSize;
fixed3 WaterMin;
sampler2D WaterTex;
fixed CaveDarkness;
fixed ColorAdjust;
fixed CamBrightness;

struct LightSample
{
    fixed4 bake;
    fixed4 dyn;
};


fixed4 BakeSample(fixed3 posW)
{
    posW.y = WaterLevel + abs(posW.y - WaterLevel);

    posW.x *= -1;
    posW -= BakeLightsMin;
    posW *= BakeLightsSize;
    
   return tex3D(BakedLights, posW);
}

fixed4 DynSample(fixed3 posW)
{
    posW.y = WaterLevel + abs(posW.y - WaterLevel);
    
    fixed3 w = BakeLightsMin;
           w.x *= -1;
    posW -= w;
    posW *= BakeLightsSize;
    
    return tex3D(DynamicLights, posW) * (DynamicMulti * (1 + (1 - saturate(CamBrightness * 1.8))));
}


LightSample Sample(fixed3 posW)
{
    LightSample sample;
    
    sample.bake = BakeSample(posW);
    sample.dyn  = DynSample(posW);
    
    return sample;
}


fixed3 Dist(fixed3 pos)
{
    pos.y -= WaterLevel;
    fixed water = step(pos.y, 0);
    fixed waterDist = abs(pos.y);
    
    pos.y = WaterLevel + abs(pos.y);
    
    return fixed3(length(pos - _WorldSpaceCameraPos), water, waterDist);
}


fixed4 Water(fixed4 color, fixed3 pos, float d)
{
    fixed4 old = color;
    fixed3 dir = normalize(pos - _WorldSpaceCameraPos);
    fixed height = _WorldSpaceCameraPos.y - WaterLevel;
    fixed3 waterHit = _WorldSpaceCameraPos + dir * (height / abs(dir.y));
    
    fixed2 waterUV = fixed2((-waterHit.x - WaterMin.x) * WaterSize, 
                             (waterHit.z - WaterMin.z) * WaterSize);
    fixed tex = 1.0;// - pow(1.0 - pow(tex2D(WaterTex, waterUV).x, 2), 2);
    //return tex.xxxx;
    
    
    LightSample sample = Sample(waterHit);
                fixed4 c  = sample.bake * 1.35;
                fixed4 c2 = sample.dyn  * 2.25;
                
    
    pos.y -= WaterLevel;
    fixed water = step(pos.y, 0);
    fixed waterDist = abs(pos.y);
    
    color *= .65 * (1.0 - CaveDarkness);
    color  = lerp(WaterColor, color, tex * .4 + .6);
    //color *= .25 + .75 * tex;
    color  = lerp(WaterColor * .3 * (1.0 - CaveDarkness), color, pow(1.0 - saturate(waterDist / 30), 3));
    color += WaterColor * .245;
    
    color *= lerp(fixed4(1, 1, 1, 1), c, .825) + c2;
    
    fixed dist      = pow(1.0 - saturate(d * CaveFog), 6);
    fixed maxShadow = pow(saturate((CaveFogMax - d) / 50.0), 4);
    
    return lerp(old, color * 1.15 * (dist * maxShadow), water);
}


fixed4 Adjusted(fixed4 color)
{
    float b = CamBrightness;
          b = saturate(CamBrightness * 2.8);
    //color += -(CamBrightness - .5)  * .25;
    return lerp(color, pow(color, 1.3 + .3 +  (b)  * -.3) * (1.85 + -(b - .5)  * 1.4), ColorAdjust);// + -(CamBrightness - .5) * .25;
    return pow(color, 1.2) * 2;
}
