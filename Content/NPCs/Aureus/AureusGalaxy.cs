using EbonianMod.Common.Graphics;
using EbonianMod.Content.Dusts;

namespace EbonianMod.Content.NPCs.Aureus;

public class AureusGalaxy : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/"+Name;
    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(400);
        Projectile.aiStyle = -1;
        Projectile.scale = 0;
        Projectile.tileCollide = false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White,
            Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2,
            new Vector2(Projectile.scale, Lerp(0, 2, InOutCirc.Invoke(Projectile.scale))), SpriteEffects.None);
        return false;
    }
    Vector2 p;
    public override void AI()
    {
        Projectile.ai[0]++;
        if (Projectile.ai[0] == 1)
            for (int i = 0; i < 10; i++)
                Projectile.NewProjectile(null, Projectile.Center + Main.rand.NextVector2Circular(100, 100), Vector2.Zero, ProjectileType<AExplosion>(), 0, 0);
        if (Projectile.ai[0] < 60)
        {
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Helper.FromAToB(Projectile.Center, Main.LocalPlayer.Center).ToRotation(), 0.1f);

            Projectile.scale = Lerp(0, 1, InOutCirc.Invoke(Projectile.ai[0] / 60f));
        }
        if (Projectile.ai[0] > 80 && Projectile.ai[0] < 200)
        {
            p = Projectile.Center + Projectile.DirectionTo(Main.LocalPlayer.Center) * 2000;
            Vector2 pos = Projectile.Center + Projectile.rotation.ToRotationVector2().RotatedByRandom(PiOver2) * Main.rand.NextFloat(50, 200);
            Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), pos.DirectionTo(Projectile.Center) * Main.rand.NextFloat(1, 6), newColor: Main.rand.NextBool() ? Color.OrangeRed : Color.Cyan, Scale: Main.rand.NextFloat(0.05f, 0.3f)).customData = Projectile.Center;

            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Helper.FromAToB(Projectile.Center, Main.LocalPlayer.Center).ToRotation(), 0.1f);

            if (Projectile.ai[0] % 5 == 0)
            {
                if (Projectile.ai[0] % 15 == 0)
                    SoundEngine.PlaySound(Aureus.pewSound.WithVolumeScale(2), Projectile.Center);
                Projectile.NewProjectile(null, Projectile.Center + new Vector2(0, Main.rand.NextFloat(-210, 210)).RotatedBy(Projectile.rotation), Projectile.rotation.ToRotationVector2() * Main.rand.NextFloat(15, 30), ProjectileType<AureusLaser>(), 20, 0);
            }

        }
        if (Projectile.ai[0] > 220)
        {
            Projectile.rotation += ToRadians(Projectile.velocity.Length() / 4f);

            if (Projectile.ai[0] < 240)
            {
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(p) * 40, InOutCirc.Invoke((Projectile.ai[0] - 220) / 20f));
            }
        }
        if (Projectile.ai[0] > 280)
        {
            Projectile.Kill();
        }
    }
}
public class AureusVortex : ModProjectile
{
    public override string Texture => Helper.AssetPath+"Extras/vortex";
    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(200);
        Projectile.aiStyle = -1;
        Projectile.scale = 0;
        Projectile.tileCollide = false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationRendering.DrawCache.Add(() =>
        {
            for (int i = 0; i < 3; i++)
            {
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.Cyan with { A = 0 },
                    Projectile.rotation, TextureAssets.Projectile[Type].Value.Size() / 2,
                    Projectile.scale, SpriteEffects.None);
                Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 },
                    Projectile.rotation * 0.5f, TextureAssets.Projectile[Type].Value.Size() / 2,
                    Projectile.scale * 0.9f, SpriteEffects.None);
            }
        });
        return false;
    }
    Vector2 p;
    public override void AI()
    {
        if (Projectile.ai[0] % 70 == 0)
            SoundEngine.PlaySound(SoundID.Item13.WithPitchOffset(-1f), Projectile.Center);
        Projectile.ai[0]++;
        if (Projectile.ai[0] < 650)
            Projectile.scale = Lerp(0, 3, InOutSine.Invoke(Projectile.ai[0] / 650f));
        else
            Projectile.scale = Lerp(3, 0, InOutSine.Invoke((Projectile.ai[0] - 650) / 50f));
        Projectile.rotation += ToRadians(10);

        if (Projectile.ai[0] < 300)
            Projectile.Center = Vector2.Lerp(Projectile.Center, Main.LocalPlayer.Center - new Vector2(0, 200), 0.01f);

        if (Projectile.ai[0] == 390)
            for (int i = 0; i < 10; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 10) + PiOver4;
                for (int j = -1; j < 2; j++)
                    Projectile.NewProjectile(null, Projectile.Center, angle.ToRotationVector2().RotatedBy(j / 2f), ProjectileType<AureusLaser>(), 10, 0, ai2: 1);
            }

        if (Projectile.ai[0] == 450)
        {
            SoundEngine.PlaySound(Aureus.primSlopSound, Projectile.Center);
            for (int i = 0; i < 10; i++)
            {
                float angle = Helper.CircleDividedEqually(i, 10) + PiOver4;
                for (int j = -1; j < 2; j++)
                    Projectile.NewProjectile(null, Projectile.Center - angle.ToRotationVector2().RotatedBy(j / 2f) * 50, angle.ToRotationVector2().RotatedBy(j / 2f), ProjectileType<AureusBeam>(), 70, 0);
            }
        }

        if (Projectile.ai[0] == 690)
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<AExplosion>(), 0, 0);
        if (Projectile.ai[0] > 700)
        {
            Projectile.Kill();
        }
    }
}
public class AExplosion : ModProjectile
{
    public override string Texture => Helper.Placeholder;
    public override void SetDefaults()
    {
        Projectile.height = 200;
        Projectile.width = 200;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 200;
    }
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
        float alpha2 = MathHelper.Lerp(0.5f, 0, Projectile.ai[0]);
        Texture2D ring = Assets.Extras.crosslight.Value;
        Main.spriteBatch.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.Cyan with { A = 0 } * (alpha), Projectile.rotation, ring.Size() / 2, Projectile.ai[0] * 1.1f * 5, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(ring, Projectile.Center - Main.screenPosition, null, Color.OrangeRed with { A = 0 }
        * alpha * (0.5f + Projectile.ai[2]), Projectile.rotation, ring.Size() / 2, Projectile.ai[0] * 7f, SpriteEffects.None, 0);

        return false;
    }
    public override void OnSpawn(IEntitySource source)
    {
        if (Projectile.ai[2] == 0)
        {
            for (int i = 0; i < 17; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<SparkleDust>(), Main.rand.NextVector2Circular(15, 15), 0, Color.OrangeRed, Main.rand.NextFloat(0.05f, 0.175f));
                Dust.NewDustPerfect(Projectile.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(20, 20), 0, Color.Cyan, Main.rand.NextFloat(0.05f, 0.24f));
            }
        }
    }
    public override void AI()
    {
        if (Projectile.ai[0] == 0)
            SoundEngine.PlaySound(Sounds.xSpirit.WithPitchOffset(-0.5f), Projectile.Center);
        Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(150 * Projectile.ai[0], 150 * Projectile.ai[0]), DustType<SparkleDust>(), Main.rand.NextVector2Circular(3, 3), 0, Main.rand.NextBool() ? Color.OrangeRed : Color.Cyan, Main.rand.NextFloat(0.05f, 0.175f));
        Projectile.ai[0] += 0.075f;
        if (Projectile.ai[0] > 1)
            Projectile.Kill();
    }
}
