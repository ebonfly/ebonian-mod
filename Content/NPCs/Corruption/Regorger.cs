using EbonianMod.Content.Projectiles.Enemy.Corruption;
using EbonianMod.Content.Projectiles.Terrortoma;
using System;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Corruption;

internal class Regorger : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Corruption/"+Name;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 7;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCorruption,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.Regorger.Bestiary"),
        });
    }
    public override bool? CanFallThroughPlatforms() => true;

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return spawnInfo.Player.ZoneCorrupt && Main.hardMode ? 0.05f : 0;
    }
    public override void SetDefaults()
    {
        NPC.width = 76;
        NPC.height = 50;
        NPC.damage = 10;
        NPC.defense = 7;
        NPC.lifeMax = 150;
        NPC.aiStyle = -1;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.value = 60f;
        NPC.knockBackResist = 0.5f;
        NPC.noGravity = true;
        NPC.buffImmune[BuffID.CursedInferno] = true;
        NPC.value = Item.buyPrice(0, 0, 1);
        NPC.noTileCollide = true;

    }
    public override void FindFrame(int frameHeight)
    {
        if (NPC.frameCounter++ % 5 == 0)
        {
            if (AIState == 2 || NPC.IsABestiaryIconDummy)
            {
                if (NPC.frame.Y < 4 * frameHeight)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
                if ((NPC.frame.Y += frameHeight) > 6 * frameHeight)
                {
                    NPC.frame.Y = 4 * frameHeight;
                }
            }
            else
            {
                if ((NPC.frame.Y += frameHeight) > 3 * frameHeight)
                {
                    NPC.frame.Y = 0;
                }
            }
        }
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
    public override void HitEffect(NPC.HitInfo hitinfo)
    {
        if (Main.dedServ)
            return;
        if (hitinfo.Damage > NPC.life && NPC.life <= 0)
        {
            for (int i = 0; i < 5; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/EbonFlyGore2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CorruptionBrickGibs2").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CorruptionBrickGibs4").Type, NPC.scale);
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(5, 5), Find<ModGore>("EbonianMod/CorruptionBrickGibs0").Type, NPC.scale);
            }
        }
    }
    Vector2 p;
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.CursedFlame, 2, 1, 3));
    }
    public override void OnSpawn(IEntitySource source)
    {
        if (NPC.ai[3] >= 1)
        {
            NPC.velocity = Main.rand.NextVector2Circular(10, 10);
            NPC.lifeMax *= 6;
            NPC.life = NPC.lifeMax;
            NPC.netUpdate = true;
        }
    }
    public override void AI()
    {
        if (NPC.ai[3] != 0)
        {
            NPC.life--;
            if (NPC.life < 2) NPC.SimpleStrikeNPC(3, 0);
        }
        Player player = Main.player[NPC.target];
        NPC.TargetClosest(false);
        if (NPC.Distance(player.Center) > 1700) return;
        NPC.spriteDirection = NPC.direction = -1;
        switch (AIState)
        {
            case 0:
                AITimer++;
                NPC.damage = 0;
                if (AITimer < 200)
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 3.5f, 0.01f);
                if (AITimer > 200)
                    NPC.velocity *= 0.9f;
                if (AITimer > 1)
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0), 0.25f);
                if (AITimer >= 230)
                {
                    AITimer2 = Main.rand.Next(4);
                    AITimer = 0;
                    AIState++;
                }
                break;
            case 1:
                AITimer++;
                if (AITimer < 50)
                {
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center + new Vector2(0, -200).RotatedBy(MathF.Sin(AITimer + Main.GlobalTimeWrappedHourly * 3) * 1.5f), false) / 40, 0.02f);
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, Helper.FromAToB(NPC.Center, player.Center).ToRotation() + (NPC.direction == -1 ? MathHelper.Pi : 0), 0.25f);
                }

                if (AITimer == 50)
                {
                    NPC.velocity = Vector2.Zero;
                    p = player.Center;
                    //MPUtils.NewProjectileServerSide(null, NPC.Center, Helper.FromAToB(NPC.Center, player.Center), ProjectileType<RegorgerTelegraph>(), 0, 0);
                    SoundEngine.PlaySound(Sounds.bloodSpit.WithPitchOffset(0.25f), NPC.Center);
                }

                if (AITimer == 90)
                {
                    NPC.velocity = -Helper.FromAToB(NPC.Center, p) * 4;
                    for (int i = 0; i < 15; i++)
                        Dust.NewDustPerfect(NPC.Center, DustID.CursedTorch, Helper.FromAToB(NPC.Center, p).RotatedByRandom(MathHelper.PiOver2) * Main.rand.NextFloat(3, 6));
                    MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, p), ProjectileType<RegorgerBolt>(), 20, 0);

                    for (int i = -1; i < 2; i++)
                    {
                        MPUtils.NewProjectile(null, NPC.Center, Helper.FromAToB(NPC.Center, p).RotatedBy(i * .3f) * 2, ProjectileType<TFlameThrower>(), 20, 0);
                    }
                }

                if (AITimer > 90)
                    NPC.velocity *= 0.9f;

                if (AITimer > 120)
                {
                    AITimer = 0;
                    AIState++;
                    NPC.frame.Y = 4 * 52;
                }
                break;
            case 2:
                AITimer++;
                if (AITimer == 1)
                {
                    SoundEngine.PlaySound(Sounds.terrortomaLaugh.WithPitchOffset(1f), NPC.Center);
                }
                if (AITimer > 60)
                {
                    AITimer2++;
                    if (AITimer2 < 4)
                    {
                        AIState = 1;
                    }
                    else
                    {
                        AIState = 0;
                    }
                    AITimer = 0;
                }
                break;
        }
    }
}
