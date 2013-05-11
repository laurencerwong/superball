float4x4 WorldViewProj;

struct OUT_DEPTH
{
	float4 position : POSITION;
	float distance : TEXCOORD0;
};

float4 PixelShaderFunction(OUT_DEPTH input) : COLOR0
{
    return float4(input.distance.x, 0, 0, 1);
}

OUT_DEPTH RenderToDepthTexVS(float4 pos : POSITION)
{
	OUT_DEPTH output;
	output.position = mul(pos, WorldViewProj);
	output.distance.x = 1-(output.position.z / output.position.w);
	return output;
}

technique Blur
{
    pass Pass1
    {
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = FALSE;
		VertexShader = compile vs_2_0 RenderToDepthTexVS();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
