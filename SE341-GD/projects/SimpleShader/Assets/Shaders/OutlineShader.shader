Shader "Shaders/OutlineShader"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "White" {}
        _Shininess("Shininess", float) = 10

        _OutlineColor("Outline Color", Color) = (0, 0, 0, 1)
        _OutlineThickness("Outline Thickness", Range(0, 0.1)) = 0.03
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #pragma shader_feature USE_SPECULAR

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            
            struct VertexData {
                float4 position : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
            };

            struct FragmentData {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            float _Shininess;

            FragmentData MyVertexProgram(VertexData v) {
                FragmentData fd;
                fd.position = UnityObjectToClipPos(v.position);
                fd.uv = TRANSFORM_TEX(v.uv, _MainTex);
                fd.normal = UnityObjectToWorldNormal(v.normal);
                fd.worldPos = mul(unity_ObjectToWorld, v.position);
                return fd;
            }

            float4 MyFragmentProgram(FragmentData i) : SV_TARGET {
                // ambient
                fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * tex2D(_MainTex, i.uv).rgb;

                // diffuse
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 lightColor = _LightColor0.rgb;
                float3 diffuse = lightColor * DotClamped(lightDir, i.normal) * tex2D(_MainTex, i.uv).rgb;

                // specular
                float3 speclar = float3(0, 0, 0);

                #if USE_SPECULAR
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);
                float3 halfVector = normalize(lightDir + viewDir);
                speclar = lightColor * pow(DotClamped(i.normal, halfVector), _Shininess);
                #endif

                return float4(ambient + diffuse + speclar, 1);
            }
            
            ENDCG
        }

        Pass
        {
            Cull front

            CGPROGRAM
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #include "UnityCG.cginc"

            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            struct FragmentData {
                float4 position : SV_POSITION;
            };

            fixed4 _OutlineColor;
            float _OutlineThickness;

            FragmentData MyVertexProgram(VertexData v) {
                FragmentData fd;
                float3 normal = normalize(v.normal);
                float3 outlineOffset = normal * _OutlineThickness;
                float3 position = v.position + outlineOffset;

                fd.position = UnityObjectToClipPos(position);
                return fd;
            }

            float4 MyFragmentProgram(FragmentData i) : SV_TARGET {
                return _OutlineColor;
            }
            
            ENDCG
        }
    }

    CustomEditor "OutlineShaderGUI"
}
