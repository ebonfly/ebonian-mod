using EbonianMod.Common.Misc;
using EbonianMod.Content.Items.Accessories;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using System;
using System.IO;
using Terraria.GameContent.Bestiary;
using Terraria.Map;

namespace EbonianMod.Content.NPCs.Overworld.Asteroid;
public class AsteroidWarden : CommonNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Asteroid/"+Name;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 4;
    }
    public override void SetDefaults()
    {
        NPC.Size = new Vector2(32);
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.damage = 20;
        NPC.defense = 5;
        NPC.lifeMax = 250;
        NPC.value = Item.buyPrice(0, 0, 5, 15);
        NPC.aiStyle = -1;
        NPC.knockBackResist = 0.6f;
        NPC.HitSound = SoundID.NPCHit13;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
        {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Sky,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Construct"),
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
        });
    }
    public Vector2 handOffset;
    public Vector2 leftHandOff, rightHandOff;
    public float leftHandRot, rightHandRot;
    public float headRot, starRot, starScale = 1;
    public int handFrameY;
    int next = 1;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(next);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        next = reader.ReadInt32();
    }
    public override void FindFrame(int frameHeight)
    {
        if (++NPC.frameCounter % 5 == 0)
        {
            if (NPC.frame.Y < frameHeight * 3)
                NPC.frame.Y += frameHeight;
            else
                NPC.frame.Y = 0;
        }
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(15, ItemType<WardingStar>()));
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            SoundEngine.PlaySound(SoundID.Item14, NPC.Center);
            Color newColor7 = Color.CornflowerBlue;
            for (int num613 = 0; num613 < 7; num613++)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Enchanted_Pink, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default, 0.8f);
            }
            for (float num614 = 0f; num614 < 1f; num614 += 0.125f)
            {
                Dust.NewDustPerfect(NPC.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
            }
            for (float num615 = 0f; num615 < 1f; num615 += 0.25f)
            {
                Dust.NewDustPerfect(NPC.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
            }
            Vector2 vector52 = new Vector2(Main.screenWidth, Main.screenHeight);
            if (NPC.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector52 / 2f, vector52 + new Vector2(400f))))
            {
                for (int num616 = 0; num616 < 7; num616++)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * NPC.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                }
            }
            Helper.SpawnGore(NPC, "EbonianMod/Warden", vel: -Vector2.UnitY * 3);
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (NPC.IsABestiaryIconDummy)
        {
            handOffset = new Vector2(-25, 0);
            AITimer3 = Pi;
        }
        Player player = Main.player[NPC.target];
        Texture2D tex = TextureAssets.Npc[Type].Value; // https://cdn.discordapp.com/attachments/795335225034670100/1114973113281675367/u23t27yzz0y21.png
        Texture2D head = Assets.NPCs.Overworld.Asteroid.AsteroidWardenHead.Value;
        Texture2D hand = Assets.NPCs.Overworld.Asteroid.AsteroidWardenHand.Value;
        Texture2D handGlow = Assets.NPCs.Overworld.Asteroid.AsteroidWardenHand_Glow.Value;
        Texture2D headGlow = Assets.NPCs.Overworld.Asteroid.AsteroidWardenHead_Glow.Value;
        Texture2D glow = Assets.NPCs.Overworld.Asteroid.AsteroidWarden_Glow.Value;

        Vector2 handOrig = new Vector2(18 / 2, 20);
        Vector2 finalHandOff = new Vector2(0, 10).RotatedBy(AITimer3 + Pi);
        spriteBatch.Draw(hand, NPC.Center - screenPos + new Vector2(AIState == 2 ? handOffset.X : -handOffset.X, -handOffset.Y) + leftHandOff - finalHandOff, new Rectangle(0, handFrameY * 24, 18, 24),
            drawColor * NPC.Opacity, AITimer3 + leftHandRot, handOrig, NPC.scale, AITimer3 < 0 && handFrameY != 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        spriteBatch.Draw(handGlow, NPC.Center - screenPos + new Vector2(AIState == 2 ? handOffset.X : -handOffset.X, -handOffset.Y) + leftHandOff - finalHandOff, new Rectangle(0, handFrameY * 24, 18, 24),
            Color.White * NPC.Opacity, AITimer3 + leftHandRot, handOrig, NPC.scale, AITimer3 < 0 && handFrameY != 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, drawColor * NPC.Opacity, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        spriteBatch.Draw(glow, NPC.Center - screenPos, NPC.frame, Color.White * NPC.Opacity, NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        spriteBatch.Draw(head, NPC.Center - new Vector2(0, 25) - screenPos, null, drawColor * NPC.Opacity, headRot, head.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        spriteBatch.Draw(headGlow, NPC.Center - new Vector2(0, 25) - screenPos, null, Color.White * NPC.Opacity, headRot, head.Size() / 2, NPC.scale, SpriteEffects.None, 0);

        spriteBatch.Draw(hand, NPC.Center - screenPos + new Vector2(handOffset.X, handOffset.Y) + rightHandOff - finalHandOff, new Rectangle(0, handFrameY * 24, 18, 24),
            drawColor * NPC.Opacity, AITimer3 + rightHandRot, handOrig, NPC.scale, AITimer3 < 0 && handFrameY != 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);

        spriteBatch.Draw(handGlow, NPC.Center - screenPos + new Vector2(handOffset.X, handOffset.Y) + rightHandOff - finalHandOff, new Rectangle(0, handFrameY * 24, 18, 24),
            Color.White * NPC.Opacity, AITimer3 + rightHandRot, handOrig, NPC.scale, AITimer3 < 0 && handFrameY != 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
        return false;
    }
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D vortex = Assets.Extras.vortex3.Value;
        Texture2D star = Assets.ExtraSprites.Projectiles.WardingStarP_Extra.Value;
        if (AIState == 2)
        {
            spriteBatch.Draw(vortex, NPC.Center + new Vector2(handOffset.X, 0) - screenPos, null, Color.CornflowerBlue with { A = 0 } * AITimer2 * 0.5f, Main.GlobalTimeWrappedHourly * 0.5f, vortex.Size() / 2, 0.15f, SpriteEffects.None, 0);
            spriteBatch.Draw(vortex, NPC.Center + new Vector2(handOffset.X, 0) - screenPos, null, Color.CornflowerBlue with { A = 0 } * AITimer2 * 0.5f, Main.GlobalTimeWrappedHourly * -0.5f, vortex.Size() / 2, 0.15f, SpriteEffects.None, 0);
            spriteBatch.Draw(star, NPC.Center + new Vector2(handOffset.X, 0) - screenPos, null, Color.White with { A = 0 } * AITimer2 * 0.5f * MathF.Pow(starScale, 3), -starRot, star.Size() / 2, starScale, SpriteEffects.None, 0);
            star = Assets.Projectiles.Friendly.Generic.WardingStarP.Value;
            spriteBatch.Draw(star, NPC.Center + new Vector2(handOffset.X, 0) - screenPos, null, Color.White with { A = 0 } * AITimer2 * 0.5f * MathF.Pow(starScale, 3), starRot, star.Size() / 2, starScale, SpriteEffects.None, 0);
        }
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return (Star.starfallBoost > 2 && !Main.dayTime && spawnInfo.Player.ZoneNormalSpace) ? 0.1f : 0;
    }
    public override void AI()
    {
        Lighting.AddLight(NPC.Center, new Vector3(195, 169, 13) / 255 * 0.5f);
        Player player = Main.player[NPC.target];
        AITimer2 = Clamp(AITimer2, 0, 1f);
        starScale = Lerp(starScale, 1, 0.1f);
        if (AIState == 0)
        {
            NPC.TargetClosest(false);
            handOffset = Vector2.Lerp(handOffset, new Vector2(-25, 0), 0.1f);
            rightHandOff = Vector2.Lerp(rightHandOff, Vector2.Zero, 0.1f);
            leftHandOff = Vector2.Lerp(leftHandOff, Vector2.Zero, 0.1f);
            rightHandRot = Utils.AngleLerp(rightHandRot, 0, 0.1f);
            leftHandRot = Utils.AngleLerp(leftHandRot, 0, 0.1f);
            AITimer3 = Utils.AngleLerp(AITimer3, Pi, 0.1f);
            headRot = Utils.AngleLerp(headRot, Helper.FromAToB(NPC.Center - new Vector2(0, 25), player.Center).ToRotation() - Pi, 0.1f);
            if (AITimer > 15 || (AITimer < 0 && AITimer > -80))
                handFrameY = 0;
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            if (!Main.dayTime && player.Center.Distance(NPC.Center) < 1050)
                AITimer++;
            else
            {
                NPC.velocity.Y -= 0.5f;
                if (++AITimer > 35)
                {
                    NPC.velocity *= 0.8f;
                    NPC.Opacity = Lerp(NPC.Opacity, 0, 0.2f);
                    if (AITimer == 36)
                    {
                        SoundStyle style = SoundID.AbigailSummon;
                        style.Volume = 0.5f;
                        SoundEngine.PlaySound(style, NPC.Center);
                        Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + NPC.velocity * 4, Vector2.Zero, ProjectileType<WardenSigil>(), 0, 0);
                    }
                    if (AITimer >= 45)
                        NPC.active = false;
                }
                return;
            }

            if (player.Center.Distance(NPC.Center) < 200)
                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(100, 0).RotatedBy(Helper.FromAToB(NPC.Center, player.Center, true, true).ToRotation()), false) / 100, 0.05f);
            else
                NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(100, 0).RotatedBy(Helper.FromAToB(NPC.Center, player.Center, true, true).ToRotation()), true) * 10, 0.05f);

            if (AITimer > 200)
            {

                NPC.velocity = Vector2.Zero;
                AITimer = 0;
                AIState = next;
                NPC.netUpdate = true;
            }
        }
        else if (AIState == 1)
        {
            NPC.velocity *= 0.98f;
            AITimer++;
            if (AITimer < 30)
            {
                handOffset = Vector2.Lerp(handOffset, new Vector2(-40, 0), 0.2f);
                if (AITimer == 1)
                    SoundEngine.PlaySound(SoundID.Item117, NPC.Center);
                if (AITimer % 3 == 0)
                    Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 25), Vector2.Zero, ProjectileType<MagicChargeUp>(), 0, 0, -1, 4, 0, 1);
                handFrameY = 1;
            }
            if (AITimer > 80)
            {
                AITimer3 = Utils.AngleLerp(AITimer3, -Pi, 0.1f);
                leftHandRot = Utils.AngleLerp(leftHandRot, 4, 0.02f);
                rightHandRot = Utils.AngleLerp(rightHandRot, -4, 0.02f);
                rightHandOff = Vector2.Lerp(rightHandOff, Vector2.UnitY.RotatedBy(.7f) * -50 - Vector2.UnitY * 30, 0.01f);
                leftHandOff = Vector2.Lerp(leftHandOff, Vector2.UnitY.RotatedBy(-.7f) * -50 - Vector2.UnitY * 30, 0.01f);
            }
            if (AITimer < 70)
                headRot = Lerp(headRot, Main.GameUpdateCount * 0.005f, 0.1f);
            else
                headRot = Lerp(headRot, 0, 0.05f);
            if (AITimer == 80)
            {
                SoundStyle style = SoundID.AbigailSummon;
                style.Volume = 0.5f;
                SoundEngine.PlaySound(style, NPC.Center);
                Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -140), Vector2.Zero, ProjectileType<WardenSigil>(), 0, 0);
            }
            if (AITimer > 180)
            {
                NPC.velocity = Vector2.Zero;
                AITimer = -100;
                AIState = 0;
                next = 2;
                NPC.netUpdate = true;
            }
        }
        else
        {
            NPC.velocity *= 0.98f;
            headRot = Lerp(headRot, Main.GameUpdateCount * 0.005f, 0.05f);
            AITimer++;
            if (AITimer < 11)
                AITimer2 += 0.1f;
            else if (AITimer > 175)
                AITimer2 -= 0.2f;
            starRot += MathHelper.ToRadians(NPC.localAI[0] * 0.075f);
            NPC.localAI[0] += 1.5f;

            handOffset = Vector2.Lerp(handOffset, new Vector2(20 * NPC.direction, 15), 0.05f);
            AITimer3 = Lerp(AITimer3, PiOver2 * NPC.direction, 0.1f);
            handFrameY = 1;
            if (AITimer >= 40 && AITimer < 150)
            {
                if (AITimer % 30 == 25)
                {
                    SoundStyle style = SoundID.Item82;
                    style.Volume = 0.5f;
                    SoundEngine.PlaySound(style, NPC.Center);
                    Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(20 * NPC.direction, 0), Helper.FromAToB(NPC.Center, player.Center) * 0.1f, ProjectileType<WardenStar>(), 10, 0, ai2: 1);
                }
                if (AITimer % 30 == 20)
                {
                    NPC.localAI[0] = 0;
                    starScale = 1.25f;
                }
            }
            if (AITimer >= 180)
            {
                AITimer = 0;
                AITimer2 = 0;
                NPC.localAI[0] = 0;
                AIState = 0;
                if (MPUtils.NotMPClient)
                {
                    next = Main.rand.NextBool(3) ? 1 : 2;
                    NPC.netUpdate = true;
                }
            }
        }
    }
}
public class WardenStar : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Asteroid/"+Name;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
    }
    public override void SetDefaults()
    {
        Projectile.damage = 10;
        Projectile.width = 26;
        Projectile.height = 60;
        Projectile.aiStyle = 0;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 500;
    }

    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, new Vector3(195, 169, 13) / 255 * 0.35f);
        Projectile.rotation = Projectile.velocity.ToRotation() + PiOver2;
        float progress = Utils.GetLerpValue(0, 500, Projectile.timeLeft);
        Projectile.scale = Clamp((float)Math.Sin(progress * Math.PI) * 5, 0, 1);

        if (Projectile.velocity.Length() < 20f)
            Projectile.velocity *= 1.1f;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        if (Projectile.timeLeft >= 499)
            return false;
        Texture2D drawTexture = TextureAssets.Projectile[Projectile.type].Value;
        Rectangle sourceRectangle = new(0, 0, drawTexture.Width, drawTexture.Height);
        Main.EntitySpriteDraw(drawTexture, Projectile.Center - Main.screenPosition, sourceRectangle, Color.White with { A = 0 } * Projectile.scale, Projectile.rotation, drawTexture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

        //3hi31mg
        var off = new Vector2(Projectile.width / 2, Projectile.height / 2);
        var texture = TextureAssets.Projectile[Projectile.type].Value;
        var frame = new Rectangle(0, Projectile.frame, Projectile.width, Projectile.height);
        var orig = frame.Size() / 2f;
        var trailLength = ProjectileID.Sets.TrailCacheLength[Projectile.type];

        frame = new Rectangle(0, 0, 2, 40);
        for (int i = 1; i < trailLength; i++)
        {
            float scale = Lerp(0.70f, 1f, (float)(trailLength - i) / trailLength);
            var fadeMult = 1f / trailLength;
            for (float j = 0; j < 10; j++)
            {
                Vector2 pos = Vector2.Lerp(i == 0 ? Projectile.position : Projectile.oldPos[i - 1], Projectile.oldPos[i], j / 10f);
                Main.spriteBatch.Draw(texture, pos - Main.screenPosition + off, frame, Color.White with { A = 0 } * Projectile.scale * (1f - fadeMult * i), Projectile.oldRot[i], frame.Size() / 2, scale * Projectile.scale, SpriteEffects.None, 0f);
            }
        }
        return false;
    }
}
public class WardenSigil : ModProjectile // Hell torture
{
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Asteroid/"+Name;
    public override void SetDefaults()
    {
        Projectile.damage = 0;
        Projectile.width = 26;
        Projectile.timeLeft = 121;
        Projectile.height = 60;
        Projectile.aiStyle = 0;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
    }
    public override bool ShouldUpdatePosition()
    {
        return Projectile.ai[2] >= 4;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D vortex = Assets.Extras.vortex3.Value;
        Texture2D tex = TextureAssets.Projectile[Type].Value;
        Color col = Color.Lerp(Color.White, Color.Gold, 0.5f) with { A = 0 } * 0.5f;
        if (Projectile.ai[2] < 2)
        {
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, col, -Main.GlobalTimeWrappedHourly * -1.5f, tex.Size() / 2, 0.2f * Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, col, -Main.GlobalTimeWrappedHourly * 0.29f, tex.Size() / 2, 0.3f * Projectile.scale, SpriteEffects.None, 0);
        }

        float alpha = MathF.Sin(sigilStartup * Pi);
        if (Projectile.ai[2] >= 3)
        {
            Main.spriteBatch.Reload(BlendState.Additive);
            Main.spriteBatch.Reload(Effects.SpriteRotation.Value);
            Effects.SpriteRotation.Value.Parameters["rotation"].SetValue(Main.GlobalTimeWrappedHourly * (Projectile.whoAmI % 2 == 0 ? -2 : 2) + Projectile.whoAmI * 7);
            Effects.SpriteRotation.Value.Parameters["scale"].SetValue(new Vector2(1, 0.25f) * Projectile.ai[1]);
            col = Color.Lerp(Color.White, Color.Gold, 0.5f);
            if (Projectile.ai[2] == 5)
                col = Color.Lerp(Color.White, Color.Crimson, 0.5f);
            Effects.SpriteRotation.Value.Parameters["uColor"].SetValue(col.ToVector4() * alpha);
        }
        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, col * alpha, 0, tex.Size() / 2, Projectile.ai[2] >= 4 ? 0.6f : 0.3f * Projectile.scale + (Projectile.ai[2] >= 4 ? 0 : (sigilStartup * .4f * (Projectile.ai[2] == 2 ? Projectile.ai[1] : 1))), SpriteEffects.None, 0);
        if (Projectile.ai[2] >= 3)
        {
            Main.spriteBatch.Reload(effect: null);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
        }
        return false;
    }
    public override void OnKill(int timeLeft)
    {
        if (Projectile.ai[2] < 1)
            SoundEngine.PlaySound(SoundID.Item10, Projectile.Center);
    }
    int maxTime = 0;
    float sigilStartup = 0;
    public override void AI()
    {
        if (maxTime == 0)
        {
            if (Projectile.ai[2] < 2)
                SoundEngine.PlaySound(Sounds.reiTP.WithPitchOffset(-0.7f), Projectile.Center);
            Projectile.scale = Projectile.ai[2] == 4 ? 1 : 0;
            maxTime = Projectile.timeLeft;
        }
        else
        {
            sigilStartup = Lerp(sigilStartup, 1, 0.12f * (Projectile.ai[2] >= 4 ? Projectile.ai[1] * 0.5f : 1));
            if (Projectile.ai[2] < 4)
            {
                float progress = Utils.GetLerpValue(0, maxTime, Projectile.timeLeft);
                Projectile.scale = Clamp((float)Math.Sin(progress * Math.PI) * (maxTime < 200 ? 3 : 5), 0, InOutElastic.Invoke(progress * 2));
            }
            else
                Projectile.velocity *= 0.94f;
        }
        Lighting.AddLight(Projectile.Center, new Vector3(195, 169, 13) / 255 * 0.35f);
        if (Projectile.ai[2] < 2)
        {
            if (Projectile.ai[0] == 25)
            {
                Color newColor7 = Color.CornflowerBlue;
                for (int num613 = 0; num613 < 7; num613++)
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Enchanted_Pink, Projectile.velocity.X * 0.1f, Projectile.velocity.Y * 0.1f, 150, default, 0.8f);
                }
                for (float num614 = 0f; num614 < 1f; num614 += 0.125f)
                {
                    Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num614 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (4f + Main.rand.NextFloat() * 4f), 150, newColor7).noGravity = true;
                }
                for (float num615 = 0f; num615 < 1f; num615 += 0.25f)
                {
                    Dust.NewDustPerfect(Projectile.Center, 278, Vector2.UnitY.RotatedBy(num615 * ((float)Math.PI * 2f) + Main.rand.NextFloat() * 0.5f) * (2f + Main.rand.NextFloat() * 3f), 150, Color.Gold).noGravity = true;
                }
                Vector2 vector52 = new Vector2(Main.screenWidth, Main.screenHeight);
                if (Projectile.Hitbox.Intersects(Utils.CenteredRectangle(Main.screenPosition + vector52 / 2f, vector52 + new Vector2(400f))))
                {
                    for (int num616 = 0; num616 < 7; num616++)
                    {
                        Gore.NewGore(Projectile.GetSource_FromThis(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Utils.SelectRandom(Main.rand, 16, 17, 17, 17, 17, 17, 17, 17));
                    }
                }
            }
            if (Projectile.ai[0] % (Projectile.ai[2] == 1 ? 15 : 5) == 0 && Projectile.ai[0] < (maxTime < 200 ? maxTime - 40 : maxTime - 120))
                Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<MagicChargeUp>(), 0, 0, -1, 13, 0.5f);
        }

        if ((++Projectile.ai[0] == 50 || Projectile.ai[0] == 65 || Projectile.ai[0] == 80) && Projectile.ai[2] == 0)
        {
            SoundStyle style = SoundID.Item82;
            style.Volume = 0.5f;
            SoundEngine.PlaySound(style, Projectile.Center);
            for (int i = 0; i < 5; i++)
            {
                Projectile.ai[1] += Pi / 2.5f;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0.05f, 0).RotatedBy(Projectile.ai[1]), ProjectileType<WardenStar>(), 10, 0);
            }
            Projectile.ai[1] += PiOver4 * 0.8f;
        }
    }
}