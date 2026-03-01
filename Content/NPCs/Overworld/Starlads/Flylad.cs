
using EbonianMod.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Terraria.Enums;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace EbonianMod.Content.NPCs.Overworld.Starlads;
public class Flylad : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Starlads/"+Name;
    enum StateID
    {
        Spawn,
        Floating,
        GroundPound,
        HitGround,
        Stunned,
        StarBombing
    };

    StateID state = StateID.Spawn;

    float counter;

    float timer;

    float heightMod = 1f;

    float widthMod = 1f;

    float maxVelocity = 5f;

    public override void SendExtraAI(BinaryWriter writer)
    {
        base.SendExtraAI(writer);
        writer.Write((int)state);
        writer.Write(counter);
        writer.Write(timer);
        writer.Write(maxVelocity);
    }

    public override void ReceiveExtraAI(BinaryReader reader)
    {
        base.ReceiveExtraAI(reader);
        state = (StateID)reader.Read();
        counter = reader.ReadSingle();
        timer = reader.ReadSingle();
        maxVelocity = reader.ReadSingle();
    }

    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 7;

        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers(0) { Velocity = 1f };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
    }

    public override void SetDefaults()
    {
        NPC.lifeMax = 45;
        NPC.damage = 20;
        NPC.defense = 15;
        NPC.knockBackResist = 0f;

        NPC.Size = new Vector2(62, 62);
        NPC.scale = 1f;

        NPC.HitSound = SoundID.DD2_LightningBugHurt;
        NPC.DeathSound = SoundID.DD2_LightningBugDeath;
        NPC.noTileCollide = true;
        NPC.noGravity = true;
        NPC.value = Item.sellPrice(0, 0, 0, 90);

        NPC.aiStyle = -1;
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

    public override void AI()
    {
        timer += 0.1f;
        if (timer >= MathHelper.Pi)
        {
            timer = 0f;
            NPC.netUpdate = true;
        }

        NPC.knockBackResist = 0f;
        Player player = Main.player[NPC.target];

        if (state == StateID.Spawn)
        {
            state = StateID.Floating;
        }

        NPC.TargetClosest(true);
        Lighting.AddLight(NPC.Center, new Color(241, 212, 62).ToVector3() * 0.5f);

        if (state == StateID.GroundPound)
        {
            NPC.noTileCollide = false;
            NPC.damage = 100;

            NPC.velocity.X *= 0.7f;
            NPC.velocity.Y += 0.5f;


            if (NPC.velocity.Y >= 0f)
            {
                heightMod = 1f + (NPC.velocity.Length() * 0.1f);
                widthMod = 1f - (NPC.velocity.Length() * 0.1f);
            }

            if (heightMod >= 2f)
            {
                heightMod = 2f;
            }

            if (widthMod <= 0.5f)
            {
                widthMod = 0.5f;
            }

            if (NPC.collideY || (NPC.velocity.Y > 0.5f && heightMod >= 2))
            {
                state = StateID.HitGround;
                SoundEngine.PlaySound(SoundID.DD2_MonkStaffGroundImpact, NPC.Center);
                NPC.netUpdate = true;
            }
        }

        if (state == StateID.Floating)
        {
            NPC.noTileCollide = true;
            NPC.damage = 0;

            NPC.spriteDirection = NPC.direction;

            if (NPC.Distance(player.Center + new Vector2(0, -200)) > 50f)
                NPC.velocity = Vector2.Lerp(NPC.velocity, ((player.Center + new Vector2(0, -200)) - NPC.Center).SafeNormalize(Vector2.Zero)*5, 0.03f);


            NPC.ai[0]++;

            if (NPC.ai[0] >= 140f)
            {
                NPC.velocity.Y -= 10f;
                NPC.ai[0] = 0f;

                counter++;

                if (counter <= 5)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, player.velocity / 2f, ModContent.ProjectileType<FlyladStarBomb>(), 10, default);
                }
                else
                {
                    counter = 0;

                    SoundEngine.PlaySound(SoundID.DD2_WyvernDiveDown, NPC.Center);
                    state = StateID.GroundPound;
                    NPC.netUpdate = true;
                }

                NPC.netUpdate = true;
            }

            if (NPC.Distance(player.Center) >= 300f)
            {
                NPC.ai[0] = 0f;
            }
        }

        if (state == StateID.HitGround)
        {
            Helper.AddCameraModifier(new PunchCameraModifier(NPC.Center, -NPC.velocity.SafeNormalize(Vector2.UnitY), 3, 10, 30, 1000));

            widthMod = 2f;
            heightMod = 0.5f;

            state = StateID.Stunned;
        }

        if (state == StateID.Stunned)
        {

            if (NPC.Center.Distance(player.Center) <= 30f && player.velocity.Y > 0 && NPC.ai[3] == 0)
            {
                NPC.ai[3] = 1;
                SoundEngine.PlaySound(Sounds.ObeseladBounce, NPC.Center);
                player.velocity.Y = -6;
                heightMod = 0.5f;
                widthMod = 2f;
                player.SyncPlayerControls();
                NPC.StrikeNPC(80, 0, 0);
                NPC.netUpdate = true;
            }
            NPC.damage = 0;

            heightMod += (1 - heightMod) / 5f;
            widthMod += (1 - widthMod) / 5f;

            NPC.ai[0]++;

            if ((int)NPC.ai[0] == 120f)
            {
                NPC.velocity.Y -= 15f;
                NPC.netUpdate = true;
            }

            NPC.velocity.Y += 0.5f;

            if (NPC.ai[0] >= 150f)
            {
                NPC.ai[0] = 0f;
                NPC.ai[3] = 0;
                state = StateID.Floating;
                NPC.netUpdate = true;
            }

            if (NPC.collideY)
            {
                NPC.velocity.Y = 0f;
                NPC.netUpdate = true;
            }
        }
    }

    public override void OnKill()
    {
        Projectile.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<FlyladDeath>(), 0, 0);
    }

    public override void FindFrame(int frameHeight)
    {
        if (state == StateID.Stunned || state == StateID.GroundPound)
        {
            NPC.frame.Y = 6 * frameHeight;
        }
        else
        {
            NPC.frameCounter += 1f;

            if (NPC.frameCounter >= 5)
            {
                NPC.frameCounter = 0;
                NPC.frame.Y += frameHeight;

                if (NPC.frame.Y >= 6 * frameHeight)
                {
                    NPC.frame.Y = 0 * frameHeight;
                }
            }
        }

    }

    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return ((spawnInfo.Player.ZoneNormalSpace ? 0.07f : 0.01f) + Star.starfallBoost * 0.01f)
            * ((spawnInfo.Player.ZoneForest || spawnInfo.Player.ZoneNormalSpace) && !Main.dayTime && Main.GetMoonPhase() == MoonPhase.Full).ToInt();
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        //npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Starspore>(), 3, 5, 35));
        npcLoot.Add(ItemDropRule.Common(ItemID.Mushroom, 5, 1, 4));
        npcLoot.Add(ItemDropRule.Common(ItemID.FallenStar, 10, 1, 3));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D texture = TextureAssets.Npc[Type].Value;

        Vector2 position = NPC.Center - Main.screenPosition + new Vector2(0, NPC.gfxOffY);

        SpriteEffects spriteEffects = SpriteEffects.None;

        if (NPC.spriteDirection > 0)
        {
            spriteEffects = SpriteEffects.FlipHorizontally;
        }

        if (NPC.IsABestiaryIconDummy)
        {
            return true;
        }

        texture = Assets.NPCs.Overworld.Starlads.FlyladStarBomb.Value;

        if (state == StateID.Stunned)
        {
            spriteBatch.Draw(texture, position + new Vector2(0, texture.Height / 2f - 10f) + (new Vector2((float)Math.Sin(timer + MathHelper.PiOver4) * 2f, (float)Math.Cos(timer + MathHelper.PiOver4) * 0.25f) * 15f), texture.Frame(), Color.White, NPC.rotation,
                new Vector2(texture.Width / 2f, texture.Height), 1f + (float)Math.Cos(timer + MathHelper.PiOver4) * 0.2f, spriteEffects, 0);
        }

        texture = TextureAssets.Npc[Type].Value;

        if (state == StateID.Stunned || state == StateID.GroundPound)
        {
            spriteBatch.Draw(texture, position + new Vector2(0, NPC.frame.Size().Y / 2f), NPC.frame, Color.White, NPC.rotation, new Vector2(NPC.frame.Size().X / 2f, NPC.frame.Size().Y), new Vector2(widthMod, heightMod), spriteEffects, 0);
        }
        else
        {
            spriteBatch.Draw(texture, position, NPC.frame, Color.White, NPC.rotation, NPC.frame.Size() / 2f, new Vector2(widthMod, heightMod), spriteEffects, 0);
        }

        texture = Assets.NPCs.Overworld.Starlads.FlyladStarBomb.Value;

        if (state == StateID.Stunned)
        {
            spriteBatch.Draw(texture, position + new Vector2(0, texture.Height / 2f - 10f) + (new Vector2((float)Math.Sin(timer + Math.PI + MathHelper.PiOver4) * 2f, (float)Math.Cos(timer + Math.PI + MathHelper.PiOver4) * 0.25f) * 15f), texture.Frame(), Color.White, NPC.rotation,
                new Vector2(texture.Width / 2f, texture.Height), 1f + (float)Math.Cos(timer + Math.PI + MathHelper.PiOver4) * 0.2f, spriteEffects, 0);
        }
        return false;
    }
}

public class FlyladDeath : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Starlads/"+Name;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 5;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.timeLeft = 1000;
        Projectile.tileCollide = false;

        Projectile.Size = new Vector2(62);
        Projectile.scale = 1f;

        Projectile.aiStyle = -1;
    }
    public override bool? CanDamage() => false;

    public override void AI()
    {
        Projectile.spriteDirection = Projectile.direction;
        Projectile.rotation += Projectile.velocity.X * 0.2f;

        Projectile.ai[1]++;

        if (Projectile.ai[1] == 1f)
        {
            Projectile.velocity.Y -= 10f;
            Projectile.velocity.X += -Projectile.direction;
        }

        Projectile.velocity.Y += 0.2f;
    }

    public override bool PreDraw(ref Color drawColor)
    {
        Texture2D texture = TextureAssets.Projectile[Type].Value;

        Vector2 position = Projectile.Center - Main.screenPosition + new Vector2(0, Projectile.gfxOffY);

        SpriteEffects spriteEffects = SpriteEffects.None;

        Main.spriteBatch.Draw(texture, position, null, Color.White, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, spriteEffects, 0);

        return false;
    }
}

public class FlyladStarBomb : ModProjectile
{
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Starlads/"+Name;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
        ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
    }

    public override void SetDefaults()
    {
        Projectile.penetrate = -1;
        Projectile.friendly = false;
        Projectile.hostile = true;

        Projectile.Size = new Vector2(18);
        Projectile.scale = 1f;

        Projectile.tileCollide = true;
        Projectile.ignoreWater = true;

        Projectile.aiStyle = -1;
    }

    public override void AI()
    {
        Projectile.rotation += 0.4f;

        Projectile.velocity.Y += 0.5f;
    }

    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.ai[0] >= 1f)
        {
            Projectile.Kill();
        }
        else
        {
            SoundEngine.PlaySound(SoundID.DD2_WitherBeastCrystalImpact, Projectile.position);

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) { Projectile.velocity.X = -oldVelocity.X; }
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) { Projectile.velocity.Y = -oldVelocity.Y; }

            int numberOfDusts = 20;
            float radius = 2;

            for (int i = 0; i < numberOfDusts; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Vector2.UnitX.RotatedBy(MathHelper.ToRadians(360f / numberOfDusts * i)) * radius, 0, new Color(241, 212, 62), 1.2f).noGravity = true;
            }
        }

        Projectile.ai[0]++;

        return false;
    }

    List<int> GoreTypes = new List<int>()
        {
            GoreType<StarG0>(),GoreType<StarG1>(),GoreType<StarG2>(),GoreType<StarG3>(),GoreType<StarG4>(),GoreType<StarG5>(),GoreType<StarG6>(),
        };
    public override void OnKill(int timeLeft)
    {
        SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath);

        for (int k = 0; k < 3; k++)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Gore.NewGore(Projectile.GetSource_Death(), Projectile.position, Main.rand.NextVector2CircularEdge(0.5f, 0.5f) * Projectile.velocity.Length(), Main.rand.NextFromList(GoreTypes.ToArray()), 1f);
            }
        }

        for (int i = 0; i < 10; i++)
        {
            Dust.NewDustPerfect(Projectile.Center, DustID.PortalBoltTrail, Main.rand.NextVector2Circular(0.5f, 0.5f) * Projectile.velocity.Length(), 0, new Color(241, 212, 62), 1.2f).noGravity = true;
        }
    }

    public override bool PreDraw(ref Color lightColor)
    {
        for (int k = 0; k < Projectile.oldPos.Length; k++)
        {
            Texture2D texture = TextureAssets.Projectile[Projectile.type].Value;

            Vector2 drawOrigin = new(texture.Width * 0.5f, Projectile.height * 0.5f);
            Vector2 drawPos = Projectile.oldPos[k] - Main.screenPosition + drawOrigin + new Vector2(0f, Projectile.gfxOffY);
            Color color = Projectile.GetAlpha(Color.White) * ((float)(Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);

            Main.EntitySpriteDraw(texture, drawPos, null, color, Projectile.rotation, drawOrigin, Projectile.scale * (ProjectileID.Sets.TrailCacheLength[Projectile.type] - k) / ProjectileID.Sets.TrailCacheLength[Projectile.type], SpriteEffects.None, 0);
        }

        return true;
    }

    public override Color? GetAlpha(Color lightColor) => Color.White;
}

