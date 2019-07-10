Shader "DW/Selection Box"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 0.2)
        _BorderColor("Border Color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _Color;
            float4 _BorderColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float  x = i.uv.x;
                float  y = i.uv.y;
                float dx = abs(ddx(x)) * 2;
                float dy = abs(ddy(y)) * 2;
                float  b = step(x, dx) + step(1 - x, dx) + step(y, dy) + step(1 - y, dy);
                return lerp(_Color, _BorderColor, b);
            }
            ENDCG
        }
    }
}
