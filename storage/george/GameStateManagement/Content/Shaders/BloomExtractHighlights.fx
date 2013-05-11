//Extract the values that do not fall within the threshold

sampler TextureSampler : register(s0);

float BloomThreshold;

float4 PixelShaderFunction(float2 screenCoord : TEXCOORD0) : COLOR0
{
    float4 color = tex2D(TextureSampler, screenCoord);
    color = (color - BloomThreshold) / (1 - BloomThreshold);
    return color;
    
}

technique BloomExtractHighlights
{
    pass Pass1
    {
	PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
