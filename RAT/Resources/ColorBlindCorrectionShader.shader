Shader "Custom/ColorBlindCorrectionShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
            
    CGINCLUDE

    #include "UnityCG.cginc"

    struct appdata
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
    };

    struct v2f
    {
        float2 uv : TEXCOORD0;
        float4 vertex : SV_POSITION;
    };

    v2f vert(appdata v)
    {
        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.uv = v.uv;
        return o;
    }

    sampler2D _MainTex;

    half3 LMS(half3 input) {
        half3 lms = 0;
        lms[0] = (17.8824 * input.r) + (43.5161 * input.g) + (4.11935 * input.b);
        lms[1] = (3.45565 * input.r) + (27.1554 * input.g) + (3.86714 * input.b);
        lms[2] = (0.0299566 * input.r) + (0.184309 * input.g) + (1.46709 * input.b);
        return lms;
    }

    half3 Correction(float3 input, float3 dlms) {
        half3 err = 0;
        err.r = (0.0809444479 * dlms[0]) + (-0.130504409 * dlms[1]) + (0.116721066 * dlms[2]);
        err.g = (-0.0102485335 * dlms[0]) + (0.0540193266 * dlms[1]) + (-0.113614708 * dlms[2]);
        err.b = (-0.000365296938 * dlms[0]) + (-0.00412161469 * dlms[1]) + (0.693511405 * dlms[2]);
        err = (input - err);
        float3 correction = 0;
        correction.g = (err.r * 0.7) + (err.g * 1.0);
        correction.b = (err.r * 0.7) + (err.b * 1.0);
        return input + correction;
    }

    half4 FragProtanopia(v2f i) : SV_Target
    {
        half4 input = tex2D(_MainTex, i.uv);
        half3 lms = LMS(input.rgb);
        float3 dlms = 0;
        dlms[0] = 0.0 * lms[0] + 2.02344 * lms[1] + -2.52581 * lms[2];
        dlms[1] = 0.0 * lms[0] + 1.0 * lms[1] + 0.0 * lms[2];
        dlms[2] = 0.0 * lms[0] + 0.0 * lms[1] + 1.0 * lms[2];
        return half4(Correction(input, dlms), input.a);
    }

    half4 FragDeuteranopia(v2f i) : SV_Target
    {
        half4 input = tex2D(_MainTex, i.uv);
        half3 lms = LMS(input.rgb);
        float3 dlms = 0;
        dlms[0] = 1.0 * lms[0] + 0.0 * lms[1] + 0.0 * lms[2];
        dlms[1] = 0.494207 * lms[0] + 0.0 * lms[1] + 1.24827 * lms[2];
        dlms[2] = 0.0 * lms[0] + 0.0 * lms[1] + 1.0 * lms[2];
        return half4(Correction(input, dlms), input.a);
    }

    half4 FragTritanopia(v2f i) : SV_Target
    {
        half4 input = tex2D(_MainTex, i.uv);
        half3 lms = LMS(input.rgb);
        float3 dlms = 0;
        dlms[0] = 1.0 * lms[0] + 0.0 * lms[1] + 0.0 * lms[2];
        dlms[1] = 0.0 * lms[0] + 1.0 * lms[1] + 0.0 * lms[2];
        dlms[2] = -0.395913 * lms[0] + 0.801109 * lms[1] + 0.0 * lms[2];
        return half4(Correction(input, dlms), input.a);
    }
    ENDCG

    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FragProtanopia
            ENDCG
        }
            
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FragDeuteranopia
            ENDCG
        }
            
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment FragTritanopia
            ENDCG
        }
    }
}
