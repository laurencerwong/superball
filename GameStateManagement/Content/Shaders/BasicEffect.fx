float4x4 World;
float4x4 View;
float4x4 Projection;
float3 AmbientLightColor;
float3 SpecularLightColor;
float SpecularLightPower;
float3 DirectionalLightColor;
float3 DirectionalLightDirection;
float3 CameraPos;
float Alpha;
Texture xTexture;

sampler TextureSampler = sampler_state {texture = <xTexture>;
magfilter = LINEAR;
minfilter = LINEAR;
mipfilter = LINEAR;
AddressU = mirror;
AddressV = mirror;};


// TODO: add effect parameters here.

struct VertexShaderInput
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderToPixelShader
{
    float4 Position  : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 Position3D : TEXCOORD2;

};

struct PixelShaderOutput
{
	float4 Color : COLOR0;
    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

float Dot(float3 light, float3 pos, float3 norm)
{
	float3 lightDirection = normalize(pos - light);
	return dot(-lightDirection, norm);
}

VertexShaderToPixelShader VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderToPixelShader output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Normal = normalize(mul(input.Normal, (float3x3)World));
	output.Position3D = mul(input.Position, World);
	output.TexCoord = input.TexCoord;

    return output;
}
PixelShaderOutput PixelShaderFunction(VertexShaderToPixelShader input)
{
	PixelShaderOutput output;
	/*float diffuseLight = saturate(dot(input.Normal, DirectionalLightDirection));*/
	/*
	output.Color = float4(saturate(AmbientLightColor + (texel.xyz * DirectionalLightColor * 
	diffuseLight * 0.6) + (specularLight * SpecularLightColor * 0.5)), texel.w);
	*/
	float4 norm = float4(input.Normal, 1.0);
	float4 diffuse = saturate(dot(-DirectionalLightDirection, norm));
	diffuse.a = Alpha;
	float4 reflection  = normalize(2*diffuse*norm - float4(DirectionalLightDirection, Alpha));
	float4 specular = pow(saturate(dot(reflection, input.Position3D)), SpecularLightPower);
	specular.a = Alpha;
	float4 texel = tex2D(TextureSampler, input.TexCoord);
	texel.a = Alpha;
	output.Color = float4(diffuse * DirectionalLightColor + texel.xyz + SpecularLightColor * specular, Alpha);
	//float diffuseFactor = Dot(DirectionalLightDirection, intput.Position3D, input.Normal);
	//diffuseFactor = saturate(diffuseFactor);
	//diffuseFactor *= SpecularLightPower;
    // TODO: add your pixel shader code here.
	//float4 color = tex2D(TextureSampler, input.TexCoord);
    return output;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
