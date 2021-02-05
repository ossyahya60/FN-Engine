#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;

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
    color.rgb = color.r * 0.2126 + color.g * 0.7152 + color.b * 0.0722;
    //if ((input.TextureCoordinates.x - 0.5) * (input.TextureCoordinates.x - 0.5) + (input.TextureCoordinates.y - 0.5) * (input.TextureCoordinates.y - 0.5) <= 0.25)
    //    color.rgb += float3(1, 1, 1)*0.5 * sqrt((input.TextureCoordinates.x - 0.5) * (input.TextureCoordinates.x - 0.5) + (input.TextureCoordinates.y - 0.5) * (input.TextureCoordinates.y - 0.5)) * 4;
    //else
    //    color.rgba = 0;
    
    return color;
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
        AlphaBlendEnable = TRUE;
        DestBlend = INVSRCALPHA;
        SrcBlend = SRCALPHA;
    }
};