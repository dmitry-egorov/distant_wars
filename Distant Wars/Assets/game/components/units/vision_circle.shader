Shader "DW/Vision Circle"
{
    Properties
    {
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off
        Blend One One
        BlendOp Max

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            float sstep(float min, float max, float width) { return smoothstep(min - width, min + width, max); }

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _Tex;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(float3(v.vertex.xy, 0));
                //uv from index
                o.uv = float2(2 * frac(v.vertex.z / 2) - 0.5, floor(v.vertex.z / 2) - 0.5);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float x = i.uv.x;
                float y = i.uv.y;
                float w = fwidth(x) * 2;
                float v = sstep(x*x + y*y, 0.5*0.5, w);
                if (v == 0) discard;
                return float4(v,v,v,1);
            }
            ENDCG
        }
    }
}