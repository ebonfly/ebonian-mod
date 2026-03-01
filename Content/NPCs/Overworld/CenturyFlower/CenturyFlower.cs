using EbonianMod.Common.Globals;
using EbonianMod.Common.Misc;
using EbonianMod.Content.Items.Weapons.Magic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace EbonianMod.Content.NPCs.Overworld.CenturyFlower
{
    // I need to rewrite this at some point because this is some mentally ill code </3 sorry whoever made this
    public class CenturyFlower : CommonNPC
    {
        public override string Texture => Helper.AssetPath + "NPCs/Overworld/CenturyFlower/"+Name;
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = 10;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
            {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.DayTime,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Plant"),
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
            });
        }
        public override void SetDefaults()
        {
            NPC.width = 30;
            NPC.height = 70;
            NPC.damage = 12;
            NPC.lifeMax = 30;
            NPC.defense = 3;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath32;
            NPC.buffImmune[BuffID.Poisoned] = true;
            NPC.aiStyle = -1;
            NPC.netAlways = true;
        }
        public override void ApplyDifficultyAndPlayerScaling(int numPlayers, float balance, float bossAdjustment)
        {
            NPC.lifeMax = (int)(NPC.lifeMax * 0.8f * balance);
        }
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = Assets.NPCs.Overworld.CenturyFlower.CenturyFlower_Glow.Value;
            Texture2D origTexture = TextureAssets.Npc[NPC.type].Value;
            Vector2 orig = new Vector2(NPC.width / 2, NPC.height - 2);
            Main.spriteBatch.Draw(origTexture, NPC.Bottom + NPC.GFX() - screenPos, NPC.frame, drawColor, NPC.rotation, orig, NPC.scale, NPC.direction < 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            Main.spriteBatch.Draw(texture, NPC.Bottom + NPC.GFX() - screenPos, NPC.frame, Color.White, NPC.rotation, orig, NPC.scale, NPC.direction < 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0f);
            return false;
        }
        public override void FindFrame(int frameHeight)
        {
            NPC.frameCounter++;
            if ((int)AIState == 0)
            {
                if (NPC.velocity.Y > 0.05f)
                    NPC.frame.Y = frameHeight;
                else if (MathF.Abs(NPC.velocity.X) < 0.5f)
                    NPC.frame.Y = 0;
                else if (NPC.frameCounter++ % 5 == 0)
                {
                    if (NPC.frame.Y < frameHeight * 4)
                        NPC.frame.Y += frameHeight;
                    else NPC.frame.Y = 0;
                }
            }
            else if (MathF.Abs(NPC.velocity.X) < 2)
            {
                if (NPC.frame.Y < frameHeight * 5)
                    NPC.frame.Y = frameHeight * 5;
                if (NPC.frameCounter++ % 5 == 0)
                {
                    if (AITimer < 150 && NPC.frame.Y < 9 * frameHeight)
                        NPC.frame.Y += frameHeight;
                    else if (AITimer >= 150 && NPC.frame.Y > frameHeight * 5)
                        NPC.frame.Y -= frameHeight;
                }
            }
        }
        public override void AI()
        {
            NPC.rotation = Utils.AngleLerp(NPC.rotation, 0, 0.1f);
            NPC.TargetClosest();
            Player player = Main.player[NPC.target];
            NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;
            NPC.spriteDirection = NPC.direction;

            if ((int)AIState == 0)
            {
                if (MathF.Abs(NPC.Center.X - player.Center.X) > 50)
                    NPC.GetGlobalNPC<FighterGlobalAI>().FighterAI(NPC, 6, 1);
                else
                    NPC.velocity.X *= 0.7f;
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);

                float dist = player.Distance(NPC.Center);
                AITimer += dist switch
                {
                    < 900 and > 400 => 1,
                    > 250 and < 400 => 2,
                    > 100 and < 250 => 3,
                    > 50 and < 100 => 4,
                    < 50 => 5,
                    _ => 0
                };
                if (AITimer > 250 && dist < 70)
                    SwitchState(1);
            }
            else
            {
                AITimer++;
                NPC.velocity.X *= 0.9f;
                if (MathF.Abs(NPC.velocity.X) < 1f)
                    NPC.velocity.X = 0;
                if (AITimer < 50 && (int)AITimer % 20 == 0)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center - new Vector2(1, 16), Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 2) * Lerp(0, 1, AITimer / 50f), ModContent.ProjectileType<CenturyFlowerSpore.CenturyFlowerSpore>(), 0, 0);
                    NPC.rotation = Main.rand.NextFloat(0.025f) * NPC.direction;
                }
                if (AITimer is > 75 and < 150 && (int)AITimer % 5 == 0)
                {
                    Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center - new Vector2(1, 19), Main.rand.NextVector2Unit() * Main.rand.NextFloat(1, 2), ModContent.ProjectileType<CenturyFlowerSpore.CenturyFlowerSpore>(), 0, 0);
                    NPC.rotation = Main.rand.NextFloat(0.035f) * NPC.direction;
                }
                if (AITimer >= 175)
                    SwitchState(0);
            }
        }

        public override void HitEffect(NPC.HitInfo hit)
        {
            if (NPC.life <= 0)
            {
                if (Main.netMode == NetmodeID.Server)
                    return;

                for (int i = 0; i < 5; i++)
                {
                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitY, 910, 0.7f + Main.rand.NextFloat() * 0.6f);
                }
                Helper.SpawnGore(NPC, "EbonianMod/Century", 4, 1, Vector2.One * hit.HitDirection);
                Helper.SpawnGore(NPC, "EbonianMod/Century", 2, 2, Vector2.One * hit.HitDirection);
                Helper.SpawnGore(NPC, "EbonianMod/Century", 2, 3, Vector2.One * hit.HitDirection);
                Helper.SpawnGore(NPC, "EbonianMod/Century", 2, 4, Vector2.One * hit.HitDirection);
                Helper.SpawnDust(NPC.position, NPC.Size, DustID.Grass, new Vector2(2 * hit.HitDirection, -1.5f), 10);
            }
        }

        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            if (Main.invasionType > 0) return 0;
            if (spawnInfo.Player.ZonePurity)
                return SpawnCondition.OverworldDay.Chance * 0.1f;
            return 0;
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CenturySprayer>(), 50));
        }
    }
}