#include "HSV.cginc"

float4 _MainTex_TexelSize;
float3 CamForward;

float3 FogHue(float3 c, float fog, float3 dir)
{
    float viewMulti = .35 + .65 * (1.0 - pow(1 - saturate(dot(CamForward, -dir)), 3));
    float light = smoothstep(0, 1, saturate(fog * viewMulti * 17 - 12));
    light *= .5 + .5 * pow(fog, 3);
    c *= .575 + .175 * light;
    c = rgb2hsv(c);
    c.x = fmod(c.x + (1 - fog) * -.125, 1);
    c.y *= .7 + .4 * (1.0 - pow(1.0 - fog, 3));
    c.z *= fog * .95 + .05;
    return hsv2rgb(c);
}


float Grid(float2 uv)
{
    //uv.y *= 2;
    float2 boxSize  = clamp(fwidth(uv) * _MainTex_TexelSize.zw, 1e-5, 1.0);
    float2 tx       = uv * _MainTex_TexelSize.zw - .5 * boxSize; 
    float2 txOffset = saturate((frac(tx) - (1.0 - boxSize)) / boxSize);
    return -.1 * (1 - pow(saturate(1.0 - max(txOffset.x, txOffset.y)), 100));
}


float3x3 Euler3x3(float3 v)
{
    float sx, cx;
    float sy, cy;
    float sz, cz;

    sincos(v.x, sx, cx);
    sincos(v.y, sy, cy);
    sincos(v.z, sz, cz);

    float3 row1 = float3(sx*sy*sz + cy*cz, sx*sy*cz - cy*sz, cx*sy);
    float3 row3 = float3(sx*cy*sz - sy*cz, sx*cy*cz + sy*sz, cx*cy);
    float3 row2 = float3(cx*sz, cx*cz, -sx);

    return float3x3(row1, row2, row3);
}