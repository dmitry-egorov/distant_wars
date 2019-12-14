Shader "DW/Vision Quad"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            sampler2D _Tex;
            float2 _GridCellSize;

            v2f vert (appdata v)
            {
                v2f o;

                float  i = v.vertex.z;
                float2 p = v.vertex.xy + _GridCellSize * float2(4 * frac(i / 2) - 1.0, -2 * floor(i / 2) + 1.0);
                o.vertex = UnityObjectToClipPos(float3(p, 0));
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return float4(1,1,1,1);
            }
            ENDCG
        }
    }
}