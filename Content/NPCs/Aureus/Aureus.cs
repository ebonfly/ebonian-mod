using EbonianMod.Core.Systems.Boss;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using Terraria.Graphics.CameraModifiers;
using Terraria.Graphics.Effects;

namespace EbonianMod.Content.NPCs.Aureus;

// https://github.com/CalamityTeam/CalamityModPublic/tree/1.4.4/NPCs/AstrumAureus
[AutoloadBossHead]
public class Aureus : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/" + Name;
    public static SoundStyle chipsSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/chips") with { PitchVariance = 0.2f };
    public static SoundStyle deathSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/death");
    public static SoundStyle hurtSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/hurt") with { MaxInstances = 4, PitchVariance = 0.2f };
    public static SoundStyle jumpSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/jump") with { MaxInstances = 4, PitchVariance = 0.2f };
    public static SoundStyle mangoSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/mango");
    public static SoundStyle loopSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/loop");
    public static SoundStyle gunshotsound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/gunshot");
    public static SoundStyle missileSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/missile") with { MaxInstances = 4, PitchVariance = 0.2f };
    public static SoundStyle splodeSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/splode") with { Variants = [0, 1], MaxInstances = 5, PitchVariance = 0.2f };
    public static SoundStyle mineSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/mineplace");
    public static SoundStyle jumpscareSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/jumpscareSound");
    public static SoundStyle MagnetSphereSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/AwesomeSauce");
    public static SoundStyle primSlopSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/primSlop") with { MaxInstances = 6, PitchVariance = 0.2f };
    public static SoundStyle pewSound => new SoundStyle("EbonianMod/Assets/Sounds/Aureus/pew") with { MaxInstances = 6, PitchVariance = 0.2f };

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 6;
    }

    public override void SetDefaults()
    {
        NPC.Size = new Vector2(88, 70);
        NPC.lifeMax = ushort.MaxValue / 4;
        NPC.defense = 30;
        NPC.boss = true;
        NPC.HitSound = hurtSound.WithVolumeScale(2);
        NPC.knockBackResist = 0f;
        NPC.aiStyle = -1;
        NPC.BossBar = GetInstance<AureusBossBar>();
    }
    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsOverPlayers.Add(index);
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return true;
    }

    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = Request<Texture2D>(Helper.AssetPath+"NPCs/Aureus/Aureus_Glow").Value;

        int frameHeight = texture.Height / Main.npcFrameCount[NPC.type];

        Rectangle sourceRectangle = new Rectangle(0, NPC.frame.Y, texture.Width, frameHeight);

        Vector2 origin = sourceRectangle.Size() / 2f;
        Vector2 position = NPC.Center - Main.screenPosition + new Vector2(0f, NPC.gfxOffY);

        Color color = NPC.GetAlpha(Color.White);

        Main.EntitySpriteDraw(texture, position, sourceRectangle, color, NPC.rotation, origin, NPC.scale, SpriteEffects.None, 0);
    }

    public override void FindFrame(int frameHeight)
    {
        NPC.frame.Y = (NPC.frame.Y < frameHeight * 5 ? NPC.frame.Y + frameHeight : 0);
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
    public float AITimer3
    {
        get => NPC.ai[3];
        set => NPC.ai[3] = value;
    }
    float nextAttack = Stomp;
    bool doneAttacksBefore;
    float amountPerformed;
    public override bool CheckDead()
    {
        if (!dead)
        {
            Music = 0;
            Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 20, 30, 30, 2000));
            AIState = Death;
            AITimer = 0;
            AITimer2 = 0;
            AITimer3 = 0;
            NPC.life = NPC.lifeMax;
            NPC.dontTakeDamage = true;
            NPC.immortal = true;
            NPC.velocity = Vector2.Zero;
            SoundEngine.PlaySound(deathSound);
            dead = true;
            return false;
        }
        return true;
    }
    bool dead;
    const int PhaseTrans = -3, Death = -2, Idle = -1, Intro = 0, Stomp = 1, MagnetSphere = 2, PrimSlopLaser = 3,
        XBeams = 4, HugeProjSpam = 5, DoGWalls = 6, Galaxy = 7, Spread = 8, Missiles = 9, Vortex = 10,
        ZigZag = 11, Planets = 12;
    public override void AI()
    {
        if (!Main.dedServ)
        {
            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/Aureus");
        }

        NPC.TargetClosest(false);
        Player player = Main.player[NPC.target];
        IdleStuff();
        NPC.scale = Lerp(0.5f, 4, (float)NPC.life / NPC.lifeMax);
        AITimer++;
        switch ((int)AIState)
        {
            case PhaseTrans:
                {
                    doneAttacksBefore = false;
                    amountPerformed = 0;
                }
                break;
            case Death:
                Dust d = Dust.NewDustPerfect(NPC.position - new Vector2(0, 150), DustType<AureusDust2>(), Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 20), 100, default, Lerp(.2f, 2, AITimer / 160));
                d.noGravity = true;
                d.customData = 1;
                if (AITimer == 30 || AITimer == 90 || AITimer == 130)
                {
                    ConglomerateSystem.conglomerateSkyFlash = 0.5f;
                    for (int k = 0; k < 50; k++)
                    {
                        Dust dd = Dust.NewDustPerfect(NPC.position - new Vector2(0, 150), DustType<AureusDust2>(), Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 20), 100, default, Lerp(.2f, 2, AITimer / 110));
                        dd.noGravity = true;
                        dd.customData = 1;
                    }

                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 20, 30, 30, 2000));
                }
                if (AITimer == 149)
                {
                    ConglomerateSystem.conglomerateSkyFlash = 10f;
                    var entitySource = NPC.GetSource_Death();

                    if (!Main.dedServ)
                        for (int k = 0; k < 4; k++)
                        {
                            int gore = Mod.Find<ModGore>("AureusGore" + k).Type;

                            Gore.NewGore(entitySource, NPC.position - new Vector2(0, 100), new Vector2(Main.rand.Next(-6, 7), Main.rand.Next(-6, 7)), gore);
                        }

                    for (int k = 0; k < 200; k++)
                    {
                        Dust ddd = Dust.NewDustPerfect(NPC.position, DustType<AureusDust2>(), Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 20), 100, default, 4);
                        ddd.noGravity = true;
                        ddd.customData = 1;
                    }

                    PunchCameraModifier modifier = new PunchCameraModifier(NPC.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 40f, 6f, 20, 1000f, FullName);
                    Main.instance.CameraModifiers.Add(modifier);

                    for (int k = 0; k < Main.maxProjectiles; k++)
                    {
                        if (Main.projectile[k].type == ModContent.ProjectileType<AureusMine>() || Main.projectile[k].type == ModContent.ProjectileType<AureusLaser>())
                        {
                            Main.projectile[k].Kill();
                        }
                    }
                    NPC.dontTakeDamage = false;
                    NPC.immortal = false;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 30, 30, 30, 2000));
                    NPC.StrikeInstantKill();
                }
                break;
            case Intro:
                int startMove = Stomp;
                AIState = startMove;
                nextAttack = startMove;
                AITimer = 0;
                break;
            case Idle:
                NPC.dontTakeDamage = false;
                NPC.damage = 0;
                NPC.rotation = Lerp(NPC.rotation, 0, 0.35f);
                NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;

                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                if (NPC.Grounded(offsetX: 0.5f) && (NPC.collideX || Helper.Raycast(NPC.Center, Vector2.UnitX, 1000).RayLength < NPC.width || Helper.Raycast(NPC.Center, -Vector2.UnitX, 1000).RayLength < NPC.width))
                    NPC.velocity.Y = -10;
                if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 300)
                    NPC.velocity.Y = -20;
                else if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 200)
                    NPC.velocity.Y = -15;
                else if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 100)
                    NPC.velocity.Y = -10;

                if (Helper.Raycast(NPC.Center, -Vector2.UnitY, NPC.height).RayLength < NPC.height - 1 && !Collision.CanHit(NPC, player))
                {
                    if (!NPC.noTileCollide)
                    {
                        NPC.noTileCollide = true;
                        NPC.netUpdate = true;
                    }

                    if (player.Center.Y < NPC.Center.Y)
                        NPC.Center -= Vector2.UnitY * 2;
                    else
                        NPC.Center += Vector2.UnitY * 2;

                    NPC.Center += new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 2, 0);
                }
                else if ((!Collision.CanHit(NPC, player) || !Collision.CanHitLine(NPC.TopLeft, 10, 10, player.position, player.width, player.height) || !Collision.CanHitLine(NPC.TopRight, 10, 10, player.position, player.width, player.height)) && player.Center.X.InRange(NPC.Center.X, NPC.width))
                {
                    if (!NPC.noTileCollide)
                    {
                        NPC.noTileCollide = true;
                        NPC.netUpdate = true;
                    }

                    if (player.Center.Y < NPC.Center.Y)
                        NPC.Center -= Vector2.UnitY * 2;
                    else
                        NPC.Center += Vector2.UnitY * 2;
                }
                else if (NPC.noTileCollide)
                {
                    NPC.noTileCollide = false;
                    NPC.netUpdate = true;
                }
                NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.1f, 0.12f);
                if (player.Distance(NPC.Center) < 70)
                    AITimer += 1;
                if (player.Distance(NPC.Center) < 40)
                    AITimer += 1;

                if (AITimer > 60)
                {
                    amountPerformed++;
                    if (amountPerformed > Main.rand.Next(2, 5))
                    {
                        nextAttack++;
                        if (nextAttack > Planets)
                        {
                            doneAttacksBefore = true;
                            nextAttack = Stomp;
                        }
                        if (doneAttacksBefore)
                            nextAttack = Main.rand.Next(Stomp, Planets + 1);
                        amountPerformed = 0;
                    }
                    AIState = nextAttack;
                    AITimer = 0;
                    AITimer2 = 0;
                    AITimer3 = 0;
                    NPC.velocity = Vector2.Zero;
                }
                break;
            case Stomp:
                if (AITimer3 == 0 && NPC.Grounded())
                {
                    NPC.noGravity = true;
                    Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 10, 30, 30, 2000));
                    SoundEngine.PlaySound(jumpSound, NPC.Center);
                    SoundEngine.PlaySound(Sounds.eggplosion, NPC.Center);
                    Projectile.NewProjectile(null, Helper.Raycast(NPC.Center, Vector2.UnitY, 100).Point, Vector2.Zero, ProjectileType<AImpact>(), 0, 0);
                    NPC.velocity = Vector2.UnitY * -30;
                    AITimer3 = 1;
                }
                if (AITimer3 > 0)
                {
                    AITimer3++;
                    if (AITimer3 > 1 && AITimer3 < 15)
                        NPC.Center += NPC.velocity * 0.1f;
                    if (AITimer3 >= 15 && AITimer3 < 45)
                        NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(-player.velocity.X, 400), false) / 2;
                    if (AITimer3 == 45)
                    {
                        SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
                        NPC.velocity = new Vector2(0, 35);
                    }
                    if (AITimer3 > 45 && NPC.Center.Y > player.Center.Y - NPC.width * 0.4f)
                    {
                        NPC.noTileCollide = false;
                        NPC.damage = 100;
                    }
                    if (AITimer3 > 45 && !NPC.collideY && NPC.noTileCollide)
                    {
                        NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-1, 1);
                        NPC.velocity.Y += 0.05f;
                        NPC.Center += NPC.velocity * 0.5f;
                    }
                    if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 0.5f)) && AITimer2 == 0 && AITimer3 >= 45)
                    {
                        NPC.damage = 0;
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 20, 30, 30, 2000));
                        NPC.noGravity = false;
                        SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                        NPC.velocity = -Vector2.UnitY * 3;
                        for (int i = -10; i < 10; i++)
                        {
                            MPUtils.NewProjectile(NPC.InheritSource(NPC), Helper.Raycast(NPC.Center - new Vector2(i * 150, 400), Vector2.UnitY, 1000).Point, Vector2.Zero, ProjectileType<AImpact>(), 50, 0);

                            if (i != 0)
                            {
                                MPUtils.NewProjectile(NPC.InheritSource(NPC), Helper.Raycast(NPC.Center - new Vector2(i * 150, 400), Vector2.UnitY, 1000).Point, new Vector2(0, -20), ProjectileType<AureusLaser>(), 100, 0);
                            }
                        }
                        ProjSpam();
                        AITimer2 = 1;
                    }
                }
                if (AITimer2 > 0)
                {
                    AITimer2++;
                    if (AITimer2 > 40)
                    {
                        AIState = Idle;
                        AITimer = 0;
                        AITimer3 = 0;
                        AITimer2 = 0;
                    }
                }
                break;
            case MagnetSphere:
                {
                    SoundEngine.PlaySound(MagnetSphereSound.WithVolumeScale(2));

                    MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center + new Vector2(200).RotatedByRandom(TwoPi), Vector2.Zero, ProjectileType<MagnetSphere>(), 0, 0);

                    AIState = Idle;
                    AITimer = 0;
                    AITimer3 = 0;
                    AITimer2 = 0;
                }
                break;
            case Spread:
                {
                    SoundEngine.PlaySound(gunshotsound.WithVolumeScale(0.4f), NPC.Center);

                    for (int i = -6; i < 7; i++)
                        MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, NPC.DirectionTo(player.Center).RotatedBy(i * 0.1f) * 10, ProjectileType<AureusLaser>(), 50, 0);

                    AIState = Idle;
                    AITimer = 40;
                    AITimer3 = 0;
                    AITimer2 = 0;
                }
                break;
            case Missiles:
                {
                    if (AITimer <= 60 && AITimer % 5 == 0)
                    {
                        SoundEngine.PlaySound(missileSound, NPC.Center);
                        Projectile a = MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-15, -7)) * 0.5f, ProjectileType<AureusMissile>(), 15, 0, player.whoAmI);
                        if (a is not null)
                        {
                            a.timeLeft = 200;
                            a.SyncProjectile();
                        }
                    }
                    if (AITimer % 3 == 0 && AITimer > 100)
                    {
                        if (AITimer % 6 == 0)
                            SoundEngine.PlaySound(missileSound, NPC.Center);
                        MPUtils.NewProjectile(NPC.GetSource_FromThis(), new Vector2(player.Center.X + 600 * Main.rand.NextFloat(-1, 1), player.Center.Y - 600), new Vector2(Main.rand.NextFloat(-1, 1), 2) * 0.5f, ProjectileType<AureusMissile>(), 15, 0, player.whoAmI);
                    }
                    if (AITimer > 160)
                    {
                        AIState = Idle;
                        AITimer = 0;
                        AITimer3 = 0;
                        AITimer2 = 0;
                    }
                }
                break;
            case HugeProjSpam:
                {
                    if (AITimer == 5 || (AITimer > 30 && AITimer % 3 == 0))
                        Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2Circular(20, 20), ProjectileType<AureusLaser>(), 20, 0);

                    if (AITimer > 90)
                    {
                        AIState = Idle;
                        AITimer = 0;
                        AITimer3 = 0;
                        AITimer2 = 0;
                    }
                }
                break;
            case DoGWalls:
                {
                    if (AITimer == 1)
                    {
                        float off = 0;

                        SoundEngine.PlaySound(primSlopSound.WithPitchOffset(1), NPC.Center);
                        for (int i = -10; i < 10; i++)
                        {
                            Projectile.NewProjectile(null, player.Center + new Vector2(i * 100, -700).RotatedBy(off), Vector2.UnitY.RotatedBy(off) * 0.1f, ProjectileType<AureusLaser>(), 50, 0, ai2: 1);
                        }
                        for (int j = -10; j < 10; j++)
                        {
                            Projectile.NewProjectile(null, player.Center + new Vector2(700, j * 100).RotatedBy(off), -Vector2.UnitX.RotatedBy(off) * 0.1f, ProjectileType<AureusLaser>(), 50, 0, ai2: 1);
                        }
                    }
                    if (AITimer > 50)
                    {
                        AIState = Idle;
                        AITimer = -100;
                        AITimer3 = 0;
                        AITimer2 = 0;
                    }
                }
                break;
            case Galaxy:
                {
                    if (AITimer == 30)
                    {
                        SoundEngine.PlaySound(missileSound.WithPitchOffset(-1f), NPC.Center);
                        Projectile.NewProjectile(null, player.Center - new Vector2(400, 300), Vector2.Zero, ProjectileType<AureusGalaxy>(), 60, 0);
                    }
                    if (AITimer > 170)
                    {
                        AIState = Idle;
                        amountPerformed = 99; // dont duplicate
                        AITimer = 0;
                        AITimer3 = 0;
                        AITimer2 = 0;
                    }
                }
                break;
            case PrimSlopLaser:
                {
                    if (AITimer == 1)
                    {
                        SoundEngine.PlaySound(missileSound, NPC.Center);
                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<AureusSpawn>(), 60, 0, ai1: Helper.FromAToB(NPC.Center, player.Center + player.velocity * 2, false).X, ai2: Main.rand.NextFloat(0.02f, 0.035f));
                    }
                    if (AITimer < 51 && AITimer % 4 == 0)
                    {
                        SoundEngine.PlaySound(missileSound, NPC.Center);
                        Projectile.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<AureusSpawn>(), 60, 0, ai1: Main.rand.NextFloat(-1500, 1500), ai2: Main.rand.NextFloat(0.02f, 0.035f));
                    }
                    if (AITimer > 200)
                    {
                        AIState = Idle;
                        amountPerformed = 3;
                        AITimer = 0;
                        AITimer3 = 0;
                        AITimer2 = 0;
                    }
                }
                break;
            case XBeams:
                {
                    if (AITimer3 == 0 && NPC.Grounded())
                    {
                        NPC.noGravity = true;
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 10, 30, 30, 2000));
                        SoundEngine.PlaySound(jumpSound, NPC.Center);
                        SoundEngine.PlaySound(Sounds.eggplosion, NPC.Center);
                        Projectile.NewProjectile(null, Helper.Raycast(NPC.Center, Vector2.UnitY, 100).Point, Vector2.Zero, ProjectileType<AImpact>(), 0, 0);
                        NPC.velocity = Vector2.UnitY * -30;
                        AITimer3 = 1;
                    }
                    if (AITimer3 > 0)
                    {
                        AITimer3++;
                        if (AITimer3 > 1 && AITimer3 < 15)
                            NPC.Center += NPC.velocity * 0.1f;
                        if (AITimer3 >= 15 && AITimer3 < 45)
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(-player.velocity.X, 400), false) / 2;
                        if (AITimer3 == 45)
                        {
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 8) + PiOver4;
                                Projectile.NewProjectile(null, NPC.Center, angle.ToRotationVector2(), ProjectileType<AureusLaser>(), 10, 0, ai2: 1);
                            }
                            NPC.velocity = Vector2.Zero;
                        }
                        if (AITimer3 == 80)
                        {
                            SoundEngine.PlaySound(primSlopSound, NPC.Center);
                            for (int i = 0; i < 8; i++)
                            {
                                float angle = Helper.CircleDividedEqually(i, 8) + PiOver4;
                                Projectile.NewProjectile(null, NPC.Center, angle.ToRotationVector2(), ProjectileType<AureusBeam>(), 70, 0);
                            }
                        }

                        if (AITimer3 == 115)
                        {
                            SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
                            NPC.velocity = new Vector2(0, 35);
                        }
                        if (AITimer3 > 115 && NPC.Center.Y > player.Center.Y - NPC.width * 0.4f)
                        {
                            NPC.noTileCollide = false;
                            NPC.damage = 100;
                        }
                        if (AITimer3 > 115 && !NPC.collideY && NPC.noTileCollide)
                        {
                            NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-1, 1);
                            NPC.velocity.Y += 0.05f;
                            NPC.Center += NPC.velocity * 0.5f;
                        }
                        if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 0.5f)) && AITimer2 == 0 && AITimer3 >= 115)
                        {
                            NPC.damage = 0;
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 20, 30, 30, 2000));
                            NPC.noGravity = false;
                            SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                            NPC.velocity = -Vector2.UnitY * 3;
                            for (int i = -10; i < 10; i++)
                            {
                                MPUtils.NewProjectile(NPC.InheritSource(NPC), Helper.Raycast(NPC.Center - new Vector2(i * 150, 400), Vector2.UnitY, 1000).Point, Vector2.Zero, ProjectileType<AImpact>(), 50, 0);
                            }
                            AITimer2 = 1;
                        }
                    }
                    if (AITimer2 > 0)
                    {
                        AITimer2++;
                        if (AITimer2 > 40)
                        {
                            AIState = Idle;
                            AITimer = 0;
                            AITimer3 = 0;
                            AITimer2 = 0;
                        }
                    }
                }
                break;
            case Vortex:
                {
                    if (AITimer == 40)
                    {
                        SoundEngine.PlaySound(missileSound.WithPitchOffset(-1f), NPC.Center);
                        Projectile.NewProjectile(null, NPC.Center - new Vector2(0, 300), Vector2.Zero, ProjectileType<AureusVortex>(), 60, 0);
                    }
                    NPC.spriteDirection = NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;

                    Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);
                    if (NPC.Grounded(offsetX: 0.5f) && (NPC.collideX || Helper.Raycast(NPC.Center, Vector2.UnitX, 1000).RayLength < NPC.width || Helper.Raycast(NPC.Center, -Vector2.UnitX, 1000).RayLength < NPC.width))
                        NPC.velocity.Y = -10;
                    if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 300)
                        NPC.velocity.Y = -20;
                    else if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 200)
                        NPC.velocity.Y = -15;
                    else if (NPC.Grounded(offsetX: 0.5f) && player.Center.Y < NPC.Center.Y - 100)
                        NPC.velocity.Y = -10;

                    if (Helper.Raycast(NPC.Center, -Vector2.UnitY, NPC.height).RayLength < NPC.height - 1 && !Collision.CanHit(NPC, player))
                    {
                        if (!NPC.noTileCollide)
                        {
                            NPC.noTileCollide = true;
                            NPC.netUpdate = true;
                        }

                        if (player.Center.Y < NPC.Center.Y)
                            NPC.Center -= Vector2.UnitY * 2;
                        else
                            NPC.Center += Vector2.UnitY * 2;

                        NPC.Center += new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 2, 0);
                    }
                    else if ((!Collision.CanHit(NPC, player) || !Collision.CanHitLine(NPC.TopLeft, 10, 10, player.position, player.width, player.height) || !Collision.CanHitLine(NPC.TopRight, 10, 10, player.position, player.width, player.height)) && player.Center.X.InRange(NPC.Center.X, NPC.width))
                    {
                        if (!NPC.noTileCollide)
                        {
                            NPC.noTileCollide = true;
                            NPC.netUpdate = true;
                        }

                        if (player.Center.Y < NPC.Center.Y)
                            NPC.Center -= Vector2.UnitY * 2;
                        else
                            NPC.Center += Vector2.UnitY * 2;
                    }
                    else if (NPC.noTileCollide)
                    {
                        NPC.noTileCollide = false;
                        NPC.netUpdate = true;
                    }
                    NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.02f, 0.12f);

                    if (AITimer == 5 || (AITimer > 30 && AITimer % 10 == 0))
                        Projectile.NewProjectile(null, NPC.Center, Main.rand.NextVector2CircularEdge(20, 20), ProjectileType<AureusLaser>(), 20, 0);


                    if (AITimer > 520)
                    {
                        AIState = Idle;
                        amountPerformed = 99;
                        AITimer = 0;
                        AITimer3 = 0;
                        AITimer2 = 0;
                    }
                }
                break;
            case ZigZag:
                {
                    if (AITimer3 == 0 && NPC.Grounded())
                    {
                        NPC.noGravity = true;
                        Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 10, 30, 30, 2000));
                        SoundEngine.PlaySound(jumpSound, NPC.Center);
                        SoundEngine.PlaySound(Sounds.eggplosion, NPC.Center);
                        Projectile.NewProjectile(null, Helper.Raycast(NPC.Center, Vector2.UnitY, 100).Point, Vector2.Zero, ProjectileType<AImpact>(), 0, 0);
                        NPC.velocity = Vector2.UnitY * -30;
                        AITimer3 = 1;
                    }
                    if (AITimer3 > 0)
                    {
                        AITimer3++;
                        if (AITimer3 > 1 && AITimer3 < 15)
                            NPC.Center += NPC.velocity * 0.1f;
                        if (AITimer3 >= 15 && AITimer3 < 115)
                        {
                            NPC.velocity = Helper.FromAToB(NPC.Center, player.Center - new Vector2(-player.velocity.X + MathF.Sin(AITimer * 0.1f) * 600, 450), false) / 2;
                            if (AITimer3 % 10 == 0)
                            {
                                Projectile.NewProjectile(null, NPC.Center, Vector2.UnitY.RotatedByRandom(PiOver4) * 0.1f, ProjectileType<AureusMissile>(), 50, 0);
                                Projectile.NewProjectile(null, NPC.Center, Vector2.UnitY.RotatedByRandom(PiOver4) * 5, ProjectileType<AureusLaser>(), 50, 0);
                            }
                        }


                        if (AITimer3 == 115)
                        {
                            SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
                            NPC.velocity = new Vector2(0, 35);
                        }
                        if (AITimer3 > 115 && NPC.Center.Y > player.Center.Y - NPC.width * 0.4f)
                        {
                            NPC.noTileCollide = false;
                            NPC.damage = 100;
                        }
                        if (AITimer3 > 115 && !NPC.collideY && NPC.noTileCollide)
                        {
                            NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-1, 1);
                            NPC.velocity.Y += 0.05f;
                            NPC.Center += NPC.velocity * 0.5f;
                        }
                        if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 0.5f)) && AITimer2 == 0 && AITimer3 >= 115)
                        {
                            NPC.damage = 0;
                            Main.instance.CameraModifiers.Add(new PunchCameraModifier(NPC.Center, Vector2.UnitY, 20, 30, 30, 2000));
                            NPC.noGravity = false;
                            SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                            NPC.velocity = -Vector2.UnitY * 3;
                            for (int i = -10; i < 10; i++)
                            {
                                MPUtils.NewProjectile(NPC.InheritSource(NPC), Helper.Raycast(NPC.Center - new Vector2(i * 150, 400), Vector2.UnitY, 1000).Point, Vector2.Zero, ProjectileType<AImpact>(), 50, 0);
                            }
                            SoundEngine.PlaySound(primSlopSound, NPC.Center);
                            for (int i = -3; i < 4; i++)
                                Projectile.NewProjectile(null, NPC.Center + new Vector2(i * 20, 200), -Vector2.UnitY, ProjectileType<AureusBeam>(), 100, 0);
                            AITimer2 = 1;
                        }
                    }
                    if (AITimer2 > 0)
                    {
                        AITimer2++;
                        if (AITimer2 > 40)
                        {
                            AIState = Idle;
                            AITimer = 0;
                            AITimer3 = 0;
                            AITimer2 = 0;
                        }
                    }
                }
                break;
            case Planets:
                {
                    if (AITimer == 1)
                    {
                        float rot = Main.rand.NextFloat(Pi);
                        float rand = Main.rand.NextFloat(float.MaxValue);
                        float off = Main.rand.NextFloat(500, 700);
                        for (int i = -1; i < 2; i++)
                        {
                            if (i == 0) continue;
                            Vector2 pos = player.Center + new Vector2(off * i, 0).RotatedBy(rot);
                            Projectile.NewProjectile(null, pos, Helper.FromAToB(pos, player.Center) * 0.1f, ProjectileType<AureusPlanet>(), 50, 0, ai2: rand);
                        }
                    }
                    if (AITimer > 50)
                    {
                        AIState = Idle;
                        AITimer = 40;
                        AITimer3 = 0;
                        AITimer2 = 0;
                    }
                }
                break;
        }
    }
    SlotId idleSound, idleSound2;
    void IdleStuff()
    {
        if (!Main.dedServ)
        {
            if (SoundEngine.TryGetActiveSound(idleSound, out var _activeSound))
            {
                _activeSound.Pitch = Lerp(-1, 1, NPC.velocity.Length() / 10);
                _activeSound.Position = NPC.Center;
            }
            else
            {
                idleSound = SoundEngine.PlaySound(loopSound.WithVolumeScale(1.6f), NPC.Center, (_) => NPC.AnyNPCs(Type));
            }


            if (SoundEngine.TryGetActiveSound(idleSound2, out var __activeSound))
            {
                __activeSound.Pitch = Lerp(-1, 1, NPC.velocity.Length() / 13);
                __activeSound.Position = NPC.Center;
            }
            else
            {
                SoundEngine.PlaySound(mangoSound.WithVolumeScale(1.6f), NPC.Center, (_) => NPC.AnyNPCs(Type));
            }
        }

        Player player = Main.player[NPC.target];

        /*if (Main.rand.NextBool(100))
        {
            SoundEngine.PlaySound(mineSound.WithPitchOffset(-1).WithVolumeScale(0.2f), NPC.Center);

            MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<AureusMine>(), 50, 0);
        }

        if (Main.rand.NextBool(200))
        {
            SoundEngine.PlaySound(jumpscareSound, NPC.Center);

            MPUtils.NewProjectile(NPC.InheritSource(NPC), player.Center, Vector2.Zero, ProjectileType<jumpscare>(), 0, 0);
        }*/

    }
    void ProjSpam()
    {
        SoundEngine.PlaySound(jumpSound, NPC.Center);
        for (int k = 0; k < Main.rand.NextFloat(10, 15); k++)
        {
            MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(10, 20), ProjectileType<AureusLaser>(), 100, 0);
        }
    }

    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.netMode == NetmodeID.Server)
        {
            return;
        }

    }
}
public class AureusSpawnItem : ModItem
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/"+Name;
    public override void SetDefaults()
    {
        Item.width = 20;
        Item.height = 20;
        Item.maxStack = 1;
        Item.value = 1000;
        Item.rare = ItemRarityID.Blue;
        Item.useAnimation = 30;
        Item.useTime = 30;
        Item.noUseGraphic = true;
        Item.useStyle = ItemUseStyleID.HoldUp;
        Item.consumable = false;
        Item.useTurn = false;
    }
    public override bool? UseItem(Player player)
    {
        NPC.SpawnBoss((int)player.Center.X, (int)player.Center.Y - 900, NPCType<Aureus>(), player.whoAmI);
        return true;
    }
}
public class AureusSpawn : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/"+Name;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        lightColor = Color.White * Projectile.Opacity;
        return true;
    }
    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.scale = 0;
    }

    Vector2 startP;
    public override void AI()
    {
        if (++Projectile.frameCounter >= 1)
        {
            Projectile.frameCounter = 0;

            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;
        }
        Projectile.scale = Lerp(Projectile.scale, 2, 0.1f);

        if (startP == Vector2.Zero)
            startP = Projectile.Center;
        Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1, 0.025f);
        Projectile.ai[0]++;
        if (Projectile.ai[0] < 20)
            Projectile.velocity *= 1.025f;
        if (Projectile.ai[0] < 80 && Projectile.ai[0] > 20 && Projectile.ai[0] % 5 == 0)
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, startP + new Vector2(Projectile.ai[1], -430), false).RotatedBy(MathF.Sin(Projectile.ai[0]) * Projectile.ai[2] * 10) * Projectile.ai[2] * 2, 0.2f);
        else
            Projectile.velocity *= 0.98f;
        if (Projectile.ai[0] > 90 && Projectile.ai[0] % 5 == 0)
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, startP + new Vector2(Projectile.ai[1], -500), true).RotatedBy(MathF.Sin(Projectile.ai[0]) * Projectile.ai[2] * 10) * Projectile.ai[2] * 200, 0.2f);

        if (Projectile.ai[0] == 150)
            Projectile.NewProjectile(null, Projectile.Center, Vector2.UnitY * 0.1f, ProjectileType<AureusLaser>(), 10, 0, ai2: 1);
        if (Projectile.ai[0] == 200)
        {
            SoundEngine.PlaySound(Aureus.primSlopSound, Projectile.Center);
            Projectile.NewProjectile(null, Projectile.Center, Vector2.UnitY, ProjectileType<AureusBeam>(), 100, 0);
        }

        if (Projectile.ai[0] == 300)
        {
            Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<AExplosion>(), 0, 0);
            Projectile.Kill();
        }

    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}
public class AureusLaser : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/"+Name;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 3;
    }

    public override void SetDefaults()
    {
        Projectile.width = 14;
        Projectile.height = 14;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.scale = 2;
    }
    float alpha = 0;
    public override bool PreDraw(ref Color lightColor)
    {
        alpha = Lerp(alpha, 1, 0.1f);
        if (Projectile.ai[2] == 1)
        {
            Utils.DrawLine(Main.spriteBatch, Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * 2000, Color.Cyan with { A = 0 } * alpha, Color.OrangeRed with { A = 0 } * alpha, 4 * alpha);
        }
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, TextureAssets.Projectile[Type].Value.Frame(1, 3, 0, Projectile.frame), Color.White, Projectile.rotation, new Vector2(14) / 2, Projectile.scale, SpriteEffects.None);
        return false;
    }
    public override void AI()
    {
        if (Projectile.ai[2] == 1)
        {
            Projectile.velocity *= 1.05f;
            if (Main.LocalPlayer.Distance(Projectile.Center) < 2000)
                Projectile.timeLeft = 100;
        }
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (++Projectile.frameCounter >= 1)
        {
            Projectile.frameCounter = 0;

            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;
        }

        if (Main.rand.NextBool(2))
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, 0, 0, 0, default, 2).noGravity = true;
        }

        if (Main.rand.NextBool(100))
        {
            Dust.NewDustPerfect(Projectile.Center, DustType<AureusDust>(), Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 10), 100, default, Main.rand.NextFloat(1, 10)).noGravity = true;
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}
public class AureusMissile : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/"+Name;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 8;
        Projectile.height = 8;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = true;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 300;
        Projectile.scale = 2;
    }

    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.Item62, Projectile.Center);
        Projectile.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<AImpact>(), 0, 0);
    }
    public override void AI()
    {
        Projectile.velocity *= 1.05f;
        Projectile.rotation = Projectile.velocity.ToRotation();

        if (++Projectile.frameCounter >= 1)
        {
            Projectile.frameCounter = 0;

            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;
        }

    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}

public class MagnetSphere : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/"+Name;
    public override void SetStaticDefaults()
    {
    }

    public override void SetDefaults()
    {
        Projectile.width = 143;
        Projectile.height = 143;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 130;
        Projectile.scale = 1;
    }

    public override void AI()
    {
        Projectile.velocity = Projectile.DirectionTo(Main.LocalPlayer.Center) * 2; //So multiplayer friendly!!!!!!!!!!!
    }

    public override void OnKill(int timeLeft)
    {
        PunchCameraModifier modifier = new PunchCameraModifier(Projectile.Center, (Main.rand.NextFloat() * ((float)Math.PI * 2f)).ToRotationVector2(), 20f, 5f, 100, 1000f, FullName);
        Main.instance.CameraModifiers.Add(modifier);

        for (int k = 0; k < Main.rand.NextFloat(30, 50); k++)
        {
            MPUtils.NewProjectile(Projectile.InheritSource(Projectile), Projectile.Center, Main.rand.NextVector2Circular(1, 1) * Main.rand.NextFloat(10, 20), ProjectileType<AureusLaser>(), 100, 0);
        }

        for (int k = 0; k < 200; k++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustType<AureusDust>(), Main.rand.NextVector2Circular(1, 1) * Main.rand.NextFloat(1, 10), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}

public class AureusMine : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/"+Name;
    public override void SetStaticDefaults()
    {
        Main.projFrames[Projectile.type] = 4;
    }

    public override void SetDefaults()
    {
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 1000;
        Projectile.scale = 1;
    }

    public override void AI()
    {
        if (++Projectile.frameCounter >= 5)
        {
            Projectile.frameCounter = 0;

            if (++Projectile.frame >= Main.projFrames[Projectile.type])
                Projectile.frame = 0;
        }

        if (Main.rand.NextBool(2))
        {
            Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.OrangeTorch, 0, 0, 0, default, 2).noGravity = true;
        }

        if (Main.rand.NextBool(100))
        {
            Dust.NewDustPerfect(Projectile.Center, DustType<AureusDust>(), Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 10), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;
        }
    }

    public override Color? GetAlpha(Color lightColor)
    {
        return Color.White;
    }
}
public class AImpact : ModProjectile
{
    public override string Texture => Helper.Placeholder;
    public override void SetDefaults()
    {
        Projectile.width = 300;
        Projectile.height = 20;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 50;
        Projectile.tileCollide = false;
        Projectile.hide = false;
        Projectile.penetrate = -1;
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
    {
        //behindNPCsAndTiles.Add(index);
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool? CanDamage() => Projectile.timeLeft > 45;
    int seed;
    public override void OnSpawn(IEntitySource source)
    {
        seed = Main.rand.Next(int.MaxValue);
        for (int k = 0; k < 50; k++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.OrangeTorch, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 30), 0, default, Main.rand.NextFloat(1, 3)).noGravity = true;
            Dust.NewDustPerfect(Projectile.Center, DustID.BlueTorch, Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 30), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;

            if (Main.rand.NextBool(40))
            {
                Dust.NewDustPerfect(Projectile.Center, DustType<AureusDust>(), Main.rand.NextVector2Unit(-MathHelper.Pi, MathHelper.Pi) * Main.rand.NextFloat(1, 10), 100, default, Main.rand.NextFloat(1, 2)).noGravity = true;
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex1 = Assets.Extras.Extras2.scratch_02.Value;
        Texture2D tex2 = Assets.Extras.cone4.Value;
        UnifiedRandom rand = new UnifiedRandom(seed);
        float max = 30;
        float alpha = MathHelper.Lerp(0.5f, 0, Projectile.ai[1]) * 2;
        Main.spriteBatch.Reload(Effects.SpriteRotation.Value);
        Effects.SpriteRotation.Value.Parameters["rotation"].SetValue(rand.NextFloat(MathHelper.Pi, MathHelper.TwoPi) * (rand.NextFloatDirection() > 0 ? 1 : -1) + Projectile.ai[1]);
        Effects.SpriteRotation.Value.Parameters["scale"].SetValue(new Vector2(1, rand.NextFloat(0.2f, 0.8f)));
        Effects.SpriteRotation.Value.Parameters["uColor"].SetValue((Color.Blue with { A = 0 }).ToVector4() * alpha * 0.5f);
        for (float i = 0; i < max; i++)
        {
            Texture2D tex = Main.rand.NextBool() ? tex1 : tex2;
            float angle = Helper.CircleDividedEqually(i, max * 2) + MathHelper.Pi;
            float scale = rand.NextFloat(0, .6f) * 2;
            Vector2 offset = new Vector2(Main.rand.NextFloat(-0, 2) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
            for (float j = 0; j < 2; j++)
                Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Cyan * alpha, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 0.6f * 4, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Reload(effect: null);
        for (float i = 0; i < max / 2; i++)
        {
            Texture2D tex = Main.rand.NextBool() ? tex1 : tex2;
            float angle = Helper.CircleDividedEqually(i, max) + MathHelper.Pi;
            float scale = rand.NextFloat(0, .8f) * 2;
            Vector2 offset = new Vector2(Main.rand.NextFloat(-0, 2) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
            for (float j = 0; j < 2; j++)
                Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.OrangeRed with { A = 0 } * alpha, angle, new Vector2(0, tex.Height / 2), new Vector2(Projectile.ai[1], alpha) * scale * 0.6f * 4, SpriteEffects.None, 0);
        }
        Lighting.AddLight(Projectile.Center, TorchID.UltraBright);
        return false;
    }
    public override void AI()
    {

        Projectile.ai[1] = MathHelper.Lerp(Projectile.ai[1], 1, 0.1f);
        if (Projectile.ai[1] > 1)
            Projectile.Kill();
    }
}
internal class AureusSky : CustomSky
{
    private bool isActive;
    private float intensity;

    public override void Update(GameTime gameTime)
    {
        if (isActive && intensity < 1f)
        {
            intensity += 0.01f;
        }
        else if (!isActive && intensity > 0)
        {
            intensity -= 0.01f;
        }
    }
    public override void Draw(SpriteBatch spriteBatch, float minDepth, float maxDepth)
    {
        GraphicsDevice gd = Main.graphics.GraphicsDevice;
        SpriteBatch sb = Main.spriteBatch;
        Rectangle rect = Helper.ScreenRect;
        sb.Draw(Helper.GetTexture(Helper.AssetPath+"NPCs/Aureus/background").Value, rect, Color.White * intensity);
        if (ConglomerateSystem.conglomerateSkyFlash > 0)
            sb.Draw(TextureAssets.MagicPixel.Value, rect, Color.White * intensity * ConglomerateSystem.conglomerateSkyFlash);
    }

    public override float GetCloudAlpha()
    {
        return 0f;
    }
    public override Color OnTileColor(Color inColor)
    {
        return inColor * 0.5f;
    }

    public override void Activate(Vector2 position, params object[] args)
    {
        isActive = true;
    }

    public override void Deactivate(params object[] args)
    {
        isActive = false;
    }

    public override void Reset()
    {
        isActive = false;
    }

    public override bool IsActive()
    {
        return isActive || intensity > 0;
    }
}

public class AureusDust : ModDust
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/" + Name;
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.frame = new Rectangle(0, 0, 191, 176);
        if (dust.customData is null)
            dust.scale = 0.2f;
    }


    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;

        dust.scale -= 0.01f;

        if (dust.scale < 0.0001f)
            dust.active = false;

        dust.frame.Y += 176;

        if (dust.frame.Y > 1055)
            dust.frame.Y = 0;

        Lighting.AddLight(dust.position, new Color(252, 161, 92).ToVector3() * 0.5f);

        return false;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        return Color.White;
    }
}

public class AureusDust2 : ModDust
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/" + Name;
    public override void OnSpawn(Dust dust)
    {
        dust.noGravity = true;
        dust.frame = new Rectangle(0, 0, 191, 176);
    }


    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;

        dust.scale -= 0.1f;

        if (dust.scale < 0.0001f)
            dust.active = false;

        dust.frame.Y += 176;

        if (dust.frame.Y > 1055)
            dust.frame.Y = 0;

        Lighting.AddLight(dust.position, new Color(252, 161, 92).ToVector3() * 0.5f);

        return false;
    }

    public override Color? GetAlpha(Dust dust, Color lightColor)
    {
        return Color.White;
    }
}

public class AureusBossBar : ModBossBar
{
    public override string Texture => Helper.AssetPath + "NPCs/Aureus/"+Name;
}