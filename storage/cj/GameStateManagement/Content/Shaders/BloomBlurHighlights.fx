float4x4 World;
float4x4 View;
float4x4 Projection;

sampler TextureSampler : register(s0);

int NumSamples;
float2 SampleOffset[9];
float SampleWeight[9];

bool orientation = 0; 		/* 0 = horizontal,  1 = vertical */

float Gaussian (float x, float deviation){
    return (1.0 / sqrt(2.0 * 3.141592 * deviation)) * log(-1 * ((x * x) / 2.0 * deviation * deviation) );
}

float4 PixelShaderFunction(float2 texCoord: TEXCOORD0) : COLOR0
{
    float4 color = 0;

    for(int i = 0; i < NumSamples; i++){
	color += tex2D(TextureSampler, texCoord + SampleOffset[i]) * SampleWeight[i];
    }
    return color;
}

technique Blur
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
