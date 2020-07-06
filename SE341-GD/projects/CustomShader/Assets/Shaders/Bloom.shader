Shader "Unlit/Bloom"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump"{}
        _Metallicness("Metallicness",Range(0,1)) = 0
        _Glossiness("Smoothness",Range(0,1)) = 1

        _BlurSize("Blur Size", float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_v
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv[5] : TEXCOORD0;
                float4 position: SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            v2f vert_v(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                half2 uv = v.uv;

                o.uv[0] = uv;
                o.uv[1] = uv + float2(0.0, _MainTex_TexelSize.y*1.0) * _BlurSize;
                o.uv[2] = uv - float2(0.0, _MainTex_TexelSize.y*1.0) * _BlurSize;
                o.uv[3] = uv + float2(0.0, _MainTex_TexelSize.y*2.0) * _BlurSize;
                o.uv[4] = uv - float2(0.0, _MainTex_TexelSize.y*2.0) * _BlurSize;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float weights[3] = { 0.4026, 0.2442, 0.0545 };
                fixed4 col = tex2D(_MainTex, i.uv[0]);

                fixed3 sum = col.rgb * weights[0];

                for (int it = 1; it < 3; it++) {
                    sum += tex2D(_MainTex, i.uv[2 * it - 1]).rgb * weights[it];
                    sum += tex2D(_MainTex, i.uv[2 * it]).rgb * weights[it];
                }

                fixed4 color = fixed4(sum, 0.5);
                return color;
            }

            ENDCG
        }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert_h
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                half2 uv[5] : TEXCOORD0;
                float4 position: SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _BlurSize;

            v2f vert_h(appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.position);
                half2 uv = v.uv;

                o.uv[0] = uv;
                o.uv[1] = uv + float2(_MainTex_TexelSize.x*1.0, 0.0) * _BlurSize;
                o.uv[2] = uv - float2(_MainTex_TexelSize.x*1.0, 0.0) * _BlurSize;
                o.uv[3] = uv + float2(_MainTex_TexelSize.x*2.0, 0.0) * _BlurSize;
                o.uv[4] = uv - float2(_MainTex_TexelSize.x*2.0, 0.0) * _BlurSize;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float weights[3] = { 0.4026, 0.2442, 0.0545 };
                fixed4 col = tex2D(_MainTex, i.uv[0]);

                fixed3 sum = col.rgb * weights[0];

                for (int it = 1; it < 3; it++) {
                    sum += tex2D(_MainTex, i.uv[2 * it - 1]).rgb * weights[it];
                    sum += tex2D(_MainTex, i.uv[2 * it]).rgb * weights[it];
                }

                fixed4 color = fixed4(sum, 0.5);
                return color;
            }

            ENDCG
        }
    }
}
