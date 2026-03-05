using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Friendly.Crimson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Content.Items.Weapons.Ranged;
public class BlindeyeBow : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/BlindeyeBow";
    public override void SetDefaults()
    {
        Item.width = 30;
        Item.height = 74;
        Item.crit = 25;
        Item.damage = 40;
        Item.useAnimation = 40;
        Item.useTime = 40;
        Item.noUseGraphic = true;
        Item.autoReuse = false;
        Item.noMelee = true;
        Item.channel = true;
        Item.value = Item.buyPrice(0, 10, 0, 0);
        Item.DamageType = DamageClass.Ranged;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.rare = ItemRarityID.LightRed;
        Item.shootSpeed = 1f;
        Item.shoot = ProjectileType<BlindeyeBowP>();
        Item.useAmmo = AmmoID.Arrow;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, velocity, ProjectileType<BlindeyeBowP>(), damage, knockback, player.whoAmI);
        return false;
    }
    public override bool? CanAutoReuseItem(Player player) => false;
    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[Item.shoot] < 1;
    }
    public override void AddRecipes() => CreateRecipe().AddIngredient<CecitiorClawMaterial>(4).AddIngredient<CecitiorMaterial>(10).Register();
}
public class BlindeyeBowP : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/BlindeyeBowP";
    const float holdOffset = 10;
    const int maxTime = 45;
    public override void SetDefaults()
    {
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.Size = new(30, 74);
        Projectile.ignoreWater = true;
        Projectile.DamageType = DamageClass.Ranged;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.timeLeft = maxTime;
    }
    public override bool? CanDamage() => false;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DontCancelChannelOnKill[Type] = true;
    }
    public virtual float Ease(float x)
    {
        return 1 - MathF.Pow(1 - x, 5);
    }
    public virtual float ScaleFunction(float progress)
    {
        return 0.7f + (float)Math.Sin(progress * Math.PI) * 0.5f;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        float off = MathHelper.Lerp(2, 0, Projectile.ai[2] / 30);
        if (Projectile.ai[2] > 30)
            off = 0;
        Vector2 connectionPoint = new Vector2(8, 22);
        Vector2 connectionPoint2 = new Vector2(4, -32);
        if (Projectile.direction == -1)
        {
            connectionPoint = new Vector2(2, 32);
            connectionPoint2 = new Vector2(8, -32);
        }
        Utils.DrawLine(Main.spriteBatch, Projectile.Center - connectionPoint.RotatedBy(Projectile.rotation), Projectile.Center - new Vector2(8 + Projectile.ai[2], -off).RotatedBy(Projectile.rotation), new Color(192, 171, 7), new Color(255, 202, 81), 2);
        Utils.DrawLine(Main.spriteBatch, Projectile.Center - connectionPoint2.RotatedBy(Projectile.rotation), Projectile.Center - new Vector2(8 + off + Projectile.ai[2], off).RotatedBy(Projectile.rotation), new Color(192, 171, 7), new Color(255, 202, 81), 2);

        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, Projectile.scale, Projectile.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
    float alpha;
    SoundStyle pull = Sounds.bowPull with
    {
        PitchVariance = 0.35f,
    };
    SoundStyle release = Sounds.bowRelease with
    {
        PitchVariance = 0.25f,
    };
    public override void PostDraw(Color lightColor)
    {
        Texture2D tex = Assets.Projectiles.Friendly.Crimson.BlindeyeArrow.Value;
        Main.spriteBatch.Draw(tex, Projectile.Center + Vector2.Lerp(new Vector2(20, 0).RotatedBy(Projectile.rotation), Vector2.Zero, alpha) - Main.screenPosition, null, lightColor * alpha, Projectile.rotation + MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
    }
    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, 0.25f, 0, 0);
        Player player = Main.player[Projectile.owner];
        if (player.HeldItem.type != ItemType<BlindeyeBow>()) { Projectile.Kill(); return; }
        if (!player.active || player.dead || player.CCed || player.noItems || !player.channel)
        {
            Projectile.Kill();
            return;
        }
        else
        {
            player.itemTime = 2;
            player.itemAnimation = 2;
            if (Projectile.timeLeft == 1 && player.channel && !(!player.active || player.dead || player.CCed || player.noItems || !player.channel || player.HeldItem.type != ItemType<BlindeyeBow>()))
            {
                Projectile.timeLeft = maxTime;
            }
        }
        Projectile.direction = Projectile.velocity.X > 0 ? 1 : -1;
        Vector2 pos = player.RotatedRelativePoint(player.MountedCenter);
        player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
        player.itemRotation = (Projectile.velocity.ToRotation() + Projectile.ai[0]) * player.direction;
        player.heldProj = Projectile.whoAmI;
        Projectile.direction = Projectile.spriteDirection = player.direction;
        pos += (Projectile.velocity.ToRotation()).ToRotationVector2() * holdOffset;
        Projectile.Center = pos;
        Projectile.rotation = Projectile.velocity.ToRotation();
        if (player.gravDir != -1)
            player.SetCompositeArmFront(true, Projectile.timeLeft > 40 || Projectile.timeLeft < 25 ? Player.CompositeArmStretchAmount.Quarter : Player.CompositeArmStretchAmount.None, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
        if (player.gravDir != -1)
            player.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, Projectile.velocity.ToRotation() - MathHelper.PiOver2);
        if (Projectile.timeLeft > 25)
        {
            if (Projectile.timeLeft == maxTime - 1)
            {
                bool success = false;
                for (int j = 0; j < 58; j++)
                {
                    if (player.inventory[j].ammo == player.HeldItem.useAmmo && player.inventory[j].stack > 0)
                    {
                        if (player.inventory[j].maxStack > 1)
                            player.inventory[j].stack--;
                        success = true;
                        break;
                    }
                }
                if (!success)
                {
                    Projectile.Kill();
                    return;
                }
            }
            if (Projectile.timeLeft == maxTime - 3)
                SoundEngine.PlaySound(pull.WithPitchOffset(2), Projectile.Center);
            float progress = Ease(Utils.GetLerpValue(0f, maxTime - 25, Projectile.timeLeft));
            alpha = MathHelper.Lerp(alpha, 1f, 0.1f);
            if (Projectile.timeLeft > 20 && Projectile.timeLeft < maxTime - 1)
                Projectile.ai[2] += 0.4f;
            if (Main.myPlayer == Projectile.owner)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(player.Center, Main.MouseWorld), 0.8f - MathHelper.Lerp(0, 0.2f, Projectile.timeLeft / 30)).SafeNormalize(Vector2.UnitX);
        }
        else
        {
            if (Projectile.timeLeft == 24)
            {
                SoundEngine.PlaySound(release, Projectile.Center);
                if (player.whoAmI == Main.myPlayer)
                    for (float i = 0.25f; i < 1; i += 0.25f)
                        Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity * 20 * i, ProjectileType<BlindeyeArrow>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                alpha = 0;
            }
            if (Projectile.timeLeft > 20)
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], -5, 0.4f);
            else
                Projectile.ai[2] = MathHelper.Lerp(Projectile.ai[2], 0, 0.2f);
        }

        player.itemTime = 2;
        player.itemAnimation = 2;
        Projectile.netUpdate = true;
    }
}
