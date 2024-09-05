float Squash;
#pragma shader_feature SCENE_VIEW
float4 Vert(float4 vertex, float a)
{
    vertex = UnityObjectToClipPos(vertex);
    #if SCENE_VIEW
    return vertex;
    #else
    vertex.y *= Squash;
    vertex.z += a * -.0015;
    return vertex;
    #endif
}