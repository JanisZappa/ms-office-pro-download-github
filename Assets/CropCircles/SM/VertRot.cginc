/*float PI = 3.1415926535897932384626433832795;

float4 setAxisAngle (float3 axis, float rad) {
  rad = rad * 0.5;
  float s = sin(rad);
  return float4(s * axis[0], s * axis[1], s * axis[2], cos(rad));
}

float3 xUnitVec3 = float3(1.0, 0.0, 0.0);
float3 yUnitVec3 = float3(0.0, 1.0, 0.0);

float4 rotationTo (float3 a, float3 b) 
{
    float vecDot = dot(a, b);
    float3 tmpvec3 = float3(0, 0, 0);
    if (vecDot < -0.999999) 
    {
        ///return float4(0,0,0,1);
    
        tmpvec3 = cross(xUnitVec3, a);
        if (length(tmpvec3) < 0.000001) 
        {   
            tmpvec3 = cross(yUnitVec3, a);
        }
        tmpvec3 = normalize(tmpvec3);
        return setAxisAngle(tmpvec3, PI);
    }
    else 
        if (vecDot > 0.999999) 
        {
            return float4(0,0,0,1);
        } 
        else 
        {
            tmpvec3 = cross(a, b);
            float4 _out = float4(tmpvec3[0], tmpvec3[1], tmpvec3[2], 1.0 + vecDot);
            return normalize(_out);
        }
}*/


float4 rotation (float3 b) 
{
    float4 _out = normalize(float4(cross(float3(0, 1, 0), b).xyz, 1.0 + b.y));
    return lerp(_out, float4(0, 0, 0, 1), step(0.999999, b.y));
}


float4 multQuat(float4 q1, float4 q2) 
{
    return float4(
        q1.w * q2.x + q1.x * q2.w + q1.z * q2.y - q1.y * q2.z,
        q1.w * q2.y + q1.y * q2.w + q1.x * q2.z - q1.z * q2.x,
        q1.w * q2.z + q1.z * q2.w + q1.y * q2.x - q1.x * q2.y,
        q1.w * q2.w - q1.x * q2.x - q1.y * q2.y - q1.z * q2.z);
}

float3 rotateVector( float4 quat, float3 vec ) 
{
    return multQuat(multQuat(quat, float4(vec, 0.0)), float4(-quat.x, -quat.y, -quat.z, quat.w)).xyz;
}
            