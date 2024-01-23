Shader "Custom/EdgeDetectShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Threshold("Threshold", float) = 0.01
        _EdgeColor("Edge color", Color) = (0,0,0,1)
        _DepthProminence("Depth prominence", float) = 0.1
        _GreyColor("Grey color", Color) = (1,1,1,1)
    }
        SubShader
        {
            // No culling or depth
            Cull Off ZWrite Off ZTest Always

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

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

                sampler2D _CameraDepthNormalsTexture;

                v2f vert(appdata v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    return o;
                }

                sampler2D _MainTex;
                sampler2D _StencilTexture;
                float4 _MainTex_TexelSize;
                float _Threshold;
                fixed4 _EdgeColor;
                float _DepthProminence;
                fixed4 _GreyColor;

                float4 GetPixelValue(in float2 uv) {
                    half3 normal;
                    float depth;
                    DecodeDepthNormal(tex2D(_CameraDepthNormalsTexture, uv), depth, normal);
                    return fixed4(normal, depth);
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 col = tex2D(_MainTex, i.uv);
                    fixed stencil = tex2D(_StencilTexture, i.uv).r;
                    if (stencil == 0.0f)
                        return col;

                    fixed4 orValue = GetPixelValue(i.uv);
                    float2 offsets[8] = {
                        float2(-1, -1),
                        float2(-1, 0),
                        float2(-1, 1),
                        float2(0, -1),
                        float2(0, 1),
                        float2(1, -1),
                        float2(1, 0),
                        float2(1, 1)
                    };
                    fixed4 sampledValue = fixed4(0,0,0,0);
                    int edgeNextToStencil = 1;
                    for (int j = 0; j < 8; j++) {
                        edgeNextToStencil =
                            max(0, edgeNextToStencil - (1 - tex2D(_StencilTexture, i.uv + offsets[j] * _MainTex_TexelSize.xy).r));

                        sampledValue += GetPixelValue(i.uv + offsets[j] * _MainTex_TexelSize.xy);
                    }
                    sampledValue /= 8;

                    fixed4 greyCol = fixed4(0,0,0,1);
                    greyCol.rgb = (col.r * 0.3f + col.g * 0.59f + col.b * 0.11f) * _GreyColor;
                    fixed4 edgeCol = lerp(greyCol, _EdgeColor, min(Linear01Depth(orValue.a) * _DepthProminence, 1.0f));
                    int stepValue = step(_Threshold, length(orValue - sampledValue));
                    fixed4 returnCol = lerp(greyCol, edgeCol, edgeNextToStencil * stepValue);
                    return returnCol;
                }
                ENDCG
            }
        }
}