namespace EbonianMod.Content.Projectiles.VFXProjectiles;

class Ripple : ModProjectile
{
    public override bool PreDraw(ref Color lightColor)
    {
        BlurRendering.DrawCache.Add(() =>
        {
            Texture2D a = TextureAssets.Projectile[Projectile.type].Value;
            Main.spriteBatch.Draw(a, Projectile.Center - Main.screenPosition, null, Color.White, 0, a.Size() / 2, Projectile.ai[0], SpriteEffects.None, 0f);

        });
        return false;
    }
    public override string Texture => Helper.AssetPath+"Extras/ripple";
    public override void SetDefaults()
    {
        Projectile.width = 100;
        Projectile.height = 100;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.timeLeft = 300;
        Projectile.tileCollide = false;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 1;
        Projectile.penetrate = -1;
        base.SetDefaults();
    }

    public override void AI()
    {
        if (Projectile.ai[1] == 0)
            Projectile.ai[1] = 0.025f;
        Projectile.ai[0] += Projectile.ai[1];
        Projectile.ai[1] *= 1.05f;
    }

}
