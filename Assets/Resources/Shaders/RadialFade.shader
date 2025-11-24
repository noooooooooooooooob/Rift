Shader "Custom/RadialFade"
{
    Properties
    {
        _Color   ("Color", Color) = (0,0,0,1)  // 검은색
        _Center  ("Center", Vector) = (0.5, 0.5, 0, 0) // 화면 중앙
        _Radius  ("Radius", Float) = 0.0
        _Softness("Softness", Float) = 0.02
        _MainTex ("MainTex", 2D) = "white" {}
    }

    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            float4 _Color;
            float4 _Center;
            float  _Radius;
            float  _Softness;
            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                // 화면 기준 UV (0~1) – UI Image 기준이니까 그냥 i.uv 써도 됨
                float2 uv = i.uv;

                // 거리 계산
                float2 center = _Center.xy;
                float dist = distance(uv, center);

                // soft edge
                float r  = _Radius;
                float s  = max(_Softness, 1e-5);
                // dist <= r - s => alpha 0 (투명)
                // dist >= r     => alpha 1 (완전 검정)
                float alpha = smoothstep(r - s, r, dist);

                float4 col = _Color;
                col.a *= alpha;   // 중심은 투명, 바깥은 검정

                return col;
            }
            ENDHLSL
        }
    }
}
