Shader "DW/HP"
{
    Properties
    {
        _FullColor ("Full Color", Color) = (1, 0, 0, 1)
        _EmptyColor ("Empty Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Off
        ZWrite Off
        Cull Off

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
                float full_or_empty: TEXCOORD0;
            };


            float _HPBarWidthPx;
            float _HPBarHeightPx;
            float _HPBarOffsetPx;

            float4 _FullColor;
            float4 _EmptyColor;

            v2f vert (appdata v)
            {
                v2f o;

                float z = v.vertex.z;
                float j = floor(z); // index 0 - 8
                float p = frac(z);  // proportion

                float xi = 2 * frac(j / 2);
                float yi = 2 * frac(floor(j / 2) / 2);
                /* full or empty */ float foe  = floor(j / 4);

                float hpbw = _HPBarWidthPx;
                float hpbh = _HPBarHeightPx;

                float fullx = p * xi;
                float emptyx = p * (1 - xi) + xi;
                float x = fullx * (1 - foe) + emptyx * foe - 0.5;
                float y = yi - 0.5;

                float4 spos = UnityObjectToClipPos(float3(v.vertex.xy, 0));

                float sx = _ScreenParams.x;
                float sy = _ScreenParams.y;
                spos.x = (floor(spos.x * sx) + hpbw * x) / sx;
                spos.y = (floor(spos.y * sy) + hpbh * y - _HPBarOffsetPx) / sy;

                o.vertex = spos;
                o.full_or_empty = foe;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return lerp(_FullColor, _EmptyColor, i.full_or_empty);
            }
            ENDCG
        }
    }
}