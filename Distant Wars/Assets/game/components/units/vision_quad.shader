Shader "DW/Vision Quad"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Assets/Plugins/shader_common/shader_common.cginc"

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
                float2 p = v.vertex.xy + _GridCellSize * float2(2 * frac(i / 2) - 0.5, -floor(i / 2) + 0.5);
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