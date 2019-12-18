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
            #include "Assets/Plugins/shader_common/shader_common.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 wp : TEXCOORD1;
            };

            float4 _Color;

            // Globals
            sampler2D _DiscoveryTex;
            float4 _DiscoveryTex_ST;
            float4 _DiscoveryTex_TexelSize;
            float  _BulletsSize;
            float  _MapScale;

            v2f vert (appdata d)
            {
                v2f o;
                float2 v = d.vertex.xy;
                float  i = d.vertex.z;
                
                o.uv = float2(2.0 * frac(i / 2.0) - 0.5, -floor(i / 2.0) + 0.5);

                float2 sc = _ScreenParams.xy;
                float  s  = _BulletsSize;
                float4 ov = UnityObjectToClipPos(float3(v.x, v.y, 0.0));
                o.vertex = float4((floor(ov.xy * sc) + s * o.uv) / sc, ov.zw);

                o.wp = mul(unity_ObjectToWorld, d.vertex).xy / _MapScale + float2(0.5, 0.5);
                
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
