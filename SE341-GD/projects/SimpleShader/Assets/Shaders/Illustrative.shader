Shader "Unlit/NewUnlitShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump"{}
        _Metallicness("Metallicness",Range(0,1)) = 0
        _Glossiness("Smoothness",Range(0,1)) = 1
        
        // use a color to present ambient color
        _AmbientCube("Ambient Cube", Color) = (1, 1, 1, 1)
        _Alpha("Alpha", float) = 3
        _Beta("Beta", float) = 0.3
        _Gamma("Gamma", float) = 1

        _Fresnel("Fresnel", Range(0, 1)) = 1
        _Kspec("Kspec", float) = 4
        _Krim("Krim", float) = 4
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"////dotclamped
            #include "AutoLight.cginc"
            #include "Lighting.cginc"

            struct VertexData
            {
                float4 position : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct FragmentData
            {
                float3 worldPos : TEXCOORD0;
                // these three vectors will hold a 3x3 rotation matrix
                // that transforms from tangent to world space
                half3 tspace0 : TEXCOORD1; // tangent.x, bitangent.x, normal.x
                half3 tspace1 : TEXCOORD2; // tangent.y, bitangent.y, normal.y
                half3 tspace2 : TEXCOORD3; // tangent.z, bitangent.z, normal.z
                // texture coordinate for the normal map
                float2 uv : TEXCOORD4;
                float4 position : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _NormalMap;
            float4 _NormalMap_ST;
            float _Glossiness;
            float _Metallicness;

            float4 _AmbientCube;
            float _Alpha, _Beta, _Gamma;
            float _Fresnel, _Krim, _Kspec;

            // use liner diffuer warping function
            float3 diffuse_warp(float w)
            {
                float i = 0;
                if (w < 0.3) {
                    i = 0.5 * w;
                }else if (w >= 0.3 && w < 0.7) {
                    i = 1.5 * w - 0.3;
                }else{
                    i = 0.5 * w + 0.4;
                }
                return float3(i, i, i);
            }

            float3 view_independent_lighting(float3 ambientCube, float alpha, float beta, float gamma, float3 NdotL, float3 lightColor, float3 albedo)
            {
                float3 light;

                // left part of equation1
                light = ambientCube;

                // right part of equation2
                light += lightColor * diffuse_warp(pow(alpha * NdotL + beta, gamma)).rgb;

                return (albedo * light).rgb;
            }

            float3 view_dependent_lighting(float3 lightColor, float fresnel, float VdotR, float kspec, float NdotV, float krim, float NdotU, float3 ambientCube)
            {
                float fr = pow(1 - NdotV, 4);
                float3 phone = lightColor * max(fresnel * pow(VdotR, kspec), fr * pow(VdotR, krim));
                float3 dedicated = NdotU * fr *ambientCube;

                return (phone + dedicated).rgb;
            }

            FragmentData vert (VertexData v)
            {
                FragmentData o;
                o.position = UnityObjectToClipPos(v.position);
                o.worldPos = mul(unity_ObjectToWorld, v.position).xyz;
                half3 wNormal = UnityObjectToWorldNormal(v.normal);
                half3 wTangent = UnityObjectToWorldDir(v.tangent.xyz);
                // compute bitangent from cross product of normal and tangent
                half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent = cross(wNormal, wTangent) * tangentSign;
                // output the tangent space matrix
                o.tspace0 = half3(wTangent.x, wBitangent.x, wNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, wNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, wNormal.z);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (FragmentData i) : SV_Target
            {
                float4 mainTex = tex2D(_MainTex, i.uv);
                float4 normalTex = tex2D(_NormalMap, i.uv);

                float3 L = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.worldPos.xyz, _WorldSpaceLightPos0.w));
                float3 V = normalize(_WorldSpaceCameraPos.xyz - i.worldPos.xyz);
                float3 H = Unity_SafeNormalize(L + V);
                float3 U = float3(0, 1, 0);

                float3 tnormal = UnpackNormal(normalTex);

                float3 N;
                N.x = dot(i.tspace0, tnormal);
                N.y = dot(i.tspace1, tnormal);
                N.z = dot(i.tspace2, tnormal);

                float3 R = Unity_SafeNormalize(reflect(-L, N));
                float3 VR = Unity_SafeNormalize(reflect(-V, N));

                float NdotL = saturate(dot(N, L));
                float NdotH = saturate(dot(N, H));
                float NdotV = saturate(dot(N, V));
                float VdotH = saturate(dot(V, H));
                float LdotH = saturate(dot(L, H));
                float VdotR = saturate(dot(V, R));
                float NdotU = saturate(dot(N, U));

                //float attenuation = LIGHT_ATTENUATION(i);

                //UnityIndirect gi =  GetUnityIndirect(_LightColor0.rgb, L, N, V, VR, attenuation, 1-_Glossiness, i.worldPos.xyz);
                
                half oneMinusReflectivity;
                half3 specColor;
                float3 albedo = DiffuseAndSpecularFromMetallic(mainTex.rgb, _Metallicness, specColor, oneMinusReflectivity);

                // view independent lighting
                float3 part1 = view_independent_lighting(_AmbientCube.rgb, _Alpha, _Beta, _Gamma, NdotL, _LightColor0.rgb, albedo.rgb);

                // view dependent lighting
                float3 part2 = view_dependent_lighting(specColor, _Fresnel, VdotR, _Kspec, NdotV, _Krim, NdotU, _AmbientCube);

                float4 color = float4((part1 + part2), 1);

                return color;
            }
            ENDCG
        }
    }
}
