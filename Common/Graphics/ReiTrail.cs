using System.Runtime.InteropServices;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;

namespace EbonianMod.Common.Graphics.Trails;

[StructLayout(LayoutKind.Sequential, Size = 1)]
public struct ReiTrail
{
    private static VertexStrip _vertexStrip = new VertexStrip();

    public void Draw(Projectile proj)
    {
        MiscShaderData miscShaderData = GameShaders.Misc["MagicMissile"];
        miscShaderData.UseSaturation(-2.8f);
        miscShaderData.UseOpacity(6f);
        miscShaderData.Apply();
        _vertexStrip.PrepareStripWithProceduralPadding(proj.oldPos, proj.oldRot, StripColors, StripWidth, -Main.screenPosition + proj.Size / 2f);
        _vertexStrip.DrawTrail();
        Main.pixelShader.CurrentTechnique.Passes[0].Apply();
    }

    public Color StripColors(float progressOnStrip)
    {
        Color result = Color.Cyan;
        result.A = 0;
        return result;
    }

    private float StripWidth(float progressOnStrip)
    {
        float t = 1f;
        float lerpValue = Utils.GetLerpValue(0f, 0.2f, progressOnStrip, clamped: true);
        t *= 1f - (1f - lerpValue) * (1f - lerpValue);
        return MathHelper.Lerp(0f, 10, t);
    }
}