Shader "Shader Graphs/RedDotReticle_Transparent"
{
    Properties
    {
        [HDR]_Emission("Emission", Color) = (0, 0, 0, 0)
        _Color("Color", Color) = (1, 0, 0, 0)
        [NoScaleOffset]_Texture2D("Texture2D", 2D) = "white" {}
        [IntRange] _StencilID ("Stencil ID", Range(0,255)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            Stencil
            {
                Ref [_StencilID]
                Comp  Less
                Pass Replace
                Fail Keep
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM

            // Pragmas
            #pragma vertex vert
            #pragma fragment frag

            // Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Packing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/NormalSurfaceGradient.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/UnityInstancing.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/EntityLighting.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariables.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/ShaderVariablesFunctions.hlsl"
            #include "Packages/com.unity.shadergraph/ShaderGraphLibrary/Functions.hlsl"

            // Structs and Packing
            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 uv0 : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 texCoord0 : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct SurfaceDescriptionInputs
            {
                float4 uv0 : TEXCOORD0;
            };

            struct VertexDescriptionInputs
            {
            };

            struct SurfaceDescription
            {
                float4 Out : SV_Target;
            };

            // Graph properties
            CBUFFER_START(UnityPerMaterial)
                float4 _Emission;
                float4 _Color;
                float4 _Texture2D_TexelSize;
            CBUFFER_END

            // Object and Global properties
            SAMPLER(sampler_Texture2D);
            TEXTURE2D(_Texture2D);

            // Graph Pixel
            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface;
                float4 _Property_e3d0786ce7e14497bf00e6e41f2820d0_Out_0_Vector4 = _Color;
                UnityTexture2D _Property_ece1feb431484f4e96324f7a9e975601_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_Texture2D);
                float4 _SampleTexture2D_a6a98230b9fc481bbf335ceaf99ab531_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_ece1feb431484f4e96324f7a9e975601_Out_0_Texture2D.tex, _Property_ece1feb431484f4e96324f7a9e975601_Out_0_Texture2D.samplerstate, _Property_ece1feb431484f4e96324f7a9e975601_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy));
                float4 _Multiply_d9907debf1ea4cf38362669b8945a425_Out_2_Vector4 = _Property_e3d0786ce7e14497bf00e6e41f2820d0_Out_0_Vector4 * _SampleTexture2D_a6a98230b9fc481bbf335ceaf99ab531_RGBA_0_Vector4;
                surface.Out = all(isfinite(_Multiply_d9907debf1ea4cf38362669b8945a425_Out_2_Vector4)) ? half4(_Multiply_d9907debf1ea4cf38362669b8945a425_Out_2_Vector4.xyz, 1.0) : half4(1.0f, 0.0f, 1.0f, 1.0f);
                return surface;
            }

            ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}
