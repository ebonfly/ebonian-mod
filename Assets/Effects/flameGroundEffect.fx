sampler2D uImage0 : register(s0);

float uWaviness = 1;
float uScale = 2;
float uIntensity = 4;
float4 uColorOverride = float4(1, 0.25, 0, 1);
float uTime;

float4 Main(float2 uv : TEXCOORD) : COLOR
{
    float gradient = smoothstep(1, 0, uv.y);
    float sTime = sin(uTime + uv.y * 0.5) * 0.5;
    float4 c = tex2D(uImage0, uScale * uv + float2(uWaviness * sTime, uTime));
    
    float4 c2 = tex2D(uImage0, uScale * 0.5 * uv + float2(uWaviness * sTime * 0.33, uTime * .7));
    
    float4 mask = pow(sin(uv.x * 3.14), (1 - uv.y) * 3 * c2 + 1) * 1.5;
    float4 final = c / c2 * pow(gradient, c + 1) + pow(gradient * 0.3, 2);
    
    return final * mask * uColorOverride * uIntensity;
}

Technique techique1
{
    pass FlameEffect
    {
        PixelShader = compile ps_3_0 Main();
    }
}