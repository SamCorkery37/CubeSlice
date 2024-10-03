Shader "HDRP/Standard Double Sided"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo", 2D) = "white" {}
        
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        _Glossiness("Smoothness", Range(0.0, 1.0)) = 0.5
        _GlossMapScale("Smoothness Scale", Range(0.0, 1.0)) = 1.0
        [Enum(Metallic Alpha,0,Albedo Alpha,1)] _SmoothnessTextureChannel("Smoothness texture channel", Float) = 0

        [Gamma] _Metallic("Metallic", Range(0.0, 1.0)) = 0.0
        _MetallicGlossMap("Metallic", 2D) = "white" {}

        [ToggleOff] _SpecularHighlights("Specular Highlights", Float) = 1.0
        [ToggleOff] _GlossyReflections("Glossy Reflections", Float) = 1.0

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        _Parallax("Height Scale", Range(0.005, 0.08)) = 0.02
        _ParallaxMap("Height Map", 2D) = "black" {}

        _OcclusionStrength("Strength", Range(0.0, 1.0)) = 1.0
        _OcclusionMap("Occlusion", 2D) = "white" {}

        _EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}
        
        _DetailMask("Detail Mask", 2D) = "white" {}

        _DetailAlbedoMap("Detail Albedo x2", 2D) = "grey" {}
        _DetailNormalMapScale("Scale", Float) = 1.0
        _DetailNormalMap("Normal Map", 2D) = "bump" {}

        [Enum(UV0,0,UV1,1)] _UVSec("UV Set for secondary textures", Float) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "HDRenderPipeline" }
        LOD 300
        Cull Off

        HLSLPROGRAM
        #pragma target 4.5

        // Include HDRP shader libraries
        #include "Packages/com.unity.render-pipelines.high-definition/Runtime/Material/Lit/Lit.hlsl"

        // Define the necessary keywords for HDRP materials
        #pragma multi_compile _ _NORMALMAP _METALLICSPECGLOSSMAP _PARALLAXMAP _DETAIL_MULX2

        // Add any additional shader passes if needed (i.e., for shadow casting or depth pass)

        // Vertex Shader
        float4 vertBase(AttributesMesh v) : SV_POSITION
        {
            // Basic vertex logic
            return TransformObjectToHClip(v.position.xyz);
        }

        // Fragment Shader
        float4 fragBase(VaryingsMeshToPS input) : SV_Target
        {
            // HDRP fragment logic
            SurfaceData surfaceData;
            InitializeSurfaceData(input, surfaceData);

            // Load textures and apply HDRP lighting model
            float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb * _Color.rgb;
            surfaceData.baseColor = albedo;
            surfaceData.metallic = _Metallic;
            surfaceData.smoothness = _Glossiness;
            
            // Handle normal mapping
            surfaceData.normalWS = UnpackNormalMap(SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, input.uv));

            // Emission and other surface properties
            surfaceData.emissiveColor = _EmissionColor.rgb;

            return FragHDLit(surfaceData);
        }

        ENDHLSL
    }
    
    Fallback "HDRP/Lit"
}
