Shader "DW/Bullet"
{
    Properties
    {
        _Color ("Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZWrite Off
        ZTest Off
        ZClip False

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
                float2 wp : TEXCOORD1; //world postion
            };

            sampler2D _DiscoveryTex;
            float4 _DiscoveryTex_ST;
            float4 _DiscoveryTex_TexelSize;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(float3(v.vertex.xy, 0));
                o.uv = float2(2 * frac(v.vertex.z / 2) - 0.5, floor(v.vertex.z / 2) - 0.5);
                o.wp = mul(unity_ObjectToWorld, v.vertex).xy / 8000 + float2(0.5,0.5);//TODO: use map scale
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float d = tex2D(_DiscoveryTex, i.wp);
                if (d == 0.0)
                    discard;

                float dx = i.uv.x;
                float dy = i.uv.y;
                float w = fwidth(dx);
                float v = sstep(dx*dx + dy*dy, 0.5*0.5 - w, w);
                if (v == 0) discard;

                return _Color;
            }
            ENDCG
        }
    }
}
