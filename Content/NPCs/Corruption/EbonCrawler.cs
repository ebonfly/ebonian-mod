using EbonianMod.Content.NPCs.Corruption.Ebonflies;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Corruption;

public class EbonCrawler : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Corruption/"+Name;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 6;
    }
    public override bool? CanFallThroughPlatforms()
    {
        return Main.player[NPC.target].Center.Y < NPC.Center.Y;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.RottenChunk, 2, 1, 4));
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.EbonCrawler.Bestiary"),
        });
    }
    public override void SetDefaults()
    {
        NPC.width = 68;
        NPC.height = 38;
        NPC.damage = 36;
        NPC.defense = 4;
        NPC.lifeMax = 35;
        NPC.aiStyle = 3;
        AIType = 218;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.knockBackResist = 0.5f;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.value = Item.buyPrice(0, 0, 1);

    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        if (spawnInfo.Player.ZoneCorrupt)
        {
            return .09f;
        }
        else
        {
            return 0;
        }
    }
    public override bool CheckDead()
    {
        for (int i = 0; i < Main.rand.Next(3, 5); i++)
        {
            MPUtils.NewNPC(NPC.Center, NPCType<Ebonfly>(), ai3: 1);
        }
        if (Main.dedServ)
            return true;
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonCrawlerGore1").Type, NPC.scale);
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonCrawlerGore2").Type, NPC.scale);
        Gore.NewGore(NPC.GetSource_Death(), NPC.position, NPC.velocity, Find<ModGore>("EbonianMod/EbonCrawlerGore3").Type, NPC.scale);
        return true;
    }
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
    public float AITimer2
    {
        get => NPC.ai[2];
        set => NPC.ai[2] = value;
    }
    int walk = 0, attack = 1;
    public override void AI()
    {
        Player player = Main.player[NPC.target];
        NPC.spriteDirection = NPC.direction;
        if (AIState == walk)
        {
            if (player.Center.Distance(NPC.Center) < 100 && (player.Center.Y - NPC.Center.Y < 100 || player.Center.Y - NPC.Center.Y > -100) && NPC.collideY)
            {
                AITimer++;
                NPC.velocity.X *= 0.8f;
                if (AITimer >= 10)
                {
                    NPC.netUpdate = true;
                    AIState = attack;
                    NPC.velocity = Vector2.Zero;
                    AITimer = 0;
                }
            }
        }
        else
        {
            NPC.direction = player.Center.X < NPC.Center.X ? -1 : 1;
            NPC.velocity.X = 0;
            NPC.aiStyle = -1;
            AIType = -1;
            AITimer++;
            if (AITimer == 40)
            {
                NPC.netUpdate = true;
                NPC.velocity = new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 10, -3);
                NPC.aiStyle = 3;
                AIType = 218;
                AIState = walk;
                AITimer = 0;
            }
        }
    }
    public override void FindFrame(int frameHeight)
    {
        NPC.frameCounter++;
        if (NPC.IsABestiaryIconDummy)
        {
            if (NPC.frameCounter < 5)
            {
                NPC.frame.Y = 0 * frameHeight;
            }
            else if (NPC.frameCounter < 10)
            {
                NPC.frame.Y = 1 * frameHeight;
            }
            else if (NPC.frameCounter < 15)
            {
                NPC.frame.Y = 2 * frameHeight;
            }
            else if (NPC.frameCounter < 20)
            {
                NPC.frame.Y = 3 * frameHeight;
            }
            else
            {
                NPC.frameCounter = 0;
            }
        }
        else
        {
            if (AIState == walk)
            {
                if (NPC.collideY)
                    if (NPC.frameCounter < 5)
                    {
                        NPC.frame.Y = 0 * frameHeight;
                    }
                    else if (NPC.frameCounter < 10)
                    {
                        NPC.frame.Y = 1 * frameHeight;
                    }
                    else if (NPC.frameCounter < 15)
                    {
                        NPC.frame.Y = 2 * frameHeight;
                    }
                    else if (NPC.frameCounter < 20)
                    {
                        NPC.frame.Y = 3 * frameHeight;
                    }
                    else
                    {
                        NPC.frameCounter = 0;
                    }
            }
            else
            {
                if (NPC.frameCounter < 5)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                else if (NPC.frameCounter < 10)
                {
                    NPC.frame.Y = 5 * frameHeight;
                }
                else
                {
                    NPC.frameCounter = 0;
                }
            }
        }
    }
}