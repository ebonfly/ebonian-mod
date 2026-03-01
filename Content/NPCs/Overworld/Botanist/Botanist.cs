using AssGen;
using EbonianMod.Content.Items.Pets.LilPilg;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.NPCs.Corruption;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Content.NPCs.Overworld.Botanist;
public class Botanist : ModNPC
{ 
    public override string Texture => Helper.AssetPath + "NPCs/Overworld/Botanist/"+Name;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[NPC.type] = 3;
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        if (spawnInfo.Player.ZoneOverworldHeight && spawnInfo.Player.ZoneForest)
            return .07f;
        else
            return 0;
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
        {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Plant"),
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
        });
    }
    public override void SetDefaults()
    {
        NPC.width = 40;
        NPC.height = 22;
        NPC.damage = 10;
        NPC.defense = 3;
        NPC.lifeMax = 35;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath2;
        NPC.value = 30;
        NPC.knockBackResist = 0.1f;
        NPC.aiStyle = -1;
        NPC.noGravity = false;
        NPC.noTileCollide = false;
        NPC.hide = true;
    }
    private const int Idle = 0;
    private const int Walk = 1;
    private const int Attack = 2;
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
    Vector2 headOffset;
    float headRotation;
    public override void DrawBehind(int index)
    {
        Main.instance.DrawCacheNPCsMoonMoon.Add(index);
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemType<BotanistHeadStaff>(), 30));
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D neck = Assets.NPCs.Overworld.Botanist.BotanistNeck.Value;
        Texture2D head = Assets.NPCs.Overworld.Botanist.BotanistHead.Value;

        Vector2 baseC = NPC.Center + new Vector2(0, 6);
        Vector2 center = baseC;
        Vector2 distVector = headOffset;
        float distance = distVector.Length();
        int attempts = 0;
        while (distance > (neck.Height / 3) + 5 && !float.IsNaN(distance) && ++attempts < 400)
        {
            distVector.Normalize();
            distVector *= (neck.Height / 3) * 0.65f;
            center += distVector;
            distVector = baseC + headOffset - center;
            distance = distVector.Length();
            int frameY = 6 * (attempts % 3);
            spriteBatch.Draw(neck, center + NPC.GFX() - Main.screenPosition, new Rectangle(0, frameY, 10, 6), NPC.HunterPotionColor(Lighting.GetColor((center).ToTileCoordinates())), distVector.ToRotation() + PiOver2, neck.Size() / 2, NPC.scale, SpriteEffects.None, 0);
        }

        void DrawHead() => spriteBatch.Draw(head, center + NPC.GFX() - screenPos + new Vector2(NPC.direction, (NPC.frame.Y == 24 ? -1 : 0)).RotatedBy(headRotation), null, NPC.HunterPotionColor(Lighting.GetColor((NPC.Center + headOffset).ToTileCoordinates())), headRotation, head.Size() / 2, NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        if (AIState == Idle)
            DrawHead();

        spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center + NPC.GFX() + new Vector2(0, 4 + (NPC.frame.Y == 24 ? -1 : 0)) - screenPos, NPC.frame, NPC.HunterPotionColor(drawColor), NPC.rotation, NPC.Size / 2, NPC.scale, NPC.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

        if (AIState != Idle)
            DrawHead();

        return false;
    }
    public override void FindFrame(int frameHeight)
    {
        if (MathF.Abs(NPC.velocity.X) > .1f)
        {
            if (++NPC.frameCounter % 5 == 0)
            {
                if (NPC.frame.Y < frameHeight * 2)
                    NPC.frame.Y += frameHeight;
                else
                    NPC.frame.Y = 0;
            }
        }
        else
            NPC.frame.Y = 0;
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            NPC.SpawnGore("EbonianMod/BotanistHead", 1);
            NPC.SpawnGore(GoreID.TreeLeaf_Normal, 10);
        }
    }
    public override void AI()
    {
        Player player = Main.player[NPC.target];
        headRotation = headOffset.ToRotation() + PiOver2;
        if (AIState == Idle)
        {
            NPC.TargetClosest(true);
            if (NPC.HasValidTarget)
            {
                if (NPC.Distance(player.Center) < 100 || NPC.life < NPC.lifeMax)
                {
                    if (AITimer <= 0)
                        AITimer = 1;
                }
            }
            if (AITimer > 0)
            {
                AITimer++;
                headOffset.Y -= 2;
                if (AITimer > 40)
                {
                    AITimer = 20;
                    AIState = Attack;
                }
            }
        }
        else if (AIState == Walk)
        {
            headRotation = Utils.AngleLerp(headRotation, 0, 0.1f);
            headOffset = Vector2.Lerp(headOffset, new Vector2(0, -20), 0.1f);
            NPC.damage = 0;
            AITimer2++;
            AITimer = MathHelper.Clamp(AITimer, 0, 500);
            if (AITimer2 % 7 == 0)
                NPC.direction = NPC.Center.X < player.Center.X ? 1 : -1;

            NPC.spriteDirection = NPC.direction;

            if (player.Center.Distance(NPC.Center) > 100 && (player.Center.Y - NPC.Center.Y < 100 || player.Center.Y - NPC.Center.Y > -100))
                NPC.velocity.X = Lerp(NPC.velocity.X, NPC.direction * 1.7f, 0.1f);
            if (player.Center.Distance(NPC.Center) < 100)
            {
                NPC.netUpdate = true;
                AITimer += 4;
                NPC.velocity.X *= 0.9f;
            }
            if (MathF.Abs(player.Center.Y - NPC.Center.Y) > 100)
            {
                AITimer--;
            }

            Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);

            if (NPC.collideY && NPC.collideX)
            {
                NPC.velocity.Y = -4;
            }

            if (AITimer >= 100)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = 0;
                AIState = Attack;
                AITimer = 0;
            }
        }
        else
        {
            AITimer++;

            if (AITimer < 20)
                headOffset.Y -= 3;
            else if (AITimer < 50)
            {
                headOffset.X -= NPC.direction * Lerp(2.5f, 0.5f, InOutCirc.Invoke((AITimer - 20) / 30f));
                headOffset.Y += 0.5f;
            }
            else if (AITimer < 90)
            {
                Vector2 point = Helper.Raycast(NPC.Center + new Vector2(Clamp(MathF.Abs(Helper.FromAToB(NPC.Center, player.Center, false).X), 0, 100) * NPC.direction, -40), Vector2.UnitY, 200).Point + new Vector2(0, 8);
                Vector2 desiredP = Helper.FromAToB(NPC.Center, point, false);
                headOffset.X = Lerp(headOffset.X, desiredP.X, Lerp(0, 0.7f, Clamp((AITimer - 50) / 5f, 0, 1)));
                if (AITimer > 55)
                    headOffset.Y = Lerp(headOffset.Y, desiredP.Y, 0.5f);
                else
                    headOffset.Y -= 2;
                if (Helper.FromAToB(headOffset, Helper.FromAToB(NPC.Center, point, false), false).Length() < 5)
                {
                    SoundEngine.PlaySound(SoundID.Item70, NPC.Center + headOffset);
                    Projectile.NewProjectile(null, NPC.Center + headOffset, Vector2.Zero, ProjectileType<BotanistP>(), 5, 0);
                    Helper.AddCameraModifier(new PunchCameraModifier(NPC.Center + headOffset, Vector2.UnitY, 1, 10, 30, 700));
                    AITimer = 90;
                    AITimer2 = 1;
                }
            }
            else headOffset -= new Vector2(7 * NPC.direction, 13) * Lerp(1, 0.5f, (AITimer - 90) / 10f);

            if (AITimer >= 100)
            {
                NPC.netUpdate = true;
                NPC.velocity.X = 0;
                AIState = Walk;
                AITimer = 0;
                AITimer2 = 0;
            }
        }
    }
}
public class BotanistP : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.Size = new(40);
        Projectile.penetrate = -1;
        Projectile.tileCollide = false;
        Projectile.aiStyle = -1;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.timeLeft = 5;
    }
    public override void OnSpawn(IEntitySource source)
    {
        for (int i = 0; i < 4; i++)
            Collision.HitTiles(Projectile.position, Vector2.UnitY * -10, 60, 30);
    }
}
