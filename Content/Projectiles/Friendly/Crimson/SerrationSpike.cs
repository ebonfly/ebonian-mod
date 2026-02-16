using System.Collections.Generic;

namespace EbonianMod.Content.Projectiles.Friendly.Crimson;

internal class SerrationSpike : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Friendly/Crimson/" + Name;
    public override void SetDefaults()
    {
        Projectile.width = 20;
        Projectile.height = 44;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.penetrate = -1;
        Projectile.scale = 1;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 40;
        Projectile.hide = true;
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCsAndTiles.Add(index);
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;
        Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(texture.Width / 2, texture.Height * 0.75f), new Vector2(Projectile.ai[2], Projectile.ai[2]), Projectile.direction == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0f);
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.NPCHit18, Projectile.position);
        for (int i = 0; i < 15; i++)
        {
            int dustType = Dust.NewDust(Projectile.Center - new Vector2(0, Projectile.height / 2 * Projectile.scale), Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(0, 101), default(Color), 1f + Main.rand.NextFloat(1));
            Main.dust[dustType].noGravity = true;
            Dust dust2 = Main.dust[dustType];
            dust2.velocity *= 2f;
        }
    }
    public override bool ShouldUpdatePosition() => false;
    public override Color? GetAlpha(Color lightColor) => Color.White;
    public override void AI()
    {
        if (Projectile.ai[2] < (Projectile.ai[0] + 1) * 0.2f)
            Projectile.ai[2] += 0.1f;
        if (Projectile.timeLeft == 38)
        {
            SoundStyle style = SoundID.NPCHit18;
            style.Volume = 0.5f;
            SoundEngine.PlaySound(style, Projectile.Center);
            for (int i = 0; i < 15; i++)
            {
                int dustType = Dust.NewDust(Projectile.Center - new Vector2(0, Projectile.height / 2 * Projectile.scale), Projectile.width, Projectile.height, DustID.Blood, Projectile.velocity.X, Projectile.velocity.Y, Main.rand.Next(0, 101), default(Color), 1 + Main.rand.NextFloat(1f));
                Main.dust[dustType].noGravity = true;
                Dust dust2 = Main.dust[dustType];
                dust2.velocity *= 2f;
            }

            if (Projectile.ai[0] < 7 && Main.myPlayer == Projectile.owner)
            {
                if(Helper.GetNearestSurface(Projectile.Center + Vector2.UnitX * (Projectile.ai[0] + 1) * 4 * Projectile.direction, 160, false, out Vector2 point))
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), new Vector2(point.X, point.Y + Projectile.ai[0] + 1), Projectile.velocity, ProjectileType<SerrationSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner, Projectile.ai[0] + 1, Projectile.ai[0]);
                }
            }
        }
        Projectile.velocity.Normalize();
        Projectile.spriteDirection = Projectile.direction = Projectile.velocity.X < 0 ? -1 : 1;
    }
}