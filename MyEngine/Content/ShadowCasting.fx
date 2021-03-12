#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

Texture2D SpriteTexture;
float2 Step;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    float Alpha = color.a;
    
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-Step.x, -Step.y));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, -Step.y));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(Step.x, -Step.y));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-Step.x, 0));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(Step.x, 0));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(-Step.x, Step.y));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(0, Step.y));
    color += tex2D(SpriteTextureSampler, input.TextureCoordinates + float2(Step.x, Step.y));
    
    color /= 9;
    
    color.a = Alpha;
    
    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
        AlphaBlendEnable = TRUE;
        //DestBlend = INVSRCALPHA;
        //SrcBlend = SRCALPHA;
    }
};