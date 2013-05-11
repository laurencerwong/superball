float4x4 World;
float4x4 View;
float4x4 Projection;

sampler TextureSampler : register(s0);

float BlurStrength;
float Width;
float Height;
float BlurRadius;
float GaussianDist[9] = {0.05, 0.09, 0.12, 0.15, 0.16, 0.15, 0.12, 0.09, 0.05};
float Offset[9] = {-4, -3, -2, -1, 0, 1, 2, 3, 4};
bool orientation; 		/* 0 = horizontal,  1 = vertical */

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{
    float4 color = 0;

    for(int i = 0; i < 9; i++){
	if(!orientation)
	    color += (tex2D(TextureSampler, float2(texCoord.x + (Offset[i]/Width) * BlurRadius, texCoord.y))
		      * GaussianDist[i]);
	else
	    color += (tex2D(TextureSampler, float2(texCoord.x, texCoord.y + (Offset[i]/Height) * BlurRadius))
		      * GaussianDist[i]);
    }
    return color * BlurStrength;
}

technique Blur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
