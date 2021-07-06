#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0
#endif

static const int MAX_LIGHTS = 15;
static const int MAX_Shadows = 15;

int LightCount;
Texture2D SpriteTexture;
float3 Color[MAX_LIGHTS];
float YOverX;
float InnerRadius[MAX_LIGHTS];
float Radius[MAX_LIGHTS];
float Attenuation[MAX_LIGHTS];
float X_Bias[MAX_LIGHTS];
float Y_Bias[MAX_LIGHTS];
float AngularRadius[MAX_LIGHTS];
float InnerIntensity[MAX_LIGHTS];
float DirectionalIntensity;
float ShadowConstant[MAX_LIGHTS];
float CastShadow[MAX_LIGHTS];
bool CastShadows;
//float2 CameraPosNorm;

//Shadows
Texture2D ShadowMap;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

sampler2D ShadowMapSampler = sampler_state
{
    Texture = <ShadowMap>;
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
    
    bool Dirty = false;
    float3 SumColors[MAX_LIGHTS];
    [unroll(MAX_LIGHTS)]
    for (int k = 0; k < LightCount; k++)
        SumColors[k] = float3(0, 0, 0);
    
    [unroll(MAX_LIGHTS)]
    for (int i = 0; i < LightCount; i++)
    {
        float D_X = (input.TextureCoordinates.x - 0.5) - X_Bias[i];
        float D_Y = ((input.TextureCoordinates.y - 0.5) - Y_Bias[i]) * YOverX;
        float2 Direction = float2(D_X, D_Y);
        float Angle = degrees(atan2(Direction.y, Direction.x)) + 180.0;
        
        //if (X_Bias[i] + 0.5 - Radius[i] > 1 || X_Bias[i] + 0.5 + Radius[i] < 0 || Y_Bias[i] + 0.5 - Radius[i] > 1 || Y_Bias[i] + 0.5 + Radius[i] < 0) //Skip if outside the screen;
        //    continue;
    
        float Dist_SQ = D_X * D_X + D_Y * D_Y;
        
        if (Angle <= AngularRadius[i])
        {
            if (color.a != 0)
            {
                if (Dist_SQ <= Radius[i] * Radius[i] && Dist_SQ > InnerRadius[i] * InnerRadius[i]) //Outer Radius
                {
                    float4 Shadow = tex2D(ShadowMapSampler, input.TextureCoordinates);
                    float ShadowAttenuation = 1;
                
                    if (CastShadows && CastShadow[i] == 1 && Shadow.r != 1) //There is no Shadow
                        ShadowAttenuation = clamp(ShadowConstant[i], 0, 1);
                                        
                    float Dist = ((Radius[i] - sqrt(Dist_SQ)) / Radius[i]);

					//New Shader
					//SumColors[i].rgb += (color.rgb + Color[i]) * (Attenuation[i] * Dist * Dist * ShadowAttenuation);
					
					//Old Shader
                    SumColors[i].rgb += color.rgb * Color[i];
                    SumColors[i].rgb *= Attenuation[i] * Dist * Dist * ShadowAttenuation;
                    Dirty = true;
                }
                else if (Dist_SQ <= InnerRadius[i] * InnerRadius[i]) //Inner Radius
                {
                    float4 Shadow = tex2D(ShadowMapSampler, input.TextureCoordinates);
                    float ShadowAttenuation = 1;
                
                    if (CastShadows && CastShadow[i] == 1 && Shadow.r != 1) //There is no Shadow
                        ShadowAttenuation = clamp(ShadowConstant[i], 0, 1);
                    
                    float Dist = ((Radius[i] - sqrt(Dist_SQ)) / Radius[i]);

					//New Shader
					//SumColors[i].rgb += (color.rgb + Color[i]) * ((InnerIntensity[i] * Attenuation[i] - (InnerIntensity[i] - 1) * Attenuation[i] * (sqrt(Dist_SQ) / InnerRadius[i])) * Dist * Dist * ShadowAttenuation);
					
					//Old Shader
                    SumColors[i].rgb += color.rgb * Color[i];
                    SumColors[i].rgb *= (InnerIntensity[i] * Attenuation[i] - (InnerIntensity[i] - 1) * Attenuation[i] * (sqrt(Dist_SQ) / InnerRadius[i])) * Dist * Dist * ShadowAttenuation;
                    Dirty = true;
                }
            }
        }
    }
    
    float3 BaseColor = color.rgb;
    
    color.rgb = BaseColor * DirectionalIntensity;
    [unroll(MAX_LIGHTS)]
    for (int j = 0; j < LightCount; j++)
        color.rgb += SumColors[j];
        
    if(!Dirty)
        color.rgb = BaseColor.rgb * DirectionalIntensity;

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