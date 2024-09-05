Shader "Unlit/CropBlock"
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
                return vertblock(v);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return ResultBlock(i);
            }
            ENDCG
        }
    }
}
