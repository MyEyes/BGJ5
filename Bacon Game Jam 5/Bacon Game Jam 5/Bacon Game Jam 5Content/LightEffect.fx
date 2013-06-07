float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: Fügen Sie Effektparameter hier hinzu.

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR;

    // TODO: Fügen Sie Eingabekanäle, wie Texturkoordinaten
    // und Vertex-Farben, hier hinzu.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 Color : COLOR;
    // TODO: Fügen Sie Vertex-Shader-Ausgaben, wie Farben und Texturkoordinaten,
    // hier hinzu. Diese Werte werden automatisch über das Dreieck
    // interpoliert und als Eingabe für Ihren Pixel-Shader geliefert.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	input.Position.w=1;
    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Position.w=1;
	output.TexCoord=input.TexCoord;
	output.Color=input.Color;
    // TODO: Fügen Sie Ihren Vertex-Shader-Code hier hinzu.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: Fügen Sie Ihren Pixel-Shader-Code hier hinzu.
	float2 relPos = input.TexCoord.xy*2-1;
	float dist = length(relPos);
	//clip(1-dist);
	return float4(1,0,0,1);
    return input.Color*(1-dist);
}

technique DrawLight
{
    pass Pass1
    {
        // TODO: Stellen Sie Renderstates hier ein.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
