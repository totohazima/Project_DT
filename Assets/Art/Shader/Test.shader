Shader "Custom/Element Properties/Test"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white"{}
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };


        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float4 maintex = tex2D(_MainTex, IN.uv_MainTex);
            o.Emission = maintex.rgb;
            o.Alpha = maintex.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
