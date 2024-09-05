 #define PI 3.14159265358979
 
float Fresnel(float roughness, float3 V, float3 H)
{
    return roughness + (1.0 - roughness) * pow(1.0 - max(dot(V, H), 0), 5.0);
}
 
 
float D(float roughness, float3 N, float3 H)
{
    float numerator = pow(roughness, 2.0);
    
    float NdotH = max(dot(N, H), 0.0);
    float denominator = max(PI * pow(pow(NdotH, 2.0) * (numerator - 1.0) + 1.0, 2.0), .000001);
          
    return numerator / denominator;
}


float G1(float roughness, float numerator)
{
    float k = roughness * .5;
    float denominator = max(numerator * (1.0 - k) + k, .000001);
          
    return numerator / denominator;
}
   
   
float3 PBR(float3 color, float3 wPos, float3 N, float3 L, float3 V, float3 dtl, float dP, float dPRange, float d2)
{
    float roughness = dtl.y;

    float3 H = normalize(V + L);
    
    float Kspec = Fresnel(roughness, V, H);
    float Kdiff = max(0.000001, 1.0 - Kspec);
    
    float cookTorranceNumerator   = D(roughness, N, H) * G1(roughness, dtl.x) * G1(roughness, dP) * Kspec;
    float cookTorranceDenominator = max(PI * dtl.x * dP, .000001);
    float cookTorrance            = cookTorranceNumerator / cookTorranceDenominator;
    
    return (color * Kdiff * dPRange * PI + cookTorrance * d2) * dtl.z;
}
               
