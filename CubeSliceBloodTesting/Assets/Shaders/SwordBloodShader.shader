Shader "Custom/SwordBloodShader_MultipleBloods"
{
    Properties
    {
        _MainTex ("Base Texture", 2D) = "white" {}
        _BumpMap ("Normal Map", 2D) = "bump" {}
        _BloodTex1 ("Blood Texture 1", 2D) = "white" {}
        _BloodTex2 ("Blood Texture 2", 2D) = "white" {}
        _BloodColor ("Blood Color", Color) = (1,0,0,1) // Default red color
        _BloodTiling ("Blood Tiling", Vector) = (1,1,0,0)
        _BlendFactor ("Blend Factor", Range(0, 1)) = 0.5 // How much to blend between blood textures
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        sampler2D _BumpMap;
        sampler2D _BloodTex1; // First blood texture
        sampler2D _BloodTex2; // Second blood texture
        fixed4 _BloodColor;
        float4 _BloodTiling;
        float _BlendFactor;  // Blend factor between the two blood textures

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_BumpMap;
            float2 uv_BloodTex1;
            float2 uv_BloodTex2;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Sample base texture
            fixed4 baseTex = tex2D(_MainTex, IN.uv_MainTex);
            o.Albedo = baseTex.rgb;

            // Apply normal map
            o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));

            // Sample and colorize the first blood texture
            float2 bloodUV1 = IN.uv_BloodTex1 * _BloodTiling.xy;
            fixed4 blood1 = tex2D(_BloodTex1, bloodUV1) * _BloodColor;

            // Sample and colorize the second blood texture
            float2 bloodUV2 = IN.uv_BloodTex2 * _BloodTiling.xy;
            fixed4 blood2 = tex2D(_BloodTex2, bloodUV2) * _BloodColor;

            // Blend the two blood textures together using the BlendFactor
            fixed4 blendedBlood = lerp(blood1, blood2, _BlendFactor);

            // Blend the final blood texture with the base texture based on the blood alpha
            o.Albedo = lerp(o.Albedo, blendedBlood.rgb, blendedBlood.a); // Blend blood with base texture based on alpha
        }
        ENDCG
    }

    FallBack "Diffuse"
}
