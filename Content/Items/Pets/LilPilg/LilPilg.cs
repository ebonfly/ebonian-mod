using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using EbonianMod.Content.Dusts;
using EbonianMod.Core.Systems.Scenes;

namespace EbonianMod.Content.Items.Pets.LilPilg
{
    public class LilPilgI : ModItem
    {
        public override string Texture => Helper.AssetPath + "Items/Pets/LilPilg/LilPilgI";
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void SetDefaults()
        {
            Item.DefaultToVanitypet(ModContent.ProjectileType<LilPilg>(), ModContent.BuffType<LilPilgB>());
            Item.rare = ItemRarityID.Cyan;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.UseSound = SoundID.NPCHit54;
            Item.useTime = Item.useAnimation = 80;
        }
    }
    public class LilPilg : ModProjectile
    {
        public override string Texture => Helper.AssetPath + "Items/Pets/LilPilg/LilPilg";
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 4;
            Main.projPet[Projectile.type] = true;

            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.CloneDefaults(ProjectileID.BabyFaceMonster);
            Projectile.Size = new Vector2(24, 44);
            AIType = ProjectileID.BabyFaceMonster;
        }

        public override bool PreAI()
        {
            Player player = Main.player[Projectile.owner];

            player.babyFaceMonster = false;

            return true;
        }
        int frameCounter;
        Rectangle frames;
        public override bool PreDraw(ref Color lightColor)
        {
            frames.Width = 24;
            frames.Height = 46;

            Texture2D trail = Assets.ExtraSprites.Projectiles.LilPilg_Trail.Value;
            Texture2D tex = TextureAssets.Projectile[Type].Value;
            Texture2D glow = Assets.ExtraSprites.Projectiles.LilPilg_Glow.Value;
            for (int i = 0; i < ProjectileID.Sets.TrailCacheLength[Type]; i++)
            {
                float fadeMult = 1f / ProjectileID.Sets.TrailCacheLength[Type];
                Main.spriteBatch.Draw(trail, Projectile.oldPos[i] + frames.Size() / 2 + new Vector2(0, 4) - Main.screenPosition, frames, Color.White * (1f - fadeMult * i) * (Projectile.velocity.Length() / 5f).Saturate(), Projectile.oldRot[i], frames.Size() / 2, Projectile.scale*0.9f, Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(tex, Projectile.Center  - Main.screenPosition + new Vector2(0, 4), frames, lightColor, Projectile.rotation, frames.Size() / 2, Projectile.scale, Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            Main.spriteBatch.Draw(glow, Projectile.Center - Main.screenPosition + new Vector2(0, 4), frames, Color.White, Projectile.rotation, frames.Size() / 2, Projectile.scale, Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
            return false;
        }

        public override void AI()
        {
            Player player = Main.player[Projectile.owner];

            if (!player.dead && player.HasBuff(ModContent.BuffType<LilPilgB>()))
            {
                Projectile.timeLeft = 10;
            }

            frameCounter++;
            int height = frames.Height;
            if (Projectile.ai[2] == 0)
            {
                if (Projectile.frame >= 8)
                {
                    Projectile.rotation = MathHelper.Clamp(Projectile.velocity.X * .15f, MathHelper.ToRadians(-30), MathHelper.ToRadians(30));
                    Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
                    if (frames.Y < 6 * height)
                        frames.Y = 6 * height;
                    if (frameCounter % 5 == 0)
                    {
                        frames.Y += height;
                        if (frames.Y > 9 * height)
                            frames.Y = 6 * height;
                    }
                }
                else if (Projectile.velocity.Y >= 0f && (double)Projectile.velocity.Y <= 0.8)
                {
                    if ((double)Projectile.velocity.X < -0.8 || (double)Projectile.velocity.X > 0.8)
                    {
                        if (frames.Y < 2 * height)
                            frames.Y = 2 * height;
                        if (frameCounter % 5 == 0)
                        {
                            frames.Y += height;
                            if (frames.Y > 5 * height)
                                frames.Y = 2 * height;
                        }
                    }
                    else frames.Y = 0;
                }
                else
                {
                    frames.Y = 2 * height;
                }
            }
            if (Projectile.oldPosition == Projectile.position)
            {
                Projectile.ai[2]++;

                if (Projectile.ai[2] > 25 * 60)
                    if (Projectile.ai[2] % 60 == 0)
                    {
                        Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<ZzzDust>(), Projectile.direction * Vector2.UnitX * Main.rand.NextFloat(0, 1.5f));
                    }

                if (Projectile.ai[2] > 20 * 60)
                {
                    if (frames.Y < 12 * frames.Height)
                        frames.Y = 12 * frames.Height;
                    if (Projectile.ai[2] % 5 == 0)
                        if (frames.Y < 16 * frames.Height)
                            frames.Y += frames.Height;
                }
                else if (Projectile.ai[2] > 10 * 60)
                {
                    if (frames.Y < 6 * frames.Height)
                        frames.Y = 6 * frames.Height;
                    if (Projectile.ai[2] % 5 == 0)
                        if (frames.Y < 12 * frames.Height)
                            frames.Y += frames.Height;
                }
                else if (RiverOfStarlight.ActiveConditions && (player.ZoneOverworldHeight || player.ZoneSkyHeight) && Projectile.ai[2] > 60 * 2)
                {
                    frames.Y = frames.Height;
                }
            }
            else Projectile.ai[2] = 0;
        }
    }
    public class LilPilgB : ModBuff
    {
        public override string Texture => Helper.AssetPath + "Items/Pets/LilPilg/LilPilgB";
        public override void SetStaticDefaults()
        {
            Main.buffNoTimeDisplay[Type] = true;
            Main.vanityPet[Type] = true;
        }
        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000;

            int projType = ModContent.ProjectileType<LilPilg>();


            if (player.whoAmI == Main.myPlayer && player.ownedProjectileCounts[projType] <= 0)
            {
                var entitySource = player.GetSource_Buff(buffIndex);

                Projectile.NewProjectile(entitySource, player.Center, Vector2.Zero, projType, 0, 0f, player.whoAmI);
            }
        }
    }
}
