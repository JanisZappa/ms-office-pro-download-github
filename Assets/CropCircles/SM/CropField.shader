Shader "Unlit/CropField"
{
    Properties
    {
        _Color ("Color", COLOR) = (1, 1, 1, 1)
        _Color2 ("Color2", COLOR) = (1, 1, 1, 1)
        _Noise ("Noise", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CULL BACK

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "Crop.cginc"

            v2f vert (appdata v)
            {
                return verttwosided(v, 1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return Result(i);
            }
            ENDCG
        }
        
        /*
        Tags { "RenderType"="Opaque" }
        CULL FRONT

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #include "Crop.cginc"

            v2f vert (appdata v)
            {
                return verttwosided(v, -1);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return Result(i);
            }
            ENDCG
        }
        */
    }
}
