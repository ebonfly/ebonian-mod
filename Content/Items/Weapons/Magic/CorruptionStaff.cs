using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Friendly.Corruption;

namespace EbonianMod.Content.Items.Weapons.Magic;

public class CorruptionStaff : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/CorruptionStaff";
    public override void SetStaticDefaults()
    {
        Item.staff[Item.type] = true;
    }

    public override void SetDefaults()
    {
        Item.damage = 19;
        Item.width = 40;
        Item.height = 40;
        Item.mana = 5;
        Item.DamageType = DamageClass.Magic;
        Item.useTime = 50;
        Item.useAnimation = 50;
        Item.useStyle = 5;
        Item.knockBack = 10;
        Item.value = 1000;
        Item.rare = ItemRarityID.LightRed;
        Item.UseSound = SoundID.Item1;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.autoReuse = true;
        Item.shoot = ProjectileType<VilethornF1>();
        Item.shootSpeed = 28;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        for (int i = -1; i < 2; i++)
        {
            if (i == 0) continue;
            Projectile.NewProjectile(source, position + velocity, velocity.RotatedBy(i), type, damage, knockback, player.whoAmI, ai2: i * -0.1f);
        }
        return false;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.Vilethorn).AddIngredient(ItemType<CorruptEyeMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
}
