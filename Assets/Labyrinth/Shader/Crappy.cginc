#include "UnityCG.cginc"
#define PI 3.1415926538


float4 Bulge(float4 vert)
{
    vert = UnityObjectToClipPos(vert);

    #if BULGE_ON
    float aspect = unity_OrthoParams.x / unity_OrthoParams.y;

    float2 pos = vert.xy / vert.w;
           pos.x *= aspect;
           
    float amount = saturate(1.0 - length(pos) * .25);
    pos *= (amount * .65 + .35)* 1.444;
    
    pos.x *= 1.0 / aspect;
           
    vert.xy = pos * vert.w;
    #endif
    
    return vert;
}


fixed3 PlayerRoot;
fixed PlayerTint(float3 wP)
{
    fixed3 dir = wP - PlayerRoot;
           dir.y = lerp(dir.y, max(dir.y - (1.37 - .3), 0), step(0, dir.y));
           dir.y *= .4;
           
    fixed v = saturate((dir.x * dir.x + dir.y * dir.y + dir.z * dir.z) * 8);
          v = 1.0 - pow(1.0 - v, 3);
          
    return v * .425 + .575;
}


fixed2 GetMatCapUV(fixed3 normal)
{
    fixed2 matCapNormal = mul(UNITY_MATRIX_V, normal).rg;
                
    return fixed2((matCapNormal.r *  .485) + .5, 
                   matCapNormal.g *  .485 + .5);
}
 
            
sampler2D _MatCap;         
fixed MatCapX(fixed3 normal)
{
    return (tex2D(_MatCap, GetMatCapUV(normal)).x - .5) * .5;
} 


fixed RefMulti(fixed3 normal)
{
    return mul(UNITY_MATRIX_V, normal).z;
}


sampler2D _NormalMap;

fixed4 BumpMap(fixed4 uv)
{
    return tex2D(_NormalMap, uv.xy * 100);//135);
}


fixed3 GetNormal(fixed3 n, fixed3 t, fixed2 bump, fixed bumpAmount)
{
    #if NORMAL_OFF
    return normalize(n);
    #endif
    
    n = normalize(n);
    t = normalize(t);
    
    fixed3 tangentNormal = fixed3(bump * 2.0 - 1.0, 0);
           tangentNormal.xy *= max(0, bumpAmount * .75);
           tangentNormal.z = saturate(sqrt(1.0 - pow(tangentNormal.x, 2) + pow(tangentNormal.y, 2)));
    
	return mul(transpose(float3x3(t, cross(n, t), n)), tangentNormal);
}


fixed2 Specular(fixed3 wP, fixed3 n, fixed multi, fixed roughness)
{
    #if SPEC_OFF
    return 0;
    #endif
    
    fixed3 dir    = normalize(wP -_WorldSpaceCameraPos);
    fixed d = dot(n, -dir);
    fixed specdot = saturate(d * .5 + .5);
    roughness *= specdot * .5 + .5;
    
    fixed po = 190.0 - roughness * 189;
    fixed a = pow(saturate((1.0 - pow(d, 7 - roughness * 6))), po) * (1.0 - roughness) * .25;
    fixed b = pow(specdot, po);
    fixed spec    = max(a, b) * (1 - roughness * .5);
    
    return fixed2(spec * .6 * multi, specdot);
}

sampler2D NoiseTex;
float3 ScreenTex(float2 coords) 
{
    return tex2D(NoiseTex, coords).xyz;
}


fixed animnoise(fixed v, fixed speed)
{   
    return pow(abs(sin(v * PI * 2 + _Time.y * speed * 1.3)), 1.5) * .025 - .015;
}