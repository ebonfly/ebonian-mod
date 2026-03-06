namespace EbonianMod.Content.Projectiles.Terrortoma;

public class TExplosion : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Terrortoma/" + Name;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Type] = 10;
        FinalDrawSystem.ProjectileTypeList.Add(Type);
    }
    public override void SetDefaults()
    {
        Projectile.width = 116;
        Projectile.height = 78;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 80, 116, 80), Color.White, Projectile.rotation, new Vector2(tex.Width / 2, 78), Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
    public override Color? GetAlpha(Color lightColor) => Color.White;
    public override void AI()
    {
        if (++Projectile.frameCounter >= 3)
        {
            Projectile.frameCounter = 0;
            if (++Projectile.frame >= 10)
            {
                Projectile.Kill();
            }
        }
    }
}