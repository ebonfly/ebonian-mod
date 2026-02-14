using EbonianMod.Content.Bossbars;
using EbonianMod.Content.Dusts;
using EbonianMod.Content.Items.Vanity;
using EbonianMod.Content.Items.BossTreasure;
using EbonianMod.Content.Items.Pets;
using EbonianMod.Content.Items.Tiles.Trophies;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.Items.Weapons.Melee;
using EbonianMod.Content.Items.Weapons.Ranged;
using EbonianMod.Content.Items.Weapons.Summoner;
using EbonianMod.Content.NPCs.ArchmageX;
using EbonianMod.Content.NPCs.Corruption;
using EbonianMod.Content.NPCs.Garbage.Projectiles;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using EbonianMod.Content.Tiles;
using EbonianMod.Core.Systems.Cinematic;
using ReLogic.Graphics;
using ReLogic.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;


namespace EbonianMod.Content.NPCs.Garbage;

[AutoloadBossHead]
public partial class HotGarbage : ModNPC
{
    public Player player => Main.player[NPC.target];
    public State NextAttack = State.OpenLid;
    public State NextAttack2 = State.TrashBags;
    public override void PostAI() => NPC.spriteDirection = NPC.direction;

    public override void AI()
    {
        AmbientVFX();
        
        if (AIState != State.Idle && AIState != State.SlamSlamSlam && AIState != State.PipeBombAirstrike)
            NPC.noTileCollide = false;

        TargetingLogic();
        
        HandleLidAI();

        switch (AIState)
        {
            case State.Death:
                DoDeath(); break;
            case State.Intro: 
                DoIntro(); break;
            case State.Idle:
                DoIdle(); break;
            
            
        }
        
        if (AIState == State.WarningForDash)
        {
            NPC.velocity.X *= 0.99f;
            AITimer++;
            if (AITimer == 20)
            {
                SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0);
            }
            FacePlayer();
            if (AITimer >= 55)
            {
                NPC.velocity.X = 0;
                AITimer = 0;
                AITimer2 = 0;
                AIState = State.Dash;
            }

        }
        else if (AIState == State.Dash)
        {
            //old code i refuse to even look at, it works.
            NPC.damage = 60;
            AITimer++;
            int num899 = 130;
            int num900 = 130;
            Vector2 position5 = new Vector2(NPC.Center.X - (float)(num899 / 2), NPC.position.Y + (float)NPC.height - (float)num900);
            if (AITimer3 == 7)
                SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
            if (Collision.SolidCollision(position5, num899, num900))
            {
                NPC.velocity.Y = -5.75f;
            }
            if (AITimer3 < 22)
            {
                NPC.velocity.X = Lerp(NPC.velocity.X, 20f * NPC.direction, 0.15f);
            }
            if (AITimer3 >= 22)
            {
                NPC.velocity *= 0.96f;
            }
            if (AITimer3 >= 22 && AITimer3 < 40 && AITimer3 % 2 == 0)
            {
                for (int i = -1; i < 1; i++)
                {
                    Projectile a = MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(2, 4) * i, NPC.height / 2 - 8), new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageFlame>(), 15, 0);

                    if (a is not null)
                    {
                        a.timeLeft = 170;
                        a.SyncProjectile();
                    }
                }
            }
            if (AITimer3 >= 40 && AITimer % 5 == 0)
            {
                NPC.spriteDirection = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
                NPC.direction = Main.player[NPC.target].Center.X > NPC.Center.X ? 1 : -1;
            }
            if (++AITimer3 >= 65)
            {
                AITimer3 = 0;
            }
            if (AITimer >= 65 * 3)
            {
                NPC.velocity = Vector2.Zero;
                NextAttack = State.OpenLid;
                NextAttack2 = State.SpewFire;
                AIState = State.Idle;
                AITimer = 0;
                AITimer2 = 0;
                AITimer3 = 0;
                NPC.damage = 0;
                NPC.netUpdate = true;
            }
        }
        else if (AIState == State.SlamPreperation)
        {
            AITimer++;
            NPC.rotation += ToRadians(-0.9f * 4 * NPC.direction);
            if (AITimer >= 25)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = 0;
                AITimer = 0;
                AITimer2 = 0;
                AIState = State.SlamSlamSlam;
            }
        }
        else if (AIState == State.SlamSlamSlam)
        {
            AITimer++;
            NPC.noGravity = true;
            NPC.damage = 60;
            if (AITimer < 50)
                NPC.velocity.Y--;
            if (AITimer < 200)
                NPC.noTileCollide = true;
            if (AITimer >= 50 && AITimer < 181)
            {
                if (AITimer < 176)
                    DisposablePosition = player.Center - new Vector2(-player.velocity.X * 20, 500);
                NPC.direction = NPC.spriteDirection = 1;
                NPC.rotation = Lerp(NPC.rotation, ToRadians(90), 0.15f);
                if (AITimer % 8 == 0)
                    NPC.velocity = Helper.FromAToB(NPC.Center, DisposablePosition, false) * 0.056f;
            }
            if (AITimer == 2)
                SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
            if (AITimer == 181)
            {
                NPC.velocity = Vector2.Zero;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);
            }
            if (AITimer == 200)
            {
                SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
                for (int i = -4; i < 4; i++)
                {
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(6 * i * Main.rand.NextFloat(0.7f, 1.2f), 3), ProjectileType<GarbageGiantFlame>(), 15, 0, ai0: 1);
                }
                NPC.velocity = new Vector2(0, 35);
            }
            if (AITimer > 200 && NPC.Center.Y > player.Center.Y - NPC.width * 0.4f)
            {
                NPC.noTileCollide = false;
            }
            if (AITimer > 200 && !NPC.collideY && NPC.noTileCollide)
            {
                NPC.Center += Vector2.UnitX * Main.rand.NextFloat(-1, 1);
                NPC.velocity.Y += 0.015f;
            }
            if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 0.5f)) && AITimer2 == 0 && AITimer >= 200)
            {
                SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                NPC.velocity = -Vector2.UnitY * 3;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 0, 0);
                AITimer2 = 1;
            }
            if (AITimer2 >= 1)
            {
                NPC.velocity.Y += 0.1f;
                AITimer2++;
            }
            if (AITimer2 >= 50)
            {
                NPC.netUpdate = true;
                NPC.noGravity = false;
                NPC.velocity = Vector2.Zero;
                AITimer = 0;
                NPC.damage = 0;
                NextAttack = State.WarningForBigDash;
                AIState = State.Idle;
            }
        }
        else if (AIState == State.WarningForBigDash)
        {
            AITimer++;
            NPC.velocity.X = Helper.FromAToB(NPC.Center, player.Center).X * -1;
            if (AITimer == 10)
            {
                SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0);
            }
            NPC.rotation += ToRadians(-0.2f * 2 * NPC.direction);
            FacePlayer();
            if (AITimer >= 50)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = 0;
                AITimer = 0;
                AITimer2 = 0;
                AIState = State.BigDash;
                NPC.velocity = Vector2.Zero;
            }
        }
        else if (AIState == State.BigDash)
        {
            AITimer++;
            NPC.damage = 90;
            NPC.rotation = Lerp(NPC.rotation, 0, 0.35f);
            if (AITimer == 2)
                SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
            if (AITimer < 12)
            {
                NPC.velocity += new Vector2(((Helper.FromAToB(NPC.Center, player.Center).X < 0) ? -3 : 3) * 1.6f, -1.2f);
            }
            if (AITimer % 6 == 0)
            {
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageFlame>(), 15, 0);
            }
            if (AITimer >= 110)
            {
                NPC.netUpdate = true;
                NPC.velocity = Vector2.Zero;
                AITimer = -50;
                NPC.damage = 0;
                NextAttack = State.OpenLid;
                NextAttack2 = State.TrashBags;
                NPC.velocity = Vector2.Zero;
                AIState = State.Idle;
            }
        }
        else if (AIState == State.SpewFire)
        {
            JumpCheck();
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 2.5f, 0.15f);
            AITimer++;
            if (AITimer % 6 == 0)
            {
                SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                for (int i = -2; i < 2; i++)
                {
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                }
            }
            if (AITimer >= 100)
            {
                NPC.netUpdate = true;
                AITimer = 0;
                NPC.damage = 0;
                NextAttack = State.SlamPreperation;
                NPC.velocity = Vector2.Zero;
                AIState = State.CloseLid;
            }
        }
        else if (AIState == State.SpewFire2)
        {
            AITimer++;
            JumpCheck();
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 2.5f, 0.15f);
            if (AITimer % 6 == 0 && AITimer > 30)
            {
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                for (int i = 0; i < 4; i++)
                {
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(-10, 10), -6 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                }
            }
            if (AITimer >= 70)
            {
                NPC.netUpdate = true;
                AITimer = 0;
                NPC.damage = 0;
                NPC.velocity = Vector2.Zero;
                NextAttack2 = State.SateliteLightning;
                NextAttack = State.OpenLid;
                AIState = State.CloseLid;
            }
        }
        else if (AIState == State.BouncingBarrels)
        {
            NPC.netUpdate = true;
            AIState = State.GiantFireball;
        }
        else if (AIState == State.GiantFireball)
        {
            JumpCheck();
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.01f, 0.15f);
            FacePlayer();
            AITimer++;
            if (AITimer == 10)
            {
                SoundEngine.PlaySound(SoundID.Item34, NPC.Center);
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
            }
            if (AITimer == 20)
            {
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot, NPC.Center);
                for (int i = 0; i < 3; i++)
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -4 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
            }
            if (AITimer == 80)
            {
                CameraSystem.ScreenShakeAmount = 12;
                SoundEngine.PlaySound(SoundID.DD2_FlameburstTowerShot.WithPitchOffset(-0.4f).WithVolumeScale(1.1f), NPC.Center);
                for (int i = 0; i < 5; i++)
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), 0), new Vector2(NPC.direction * Main.rand.NextFloat(5, 10), -7 - Main.rand.NextFloat(4, 7)), ProjectileType<GarbageGiantFlame>(), 15, 0, ai2: 1);
            }
            if (AITimer >= 80)
            {
                NPC.netUpdate = true;
                AITimer = 0;
                NPC.damage = 0;
                NPC.velocity = Vector2.Zero;
                NextAttack = State.MassiveLaser;
                AIState = State.CloseLid;
            }
        }
        else if (AIState == State.TrashBags)
        {
            if (AITimer == 1 && !MPUtils.NotMPClient)
            {
                AITimer3 = Main.rand.Next(10000000);
                NPC.netUpdate = true;
            }
            AITimer3++;
            UnifiedRandom rand = new((int)AITimer3);
            AITimer++;
            JumpCheck();
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.043f, 0.12f);
            FacePlayer();
            if (AITimer <= 60 && AITimer % 5 == 0)
            {
                Projectile a = MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(Main.rand.NextFloat(-2.5f, 2.5f), Main.rand.NextFloat(-15, -7)) * 0.5f, ProjectileType<GarbageBag>(), 15, 0, player.whoAmI);
                if (a is not null)
                {
                    a.timeLeft = 200;
                    a.SyncProjectile();
                }
            }
            if (AITimer % 3 == 0 && AITimer > 100)
            {
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), new Vector2(player.Center.X + 600 * rand.NextFloat(-1, 1), player.Center.Y - 600), new Vector2(rand.NextFloat(-1, 1), 2) * 0.5f, ProjectileType<GarbageBag>(), 15, 0, player.whoAmI);
            }
            if (AITimer >= 100 && AITimer < 120)
            {
                if (NPC.frameCounter % 5 == 0)
                {
                    if (NPC.frame.Y == 76)
                        SoundEngine.PlaySound(SoundID.Item37, NPC.Center);
                    if (NPC.frame.Y > 0)
                    {
                        NPC.frame.Y -= 76;
                    }
                }
            }
            if (AITimer >= 150)
            {
                NPC.netUpdate = true;
                AITimer = 0;
                AITimer3 = 0;
                NPC.damage = 0;
                NPC.velocity = Vector2.Zero;
                NextAttack2 = State.SodaMissiles;
                NextAttack = State.OpenLid;
                AIState = State.Idle;
            }
        }
        else if (AIState == State.SodaMissiles)
        {
            if (AITimer == 1 && !MPUtils.NotMPClient)
            {
                AITimer3 = Main.rand.Next(10000000);


                NPC.netUpdate = true;
            }
            AITimer3++;
            UnifiedRandom rand = new((int)AITimer3);
            AITimer++;
            JumpCheck();
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center).X * 4, 0.15f);
            if (AITimer % 5 == 0 && AITimer < 60 && AITimer > 20)
            {
                SoundEngine.PlaySound(SoundID.Item156, NPC.Center);
                Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, new Vector2(rand.NextFloat(-4, 4), -7), ProjectileType<GarbageMissile>(), 15, 0, player.whoAmI, ToRadians(rand.NextFloat(-3, 3)));
            }
            if (AITimer >= 60)
            {
                NPC.netUpdate = true;
                AITimer = 0;
                NPC.damage = 0;
                NPC.velocity = Vector2.Zero;
                NextAttack = State.MailBoxes;
                AIState = State.CloseLid;
            }
        }
        else if (AIState == State.MailBoxes)
        {
            JumpCheck();
            NPC.velocity.X = Lerp(NPC.velocity.X, Helper.FromAToB(NPC.Center, player.Center + Helper.FromAToB(player.Center, NPC.Center) * 70, false).X * 0.043f, 0.12f);
            AITimer++;
            if (AITimer == 20)
                SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
            if (AITimer >= 20 && AITimer <= 40 && AITimer % 10 == 0)
                MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<GreenShockwave>(), 0, 0);

            if (AITimer > 60 && AITimer < 82)
            {
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), Helper.Raycast(NPC.Center - new Vector2(Main.rand.NextFloat(-2000, 2000), 200), Vector2.UnitY, 600, true).Point, Vector2.Zero, ProjectileType<Mailbox>(), 15, 0);
            }
            if (AITimer >= 120)
            {
                NPC.netUpdate = true;
                AITimer = -80;
                NPC.damage = 0;
                NPC.velocity = Vector2.Zero;
                NextAttack2 = State.SpewFire2;
                NextAttack = State.OpenLid;
                AIState = State.Idle;
            }
        }
        else if (AIState == State.SateliteLightning)
        {
            AITimer++;
            if (AITimer == 1 && !MPUtils.NotMPClient)
            {
                AITimer3 = Main.rand.Next(10000000);


                NPC.netUpdate = true;
            }
            AITimer3++;
            UnifiedRandom rand = new((int)AITimer3);
            if (AITimer >= 20 && AITimer % 20 == 0)
            {
                SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
                MPUtils.NewProjectile(null, NPC.Center, Main.rand.NextVector2Circular(10, 10), ProjectileType<GarbageDrone>(), 20, 0, ai1: Helper.FromAToB(NPC.Center, player.Center + player.velocity * 2, false).X, ai2: rand.NextFloat(0.02f, 0.035f));
            }
            if (AITimer > 20 && AITimer % 5 == 0)
            {
                MPUtils.NewProjectile(null, NPC.Center, Main.rand.NextVector2Circular(10, 10), ProjectileType<GarbageDrone>(), 20, 0, ai1: rand.NextFloat(-1500, 1500), ai2: rand.NextFloat(0.02f, 0.035f));
            }
            if (AITimer >= 100)
            {
                NPC.netUpdate = true;
                AITimer = 0;
                AITimer3 = 0;
                NPC.damage = 0;
                AIState = State.CloseLid;
                NextAttack = State.PipeBombAirstrike;
                NPC.velocity = Vector2.Zero;
            }
        }
        else if (AIState == State.PipeBombAirstrike)
        {
            AITimer++;
            if (AITimer == 2)
                SoundEngine.PlaySound(SoundID.Zombie67, NPC.Center);
            if (AITimer <= 25)
                NPC.rotation += ToRadians(-0.9f * 4 * NPC.direction);
            if (AITimer < 75 && AITimer > 25)
            {
                NPC.noTileCollide = true;
                NPC.velocity.Y--;
            }
            if (AITimer >= 75 && AITimer < 150)
            {
                NPC.damage = 60;
                if (AITimer < 150)
                    DisposablePosition = player.Center;
                NPC.direction = NPC.spriteDirection = 1;
                NPC.rotation = Lerp(NPC.rotation, ToRadians(90), 0.15f);
                NPC.velocity = Helper.FromAToB(NPC.Center, DisposablePosition - new Vector2(-player.velocity.X * 10, 700), false) * 0.05f;
            }
            if (AITimer == 150)
            {
                NPC.velocity = Vector2.Zero;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);
            }
            if (AITimer > 170 && AITimer <= 200 && AITimer % 3 == 0)
            {
                MPUtils.NewProjectile(null, Main.rand.NextVector2FromRectangle(NPC.getRect()), Vector2.UnitY.RotatedByRandom(PiOver2) * Main.rand.NextFloat(5, 10), ProjectileType<Pipebomb>(), 15, 0);
            }
            if (AITimer == 200)
            {
                SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
                NPC.velocity = new Vector2(0, 50);
            }
            if (AITimer > 200 && NPC.Center.Y > player.Center.Y - NPC.width * 0.4f)
                NPC.noTileCollide = false;
            if (AITimer > 200 && !NPC.collideY && NPC.noTileCollide)
            {
                NPC.position.Y += NPC.velocity.Y;
            }
            if (!NPC.noTileCollide && (NPC.collideY || NPC.Grounded(offsetX: 0.5f)) && AITimer2 == 0 && AITimer >= 200)
            {
                SoundEngine.PlaySound(SoundID.Item62, NPC.Center);
                NPC.velocity = -Vector2.UnitY * 3;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 16, 0);
                AITimer2 = 1;
            }
            if (AITimer2 >= 1)
            {
                NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
                NPC.velocity.Y += 0.1f;
                AITimer2++;
            }
            if (AITimer2 >= 50)
            {
                NPC.netUpdate = true;
                AITimer2 = 0;
                AITimer = 0;
                NPC.damage = 0;
                AIState = State.Idle;
                NextAttack = State.OpenLid;
                NextAttack2 = State.BouncingBarrels;
                NPC.velocity = Vector2.Zero;
            }
        }
        else if (AIState == State.MassiveLaser)
        {
            AITimer++;
            if (AITimer < 60)
            {
                NPC.velocity.X = 0;
                if (AITimer < 40)
                    NPC.velocity.Y = Lerp(NPC.velocity.Y, -30, 0.1f);
                else
                    NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Zero, 0.3f);
                if (AITimer > 40)
                    NPC.rotation = Utils.AngleLerp(NPC.rotation, PiOver2 * NPC.direction, 0.05f);
            }
            if (AITimer == 60)
                NPC.rotation = PiOver2 * NPC.direction;
            if (AITimer > 60 && AITimer3 != 3)
            {
                NPC.damage = 60;
                DisposablePosition = NPC.Center;
                if (NPC.velocity.Length() < 20)
                    NPC.velocity.Y += 1 + NPC.velocity.Y;
                NPC.Center += Vector2.UnitY * NPC.velocity.Y;
            }
            bool colliding = Helper.Raycast(NPC.Center, Vector2.UnitY, NPC.width * 0.6f).RayLength < NPC.width * 0.3f ||
                Helper.Raycast(NPC.BottomRight, Vector2.UnitY, NPC.width * 0.6f).RayLength < NPC.width * 0.3f ||
                Helper.Raycast(NPC.BottomLeft, Vector2.UnitY, NPC.width * 0.6f).RayLength < NPC.width * 0.3f;
            if (colliding && AITimer > 60 && AITimer < 320)
            {
                if (AITimer3 != 3)
                {
                    DisposablePosition = NPC.Center + new Vector2(0, NPC.height * 0.5f);
                    AITimer3 = 3;
                    for (int i = 0; i < 4; i++)
                        MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center + Main.rand.NextVector2Circular(15, 15), Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 0, 0);
                    Projectile a = MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<FatSmash>(), 0, 0);
                    if (a is not null)
                    {
                        a.scale = Main.rand.NextFloat(0.4f, 0.7f);
                        a.SyncProjectile();
                    }
                }
                else
                {
                    NPC.Center = Vector2.Lerp(NPC.Center, DisposablePosition + Main.rand.NextVector2Circular(AITimer2 * 10f, AITimer2), 0.2f);
                    NPC.velocity = Vector2.Zero;
                }
                if (AITimer % 3 - (int)AITimer2 == 0)
                {
                    Vector2 pos = NPC.Center + new Vector2(Main.rand.NextFloat(-NPC.width, NPC.width) * 0.7f, NPC.height * 0.3f);
                    Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(NPC.Center, pos) * Main.rand.NextFloat(10, 15),
                        newColor: Color.OrangeRed, Scale: Main.rand.NextFloat(0.105f, 0.25f)).customData = NPC.Center - new Vector2(0, 100);
                }
            }
            if (AITimer == 300)
                MPUtils.NewProjectile(null, NPC.Center - new Vector2(6 * NPC.direction, 40), -Vector2.UnitY * 10, ProjectileType<GarbageGiantFlame>(), 20, 0, ai2: 1);
            if (AITimer > 100 && AITimer < 300 && AITimer % 20 == 0)
            {
                CameraSystem.ScreenShakeAmount = 5 * AITimer2;
                for (int i = 0; i < 3; i++)
                {
                    MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), -40), new Vector2(NPC.direction * Main.rand.NextFloat(-10, 10), -6 - Main.rand.NextFloat(2, 4)), ProjectileType<GarbageFlame>(), 15, 0);
                }
                MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-15, 15), -40), new Vector2(NPC.direction * Main.rand.NextFloat(-6, 6) * AITimer2, -6 - Main.rand.NextFloat(3, 5) * AITimer2), ProjectileType<GarbageFlame>(), 15, 0);
            }
            if (AITimer == 5)
            {
                SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);
            }
            if (AITimer == 40)
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.UnitY, ProjectileType<GarbageTelegraph>(), 0, 0);

            if (AITimer == 60)
            {
                SoundEngine.PlaySound(Sounds.eruption.WithVolumeScale(0.8f), NPC.Center);
                if (!Main.dedServ)
                    LaserSoundSlot = SoundEngine.PlaySound(Sounds.garbageLaser.WithVolumeScale(1.35f), NPC.Center);
                CameraSystem.ScreenShakeAmount = 5;
                AITimer2 = 1;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageLaserSmall1>(), 100, 0, ai0: NPC.whoAmI);
            }
            if (AITimer == 140)
            {
                if (!Main.dedServ)
                    if (SoundEngine.TryGetActiveSound(LaserSoundSlot, out var sound))
                    {
                        sound.Pitch += 0.3f;
                        sound.Volume += 0.3f;
                    }
                CameraSystem.ScreenShakeAmount = 10;
                AITimer2 = 1.5f;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageLaserSmall2>(), 100, 0, ai0: NPC.whoAmI);
            }
            if (AITimer == 200)
            {
                if (!Main.dedServ)
                    if (SoundEngine.TryGetActiveSound(LaserSoundSlot, out var sound))
                    {
                        sound.Pitch += 0.4f;
                        sound.Volume += 0.4f;
                    }
                CameraSystem.ScreenShakeAmount = 15;
                AITimer2 = 2.25f;
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center - new Vector2(-6 * NPC.direction, NPC.height * 0.75f), -Vector2.UnitY, ProjectileType<HeatBlastVFX>(), 0, 0);
                MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, -Vector2.UnitY, ProjectileType<GarbageLaserSmall3>(), 100, 0, ai0: NPC.whoAmI);
            }
            if (AITimer > 200 && AITimer < 320)
            {
                for (float i = 0; i < 0.99f; i += 0.33f)
                    Helper.DustExplosion(NPC.Center - new Vector2(6, NPC.height * 0.2f), Vector2.One, 2, Color.Gray * 0.1f, false, false, 0.1f, 0.125f, -Vector2.UnitY.RotatedByRandom(PiOver4 * i) * Main.rand.NextFloat(2f, 8f));
            }
            if (AITimer >= 360)
                NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
            if (AITimer >= 400)
            {

                NPC.netUpdate = true;
                if (!Main.dedServ)
                    if (SoundEngine.TryGetActiveSound(LaserSoundSlot, out var sound))
                    {
                        sound.Stop();
                    }
                AITimer3 = 0;
                AITimer2 = 0;
                AITimer = 0;
                NPC.damage = 0;
                AIState = State.Idle;
                PerformedFullMoveset = true;
                NextAttack = State.WarningForDash;
                NPC.velocity = Vector2.Zero;
            }
        }
        else if (AIState == State.SummonDrones)
        {
            AITimer++;
            if (AITimer == 10)
                SoundEngine.PlaySound(SoundID.Zombie67.WithPitchOffset(-1f), NPC.Center);
            
            if (AITimer > 10 && AITimer < 30&& AITimer % 2 == 0)
                MPUtils.NewProjectile(null, NPC.Center + Main.rand.NextVector2Circular(AITimer -10f, AITimer- 10f), Vector2.Zero, ProjectileType<GreenShockwave>(), 0, 0); 

            if (AITimer > 45 && AITimer < 180 && AITimer % 7 == 0)
            {
                MPUtils.NewProjectile(null, NPC.Center - new Vector2(850, 600), new Vector2(7, 10) * Main.rand.NextFloat(0.9f, 1.1f), ProjectileType<LaserDrone>(), 10, 0);
            }
        }
    }
}