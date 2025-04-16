Shader "Shader Graphs/HealthBarStandart"
{
    Properties
    {
        Health("Health", Float) = 0.55
        HealthDifference("HealthDifference", Float) = 0.62
        _HealthDifferenceColor("HealthDifferenceColor", Color) = (1, 1, 1, 0)
        _BackgroundColor("BackgroundColor", Color) = (0.4528302, 0.441723, 0.441723, 0.3921569)
        [NoScaleOffset]_HealthBarTexture("HealthBarTexture", 2D) = "white" {}
        [NoScaleOffset]_BackgroundTexture("BackgroundTexture", 2D) = "black" {}
        _ColorBrightness("ColorBrightness", Float) = 1.2
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Transparent"
            "DisableBatching"="False"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        #define _SURFACE_TYPE_TRANSPARENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 positionWS : INTERP1;
             float3 normalWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.positionWS.xyz = input.positionWS;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.positionWS = input.positionWS.xyz;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        float4 _BackgroundColor;
        float4 _HealthDifferenceColor;
        float4 _HealthBarTexture_TexelSize;
        float4 _BackgroundTexture_TexelSize;
        float _ColorBrightness;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, Health)
            UNITY_DEFINE_INSTANCED_PROP(float, HealthDifference)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        static Gradient _HealthGradient = {0,3,2,{float4(0.1698113,0.0006407847,0.0006407847,0.05587854),float4(1,0,0,0.2235294),float4(0.04143238,1,0,1),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0)},{float2(1,0),float2(1,1),float2(0,0),float2(0,0),float2(0,0),float2(0,0),float2(0,0),float2(0,0)}};
        
        TEXTURE2D(_HealthBarTexture);
        SAMPLER(sampler_HealthBarTexture);
        TEXTURE2D(_BackgroundTexture);
        SAMPLER(sampler_BackgroundTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Subtract_float(float A, float B, out float Out)
        {
            Out = A - B;
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Rectangle_Fastest_float(float2 UV, float Width, float Height, out float Out)
        {
            float2 d = abs(UV * 2 - 1) - float2(Width, Height);
        #if defined(SHADER_STAGE_RAY_TRACING)
            d = saturate((1 - saturate(d * 1e7)));
        #else
            d = saturate(1 - d / fwidth(d));
        #endif
            Out = min(d.x, d.y);
        }
        
        void Unity_SampleGradientV1_float(Gradient Gradient, float Time, out float4 Out)
        {
            float3 color = Gradient.colors[0].rgb;
            [unroll]
            for (int c = 1; c < Gradient.colorsLength; c++)
            {
                float colorPos = saturate((Time - Gradient.colors[c - 1].w) / (Gradient.colors[c].w - Gradient.colors[c - 1].w)) * step(c, Gradient.colorsLength - 1);
                color = lerp(color, Gradient.colors[c].rgb, lerp(colorPos, step(0.01, colorPos), Gradient.type));
            }
        #ifdef UNITY_COLORSPACE_GAMMA
            color = LinearToSRGB(color);
        #endif
            float alpha = Gradient.alphas[0].x;
            [unroll]
            for (int a = 1; a < Gradient.alphasLength; a++)
            {
                float alphaPos = saturate((Time - Gradient.alphas[a - 1].y) / (Gradient.alphas[a].y - Gradient.alphas[a - 1].y)) * step(a, Gradient.alphasLength - 1);
                alpha = lerp(alpha, Gradient.alphas[a].x, lerp(alphaPos, step(0.01, alphaPos), Gradient.type));
            }
            Out = float4(color, alpha);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Add_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A + B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_BackgroundTexture);
            float4 _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.tex, _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.samplerstate, _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_R_4_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.r;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_G_5_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.g;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_B_6_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.b;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_A_7_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.a;
            float4 _Property_c9f618c813384349b297c4e328674388_Out_0_Vector4 = _BackgroundColor;
            float4 _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4, _Property_c9f618c813384349b297c4e328674388_Out_0_Vector4, _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4);
            UnityTexture2D _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HealthBarTexture);
            float4 _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.tex, _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.samplerstate, _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_R_4_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.r;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_G_5_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.g;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_B_6_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.b;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_A_7_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.a;
            float _Property_5505476f6e7947e8a4beb56784029d73_Out_0_Float = UNITY_ACCESS_INSTANCED_PROP(Props, Health);
            float _Subtract_1043e369eb3140518050cbd7023456b0_Out_2_Float;
            Unity_Subtract_float(1, _Property_5505476f6e7947e8a4beb56784029d73_Out_0_Float, _Subtract_1043e369eb3140518050cbd7023456b0_Out_2_Float);
            float _Multiply_83be7159f1bd4bc6a2e0bb9b336513bc_Out_2_Float;
            Unity_Multiply_float_float(_Subtract_1043e369eb3140518050cbd7023456b0_Out_2_Float, 0.5, _Multiply_83be7159f1bd4bc6a2e0bb9b336513bc_Out_2_Float);
            float2 _Vector2_8268ab8731bd44bbb8156bb71a9d6ff3_Out_0_Vector2 = float2(_Multiply_83be7159f1bd4bc6a2e0bb9b336513bc_Out_2_Float, 0);
            float2 _TilingAndOffset_24fdffaf3c4a49f6a137b549abe2f695_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_8268ab8731bd44bbb8156bb71a9d6ff3_Out_0_Vector2, _TilingAndOffset_24fdffaf3c4a49f6a137b549abe2f695_Out_3_Vector2);
            float _Rectangle_2c855115457342daa7fc07ffc511fd8f_Out_3_Float;
            Unity_Rectangle_Fastest_float(_TilingAndOffset_24fdffaf3c4a49f6a137b549abe2f695_Out_3_Vector2, _Property_5505476f6e7947e8a4beb56784029d73_Out_0_Float, 1, _Rectangle_2c855115457342daa7fc07ffc511fd8f_Out_3_Float);
            Gradient _Property_e559015eb63a4b1fa3b298a60b06302a_Out_0_Gradient = _HealthGradient;
            float4 _SampleGradient_8bf58facbbdd46dc9c568969e9d04779_Out_2_Vector4;
            Unity_SampleGradientV1_float(_Property_e559015eb63a4b1fa3b298a60b06302a_Out_0_Gradient, _Property_5505476f6e7947e8a4beb56784029d73_Out_0_Float, _SampleGradient_8bf58facbbdd46dc9c568969e9d04779_Out_2_Vector4);
            float4 _Multiply_ffb206b027df4457ae9cfbfe16118bd1_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Rectangle_2c855115457342daa7fc07ffc511fd8f_Out_3_Float.xxxx), _SampleGradient_8bf58facbbdd46dc9c568969e9d04779_Out_2_Vector4, _Multiply_ffb206b027df4457ae9cfbfe16118bd1_Out_2_Vector4);
            float _OneMinus_68a85dd202e041dfb7f744ca72c203ac_Out_1_Float;
            Unity_OneMinus_float(_Rectangle_2c855115457342daa7fc07ffc511fd8f_Out_3_Float, _OneMinus_68a85dd202e041dfb7f744ca72c203ac_Out_1_Float);
            float _Property_5d6ae62ddd414db3ad55ef89ce61cddd_Out_0_Float = UNITY_ACCESS_INSTANCED_PROP(Props, HealthDifference);
            float _Subtract_11305cd290e94d04b9f625f29de0d042_Out_2_Float;
            Unity_Subtract_float(1, _Property_5d6ae62ddd414db3ad55ef89ce61cddd_Out_0_Float, _Subtract_11305cd290e94d04b9f625f29de0d042_Out_2_Float);
            float _Multiply_064aecf9cb1443c5afe4a82a42624770_Out_2_Float;
            Unity_Multiply_float_float(_Subtract_11305cd290e94d04b9f625f29de0d042_Out_2_Float, 0.5, _Multiply_064aecf9cb1443c5afe4a82a42624770_Out_2_Float);
            float2 _Vector2_b8c7ff64346a49bf8fd94987ea9819d2_Out_0_Vector2 = float2(_Multiply_064aecf9cb1443c5afe4a82a42624770_Out_2_Float, 0);
            float2 _TilingAndOffset_9599ab50a03048418f6f6af27d3df972_Out_3_Vector2;
            Unity_TilingAndOffset_float(IN.uv0.xy, float2 (1, 1), _Vector2_b8c7ff64346a49bf8fd94987ea9819d2_Out_0_Vector2, _TilingAndOffset_9599ab50a03048418f6f6af27d3df972_Out_3_Vector2);
            float _Rectangle_bee6a96b38f44f8588d1aaaa7d2f52c8_Out_3_Float;
            Unity_Rectangle_Fastest_float(_TilingAndOffset_9599ab50a03048418f6f6af27d3df972_Out_3_Vector2, _Property_5d6ae62ddd414db3ad55ef89ce61cddd_Out_0_Float, 1, _Rectangle_bee6a96b38f44f8588d1aaaa7d2f52c8_Out_3_Float);
            float _Multiply_cb3e8df24bd84f9596a80db4f3b2aa5f_Out_2_Float;
            Unity_Multiply_float_float(_OneMinus_68a85dd202e041dfb7f744ca72c203ac_Out_1_Float, _Rectangle_bee6a96b38f44f8588d1aaaa7d2f52c8_Out_3_Float, _Multiply_cb3e8df24bd84f9596a80db4f3b2aa5f_Out_2_Float);
            float4 _Property_3c307a4c71d64cd4953b614171f807a1_Out_0_Vector4 = _HealthDifferenceColor;
            float4 _Multiply_1f675a3283b14a308724b93ccc974a57_Out_2_Vector4;
            Unity_Multiply_float4_float4((_Multiply_cb3e8df24bd84f9596a80db4f3b2aa5f_Out_2_Float.xxxx), _Property_3c307a4c71d64cd4953b614171f807a1_Out_0_Vector4, _Multiply_1f675a3283b14a308724b93ccc974a57_Out_2_Vector4);
            float4 _Add_76e64abd44704754b85caa2e6e74a07e_Out_2_Vector4;
            Unity_Add_float4(_Multiply_ffb206b027df4457ae9cfbfe16118bd1_Out_2_Vector4, _Multiply_1f675a3283b14a308724b93ccc974a57_Out_2_Vector4, _Add_76e64abd44704754b85caa2e6e74a07e_Out_2_Vector4);
            float _Property_2f48ef2e44774035b304cd9dc28acd6f_Out_0_Float = _ColorBrightness;
            float4 _Multiply_3828a38525e240488b95d3da1284a379_Out_2_Vector4;
            Unity_Multiply_float4_float4(_Add_76e64abd44704754b85caa2e6e74a07e_Out_2_Vector4, (_Property_2f48ef2e44774035b304cd9dc28acd6f_Out_0_Float.xxxx), _Multiply_3828a38525e240488b95d3da1284a379_Out_2_Vector4);
            float _OneMinus_f8c140c0fd8840488119025b59025215_Out_1_Float;
            Unity_OneMinus_float(_Rectangle_bee6a96b38f44f8588d1aaaa7d2f52c8_Out_3_Float, _OneMinus_f8c140c0fd8840488119025b59025215_Out_1_Float);
            float4 _Property_9852685213b949c68a5b12d383b53f12_Out_0_Vector4 = _BackgroundColor;
            float4 _Multiply_cd5febdfbf54400489bbfee8108bf632_Out_2_Vector4;
            Unity_Multiply_float4_float4((_OneMinus_f8c140c0fd8840488119025b59025215_Out_1_Float.xxxx), _Property_9852685213b949c68a5b12d383b53f12_Out_0_Vector4, _Multiply_cd5febdfbf54400489bbfee8108bf632_Out_2_Vector4);
            float4 _Add_f8f0cdad177a49e6bee142e317393c11_Out_2_Vector4;
            Unity_Add_float4(_Multiply_3828a38525e240488b95d3da1284a379_Out_2_Vector4, _Multiply_cd5febdfbf54400489bbfee8108bf632_Out_2_Vector4, _Add_f8f0cdad177a49e6bee142e317393c11_Out_2_Vector4);
            float4 _Multiply_b69debfa3c4f478f8e7ad2d2a9d22704_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4, _Add_f8f0cdad177a49e6bee142e317393c11_Out_2_Vector4, _Multiply_b69debfa3c4f478f8e7ad2d2a9d22704_Out_2_Vector4);
            float4 _Add_a863ed902b7c48b2ba82019b112f0222_Out_2_Vector4;
            Unity_Add_float4(_Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4, _Multiply_b69debfa3c4f478f8e7ad2d2a9d22704_Out_2_Vector4, _Add_a863ed902b7c48b2ba82019b112f0222_Out_2_Vector4);
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_R_1_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[0];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_G_2_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[1];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_B_3_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[2];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_A_4_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[3];
            float _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float;
            Unity_Add_float(_Split_5d80f8b6dda349c39c688d6d542ce2a4_A_4_Float, _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_A_7_Float, _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float);
            surface.BaseColor = (_Add_a863ed902b7c48b2ba82019b112f0222_Out_2_Vector4.xyz);
            surface.Alpha = _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Off
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma multi_compile_instancing
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        #pragma multi_compile_fragment _ _GBUFFER_NORMALS_OCT
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        #define _SURFACE_TYPE_TRANSPARENT 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/RenderingLayers.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float3 normalWS : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.normalWS.xyz = input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.normalWS = input.normalWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        float4 _BackgroundColor;
        float4 _HealthDifferenceColor;
        float4 _HealthBarTexture_TexelSize;
        float4 _BackgroundTexture_TexelSize;
        float _ColorBrightness;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, Health)
            UNITY_DEFINE_INSTANCED_PROP(float, HealthDifference)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        static Gradient _HealthGradient = {0,3,2,{float4(0.1698113,0.0006407847,0.0006407847,0.05587854),float4(1,0,0,0.2235294),float4(0.04143238,1,0,1),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0)},{float2(1,0),float2(1,1),float2(0,0),float2(0,0),float2(0,0),float2(0,0),float2(0,0),float2(0,0)}};
        
        TEXTURE2D(_HealthBarTexture);
        SAMPLER(sampler_HealthBarTexture);
        TEXTURE2D(_BackgroundTexture);
        SAMPLER(sampler_BackgroundTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_BackgroundTexture);
            float4 _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.tex, _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.samplerstate, _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_R_4_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.r;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_G_5_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.g;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_B_6_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.b;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_A_7_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.a;
            float4 _Property_c9f618c813384349b297c4e328674388_Out_0_Vector4 = _BackgroundColor;
            float4 _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4, _Property_c9f618c813384349b297c4e328674388_Out_0_Vector4, _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4);
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_R_1_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[0];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_G_2_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[1];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_B_3_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[2];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_A_4_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[3];
            UnityTexture2D _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HealthBarTexture);
            float4 _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.tex, _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.samplerstate, _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_R_4_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.r;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_G_5_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.g;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_B_6_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.b;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_A_7_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.a;
            float _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float;
            Unity_Add_float(_Split_5d80f8b6dda349c39c688d6d542ce2a4_A_4_Float, _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_A_7_Float, _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float);
            surface.Alpha = _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        float4 _BackgroundColor;
        float4 _HealthDifferenceColor;
        float4 _HealthBarTexture_TexelSize;
        float4 _BackgroundTexture_TexelSize;
        float _ColorBrightness;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, Health)
            UNITY_DEFINE_INSTANCED_PROP(float, HealthDifference)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        static Gradient _HealthGradient = {0,3,2,{float4(0.1698113,0.0006407847,0.0006407847,0.05587854),float4(1,0,0,0.2235294),float4(0.04143238,1,0,1),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0)},{float2(1,0),float2(1,1),float2(0,0),float2(0,0),float2(0,0),float2(0,0),float2(0,0),float2(0,0)}};
        
        TEXTURE2D(_HealthBarTexture);
        SAMPLER(sampler_HealthBarTexture);
        TEXTURE2D(_BackgroundTexture);
        SAMPLER(sampler_BackgroundTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_BackgroundTexture);
            float4 _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.tex, _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.samplerstate, _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_R_4_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.r;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_G_5_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.g;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_B_6_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.b;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_A_7_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.a;
            float4 _Property_c9f618c813384349b297c4e328674388_Out_0_Vector4 = _BackgroundColor;
            float4 _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4, _Property_c9f618c813384349b297c4e328674388_Out_0_Vector4, _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4);
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_R_1_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[0];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_G_2_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[1];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_B_3_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[2];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_A_4_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[3];
            UnityTexture2D _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HealthBarTexture);
            float4 _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.tex, _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.samplerstate, _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_R_4_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.r;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_G_5_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.g;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_B_6_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.b;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_A_7_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.a;
            float _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float;
            Unity_Add_float(_Split_5d80f8b6dda349c39c688d6d542ce2a4_A_4_Float, _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_A_7_Float, _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float);
            surface.Alpha = _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma vertex vert
        #pragma fragment frag
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define VARYINGS_NEED_TEXCOORD0
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        float4 _BackgroundColor;
        float4 _HealthDifferenceColor;
        float4 _HealthBarTexture_TexelSize;
        float4 _BackgroundTexture_TexelSize;
        float _ColorBrightness;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            UNITY_DEFINE_INSTANCED_PROP(float, Health)
            UNITY_DEFINE_INSTANCED_PROP(float, HealthDifference)
        UNITY_INSTANCING_BUFFER_END(Props)
        
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        static Gradient _HealthGradient = {0,3,2,{float4(0.1698113,0.0006407847,0.0006407847,0.05587854),float4(1,0,0,0.2235294),float4(0.04143238,1,0,1),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0),float4(0,0,0,0)},{float2(1,0),float2(1,1),float2(0,0),float2(0,0),float2(0,0),float2(0,0),float2(0,0),float2(0,0)}};
        
        TEXTURE2D(_HealthBarTexture);
        SAMPLER(sampler_HealthBarTexture);
        TEXTURE2D(_BackgroundTexture);
        SAMPLER(sampler_BackgroundTexture);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Multiply_float4_float4(float4 A, float4 B, out float4 Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            description.Position = IN.ObjectSpacePosition;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float Alpha;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_BackgroundTexture);
            float4 _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.tex, _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.samplerstate, _Property_7dda419f8e9940ed84509c572b1af571_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_R_4_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.r;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_G_5_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.g;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_B_6_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.b;
            float _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_A_7_Float = _SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4.a;
            float4 _Property_c9f618c813384349b297c4e328674388_Out_0_Vector4 = _BackgroundColor;
            float4 _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4;
            Unity_Multiply_float4_float4(_SampleTexture2D_fcfa1923566046398e1a7b700ea0205d_RGBA_0_Vector4, _Property_c9f618c813384349b297c4e328674388_Out_0_Vector4, _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4);
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_R_1_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[0];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_G_2_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[1];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_B_3_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[2];
            float _Split_5d80f8b6dda349c39c688d6d542ce2a4_A_4_Float = _Multiply_0cdb46a107ce4c3295778a136b6beef1_Out_2_Vector4[3];
            UnityTexture2D _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_HealthBarTexture);
            float4 _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.tex, _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.samplerstate, _Property_48f88386916c4f5cb625380914b65de8_Out_0_Texture2D.GetTransformedUV(IN.uv0.xy) );
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_R_4_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.r;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_G_5_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.g;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_B_6_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.b;
            float _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_A_7_Float = _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_RGBA_0_Vector4.a;
            float _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float;
            Unity_Add_float(_Split_5d80f8b6dda349c39c688d6d542ce2a4_A_4_Float, _SampleTexture2D_c25303dc8e3746a9ba4c663cfdb4ad70_A_7_Float, _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float);
            surface.Alpha = _Add_e4dbc8bbc3c749b8b5427d495a134530_Out_2_Float;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
        #if VFX_USE_GRAPH_VALUES
            uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
            /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
        #endif
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        
            #if UNITY_UV_STARTS_AT_TOP
            #else
            #endif
        
        
            output.uv0 = input.texCoord0;
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    FallBack "Hidden/Shader Graph/FallbackError"
}