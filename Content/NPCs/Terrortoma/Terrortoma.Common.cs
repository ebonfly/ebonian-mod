using EbonianMod.Content.Bossbars;
using EbonianMod.Content.Items.Vanity;
using EbonianMod.Content.Items.BossTreasure;
using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Items.Misc;
using EbonianMod.Content.Items.Pets.Hightoma;
using EbonianMod.Content.Items.Tiles;
using EbonianMod.Content.Items.Tiles.Trophies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Terrortoma;


public partial class Terrortoma : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Terrortoma/"+Name;
    public const int PhaseTransition = 999, Idle = -2, Death = -1, Intro = 0, Dash = 1, DifferentClingerAttacks = 2, ClingerSlam = 3,
        CursedFlamesRain = 4, Pendulum = 5, ThrowUpVilethorns = 6, BodySlam = 7, Ostertagi = 8, BranchingFlame = 9, GeyserSweep = 10,
        EyeHomingFlames = 11, RangedHeadSlam = 12, CursedDollCopy = 13, ShadowOrbVomit = 14, TitteringSpawn = 15;
    public override void SetStaticDefaults()
    {
        NPCID.Sets.BossBestiaryPriority.Add(Type);
        NPCID.Sets.ShouldBeCountedAsBoss[Type] = true;
        Main.npcFrameCount[NPC.type] = 14;
        NPCID.Sets.TrailCacheLength[NPC.type] = 4;
        NPCID.Sets.TrailingMode[NPC.type] = 0;
    }


    public override void SetDefaults()
    {
        NPC.aiStyle = -1;
        NPC.lifeMax = 17500;
        if (Main.masterMode)
            NPC.lifeMax = 14000;
        NPC.boss = true;
        NPC.damage = 40;
        NPC.noTileCollide = true;
        NPC.defense = 42;
        NPC.value = Item.buyPrice(0, 10);
        NPC.knockBackResist = 0;
        NPC.width = 118;
        NPC.height = 106;
        NPC.rarity = 999;
        NPC.lavaImmune = true;
        NPC.noGravity = true;
        NPC.BossBar = GetInstance<TerrortomaBar>();
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
        Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EvilMiniboss");
        NPC.buffImmune[24] = true;
        NPC.buffImmune[BuffID.CursedInferno] = true;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.netAlways = true;
        NPC.dontTakeDamage = true;
    }


    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Cecitoma"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.Terrortoma.Bestiary"),
        });
    }


    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        LeadingConditionRule notExpertRule = new LeadingConditionRule(new Conditions.NotExpert());
        notExpertRule.OnSuccess(new CommonDrop(ItemType<CorruptEyeMaterial>(), 1, 40, 60));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<TerrortomaMaterial>(), 1, 10, 15));
        notExpertRule.OnSuccess(new CommonDrop(ItemType<Ostertagi>(), 1));
        npcLoot.Add(notExpertRule);

        npcLoot.Add(ItemDropRule.Common(ItemType<TTomaMask>(), 4));
        npcLoot.Add(ItemDropRule.Common(ItemType<TerrortomaTrophy>(), 4));
        npcLoot.Add(ItemDropRule.MasterModeDropOnAllPlayers(ItemType<CursedCone>(), 4));
        npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ItemType<TerrortomaRelic>()));
        npcLoot.Add(ItemDropRule.Common(ItemType<DappertomaI>(), 5));

        npcLoot.Add(ItemDropRule.BossBag(ItemType<TerrortomaBag>()));
    }


    public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
    {
        NPC.lifeMax = (int)(NPC.lifeMax * balance);
    }


    public int AIState
    {
        get => (int)NPC.ai[0];
        set => NPC.ai[0] = value;
    }

    public int AITimer
    {
        get => (int)NPC.ai[1];
        set => NPC.ai[1] = value;
    }
    public float SelectedClinger
    {
        get => NPC.ai[2];
        set => NPC.ai[2] = value;
    }
    public float AITimer2 = 0;
    public bool phase2 = false;
    public float rotation;
    public bool ded;
    public Vector2 lastPos;
    Player player => Main.player[NPC.target];


    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(rotation);
        writer.Write(ded);
        writer.Write(AITimer2);
        writer.Write(phase2);
        writer.WriteVector2(lastPos);
    }


    public override void ReceiveExtraAI(BinaryReader reader)
    {
        rotation = reader.ReadSingle();
        ded = reader.ReadBoolean();
        AITimer2 = reader.ReadSingle();
        phase2 = reader.ReadBoolean();
        lastPos = reader.ReadVector2();
    }
}
