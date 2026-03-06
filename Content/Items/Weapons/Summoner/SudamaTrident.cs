using EbonianMod.Common.Players;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.Projectiles.Enemy.Overworld;
using EbonianMod.Content.Projectiles.Friendly.Corruption;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using EbonianMod.Common.Graphics;

namespace EbonianMod.Content.Items.Weapons.Summoner;
public class SudamaTrident : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Summoner/"+Name;
    public override void SetStaticDefaults()
    {
        Item.staff[Item.type] = true;
        ItemID.Sets.ShimmerTransformToItem[Type] = ModContent.ItemType<KodamaStaff>();
    }
    public override void SetDefaults()
    {
        Item.damage = 20;
        Item.width = 40;
        Item.height = 40;
        Item.mana = 20;
        Item.DamageType = DamageClass.Summon;
        Item.useTime = 20;
        Item.useAnimation = 20;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 10;
        Item.rare = ItemRarityID.Green;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.UseSound = SoundID.Item8;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<SudamaF>();
        Item.buffType = BuffType<Buffs.SudamaBuff>();
        Item.shootSpeed = 1;
    }
    public override void HoldItem(Player player)
    {
        if (!player.ItemTimeIsZero)
            Lighting.AddLight(player.itemLocation, 197f / 255f, 226f / 255f, 105f / 255f);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        if (Main.myPlayer != player.whoAmI) return false;
        player.AddBuff(Item.buffType, 2);
        Projectile projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
        projectile.originalDamage = Item.damage;
        projectile.SyncProjectile();
        return false;
    }
}
public class SudamaF : ModProjectile
{
    public override string Texture => Helper.AssetPath+"NPCs/Overworld/Sudama";
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
        ProjectileID.Sets.MinionSacrificable[Type] = true;
        ProjectileID.Sets.MinionTargettingFeature[Type] = true;
    }
    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(40);
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Summon;
        Projectile.tileCollide = false;
        Projectile.ignoreWater = true;
        Projectile.aiStyle = -1;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 70;
        Projectile.minion = true;
        Projectile.minionSlots = 1;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        PixelationRendering.DrawCache.Add(() =>
        {
            List<VertexPositionColorTexture> vertices = new();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                Vector2 oldPos = Projectile.oldPos[i] + new Vector2(0, MathF.Sin(i * 0.5f) * 4).RotatedBy(Projectile.rotation);
                float mult = (1 - 1f / Projectile.oldPos.Length * i);
                float rotOffset = Helper.FromAToB(oldPos, Projectile.position).ToRotation();
                if (i > 0)
                {
                    Vector2 oldPos2 = Projectile.oldPos[i - 1] + new Vector2(0, MathF.Sin(i * 0.5f) * 4).RotatedBy(Projectile.rotation);
                    rotOffset = Helper.FromAToB(oldPos2, oldPos).ToRotation();
                }
                rotOffset += MathF.Sin(Main.GlobalTimeWrappedHourly * 6 + Projectile.whoAmI * 10) * SmoothStep(1, 0, mult);
                Vector2 off = i <= 1 ? Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length() * 0.5f : Vector2.Zero;
                Vector2 pos = oldPos + Projectile.Size / 2 + new Vector2(0, 4) - rotOffset.ToRotationVector2() * 10 + off - Main.screenPosition;
                vertices.Add(Helper.AsVertex(pos + new Vector2(21 * mult, 0).RotatedBy(PiOver2 + rotOffset), Color.White * (i < 2 ? 0 : 1), new Vector2((float)i / Projectile.oldPos.Length * 3 - Main.GlobalTimeWrappedHourly * 1.5f, 0)));
                vertices.Add(Helper.AsVertex(pos + new Vector2(21 * mult, 0).RotatedBy(-PiOver2 + rotOffset), Color.White * (i < 2 ? 0 : 1), new Vector2((float)i / Projectile.oldPos.Length * 3 - Main.GlobalTimeWrappedHourly * 1.5f, 1)));
            }
            if (vertices.Count > 2)
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.NPCs.Overworld.SudamaTrail.Value, false, true);
        }
        );
        FinalDrawSystem.DrawCache.Add(() =>
            Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White, Projectile.rotation, Projectile.Size / 2, Projectile.scale, SpriteEffects.FlipHorizontally, 0)
        );
        return false;
    }
    public override bool? CanCutTiles() => false;
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        SummonPlayer modPlayer = player.GetModPlayer<SummonPlayer>();
        if (player.dead)
            modPlayer.sudamaMinion = false;
        if (modPlayer.sudamaMinion)
            Projectile.timeLeft = 10;

        Projectile.direction = Projectile.spriteDirection = -1;
        if (!player.active) return;
        Projectile.ai[0]++;

        foreach (Projectile p in Main.ActiveProjectiles)
        {
            if (p.type == Type && p.whoAmI != Projectile.whoAmI && Projectile.Distance(p.Center) < p.Size.Length() / 2f)
            {
                if (p.Center.Distance(Projectile.Center) < p.width * p.scale)
                {
                    Projectile.Center += Projectile.Center.FromAToB(p.Center, true, true) * 2;
                }
                if (p.Center == Projectile.Center)
                {
                    Projectile.Center += Main.rand.NextVector2Unit() * 5;
                }
            }
        }

        Vector2 targetPos = Projectile.position;
        Vector2 targetVel = Projectile.velocity;
        Vector2 targetSize = Vector2.One;
        int index = -1;
        float targetDist = 1000;
        bool target = false;
        if (player.HasMinionAttackTargetNPC)
        {
            NPC npc = Main.npc[player.MinionAttackTargetNPC];
            targetPos = npc.Center;
            targetDist = Vector2.Distance(Projectile.Center, targetPos);
            targetVel = npc.velocity;
            targetSize = npc.Size;
            index = npc.whoAmI;
            target = true;
        }
        else
        {
            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.active)
                    if (npc.CanBeChasedBy(this, false))
                    {
                        float distance = Vector2.Distance(npc.Center, Projectile.Center);
                        if (distance < targetDist || !target)
                        {
                            targetDist = distance;
                            targetPos = npc.Center;
                            targetVel = npc.velocity;
                            targetSize = npc.Size;
                            index = npc.whoAmI;
                            target = true;
                        }
                    }
            }
        }
        if (player.Center.Distance(Projectile.Center) > 2000)
            Projectile.Center = player.Center;
        float off = 0;
        if (player.ownedProjectileCounts[Type] > 1)
            off = Helper.CircleDividedEqually(Projectile.minionPos, player.ownedProjectileCounts[Type]);
        if (target && targetDist < 1000)
        {
            if (targetDist < 100)
                Projectile.ai[2] = Lerp(Projectile.ai[2], 50, 0.05f);
            else
                Projectile.ai[2] = Lerp(Projectile.ai[2], 15, 0.025f);
            if (Projectile.ai[0] % 2 == 0 && !Collision.CheckAABBvLineCollision(targetPos, targetSize, Projectile.Center, Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.UnitY) * 500))
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.position.FromAToB(targetPos + targetVel + new UnifiedRandom(index).NextVector2Circular(targetSize.X * 0.5f, targetSize.Y * 0.5f)).RotatedByRandom(0.1f) * Projectile.ai[2], 0.1f);
            else if (Projectile.velocity.Length() < Projectile.ai[2])
                Projectile.velocity *= 1.05f;
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.2f);
        }
        else
        {
            Projectile.ai[2] = Lerp(Projectile.ai[2], 0, 0.1f);
            UnifiedRandom rand = new UnifiedRandom(Projectile.whoAmI * 5);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.FromAToB(player.Center + new Vector2(0, -100 - player.ownedProjectileCounts[Type] * 10 + MathF.Sin(Projectile.ai[0] * 0.01f + rand.Next(10000)) * 40).RotatedBy(off + ToRadians(Projectile.ai[0] * 3)), false) / rand.NextFloat(10, 20), rand.NextFloat(0.05f, 0.2f));
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, Projectile.velocity.ToRotation(), 0.2f);
        }
    }
}
