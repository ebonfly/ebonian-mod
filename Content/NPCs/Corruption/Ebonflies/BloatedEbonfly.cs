using EbonianMod.Content.Items.Misc;
using EbonianMod.Content.Items.Tiles;
using System.IO;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Corruption.Ebonflies;

public class BloatedEbonfly : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Corruption/Ebonflies/"+Name;
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<WaspPaintingI>(), 200));
    }
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 6;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.BloatedEbonfly.Bestiary"),
        });
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {

        Texture2D tex = Assets.NPCs.Corruption.Ebonflies.BloatedEbonfly_Glow.Value;
        Texture2D tex2 = TextureAssets.Npc[Type].Value;
        Texture2D tex3 = Assets.NPCs.Corruption.Ebonflies.BloatedEbonfly_Glow2.Value;
        SpriteEffects effects = NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Main.EntitySpriteDraw(tex2, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
        Main.EntitySpriteDraw(tex, NPC.Center - screenPos, NPC.frame, Color.White, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
        Main.spriteBatch.Reload(BlendState.Additive);
        Main.EntitySpriteDraw(tex3, NPC.Center - screenPos, NPC.frame, Color.LawnGreen * glowAlpha, NPC.rotation, NPC.Size / 2, NPC.scale, effects, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        if (spawnInfo.Player.ZoneCorrupt)
        {
            return .14f;
        }
        else
        {
            return 0;
        }
    }

    public override void SetDefaults()
    {
        NPC.aiStyle = 5;
        AIType = 205;
        NPC.width = 40;
        NPC.height = 38;
        NPC.npcSlots = 0.1f;
        NPC.lifeMax = 25;
        NPC.damage = 0;
        NPC.lavaImmune = true;
        NPC.noGravity = true;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        NPC.buffImmune[24] = true;
        NPC.noTileCollide = false;
        NPC.defense = 0;
    }
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.frameCounter % 5 == 0)
        {
            if (NPC.frame.Y < frameHeight * 5)
                NPC.frame.Y += frameHeight;
            else
                NPC.frame.Y = 0;
        }
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(AITimer);
        writer.WriteVector2(lastPos);
        writer.Write(glowAlpha);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        AITimer = reader.ReadSingle();
        lastPos = reader.ReadVector2();
        glowAlpha = reader.ReadSingle();
    }
    float glowAlpha = 0;
    Vector2 lastPos;
    float AITimer;
    public override void PostAI()
    {
        Player targetPlayer = Main.player[NPC.target];
        NPC.TargetClosest(false);
        if (NPC.ai[1] < 1 && Main.netMode == 0)
        {
            NPC.scale = Main.rand.NextFloat(0.8f, 1.2f);
            NPC.Center += Main.rand.NextVector2CircularEdge(40, 40);
            NPC.velocity = Main.rand.NextVector2Unit();
            NPC.ai[1] = 2;
        }
        if (NPC.ai[3] < -2)
            NPC.dontTakeDamage = true;
        foreach (NPC npc in Main.ActiveNPCs)
        {
            if (npc.active && npc.whoAmI != NPC.whoAmI)
            {
                if (npc.Center.Distance(NPC.Center) < npc.width * npc.scale)
                {
                    NPC.velocity += NPC.Center.FromAToB(npc.Center, true, true) * 0.5f;
                    NPC.netUpdate = true;
                }
                if (npc.Center == NPC.Center)
                {
                    NPC.velocity = Main.rand.NextVector2Unit() * 5;
                    NPC.netUpdate = true;
                }
            }
        }
        foreach (Player player in Main.ActivePlayers)
        {
            if (player.Center.Distance(NPC.Center) < 450 && NPC.ai[3] < 1)
            {
                NPC.ai[3] = 1;
                NPC.netUpdate = true;
            }
        }
        if (NPC.ai[3] < 1)
            lastPos = NPC.Center;
        
        if ((NPC.dontTakeDamage && ++AITimer > 60 * 8) || NPC.ai[3] > 0)
            if (++NPC.ai[3] > 100 * NPC.scale)
            {
                NPC.velocity *= 0.7f;
                if (NPC.ai[3] >= 120 && lastPos.Distance(NPC.Center) < 40)
                    NPC.Center = lastPos + Main.rand.NextVector2Circular(4 * glowAlpha, 4 * glowAlpha);
                else
                    lastPos = NPC.Center;
                glowAlpha += 0.03f;
                if (NPC.ai[3] > 150)
                {
                    if (MPUtils.NotMPClient)
                        Main.BestiaryTracker.Kills.RegisterKill(NPC);
                    MPUtils.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<HostileCorruptExplosion>(), 50, 0);
                    NPC.dontTakeDamage = false;
                    if (MPUtils.NotMPClient)
                        NPC.StrikeInstantKill();
                }
            }
        
        if (!NPC.dontTakeDamage && NPC.velocity.Length() < 6 && NPC.ai[3] < 1)
            NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.FromAToB(targetPlayer.Center, true) * .3f, 0.1f);
    }
    public override bool CheckDead()
    {
        if (Main.dedServ)
            return true;
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore").Type, NPC.scale);
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonFlyGore3").Type, NPC.scale);
        return true;
    }
}
