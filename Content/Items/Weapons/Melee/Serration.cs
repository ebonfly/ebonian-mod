using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Crimson;
using System;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Content.Items.Weapons.Melee;

internal class Serration : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Melee/Serration";
    public override void SetDefaults()
    {
        Item.knockBack = 10f;
        Item.Size = new Vector2(60);
        Item.crit = 10;
        Item.damage = 100;
        Item.useAnimation = 40;
        Item.useTime = 40;
        Item.noUseGraphic = true;
        Item.autoReuse = false;
        Item.noMelee = true;
        Item.channel = true;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.DamageType = DamageClass.Melee;
        Item.UseSound = SoundID.Item1;
        Item.useStyle = ItemUseStyleID.Swing;
        Item.rare = ItemRarityID.LightRed;
        Item.shootSpeed = 1f;
        Item.shoot = ProjectileType<SerrationP>();
    }

    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemType<ToothToothbrush>()).AddIngredient(ItemType<CecitiorMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool? CanAutoReuseItem(Player player)
    {
        return false;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Projectile.NewProjectile(source, position, Vector2.UnitX * player.direction, type, damage, knockback, player.whoAmI, 0, -player.direction, 1);
        return false;
    }
}
public class SerrationP : HeldSword
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Melee/Serration";
    public override void SetExtraDefaults()
    {
        Projectile.Size = new Vector2(60);
        swingTime = 70;
        minSwingTime = 57;
        holdOffset = 30;
        Projectile.tileCollide = false;
        modifyCooldown = true;
        Projectile.localNPCHitCooldown = 10;
    }
    public override float Ease(float x)
    {
        return x == 0 ? 0 : x == 1 ? 1 : x < 0.5 ? MathF.Pow(2, 20 * x - 10) / 2 : (2 - MathF.Pow(2, -20 * x + 10)) / 2;
    }
    float lerpProgress = 1, swingProgress, rotation;
    public override void OnKill(int timeLeft)
    {
        Player player = Main.player[Projectile.owner];
        player.ChangeDir(Main.MouseWorld.X < player.Center.X ? -1 : 1);
        if (player.active && player.channel && !player.dead && !player.CCed && !player.noItems && Projectile.owner == Main.myPlayer)
            Projectile.NewProjectile(null, Projectile.Center, Vector2.UnitX * player.direction, Projectile.type, Projectile.damage, Projectile.knockBack, player.whoAmI, 0, -player.direction, 1);
    }
    public override void ExtraAI()
    {
        Player player = Main.player[Projectile.owner];
        if (lerpProgress != 1 && lerpProgress != -1)
            lerpProgress = SmoothStep(lerpProgress, 1, 0.1f);
        if (swingProgress > 0.25f && swingProgress < 0.85f)
            if (Projectile.ai[0] == 0 && Helper.Raycast(Projectile.Center, Vector2.UnitY, 100).RayLength < 15)
            {
                Projectile.ai[0] = 1;
                Helper.AddCameraModifier(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 5, 6, 30, 1000));
                Projectile.timeLeft = 15;
                SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);

                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Helper.Raycast(Projectile.Center - Vector2.UnitY * 30, Vector2.UnitY, 500, true).Point, new Vector2((float)player.direction, 0), ProjectileType<SerrationSpike>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                lerpProgress = -1;
            }
        int direction = (int)Projectile.ai[1];
        if (lerpProgress >= 0)
            swingProgress = Lerp(swingProgress, Ease(Utils.GetLerpValue(0f, swingTime, Projectile.timeLeft)), lerpProgress);
        float defRot = Projectile.velocity.ToRotation();
        float start = defRot - (PiOver2 + PiOver4);
        float end = defRot + (PiOver2 + PiOver4);
        if (lerpProgress >= 0)
            rotation = Lerp(rotation, direction == 1 ? start + Pi * 3 / 2 * swingProgress : end - Pi * 3 / 2 * swingProgress, lerpProgress);
        Vector2 position = player.GetFrontHandPosition(stretch, rotation - PiOver2) +
            rotation.ToRotationVector2() * holdOffset * ScaleFunction(swingProgress);
        Projectile.Center = position;
        Projectile.rotation = (position - player.Center).ToRotation() + PiOver4;
        player.ChangeDir(Projectile.velocity.X < 0 ? -1 : 1);
        player.heldProj = Projectile.whoAmI;
        if (player.gravDir != -1)
            player.SetCompositeArmFront(true, stretch, rotation - PiOver2);
    }
    public override bool? CanDamage()
    {
        return Projectile.ai[0] < 1 && swingProgress > 0.35f && swingProgress < 0.65f;
    }
    bool _hit = false;
    public override void OnHit(NPC target, NPC.HitInfo hit, int damageDone)
    {
        Helper.AddCameraModifier(new PunchCameraModifier(Projectile.Center, Main.rand.NextVector2Unit(), 2, 6, 30, 1000));
        if (!_hit)
        {
            lerpProgress = -.25f;
            _hit = true;
        }
    }
}
