
using System;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Snow
{
    public class BorealBlade : ModNPC
    {
        public override string Texture => Helper.AssetPath + "NPCs/Snow/"+Name;
        public override void SetDefaults()
        {
            NPC.width = 56;
            NPC.height = 60;
            NPC.aiStyle = -1;
            NPC.damage = 0;
            NPC.defense = 10;
            NPC.lifeMax = 40;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.HitSound = SoundID.Item49;
            NPC.DeathSound = SoundID.Item27;
            NPC.value = Item.buyPrice(silver: 5);
        }
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.UndergroundSnow,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Construct"),
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
            });
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.invasionType > 0) return 0;
            return (spawnInfo.Player.ZoneSnow && (spawnInfo.Player.ZoneNormalUnderground || spawnInfo.Player.ZoneNormalCaverns)) || (spawnInfo.Player.ZoneSnow && spawnInfo.Player.ZoneRain) ? 0.05f : 0;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if (NPC.frameCounter % 5 == 0)
            {
                switch (NPC.ai[0])
                {
                    case 0:
                        if (NPC.frame.Y < 3 * 60)
                            NPC.frame.Y += 60;
                        else
                            NPC.frame.Y = 0;
                        break;
                    case 1:
                        if (NPC.frame.Y < 19 * 60)
                            NPC.frame.Y += 60;
                        break;
                    case 2:
                        if (NPC.frame.Y < 24 * 60)
                            NPC.frame.Y += 60;
                        break;
                }
            }
        }
        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                Helper.SpawnDust(NPC.Center, NPC.Size, DustID.Snow, new Vector2(Main.rand.NextFloat(0.1f, 2.5f) * hit.HitDirection, Main.rand.NextFloat(-2, 2)), 25, new Action<Dust>((target) => { target.noGravity = true; target.scale = Main.rand.NextFloat(0.6f, 0.9f); }
                ));
                Helper.SpawnGore(NPC, "EbonianMod/BorealBladeG");
                Helper.SpawnGore(NPC, "EbonianMod/BorealBladeG2");
                Helper.SpawnGore(NPC, "EbonianMod/BorealBladeG3");
                Helper.SpawnGore(NPC, "EbonianMod/BorealBladeG4");
            }
        }
        bool glogged;
        Vector2 p;
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            if (!glogged && NPC.velocity.Length() > 4f)
            {
                Projectile.NewProjectile(NPC.GetSource_FromThis(), target.Center, Helper.FromAToB(NPC.Center, target.Center), ModContent.ProjectileType<BorealBladeSlashP>(), 0, 0);
                glogged = true;
            }
        }
        public override void AI()
        {
            Lighting.AddLight(NPC.Center, new Vector3(0f, .09f, .07f));
            Player player = Main.player[NPC.target];
            NPC.TargetClosest();
            NPC.FaceTarget();
            NPC.spriteDirection = NPC.direction;
            switch (NPC.ai[0])
            {
                case 0:
                    glogged = true;
                    if (player.Center.Distance(NPC.Center) < 100)
                    {
                        if (NPC.ai[2] == 0)
                        {
                            NPC.ai[3] = Main.rand.NextFloat(-PiOver4, PiOver4);
                            NPC.ai[2] = 1;
                        }
                    }
                    else if (player.Center.Distance(NPC.Center) > 150)
                        NPC.ai[2] = 0;
                    if (NPC.ai[2] == 1)
                    {
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center).RotatedBy(NPC.ai[3]) * -4, 0.03f);
                        NPC.ai[1]++;
                    }
                    else
                        NPC.velocity = Vector2.Lerp(NPC.velocity, Helper.FromAToB(NPC.Center, player.Center) * 4, 0.05f);
                    if (NPC.ai[1] >= 150)
                    {
                        NPC.ai[0] = 1;
                        NPC.ai[1] = 0;
                        NPC.ai[2] = 0;
                        NPC.ai[3] = 0;
                        NPC.frameCounter = 1;
                        NPC.frame.Y = 60 * 4;
                        NPC.netUpdate = true;
                    }
                    break;
                case 1:
                    glogged = true;
                    NPC.ai[3] = 0;
                    NPC.ai[1]++;
                    NPC.velocity *= 0.95f;
                    if (NPC.ai[1] == 5 * 5)
                        NPC.dontTakeDamage = true;
                    if (NPC.ai[1] == 7 * 5)
                    {
                        SoundEngine.PlaySound(SoundID.Item25, NPC.Center);
                        if (player.Center.Distance(NPC.Center) < 50)
                            NPC.Center = player.Center + -Helper.FromAToB(player.Center + Helper.FromAToB(player.Center, NPC.Center, reverse: true) * 100, NPC.Center, false).RotatedByRandom(0.2f);
                        NPC.Center = player.Center + -Helper.FromAToB(player.Center, NPC.Center, false).RotatedByRandom(PiOver2);
                    }
                    if (NPC.ai[1] == 7 * 5 + 2)
                        Helper.SpawnDust(NPC.Center, NPC.Size, DustID.Frost, Vector2.Zero, 25, new Action<Dust>((target) => { target.noGravity = true; target.scale = Main.rand.NextFloat(0.6f, 0.9f); }
                        ));
                    if (NPC.ai[1] >= 16 * 5)
                    {
                        glogged = false;
                        NPC.dontTakeDamage = false;
                        NPC.ai[1] = 0;
                        NPC.ai[0] = 2;
                        NPC.frameCounter = 1;
                        NPC.frame.Y = 60 * 21;
                        NPC.ai[2] = player.Center.X;
                        NPC.ai[3] = player.Center.Y;
                        NPC.netUpdate = true;
                    }
                    break;
                case 2:
                    NPC.damage = 50;
                    if (NPC.ai[1] == 1)
                    {
                        p = NPC.Center;
                        NPC.velocity = Vector2.Zero;
                        SoundEngine.PlaySound(SoundID.Item30, NPC.Center);
                        NPC.netUpdate = true;
                    }
                    if (NPC.ai[1] < 5)
                    {
                        NPC.velocity += Helper.FromAToB(p, new Vector2(NPC.ai[2], NPC.ai[3])) * 3.5f;
                    }
                    if (NPC.ai[1] > 15)
                        NPC.velocity *= 0.98f;
                    if (++NPC.ai[1] >= 30)
                    {
                        NPC.ai[1] = 0;
                        NPC.ai[0] = 0;
                        NPC.damage = 0;
                        glogged = false;
                        NPC.netUpdate = true;
                    }
                    break;
            }
        }
    }
    public class BorealBladeSlashP : ModProjectile
    {
        public override void SetDefaults()
        {
            Projectile.width = 5;
            Projectile.height = 5;
            Projectile.aiStyle = -1;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }
        public override bool? CanDamage() => false;
        public override bool ShouldUpdatePosition() => false;
        public override string Texture => Helper.Empty;
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D tex = Assets.Extras.Extras2.scratch_02.Value;
            float alpha = MathHelper.Lerp(1, 0, Projectile.ai[0]);
            Main.spriteBatch.Reload(BlendState.Additive);
            for (int i = 0; i < 2; i++)
                Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation, tex.Size() / 2, new Vector2(Projectile.ai[0], 1 + alpha * 0.1f) * 0.65f * 2, SpriteEffects.None, 0);
            Main.spriteBatch.Reload(BlendState.AlphaBlend);
            return false;
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.ai[0] += 0.05f;
            if (Projectile.ai[0] > 1)
                Projectile.Kill();
        }
    }
}
