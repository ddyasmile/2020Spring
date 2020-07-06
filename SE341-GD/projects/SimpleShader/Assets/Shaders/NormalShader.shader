Shader "Shaders/NormalShader"
{
    Properties
    {
		_MainColor("Main Color", Color) = (1, 1, 1, 1)
    }

    SubShader
    {
        Pass
        {
            CGPROGRAM
            
            #pragma vertex MyVertexProgram
            #pragma fragment MyFragmentProgram

            #pragma shader_feature USE_NORMAL

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            
            struct VertexData {
                float4 position : POSITION;
                float3 normal : NORMAL;
            };

            struct FragmentData {
                float4 position : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            float4 _MainColor;

            FragmentData MyVertexProgram(VertexData v) {
                FragmentData fd;
                fd.position = UnityObjectToClipPos(v.position);
                fd.normal = UnityObjectToWorldNormal(v.normal);
                return fd;
            }

            float4 MyFragmentProgram(FragmentData i) : SV_TARGET {
                float4 color = _MainColor;

                #if USE_NORMAL
                color = float4(i.normal, 1);
                #endif
                return color;
            }
            
            ENDCG
        }
    }

    CustomEditor "NormalShaderGUI"
}
