using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Friendly.Corruption;
using System;

namespace EbonianMod.Content.Items.Weapons.Summoner;

public class TerrorStaff : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Summoner/"+Name;
    public override void SetStaticDefaults()
    {
        Item.staff[Item.type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 40;
        Item.width = 40;
        Item.height = 40;
        Item.mana = 10;
        Item.reuseDelay = 20;
        Item.useTime = 10;
        Item.useAnimation = 30;
        Item.DamageType = DamageClass.Summon;
        Item.useStyle = 5;
        Item.knockBack = 10;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.rare = ItemRarityID.LightRed;
        Item.UseSound = SoundID.Item1;
        Item.noMelee = true;
        Item.autoReuse = true;
        Item.shoot = ProjectileType<TerrorStaffP>();
        Item.shootSpeed = 14;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient<TerrortomaMaterial>(4).AddIngredient(ItemType<CorruptEyeMaterial>(), 10).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        Vector2 mouse = Main.MouseWorld - player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true) - new Vector2(0, 1000);
        player.itemRotation = MathF.Atan2(mouse.Y * player.direction, mouse.X * player.direction);
        NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
        NetMessage.SendData(MessageID.ShotAnimationAndSound, number: player.whoAmI);

        Projectile.NewProjectile(source, position, new Vector2(velocity.X * 0.5f * Main.rand.NextFloat(1, 2), -Item.shootSpeed), type, damage, knockback, player.whoAmI);
        return false;
    }
}
