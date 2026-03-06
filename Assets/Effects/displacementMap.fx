sampler uImage0 : register(s0);
sampler uImage1 : register(s1);

float uTime;
float offset = 1;
float offsetX, offsetY, scale = 1;
float alpha;
bool wrapped;
float uIntensity = 1;

float2 Wrap(float2 uv)
{
    uv %= 1;
    uv += 1;
    uv %= 1;
    return uv;
}
float2 rotated(float2 v, float radians)
{
    float num = cos(radians);
    float num2 = sin(radians);

    float2 vec = v;
    float2 result = float2(0, 0);
    result.x += vec.x * num - vec.y * num2;
    result.y += vec.x * num2 + vec.y * num;
    return result;
}
float toRotation(float2 v)
{
    return atan2(v.y, v.x);
}

float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float2 _uv = (coords + float2(offsetX, offsetY)) * scale;
    if (wrapped)
    {
        _uv = Wrap(_uv);
    }

    float4 noise = tex2D(uImage1, _uv);
    float noiseA = max(noise.r, max(noise.g, noise.b)) - 0.5;
    float2 uv = coords + float2(sin(noiseA), cos(noiseA)) * offset;
    float4 c = tex2D(uImage0, uv);
    float a = max(c.r, max(c.g, c.b));
    return float4(c.rgb, a * (noiseA + 0.5f) * alpha) * uIntensity;
}

technique Technique1
{
    pass displacementMap
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}