#include "noise.cginc"

float4 setAxisAngle (float3 axis, float rad) 
{
    rad = rad * 0.5;
    float s = sin(rad);
    return float4(s * axis[0], s * axis[1], s * axis[2], cos(rad));
}

            
float4 multQuat(float4 q1, float4 q2) 
{
    return float4(
    q1.w * q2.x + q1.x * q2.w + q1.z * q2.y - q1.y * q2.z,
    q1.w * q2.y + q1.y * q2.w + q1.x * q2.z - q1.z * q2.x,
    q1.w * q2.z + q1.z * q2.w + q1.y * q2.x - q1.x * q2.y,
    q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z
    );
}
            
            
float3 rotateVector( float4 quat, float3 vec ) 
{
    // https://twistedpairdevelopment.wordpress.com/2013/02/11/rotating-a-vector-by-a-quaternion-in-glsl/
    float4 qv = multQuat( quat, float4(vec, 0.0) );
    return multQuat( qv, float4(-quat.x, -quat.y, -quat.z, quat.w) ).xyz;
}
            
            
float3 noiseDir(float3 p)
{
    return float3(snoise(p), snoise(p + float3(-100, 10, 10000)), snoise(p + float3(100, 4532, -543)));
}