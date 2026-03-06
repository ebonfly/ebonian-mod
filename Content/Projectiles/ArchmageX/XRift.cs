using EbonianMod.Content.Dusts;
using System;

namespace EbonianMod.Content.Projectiles.ArchmageX;

public class XRift : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetStaticDefaults()
    {
        FinalDrawSystem.ProjectileTypeList.Add(Type);
    }
    public override void SetDefaults()
    {
        Projectile.width = 5;
        Projectile.height = 5;
        Projectile.aiStyle = 0;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool? CanDamage() => false;
    public override void AI()
    {
        Projectile.velocity.SafeNormalize(Vector2.UnitX);
        if (Projectile.ai[0] > 100 && Projectile.ai[0] < 200 && Projectile.ai[0] % 10 == 0 && Projectile.damage != 0)
        {
            Vector2 pos = Projectile.Center - new Vector2(2, Main.rand.NextFloat(-15, 15)).RotatedBy(Projectile.velocity.ToRotation());
            Vector2 vel = Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.7f);
            SoundEngine.PlaySound(Sounds.xSpirit, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                if (i % 2 == 0)
                    Dust.NewDustPerfect(pos, DustType<SparkleDust>(), vel.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2, 5), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
                else
                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), vel.RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(2, 5), 0, Color.DarkOrchid, Main.rand.NextFloat(0.05f, 0.175f));
            }
            pos = Projectile.Center - new Vector2(10, Main.rand.NextFloat(-15, 15)).RotatedBy(Projectile.velocity.ToRotation());
            MPUtils.NewProjectile(null, pos, vel, ProjectileType<XTentacle>(), Projectile.damage, 0, ai0: Main.rand.Next(50, 90),
                ai1: Main.rand.NextFloat(0.5f, 2f));
        }
        Projectile.ai[0]++;
        if (Projectile.ai[0] < 60)
            Projectile.ai[2] = MathHelper.SmoothStep(-.5f, 1, InOutCubic.Invoke(Projectile.ai[0] / 60)) + Lerp(.5f, 0, InOutCirc.Invoke(Projectile.ai[0] / 60));
        else if (Projectile.ai[0] > 300 - 60)
            Projectile.ai[2] = MathHelper.SmoothStep(1, 0, InOutCubic.Invoke((Projectile.ai[0] - (300 - 60)) / 60));

        if (Projectile.ai[0] == 25)
            SoundEngine.PlaySound(Sounds.cursedToyCharge.WithPitchOffset(-0.1f), Projectile.Center);

        Projectile.localAI[1] = MathHelper.Lerp(Projectile.ai[1], MathF.Sin(Projectile.ai[0] * .05f) * 0.05f, 0.25f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        FinalDrawSystem.DrawCache.Add(() =>
        {
            Texture2D tex = Assets.Extras.vortex.Value;
            Texture2D s_tex2 = Assets.Extras.cone7.Value;

            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Draw(s_tex2, Projectile.Center - Projectile.velocity * 30 - Main.screenPosition, null, Color.Indigo * Projectile.ai[2] * 0.5f, Projectile.velocity.ToRotation(), new Vector2(0, s_tex2.Height / 2), new Vector2(1, 2) * Projectile.ai[2] * 2, SpriteEffects.None, 0);
            Vector2 scale = new Vector2(MathF.Pow(Projectile.ai[2], 2) * (1.5f + Projectile.localAI[1]), .6f - Projectile.ai[1] * 0.5f);
            Main.spriteBatch.Reload(Effects.SpriteRotation.Value);
            Effects.SpriteRotation.Value.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 125));
            Effects.SpriteRotation.Value.Parameters["scale"].SetValue(Projectile.ai[1] == 0 ? scale * 0.75f * Projectile.ai[2] : new Vector2(MathF.Pow(Projectile.ai[2], 2) * 0.6f, 0.75f) + new Vector2(0.1f).RotatedBy(MathF.Sin((1 + Projectile.whoAmI) * Main.GlobalTimeWrappedHourly)));
            Vector4 col = (Color.DarkOrchid * Projectile.ai[2]).ToVector4();
            col.W = Projectile.ai[2] * 0.15f;
            Effects.SpriteRotation.Value.Parameters["uColor"].SetValue(col * (Projectile.ai[1] == 0 ? 1 : 0.5f));
            for (int i = 0; i < 75; i++)
            {
                float s = MathHelper.SmoothStep(Projectile.ai[2], 0, (float)i / 75);
                Vector2 pos = Projectile.Center - new Vector2(i * s, 0).RotatedBy(Projectile.velocity.ToRotation());
                /*if (i % 5 == 0)
                    tex = Assets.Extras.vortex3");
                else tex = Assets.Extras.vortex");*/
                Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() / 2, /*(i % 5 == 0 ? s * 0.25f : s)*/s, i % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0); ;
            }
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            Main.spriteBatch.Reload(effect: null);
        });
        return false;
    }
}
public class XRiftSmall : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetStaticDefaults()
    {
        FinalDrawSystem.ProjectileTypeList.Add(Type);
    }
    public override void SetDefaults()
    {
        Projectile.width = 5;
        Projectile.height = 5;
        Projectile.aiStyle = 0;
        Projectile.tileCollide = false;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 350;
        Projectile.extraUpdates = 2;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool? CanDamage() => false;
    public override void AI()
    {
        Projectile.velocity.SafeNormalize(Vector2.UnitX);
        Projectile.ai[0]++;
        if (Projectile.ai[0] < 60)
            Projectile.ai[2] = MathHelper.SmoothStep(0, 1, Projectile.ai[0] / 60);
        else if (Projectile.ai[0] > 350 - 60)
            Projectile.ai[2] = MathHelper.SmoothStep(1, 0, (Projectile.ai[0] - (350 - 60)) / 60);

        if (Projectile.ai[0] == 25)
            SoundEngine.PlaySound(Sounds.cursedToyCharge.WithPitchOffset(-0.1f), Projectile.Center);

        Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], MathF.Sin(Projectile.ai[0] * .05f) * 0.05f, 0.25f);

        if (Projectile.timeLeft % 16 == 0 && Projectile.timeLeft < 280 && Projectile.timeLeft > 140)
        {
            MPUtils.NewProjectile(null, Projectile.Center, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4 * 0.5f), ProjectileType<XTentacle>(), 15, 0, ai0: Main.rand.Next(50, 90), ai1: Main.rand.NextFloat(2.5f, 5f));
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.vortex.Value;
        Vector2 scale = new Vector2(1f + Projectile.ai[1], 0.25f - Projectile.ai[1] * 0.5f);
        Main.spriteBatch.Reload(BlendState.Additive);
        Main.spriteBatch.Reload(Effects.SpriteRotation.Value);
        Effects.SpriteRotation.Value.Parameters["rotation"].SetValue(MathHelper.ToRadians(Main.GlobalTimeWrappedHourly * 125));
        Effects.SpriteRotation.Value.Parameters["scale"].SetValue(scale * 0.75f * Projectile.ai[2]);
        Vector4 col = (Color.DarkOrchid * Projectile.ai[2]).ToVector4();
        col.W = Projectile.ai[2] * 0.15f;
        Effects.SpriteRotation.Value.Parameters["uColor"].SetValue(col);
        for (int i = 0; i < 75; i++)
        {
            float s = MathHelper.SmoothStep(Projectile.ai[2] * 0.5f, 0, (float)i / 75);
            Vector2 pos = Projectile.Center - new Vector2(i * s, 0).RotatedBy(Projectile.velocity.ToRotation());
            /*if (i % 5 == 0)
                tex = Assets.Extras.vortex3");
            else tex = Assets.Extras.vortex");*/
            Main.spriteBatch.Draw(tex, pos + Main.rand.NextVector2Circular(5, 5) - Main.screenPosition, null, Color.White, Projectile.velocity.ToRotation() + MathHelper.PiOver2, tex.Size() / 2, /*(i % 5 == 0 ? s * 0.25f : s)*/s, i % 2 == 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0); ;
        }
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        Main.spriteBatch.Reload(effect: null);
        return false;
    }
}
