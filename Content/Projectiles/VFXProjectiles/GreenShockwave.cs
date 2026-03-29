using System;

namespace EbonianMod.Content.Projectiles.VFXProjectiles;

public class GreenShockwave : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 300;
        Projectile.width = 300;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        Projectile.ai[1] = 1;
    }
    public override void PostAI()
    {
        if (Projectile.ai[1] == 1)
            Projectile.damage = 0;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Projectile.ai[2] == 0 ? Assets.Extras.Extras2.circle_02.Value : Assets.Extras.explosion.Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
        for (int i = 0; i < 2; i++)
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Green * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0] * 2, SpriteEffects.None, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.05f;
        if (Projectile.ai[0] > 1)
            Projectile.Kill();
    }
}
public class YellowShockwave : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 300;
        Projectile.width = 300;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        Projectile.ai[1] = 1;
    }
    public override void PostAI()
    {
        if (Projectile.ai[1] == 1)
            Projectile.damage = 0;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.Extras2.light_02.Value;
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
        for (int i = 0; i < 4; i++)
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.LawnGreen, Color.Orange, Projectile.ai[0]) with { A = 0 } * alpha * 0.4f, Main.rand.NextFloat(MathF.PI), tex.Size() / 2, Projectile.ai[0] * Main.rand.NextFloat(1f, 1.5f), SpriteEffects.None, 0);

        return false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.05f;
        if (Projectile.ai[0] > 1)
            Projectile.Kill();
    }
}
public class BigGrayShockwave : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 300;
        Projectile.width = 300;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        Projectile.ai[1] = 1;
    }
    public override void PostAI()
    {
        if (Projectile.ai[1] == 1)
            Projectile.damage = 0;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.PulseCircle.Value;
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0] / 2);
        for (int i = 0; i < 2; i++)
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Gray with { A = 0 } * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0] * 2, SpriteEffects.None, 0);

        return false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.05f;
        if (Projectile.ai[0] > 2)
            Projectile.Kill();
    }
}
public class BigGreenShockwave : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 300;
        Projectile.width = 300;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hitinfo, int damage)
    {
        Projectile.ai[1] = 1;
    }
    public override void PostAI()
    {
        if (Projectile.ai[1] == 1)
            Projectile.damage = 0;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.PulseCircle.Value;
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0] / 2);
        for (int i = 0; i < 2; i++)
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.LawnGreen with { A = 0 } * alpha, Projectile.rotation, tex.Size() / 2, Projectile.ai[0] * 2, SpriteEffects.None, 0);

        return false;
    }
    public override void AI()
    {
        Projectile.ai[0] += 0.1f;
        if (Projectile.ai[0] > 2)
            Projectile.Kill();
    }
}
