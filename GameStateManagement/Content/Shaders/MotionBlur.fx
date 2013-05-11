float4x4 ViewProjectionInverse;
float4x4 PreviousViewProjection;
#define numSamples 3

sampler DepthTexture : register(s0);
sampler ColorTexture : register(s1);

struct OUT_DEPTH
{
	float4 position : POSITION;
	float distance : TEXCOORD0;
};

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
    float ZOverW = tex2D(DepthTexture, texCoord);
    float4 H = float4(texCoord.x * 2 - 1, (1 - texCoord.y) * 2 - 1, ZOverW, 1);
	float4 D = mul(H, ViewProjectionInverse);
	float4 worldPosition = D / D.w;
	float4 previousH = mul(worldPosition, PreviousViewProjection);
	previousH /= previousH.w;
	float2 velocity = (H - previousH)/2.0f;
	float4 color = tex2D(ColorTexture, texCoord);
	texCoord += velocity;  
	for(int i = 1; i < numSamples; ++i, texCoord += velocity)
	{
		float4 currentColor = tex2D(ColorTexture, texCoord);
		color+= currentColor;
	}
    return color / numSamples;
    
}

technique MotionBlur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
