#define SHADOWCOUNT 30
uniform float3 Shadows[SHADOWCOUNT];

inline float ShadowLerp(float3 pos)
{
    return 1;

    float minDist = 3.402823466e+38F;

    for (int index = 0; index < SHADOWCOUNT; index++) 
    {
        float3 shadow = Shadows[index];
        float3 dir = shadow - pos;

        float thisLightLerp = dir.x * dir.x + dir.y * dir.y + dir.z * dir.z;
        minDist = min(minDist, thisLightLerp);
    }

    return .65 + (1 - pow(1 - saturate(minDist * .27), 4)) * .35;
}
