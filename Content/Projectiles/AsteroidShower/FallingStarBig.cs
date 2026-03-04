
using EbonianMod.Content;
using EbonianMod.Content.Items.Accessories;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.NPCs.Overworld.Asteroid;
using EbonianMod.Core.Systems.Cinematic;
using System;
using System.Collections.Generic;

namespace EbonianMod.Content.Projectiles.AsteroidShower;

public class FallingStarBig : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/AsteroidShower/"+Name;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontAttachHideToAlpha[Type] = true;
    }
    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.friendly = true;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 400;
        Projectile.light = 1f;
        Projectile.penetrate = -1;
        Projectile.hide = true;
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        behindNPCsAndTiles.Add(index);
    }

    public override void PostDraw(Color lightColor)
    {
        Texture2D tex = TextureAssets.Extra[ExtrasID.FallingStar].Value;
        if (Projectile.ai[0] == 0)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = Clamp(Projectile.velocity.Length(), 0, 1f);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(3).RotatedBy(Projectile.rotation * 3) - Main.screenPosition, null, Color.CornflowerBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(3).RotatedBy(-Projectile.rotation * 3) - Main.screenPosition, null, Color.CornflowerBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Cyan * ((Projectile.ai[2] - 0.5f) * 0.5f * alpha), Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), Projectile.ai[2], SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Cyan * 0.25f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(10).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.DarkSlateBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(10).RotatedBy(-Projectile.rotation) - Main.screenPosition, null, Color.DarkSlateBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        Main.spriteBatch.Reload(BlendState.Additive);
        Texture2D tex2 = TextureAssets.Projectile[Type].Value;
        Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * ((MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) + 1) / 2), Projectile.rotation, tex2.Size() / 2, 1f, SpriteEffects.None, 0);

        Main.spriteBatch.Reload(BlendState.AlphaBlend);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        for (int i = 0; i < 4; i++)
        {
            float angle = Helper.CircleDividedEqually(i, 4) + Main.GlobalTimeWrappedHourly * 3;
            Vector2 pos = Projectile.Center + new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 6).RotatedBy(angle);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.CornflowerBlue * 0.6f, Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None, 0);
        }
        for (int i = 0; i < 4; i++)
        {
            float angle = Helper.CircleDividedEqually(i, 4) - Main.GlobalTimeWrappedHourly * 3;
            Vector2 pos = Projectile.Center - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 6).RotatedBy(angle);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.CornflowerBlue * 0.6f, Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return true;
    }
    public override void AI()
    {
        if (Projectile.ai[1]++ % 30 == 0 && Projectile.ai[0] == 0)
            SoundEngine.PlaySound(SoundID.Item9.WithPitchOffset(Main.rand.NextFloat(-0.2f, -0.15f)), Projectile.Center);
        if (Main.dayTime) Projectile.Kill();
        Projectile.ai[2] = Clamp(Projectile.ai[2] + 0.05f, 1f, 1.5f);
        if (Projectile.ai[2] > 1.49f)
            Projectile.ai[2] = 1f;
        Projectile.timeLeft = 10;
        Projectile.rotation += ToRadians(Math.Clamp(Projectile.velocity.Length(), 0, 5));
        if (Projectile.ai[0] > 0)
        {
            Projectile.velocity *= 0.75f;
            if (Helper.Raycast(Projectile.Center, Vector2.UnitY, 60).RayLength > 40)
                Projectile.velocity.Y = 1f;
            else
                Projectile.velocity.Y = 0;
        }

        if (Projectile.ai[0] == 0 && Helper.Raycast(Projectile.Center, Vector2.UnitY, 60).RayLength < 40)
        {
            Projectile.damage = 0;
            Projectile.Center += Projectile.velocity * 0.5f;
            NPC.NewNPCDirect(Projectile.GetSource_FromThis(), Projectile.Center, NPCType<FallingStar>(), ai0: Projectile.whoAmI);
            CameraSystem.ScreenShakeAmount = 6.5f;
            SoundEngine.PlaySound(Sounds.bigassstar with { PitchVariance = 0.15f }, Projectile.Center);
            Projectile.velocity *= 0.35f;
            for (int num905 = 0; num905 < 10; num905++)
            {
                Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
                int num906 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 0, default(Color), 2.5f);
                Main.dust[num906].position = Projectile.position + Vector2.UnitX.RotatedByRandom(3.1415927410125732) * Projectile.Size.X / 2f;
                Main.dust[num906].noGravity = true;
                Dust dust2 = Main.dust[num906];
                dust2.velocity *= 3f;
            }
            Gore.NewGorePerfect(null, Projectile.Center, Main.rand.NextVector2Circular(10, 10), GoreID.PirateShip10);
            for (int num899 = 0; num899 < 4; num899++)
            {
                Gore.NewGorePerfect(null, Projectile.Center, Main.rand.NextVector2Circular(10, 10), GoreID.Smoke1);
                Gore.NewGorePerfect(null, Projectile.Center, Main.rand.NextVector2Circular(10, 10), GoreID.Smoke2);
                Gore.NewGorePerfect(null, Projectile.Center, Main.rand.NextVector2Circular(10, 10), GoreID.Smoke3);
                int num900 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 100, default(Color), 1.5f);
                Main.dust[num900].position = Projectile.position + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * Projectile.Size.X / 2f;
            }
            Color newColor7 = Color.CornflowerBlue;
            for (int num613 = 0; num613 < 15; num613++)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 0.8f);
            }
            for (float num614 = 0f; num614 < 2; num614 += 0.125f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (9f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
            }
            for (float num615 = 0f; num615 < 2f; num615 += 0.25f)
            {
                Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (6f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
            Vector2 vector52 = new Vector2(Main.screenWidth, Main.screenHeight);
            if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector52 / 2f, vector52 + new Vector2(400f))))
            {
                for (int num616 = 0; num616 < 10; num616++)
                {
                    Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
            Projectile.tileCollide = false;
            Projectile.ai[0] = 1;
        }
    }
    public override void OnKill(int timeLeft)
    {
        for (int num905 = 0; num905 < 10; num905++)
        {
            int num906 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 0, default(Color), 2.5f);
            Main.dust[num906].position = Projectile.position + Vector2.UnitX.RotatedByRandom(3.1415927410125732) * Projectile.Size.X / 2f;
            Main.dust[num906].noGravity = true;
            Dust dust2 = Main.dust[num906];
            dust2.velocity *= 3f;
        }
        for (int num899 = 0; num899 < 4; num899++)
        {
            int num900 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 100, default(Color), 1.5f);
            Main.dust[num900].position = Projectile.position + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * Projectile.Size.X / 2f;
        }
        Color newColor7 = Color.CornflowerBlue;
        for (int num613 = 0; num613 < 15; num613++)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 0.8f);
        }
        for (float num614 = 0f; num614 < 2; num614 += 0.125f)
        {
            Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (9f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
        }
        for (float num615 = 0f; num615 < 2f; num615 += 0.25f)
        {
            Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (6f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
        }
        Vector2 vector52 = new Vector2(Main.screenWidth, Main.screenHeight);
        if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector52 / 2f, vector52 + new Vector2(400f))))
        {
            for (int num616 = 0; num616 < 10; num616++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
            }
        }

        bool closeToAnyPlayer = false;

        foreach (var player in Main.ActivePlayers)
        {
            if (player.Distance(Projectile.Center) < 800)
            {
                closeToAnyPlayer = true;
                break;
            }
        }
        
        if (Main.rand.NextBool(20) && closeToAnyPlayer) 
        {
            NPC.NewNPCDirect(null, Projectile.Center, NPCType<AsteroidHerder>());
            SoundEngine.PlaySound(Sounds.reiTP.WithPitchOffset(-0.7f), Projectile.Center);
            MPUtils.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<WardenSigil>(), 0, 0, ai1: 1, ai2: 2);
        }
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        return false;
    }
}
public class FallingStar : ModNPC
{
    public override string Texture => Helper.Empty;
    public override void SetStaticDefaults()
    {
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, new NPCID.Sets.NPCBestiaryDrawModifiers() { Hide = true });
    }
    public override void SetDefaults()
    {
        NPC.width = 50;
        NPC.height = 50;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.defense = 30;
        NPC.lifeMax = 200;
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0f;
        NPC.chaseable = false;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.FallenStar, 1, 10, 20));
        npcLoot.Add(ItemDropRule.OneFromOptions(2, ItemType<StarBit>(), ItemType<WardingStar>()));
    }

    List<int> GoreTypes = new List<int>()
        {
            GoreType<StarG0>(),GoreType<StarG1>(),GoreType<StarG2>(),GoreType<StarG3>(),GoreType<StarG4>(),GoreType<StarG5>(),GoreType<StarG6>(),
        };
    public override bool CheckDead()
    {
        Projectile projectile = Main.projectile[(int)NPC.ai[0]];
        projectile.Kill();
        SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, NPC.Center);
        SoundEngine.PlaySound(SoundID.NPCHit3, NPC.Center);
        for (int i = 0; i < 10; i++)
            Gore.NewGore(default, NPC.Center, Main.rand.NextVector2Circular(10, 10), Main.rand.Next(GoreTypes));
        return true;
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        SoundEngine.PlaySound(SoundID.DD2_CrystalCartImpact, NPC.Center);
        if (Main.rand.NextBool(3))
            Gore.NewGore(default, NPC.Center, Main.rand.NextVector2Circular(10, 10), Main.rand.Next(GoreTypes));
        Color newColor7 = Color.CornflowerBlue;
        if (Main.rand.NextBool(3))
        {
            for (int num613 = 0; num613 < 5; num613++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, 58, 4, 4, 150, default, 0.8f);
            }
            for (float num614 = 0f; num614 < 1; num614 += 0.125f)
            {
                Dust.NewDustPerfect(NPC.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
            }
            for (float num615 = 0f; num615 < 0.75f; num615 += 0.25f)
            {
                Dust.NewDustPerfect(NPC.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
        }
    }
    public override void AI()
    {
        Projectile projectile = Main.projectile[(int)NPC.ai[0]];
        if (!projectile.active || projectile.type != ProjectileType<FallingStarBig>())
        { 
             return;
        }

        NPC.timeLeft = 10;
        NPC.Center = projectile.Center;
    }
}


public class FallingStarBigHostile : ModProjectile
{
    public override string Texture => Helper.AssetPath+"Projectiles/AsteroidShower/FallingStarBig";
    public override void SetDefaults()
    {
        Projectile.width = 50;
        Projectile.height = 50;
        Projectile.aiStyle = -1;
        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;
        Projectile.timeLeft = 400;
        Projectile.light = 1f;
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;
    }
    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
    public override void PostDraw(Color lightColor)
    {
        Texture2D tex = TextureAssets.Extra[ExtrasID.FallingStar].Value;
        if (Projectile.ai[0] == 0)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            float alpha = Clamp(Projectile.velocity.Length(), 0, 1f);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(3).RotatedBy(Projectile.rotation * 3) - Main.screenPosition, null, Color.Crimson * 0.5f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(3).RotatedBy(-Projectile.rotation * 3) - Main.screenPosition, null, Color.CornflowerBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 2f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Crimson * ((Projectile.ai[2] - 0.5f) * 0.5f * alpha), Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), Projectile.ai[2], SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Crimson * 0.25f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1.5f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(10).RotatedBy(Projectile.rotation) - Main.screenPosition, null, Color.Crimson * 0.5f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(10).RotatedBy(-Projectile.rotation) - Main.screenPosition, null, Color.CornflowerBlue * 0.5f * alpha, Projectile.velocity.ToRotation() + PiOver2, new Vector2(tex.Width / 2, tex.Height / 4), 1f, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        Main.spriteBatch.Reload(BlendState.Additive);
        Texture2D tex2 = TextureAssets.Projectile[Type].Value;
        Main.spriteBatch.Draw(tex2, Projectile.Center - Main.screenPosition, null, Color.White * ((MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) + 1) / 2), Projectile.rotation, tex2.Size() / 2, 1f, SpriteEffects.None, 0);

        Main.spriteBatch.Reload(BlendState.AlphaBlend);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        for (int i = 0; i < 4; i++)
        {
            float angle = Helper.CircleDividedEqually(i, 4) + Main.GlobalTimeWrappedHourly * 3;
            Vector2 pos = Projectile.Center + new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 6).RotatedBy(angle);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.CornflowerBlue * 0.6f, Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None, 0);
        }
        for (int i = 0; i < 4; i++)
        {
            float angle = Helper.CircleDividedEqually(i, 4) - Main.GlobalTimeWrappedHourly * 3;
            Vector2 pos = Projectile.Center - new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 1.5f) * 6).RotatedBy(angle);
            Main.spriteBatch.Draw(tex, pos - Main.screenPosition, null, Color.Crimson * 0.6f, Projectile.rotation, tex.Size() / 2, 1f, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return true;
    }
    public override void AI()
    {
        if (Projectile.ai[1]++ % 30 == 0 && Projectile.ai[0] == 0)
            SoundEngine.PlaySound(SoundID.Item9.WithPitchOffset(Main.rand.NextFloat(-0.2f, -0.15f)), Projectile.Center);
        Projectile.ai[2] = Clamp(Projectile.ai[2] + 0.05f, 1f, 1.5f);
        if (Projectile.ai[2] > 1.49f)
            Projectile.ai[2] = 1f;
        Projectile.timeLeft = 10;
        Projectile.rotation += ToRadians(Math.Clamp(Projectile.velocity.Length(), 0, 5));
    }
    public override void OnKill(int timeLeft)
    {
        Projectile.Center += Projectile.velocity;
        CameraSystem.ScreenShakeAmount = 5f;
        SoundEngine.PlaySound(Sounds.bigassstar with { PitchVariance = 0.15f }, Projectile.Center);
        for (int num905 = 0; num905 < 10; num905++)
        {
            int num906 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 0, default(Color), 2.5f);
            Main.dust[num906].position = Projectile.position + Vector2.UnitX.RotatedByRandom(3.1415927410125732) * Projectile.Size.X / 2f;
            Main.dust[num906].noGravity = true;
            Dust dust2 = Main.dust[num906];
            dust2.velocity *= 3f;
        }
        for (int num899 = 0; num899 < 4; num899++)
        {
            int num900 = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), (int)Projectile.Size.X, (int)Projectile.Size.Y, 31, 0f, 0f, 100, default(Color), 1.5f);
            Main.dust[num900].position = Projectile.position + Vector2.UnitY.RotatedByRandom(3.1415927410125732) * (float)Main.rand.NextDouble() * Projectile.Size.X / 2f;
        }
        Color newColor7 = Color.CornflowerBlue;
        for (int num613 = 0; num613 < 15; num613++)
        {
            Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, 58, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 150, default, 0.8f);
        }
        for (float num614 = 0f; num614 < 2; num614 += 0.125f)
        {
            Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (9f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
        }
        for (float num615 = 0f; num615 < 2f; num615 += 0.25f)
        {
            Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (6f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
        }
        Vector2 vector52 = new Vector2(Main.screenWidth, Main.screenHeight);
        if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector52 / 2f, vector52 + new Vector2(400f))))
        {
            for (int num616 = 0; num616 < 10; num616++)
            {
                Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
            }
        }
    }
}