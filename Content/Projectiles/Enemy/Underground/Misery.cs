using EbonianMod.Content.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;

namespace EbonianMod.Content.Projectiles.Enemy.Underground;

public class Misery : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Enemy/Underground/" + Name;
    public override void SetDefaults()
    {
        Projectile.CloneDefaults(ProjectileID.Boulder);
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[ProjectileID.Boulder].Value;
        Texture2D tex2 = TextureAssets.Projectile[Type].Value;
        Rectangle frame = tex2.Frame(1, 4, 0, Projectile.frame);

        for (int i = -1; i < 2; i++)
        {
            if (i == 0) continue;
            Main.spriteBatch.Draw(tex2, Projectile.Center + new Vector2(i * 10 * Projectile.ai[1], 0) - Main.screenPosition, frame, lightColor * Projectile.ai[1], ToRadians(Projectile.velocity.X), frame.Size() / 2, Projectile.scale, i == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
        }

        Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(1, 2) - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.None, 0);
        return false;
    }
    public override void OnSpawn(IEntitySource source)
    {
        Projectile.ai[0] = 0;
        Projectile.ai[1] = 0;
        Projectile.ai[2] = 0;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1; //idk just making sure
        Projectile.netUpdate = true; // TEST
    }
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        
        
        if (player.Distance(Projectile.Center) > 1000)
        {
            Projectile.Kill();
        }
        
        for (int i = -1; i < 2; i++)
        {
            if (i == 0) continue;
            Lighting.AddLight(Projectile.Center + new Vector2(i * 10 * Projectile.ai[1], 12), Projectile.ai[1], Projectile.ai[1] * 0.4f, 0);
        }
        if (++Projectile.frameCounter % 5 == 0)
            Projectile.frame = Projectile.frame > 1 ? 0 : Projectile.frame + 1;

        Projectile.timeLeft = 10;
        Projectile.ai[0]++;
        if (Projectile.ai[0] == 60)
            SoundEngine.PlaySound(Sounds.garbageAwaken.WithPitchOffset(0.5f), Projectile.Center);
        if (Projectile.ai[0] > 40 && Projectile.ai[0] < 100)
        {
            Projectile.ai[1] = Lerp(0, 1, InOutBounce((Projectile.ai[0] - 40) / 60f));
        }

        if (Projectile.ai[0] > 60)
            for (int i = -1; i < 2; i++)
            {
                if (i == 0) continue;
                for (int j = 2; j < 4; j++)
                {
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(i * 14 * Projectile.ai[1], 12), DustID.Torch, Vector2.UnitY * j + Projectile.velocity * 0.2f).noGravity = true;
                    Dust.NewDustPerfect(Projectile.Center + new Vector2(i * 24 * Projectile.ai[1], 8), DustID.Torch, Vector2.UnitY * j + Projectile.velocity * 0.2f).noGravity = true;
                }
            }

        if (Projectile.ai[0] > 80 && Projectile.ai[0] < 300)
        {
            Projectile.ai[2] = Lerp(Projectile.ai[2], 0.5f, 0.1f);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.Center.FromAToB(player.Center + player.velocity * 10 - new Vector2(0, 100 + MathF.Sin(Projectile.ai[0] * 0.1f) * 10), false) * 0.1f, Projectile.ai[2]);
        }
        if (Projectile.ai[0] == 300)
        {
            Projectile.velocity = Vector2.Zero;
        }
        if (Projectile.ai[0] > 300 && Projectile.ai[0] < 320)
        {
            Projectile.ai[1] = Lerp(1, 0, InOutCirc.Invoke((Projectile.ai[0] - 300) / 20f));
            Projectile.velocity.Y = Lerp(Projectile.velocity.Y, -15, InOutCirc.Invoke((Projectile.ai[0] - 300) / 20f));
        }
        if (Projectile.ai[0] > 320)
        {
            if (Projectile.Center.Y > player.Top.Y - 10)
                Projectile.tileCollide = true;

            Projectile.velocity.Y = Lerp(Projectile.velocity.Y, 18, 0.2f);
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.ai[0] > 320)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.Center);
            SoundEngine.PlaySound(Sounds.genericExplosion.WithVolumeScale(0.5f), Projectile.Center);

            for (int i = 0; i < 30; i++)
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Stone, Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-10, 10));

            for (int i = 0; i < 10; i++)
                Gore.NewGoreDirect(null, Projectile.Center, Main.rand.NextVector2Circular(20, 20), GoreID.Smoke1 + Main.rand.Next(3));
            Projectile.NewProjectile(null, Helper.Raycast(Projectile.Center, Vector2.UnitY, 200).Point, Vector2.Zero, ProjectileType<GarbageImpact>(), 100, 0);
        }
        return Projectile.ai[0] > 320;
    }
}
public class MiseryGlobalProjectile : GlobalProjectile
{
    public override void OnSpawn(Projectile projectile, IEntitySource source)
    {
        if (projectile.type == ProjectileID.Boulder && Main.rand.NextBool(10))
        {
            int owner = -1;
            Dictionary<int, float> owners = new Dictionary<int, float>();
            foreach (Player p in Main.ActivePlayers)
            {
                if (!p.dead) owners.Add(owner, p.Distance(projectile.Center));
            }
            owner = owners.Values.IndexOfClosestTo(0);
            projectile.active = false;
            Projectile.NewProjectile(Projectile.InheritSource(projectile), projectile.Center, Vector2.Zero, ProjectileType<Misery>(), projectile.damage, projectile.knockBack, owner);
        }
    }
}
