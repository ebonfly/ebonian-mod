using EbonianMod.Content.Dusts;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageBag : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Garbage/" + Name;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 2;
    }
    public override void SetDefaults()
    {
        Projectile.aiStyle = -1;
        Projectile.width = 22;
        Projectile.timeLeft = 500;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.height = 24;
        Projectile.extraUpdates = 1;
    }
    public override bool? CanDamage() => Projectile.Opacity > 0.5f;
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        if (Projectile.Center.Y >= Main.LocalPlayer.Center.Y - 100)
            fallThrough = false;
        return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.frame == 0 && Projectile.timeLeft > 100)
        {
            SoundEngine.PlaySound(SoundID.Item177, Projectile.Center);
            for (int i = 0; i < 15; i++)
                Dust.NewDustPerfect(Projectile.Center + Vector2.UnitY.RotatedByRandom(1) * Main.rand.NextFloat(5, 10), DustType<GarbageFlameDust>(), Projectile.velocity.SafeNormalize(Vector2.UnitY).RotatedByRandom(1f) * Main.rand.NextFloat(2, 10), 0, Color.OrangeRed, 0.1f).noGravity = true;
            Projectile.timeLeft = 100;
        }
        Projectile.velocity.Y *= 0.5f;
        Projectile.velocity.X = 0;
        Projectile.frame = 1;
        Projectile.netUpdate = true; // TEST
        return false;
    }
    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, 0.85f, 0.3f, 0.1f);
        
        if (Projectile.frame == 0)
        {
            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustType<GarbageFlameDust>(), Projectile.velocity.RotatedByRandom(0.3f) * 0.4f, 0, Color.OrangeRed, 0.1f).noGravity = true;
        }
        else
        {
            Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(7, 7) + new Vector2(0, 10), DustType<GarbageFlameDust>(), new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-5, -1)), 0, Color.OrangeRed, 0.1f).noGravity = true;
        }
        Projectile.tileCollide = Projectile.Center.Y > Main.player[Projectile.owner].Center.Y - 20;
        Projectile.velocity *= 1.01f;
        if (Projectile.velocity.Y > 0)
            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 0, 0.01f);
        else {
            if (Projectile.timeLeft > 200)
                Projectile.timeLeft = 200;
            Projectile.velocity.X *= 1.02f;
        }
        
        if (Projectile.frame == 0)
            Projectile.rotation = Projectile.velocity.ToRotation() - MathHelper.PiOver2;
        else
        {
            Projectile.rotation = 0;
            if (Projectile.timeLeft < 40)
                Projectile.Opacity -= 0.03f;
        }

    }
}
