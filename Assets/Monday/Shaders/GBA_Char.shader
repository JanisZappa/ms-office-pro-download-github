Shader "GBA/Char"
{
    Properties
    {
        _A ("A", Color) = (0, 0, 0, 0)
        _B ("B", Color) = (0, 0, 0, 0)
        _C ("C", Color) = (0, 0, 0, 0)
        _D ("D", Color) = (0, 0, 0, 0)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "GBA_Math.cginc"
            

            struct appdata
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 color : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _A, _B, _C, _D;
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = Vert(v.vertex, v.color.a);
                o.color  = v.color.x * _A + v.color.y * _B + v.color.z * _D;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
