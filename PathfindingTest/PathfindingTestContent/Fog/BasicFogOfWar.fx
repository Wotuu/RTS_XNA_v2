texture LightsTexture;

sampler  ColorSampler  : register(s0);

sampler LightsSampler = sampler_state{
	Texture = <LightsTexture>;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float2 tex = input.TexCoord;
	
    float4 color = tex2D(ColorSampler, tex);
    float4 alpha = tex2D(LightsSampler, tex);
    
    return color * alpha;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
