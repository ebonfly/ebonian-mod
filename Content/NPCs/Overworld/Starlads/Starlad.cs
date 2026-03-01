using System;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Overworld.Starlads;
public class Starlad : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Starlads/"+Name;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 27;
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return ((spawnInfo.Player.ZoneNormalSpace ? 0.2f : 0.15f) + Star.starfallBoost * 0.05f)
            * ((spawnInfo.Player.ZoneForest || spawnInfo.Player.ZoneNormalSpace) && !Main.dayTime).ToInt();
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starspore>(), 3, 5, 35));
        npcLoot.Add(ItemDropRule.Common(ItemID.Mushroom, 5, 1, 4));
        npcLoot.Add(ItemDropRule.Common(ItemID.FallenStar, 10, 1, 3));
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Visuals.Moon,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Fungus"),
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey()),
            });
    }
    public override void SetDefaults()
    {
        NPC.height = 90;
        NPC.width = 68;
        NPC.damage = 1;
        NPC.friendly = false;
        NPC.lifeMax = 65;
        NPC.defense = 2;
        NPC.aiStyle = 0;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }
    public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
    {
        npcHitbox.Height = 40;
        npcHitbox.Width = 30;
        npcHitbox.Y += 45;
        npcHitbox.X += 20;
        return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
    }
    public override bool PreDraw(SpriteBatch sb, Vector2 screenPos, Color drawColor)
    {
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Vector2 origin = new Vector2(Assets.NPCs.Overworld.Starlads.Starlad.Value.Width / 2, NPC.height / 2);
        sb.Draw(Assets.NPCs.Overworld.Starlads.Starlad.Value, NPC.Center - screenPos + new Vector2(0, NPC.gfxOffY + 2), NPC.frame, Color.White, NPC.rotation, origin, NPC.scale, effects, 0f);
        return false;
    }
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (AIState == Idle)
        {
            if (Main.rand.NextBool(500) && NPC.frameCounter > 300)
            {
                NPC.frame.Y = 0;
                NPC.frameCounter = -400;
            }
            if (NPC.frameCounter % 5 == 0 && NPC.frameCounter > 0)
            {
                if (NPC.frame.Y < 4 * frameHeight)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        else if (AIState == Angry)
        {
            if (NPC.frameCounter % 5 == 0)
            {
                if (NPC.frame.Y > 4 * frameHeight && NPC.frame.Y < 19 * frameHeight)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 5 * frameHeight;
            }
        }
        else if (AIState == Attack)
        {
            if (NPC.frameCounter % 5 == 0)
            {
                if (NPC.frame.Y > 19 * frameHeight && NPC.frame.Y < 26 * frameHeight)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 20 * frameHeight;
            }
        }
    }
    private const int Idle = 0;
    private const int Angry = 1;
    private const int Attack = 2;
    public float AIState
    {
        get => NPC.ai[0];
        set => NPC.ai[0] = value;
    }

    public float AITimer
    {
        get => NPC.ai[1];
        set => NPC.ai[1] = value;
    }
    public override bool CheckDead()
    {
        Color newColor7 = Color.CornflowerBlue;
        for (int num613 = 0; num613 < 7; num613++)
        {
            Dust.NewDust(NPC.position, NPC.width, NPC.height, 58, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default, 0.8f);
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
        return true;
    }
    public override void AI()
    {
        Player player = Main.player[NPC.target];
        NPC.TargetClosest(true);
        Lighting.AddLight(NPC.Center, new Color(241, 212, 62).ToVector3() * 0.5f);
        if (NPC.life != NPC.lifeMax && NPC.ai[2] == 0)
        {
            NPC.frameCounter = 0;
            AIState = Angry;
            NPC.ai[2] = 1;
        }
        if (NPC.ai[2] == 0)
        {
            NPC.damage = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                if (Main.npc[i].active && Main.npc[i].type == ModContent.NPCType<Starlad>() && Main.npc[i].life < Main.npc[i].lifeMax)
                {
                    NPC.frameCounter = 0;
                    AIState = Angry;
                    NPC.ai[2] = 1;
                }
            }
        }
        if (AIState == Angry)
        {
            AITimer++;
            if (AITimer > 70)
            {
                AITimer = Main.rand.Next(-60, 0);
                AIState = Attack;
                NPC.damage = 15;
                NPC.frameCounter = 0;
                NPC.knockBackResist = 0f;
                NPC.noGravity = true;
                NPC.noTileCollide = true;
            }
        }
        else if (AIState == Attack)
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.active && npc.whoAmI != NPC.whoAmI)
                {
                    if (npc.Center.Distance(NPC.Center) < npc.width * npc.scale)
                    {
                        NPC.Center += NPC.Center.FromAToB(npc.Center, true, true);
                        NPC.netUpdate = true;
                    }
                    if (npc.Center == NPC.Center)
                    {
                        NPC.velocity = Main.rand.NextVector2Unit() * 5;
                        NPC.netUpdate = true;
                    }
                }
            }
            if (AITimer >= 60 || player.Distance(NPC.Center) < 400)
                AITimer++;
            if (AITimer < 60)
            {
                NPC.velocity = Helper.FromAToB(NPC.Center, player.Center + new Vector2(100 * MathF.Sign(NPC.Center.X - player.Center.X), (MathF.Sin(AITimer * 0.1f) + 1) * -20), false) / 15f;
            }
            if (NPC.velocity.Length() > 10f)
            {
                for (int num622 = 0; num622 < 4; num622++)
                {
                    Dust d = Main.dust[Dust.NewDust(NPC.Center, NPC.width / 3, NPC.height / 3, 57, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default, 1.2f)];
                    d.noGravity = true;
                }
            }
            if (AITimer == 60)
            {
                NPC.velocity = Vector2.Zero;
                Color newColor7 = Color.CornflowerBlue;
                for (int num613 = 0; num613 < 7; num613++)
                {
                    Dust.NewDust(NPC.position, NPC.width, NPC.height, 58, NPC.velocity.X * 0.1f, NPC.velocity.Y * 0.1f, 150, default, 0.8f);
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
                SoundEngine.PlaySound(SoundID.Item9, NPC.Center);
            }
            if (AITimer == 80)
            {
                SoundEngine.PlaySound(SoundID.Item9, NPC.Center);
                Vector2 vel = Helper.FromAToB(NPC.Center, player.Center);
                NPC.velocity = new Vector2(MathF.Sign(vel.X) * Main.rand.NextFloat(20, 30), vel.Y);
            }
            if (AITimer >= 165)
            {
                NPC.velocity *= 0.97f;
            }
            if (AITimer >= 175)
                AITimer = Main.rand.Next(-30, 0);
        }
    }
}
