using EbonianMod.Core.Systems.Verlets;
using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Cecitior;
using System.IO;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Crimson.BabyCecity;

public class BabyCecity : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Crimson/BabyCecity/"+Name;
    public override void SetDefaults()
    {
        NPC.width = 68;
        NPC.height = 76;
        NPC.damage = 15;
        NPC.defense = 18;
        NPC.lifeMax = 450;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = Sounds.fleshHit;
        NPC.value = 60f;
        NPC.knockBackResist = 0.5f;
        NPC.aiStyle = -1;
        NPC.buffImmune[BuffID.Confused] = true;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.buffImmune[BuffID.Ichor] = true;

    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return spawnInfo.Player.ZoneCrimson && Main.hardMode && spawnInfo.Player.ZoneOverworldHeight ? 0.035f : 0;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<CecitiorMaterial>(), 2, 1, 3));
        npcLoot.Add(ItemDropRule.Common(ItemID.Ichor, 1, 5, 15));
        npcLoot.Add(ItemDropRule.Common(ItemType<Items.Weapons.Melee.Spinax>(), 5));
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.BabyCecity.Bestiary"),
        });
    }
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 6;
    }
    public override void FindFrame(int frameHeight)
    {
        if (++NPC.frameCounter % 5 == 0)
        {
            if (NPC.frame.Y < frameHeight * 5)
                NPC.frame.Y += frameHeight;
            else
                NPC.frame.Y = 0;
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (!NPC.IsABestiaryIconDummy)
        {
            if (verlet[0] is null)
            {
                for (int i = 0; i < 2; i++)
                    verlet[i] = new Verlet(NPC.Center, 10, 42, gravity: -5f, lastPointLocked: true, stiffness: 40);
            }
            else
            {
                for (int i = 0; i < 2; i++)
                {
                    if (!Main.gamePaused)
                        for (int j = 0; j < 2; j++)
                            verlet[i].Update(NPC.Center, ogPos[i]);
                    verlet[i].Draw(spriteBatch, new VerletDrawData(new VerletTextureData(Helper.AssetPath+"NPCs/Crimson/BabyCecity/BabyCecity_Hook0", _endTex: Helper.AssetPath+"NPCs/Crimson/BabyCecity/BabyCecity_Hook2")));
                }
            }
        }
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
    Verlet[] verlet = new Verlet[2];
    Vector2[] dir = new Vector2[2];
    Vector2[] ogPos = new Vector2[2];
    Vector2 savedP;
    Vector2 savedP2;
    int seed = 55;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(seed);
        writer.WriteVector2(savedP);
        writer.WriteVector2(savedP2);
        for (int i = 0; i < 2; i++)
        {
            writer.WriteVector2(dir[i]);
            writer.WriteVector2(ogPos[i]);
        }
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        seed = reader.ReadInt32();
        savedP = reader.ReadVector2();
        savedP2 = reader.ReadVector2();
        for (int i = 0; i < 2; i++)
        {
            dir[i] = reader.ReadVector2();
            ogPos[i] = reader.ReadVector2();
        }
    }
    public override void OnSpawn(IEntitySource source)
    {
        NPC.Center = Helper.Raycast(NPC.Center, Vector2.UnitY, 1000).Point - Vector2.UnitY * Main.rand.NextFloat(200, 300);
        for (int i = 0; i < 2; i++)
        {
            dir[i] = -Helper.CircleDividedEqually(i + 1, 6).ToRotationVector2().RotatedBy(MathHelper.Pi);
            ogPos[i] = Helper.Raycast(NPC.Center, dir[i], 350).Point + Vector2.UnitY * 30;
        }
    }
    public override bool CheckDead()
    {
        if (Main.dedServ)
            return true;
        if (NPC.life <= 0)
        {
            for (int i = 0; i < 4; i++)
            {
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/Crimorrhage" + i).Type);
            }
            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < verlet[i].segments.Count; j++)
                    Gore.NewGore(NPC.GetSource_Death(), verlet[i].segments[j].pointA.position, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/CrimorrhageChain").Type);
            }
        }
        return true;
    }
    public override void AI()
    {
        Player player = Main.player[NPC.target];
        NPC.direction = NPC.velocity.X < 0 ? -1 : 1;
        if (Helper.Raycast(NPC.Center, Vector2.UnitY, 600).RayLength >= 580)
        {
            NPC.velocity.Y += 2;
            ogPos[0].Y = MathHelper.Lerp(ogPos[0].Y, Helper.Raycast(new Vector2(ogPos[0].X, NPC.Center.Y) - new Vector2(0, 100), Vector2.UnitY, 600).Point.Y + 16, MathHelper.SmoothStep(0.05f, 0.15f, (AITimer2 - 40) / 40));
            ogPos[1].Y = MathHelper.Lerp(ogPos[1].Y, Helper.Raycast(new Vector2(ogPos[1].X, NPC.Center.Y) - new Vector2(0, 100), Vector2.UnitY, 600).Point.Y + 16, MathHelper.SmoothStep(0.05f, 0.15f, (AITimer2 - 40) / 40));
        }
        NPC.rotation = NPC.Center.FromAToB(player.Center).ToRotation() - MathHelper.PiOver2;
        switch ((int)AIState)
        {
            case 0:
                NPC.TargetClosest(false);
                if (player.Distance(NPC.Center) > 1800) return;
                if (player.Center.Distance(NPC.Center) < 1300)
                    AITimer++;

                if (NPC.ai[3] == 0)
                    NPC.ai[3] = 1;

                UnifiedRandom rand = new(seed);
                float legOffset = 1;
                if (++AITimer2 < 80)
                {

                    if (AITimer2 > 30 && AITimer2 % 2 == 0)
                        NPC.velocity = Vector2.Lerp(NPC.velocity, NPC.Center.FromAToB(Helper.Raycast(player.Center + new Vector2(0, -200), Vector2.UnitY, 800).Point - new Vector2(0, 100)) * 4, 0.2f);
                    else
                        NPC.velocity *= 0.95f;
                    if (NPC.ai[3] == 1)
                    {
                        if (NPC.velocity.X > 0 && ogPos[1].X > NPC.Center.X + 30)
                            legOffset = 0.6f;
                        else if (NPC.velocity.X < 0 && ogPos[1].X < NPC.Center.X - 30)
                            legOffset = 0.6f;
                        else if (NPC.velocity.X > 0 && ogPos[1].X < NPC.Center.X - 30 && ogPos[0].X < NPC.Center.X - 30)
                            legOffset = 1.4f;
                        else if (NPC.velocity.X < 0 && ogPos[1].X > NPC.Center.X + 30 && ogPos[0].X > NPC.Center.X + 30)
                            legOffset = 1.4f;
                        if (Helper.FromAToB(ogPos[0], player.Center).X != 0)
                        {
                            if (AITimer2 < 40)
                                ogPos[0] += new Vector2(NPC.velocity.X * rand.NextFloat(4, 5) * legOffset + Helper.FromAToB(ogPos[0], player.Center).X, -MathHelper.SmoothStep(5, 2, AITimer2 / 50));
                            else
                            {
                                if (AITimer2 < 60)
                                    ogPos[0] += new Vector2((NPC.velocity.X * rand.NextFloat(4, 5) * legOffset + Helper.FromAToB(ogPos[0], player.Center).X) * MathHelper.SmoothStep(1, 0, (AITimer2 - 40) * 20 / 40), 0);

                                ogPos[0].Y = MathHelper.Lerp(ogPos[0].Y, Helper.Raycast(new Vector2(ogPos[0].X, NPC.Center.Y) - new Vector2(0, 100), Vector2.UnitY, 600).Point.Y + 16, MathHelper.SmoothStep(0.05f, 0.15f, (AITimer2 - 40) / 40));
                            }
                        }
                    }
                    else
                    {
                        if (NPC.velocity.X > 0 && ogPos[0].X > NPC.Center.X + 30)
                            legOffset = 0.6f;
                        else if (NPC.velocity.X < 0 && ogPos[0].X < NPC.Center.X - 30)
                            legOffset = 0.6f;
                        else if (NPC.velocity.X > 0 && ogPos[1].X < NPC.Center.X - 30 && ogPos[0].X < NPC.Center.X - 30)
                            legOffset = 1.4f;
                        else if (NPC.velocity.X < 0 && ogPos[1].X > NPC.Center.X + 30 && ogPos[0].X > NPC.Center.X + 30)
                            legOffset = 1.4f;
                        if (Helper.FromAToB(ogPos[1], player.Center).X != 0)
                        {
                            if (AITimer2 < 40)
                                ogPos[1] += new Vector2(NPC.velocity.X * rand.NextFloat(4, 5) * legOffset + Helper.FromAToB(ogPos[1], player.Center).X, -MathHelper.SmoothStep(5, 2, AITimer2 / 50));
                            else
                            {
                                if (AITimer2 < 60)
                                    ogPos[1] += new Vector2((NPC.velocity.X * rand.NextFloat(4, 5) * legOffset + Helper.FromAToB(ogPos[1], player.Center).X) * MathHelper.SmoothStep(1, 0, (AITimer2 - 40) * 2 / 40), 0);

                                ogPos[1].Y = MathHelper.Lerp(ogPos[1].Y, Helper.Raycast(new Vector2(ogPos[1].X, NPC.Center.Y) - new Vector2(0, 100), Vector2.UnitY, 600).Point.Y + 16, MathHelper.SmoothStep(0.05f, 0.15f, (AITimer2 - 40) / 40));
                            }
                        }
                    }
                }
                else
                {
                    NPC.netUpdate = true;
                    AITimer2 = 0;
                    NPC.ai[3] = -NPC.ai[3];
                }
                if ((int)AITimer2 % 80 == 0)
                {
                    NPC.netUpdate = true;
                    if (Main.netMode != NetmodeID.MultiplayerClient)
                        seed = Main.rand.Next(9999999);
                }

                if (ogPos[0].Distance(NPC.Center) > 600)
                {
                    ogPos[0] = Vector2.Lerp(ogPos[0], new Vector2(NPC.Center.X, Helper.Raycast(ogPos[0], Vector2.UnitY, 800).Point.Y + 16), 0.01f);
                }
                if (ogPos[1].Distance(NPC.Center) > 600)
                {
                    ogPos[1] = Vector2.Lerp(ogPos[1], new Vector2(NPC.Center.X, Helper.Raycast(ogPos[1], Vector2.UnitY, 800).Point.Y + 16), 0.01f);
                }
                if (AITimer >= 400)
                {
                    NPC.netUpdate = true;
                    AIState++;
                    AITimer = 0;
                    AITimer2 = 0;
                    if (MPUtils.NotMPClient)
                    {
                        if (player.Center.Distance(NPC.Center) < 500)
                            seed = Main.rand.Next(2);
                        else seed = 0;
                    }
                }
                break;
            case 1:
                AITimer++;
                if (seed > 1) return;

                if (ogPos[0].Distance(NPC.Center) > 600)
                {
                    ogPos[0] = Vector2.Lerp(ogPos[0], new Vector2(NPC.Center.X, Helper.Raycast(ogPos[0], Vector2.UnitY, 800).Point.Y + 16), 0.01f);
                }
                else
                    ogPos[0].Y = MathHelper.Lerp(ogPos[0].Y, Helper.Raycast(ogPos[0] - new Vector2(0, 400), Vector2.UnitY, 1200).Point.Y + 16, 0.1f);

                NPC.velocity *= 0.9f;
                if (seed == 0)
                {
                    if (ogPos[1].Distance(NPC.Center) > 600)
                    {
                        ogPos[1] = Vector2.Lerp(ogPos[1], new Vector2(NPC.Center.X, Helper.Raycast(ogPos[1], Vector2.UnitY, 800).Point.Y + 16), 0.01f);
                    }
                    else
                        ogPos[1].Y = MathHelper.Lerp(ogPos[1].Y, Helper.Raycast(ogPos[1] - new Vector2(0, 400), Vector2.UnitY, 1200).Point.Y + 16, 0.1f);

                    if (AITimer > 45)
                        AITimer2++;
                    else
                    {
                        Vector2 pos = NPC.Center + NPC.rotation.ToRotationVector2().RotatedBy(PiOver2).RotatedByRandom(PiOver4) * Main.rand.NextFloat(10, 100);
                        Dust.NewDustPerfect(pos, DustID.IchorTorch, Helper.FromAToB(pos, NPC.Center) * Main.rand.NextFloat(10, 20)).noGravity = true;
                    }
                    if (AITimer2 % 20 == 5)
                    {
                        SoundStyle sound = Sounds.bloodSpit;
                        SoundEngine.PlaySound(sound, NPC.Center);
                    }
                    if ((int)AITimer2 % 25 == 10)
                    {
                        Vector2 vel = NPC.Center.FromAToB(player.Center - new Vector2(0, Main.rand.NextFloat(100))).RotatedByRandom(0.3f);
                        for (int i = 0; i < 15; i++)
                        {
                            Dust.NewDustDirect(NPC.Center, NPC.width / 2, NPC.height / 2, DustID.IchorTorch, vel.X * Main.rand.NextFloat(5, 8), vel.Y * Main.rand.NextFloat(5, 8));
                        }
                        MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, vel * Main.rand.NextFloat(4, 9), ProjectileType<CIchor>(), 20, 0);
                    }
                }
                else
                {
                    if (AITimer < 20)
                    {
                        savedP = ogPos[1];
                        savedP2 = player.Center;
                        ogPos[1] = Vector2.Lerp(ogPos[1], player.Center - new Vector2(0, 200), 0.1f);
                    }
                    if ((int)AITimer == 40 && savedP.Distance(NPC.Center) < 1000)
                        MPUtils.NewProjectile(NPC.GetSource_FromAI(), savedP + Helper.FromAToB(savedP, savedP2) * 110, Helper.FromAToB(savedP, savedP2), ProjectileType<CecitiorClawSlash>(), 30, 0);

                    if (AITimer > 40 && AITimer < 65)
                    {
                        ogPos[1] = Vector2.Lerp(ogPos[1], savedP2 + Helper.FromAToB(savedP, savedP2) * 200, 0.1f);
                    }
                    if (AITimer > 65)
                    {
                        ogPos[1].Y = MathHelper.Lerp(ogPos[1].Y, Helper.Raycast(ogPos[1] - new Vector2(0, 300), Vector2.UnitY, 1000).Point.Y + 16, 0.1f);
                    }

                    if (AITimer >= 90)
                    {
                        NPC.netUpdate = true;
                        AIState = 0;
                        AITimer = 0;
                        AITimer2 = 0;
                        NPC.ai[3] = -NPC.ai[3];
                    }
                }
                if (AITimer >= 160)
                {
                    NPC.netUpdate = true;
                    AIState = 0;
                    AITimer = 0;
                    AITimer2 = 0;
                    NPC.ai[3] = -NPC.ai[3];
                }
                break;
        }
    }
}
