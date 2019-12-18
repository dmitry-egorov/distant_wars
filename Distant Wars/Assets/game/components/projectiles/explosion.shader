Shader "DW/Explosion"
{
    Properties
    {
        _PulsePower ("Pulse Power", Range(0, 10)) = 1
        _Color ("Color", Color) = (1, 0, 0, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        ZTest Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

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
                float2 uv: TEXCOORD0;
                float time: TEXCOORD1;
            };


            float _PulsePower;
            float4 _Color;

            v2f vert (appdata v)
            {
                v2f o;

                float z = v.vertex.z;
                float j = floor(z); // index
                float t = frac(z);  // time
                
                o.vertex = UnityObjectToClipPos(float3(v.vertex.xy, 0));
                o.uv = float2(2 * frac(j / 2) - 0.5, floor(j / 2) - 0.5);
                o.time = t;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = i.time * 2;

                float2 uv = 2 * i.uv;

                float r2 = sat(exp_impulse(_PulsePower, t) - cube(dot(uv, uv)));
                float4 c = float4(r2, sqr(r2), sqr(r2), r2);
                return c * float4(_Color.xyz, (1 - t));
            }
            ENDCG
        }
    }
}