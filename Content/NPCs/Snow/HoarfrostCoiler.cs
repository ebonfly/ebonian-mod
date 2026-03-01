using EbonianMod.Content.Projectiles.Enemy.Snow;
using System;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Snow;


public class HoarfrostCoiler : WormHead
{
    public override string Texture => Helper.AssetPath + "NPCs/Snow/Coiler/"+Name;
    public override void SetStaticDefaults()
    {

        var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            CustomTexturePath = Helper.AssetPath+"NPCs/Snow/Coiler/HoarfrostCoilerBestiary",
            Position = new Vector2(0, 30f),
            PortraitPositionXOverride = 0f,
            PortraitPositionYOverride = 12f
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Construct"),
            new FlavorTextBestiaryInfoElement(NPC.BestiaryKey()),
        });
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return (spawnInfo.Player.ZoneSnow && (spawnInfo.Player.ZoneNormalUnderground || spawnInfo.Player.ZoneNormalCaverns)) || (spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneRain) ? 0.06f : 0;
    }
    public override bool byHeight => true;
    public override int BodyType => NPCType<HoarfrostCoilerBody>();
    public override int TailType => NPCType<HoarfrostCoilerTail>();
    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerHead);
        NPC.Size = new Vector2(34, 36);
        NPC.damage = 15;
        NPC.aiStyle = -1;
        NPC.lifeMax = 50;
        NPC.defense = 15;
        NPC.value = Item.buyPrice(0, 0, 2);
        NPC.HitSound = SoundID.Item49;
        NPC.DeathSound = SoundID.Item27;

    }

    public override bool CheckDead()
    {
        if (Main.dedServ)
            return base.CheckDead();
        for (int i = 0; i < 4; i++)
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(7,7), Find<ModGore>("EbonianMod/BorealDancer3").Type, NPC.scale);
        return base.CheckDead();
    }

    private bool collisionState;
    public override void ExtraAI()
    {
        Lighting.AddLight(NPC.Center, new Vector3(0f, .09f, .07f));
        Player player = Main.player[NPC.target];
        NPC.ai[3]--;
        if (collisionState != (Helper.Raycast(NPC.Center, NPC.velocity.SafeNormalize(Vector2.One), NPC.height*2).RayLength < NPC.height) 
            && NPC.ai[3] <= 0 && NPC.velocity.Y > 0 && NPC.Center.Y > player.Center.Y)
        {
            for (int i = -1; i< 2;i+=2)
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), Helper.Raycast(NPC.Center - new Vector2(0, 35), Vector2.UnitY, 300, true).Point + new Vector2(0, 3), Vector2.Zero, ProjectileType<BorealSpike>(), 15, 0, ai0: 2, ai1: i);

            SoundEngine.PlaySound(SoundID.Item1.WithPitchOffset(Main.rand.NextFloat(0f, 1f)), NPC.Center);
            NPC.ai[3] = 60;
        }
    }

    public override void PostAI()
    {
        base.PostAI();
        if (NPC.ai[3] % 2 == 0)
            collisionState = Helper.Raycast(NPC.Center, NPC.velocity.SafeNormalize(Vector2.One), NPC.height * 2).RayLength < NPC.height;
    }

    public override void Init()
    {
        MinSegmentLength = 6;
        MaxSegmentLength = 10;
        MoveSpeed = 12f;
        Acceleration = 0.1f;
    }
}
public class HoarfrostCoilerBody : WormBody
{
    public override string Texture => Helper.AssetPath + "NPCs/Snow/Coiler/"+Name;
    public override bool byHeight => true;
    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerHead);
        NPC.Size = new Vector2(30, 24);
        NPC.damage = 23;
        NPC.aiStyle = -1;
        NPC.lifeMax = 50;
        NPC.HitSound = SoundID.Item49;
        NPC.DeathSound = SoundID.Item27;

    }
    public override void HitEffect(NPC.HitInfo hitinfo)
    {
        if (Main.dedServ)
            return;
        if (hitinfo.Damage > NPC.life && NPC.life <= 0)
        {
            for (int i = 0; i < 4; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(7,7), Find<ModGore>("EbonianMod/BorealDancer3").Type, NPC.scale);
        }
    }
    public override void Init()
    {
        MoveSpeed = 12f;
        Acceleration = 0.1f;
    }
}
public class HoarfrostCoilerTail : WormTail
{
    public override string Texture => Helper.AssetPath + "NPCs/Snow/Coiler/"+Name;
    public override bool byHeight => true;
    public override void SetDefaults()
    {
        NPC.CloneDefaults(NPCID.DiggerHead);
        NPC.Size = new Vector2(22, 28);
        NPC.damage = 23;
        NPC.aiStyle = -1;
        NPC.lifeMax = 50;
        NPC.HitSound = SoundID.Item49;
        NPC.DeathSound = SoundID.Item27;

    }
    public override void HitEffect(NPC.HitInfo hitinfo)
    {
        if (Main.dedServ)
            return;
        if (hitinfo.Damage > NPC.life && NPC.life <= 0)
        {
            for (int i = 0; i < 4; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(7,7), Find<ModGore>("EbonianMod/BorealDancer3").Type, NPC.scale);
        }
    }
    public override void Init()
    {
        MoveSpeed = 12f;
        Acceleration = 0.1f;
    }
}
