//Extract the values that do not fall within the threshold

sampler BlurSampler : register(s0);
sampler SceneSampler : register(s1);
float motionBlurAmount;

float4 PixelShaderFunction(float2 screenCoord : TEXCOORD0) : COLOR0
{
    float4 blur = tex2D(BlurSampler, screenCoord);
    float4 scene = tex2D(SceneSampler, screenCoord);
    float4 color = clamp((scene*motionBlurAmount + blur*(1.0-motionBlurAmount)), 0.0, 1.0);

    return color;
    
}

technique BloomExtractHighlights
{
    pass Pass1
    {
	PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
