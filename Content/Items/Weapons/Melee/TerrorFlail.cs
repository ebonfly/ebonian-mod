using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Friendly.Corruption;
namespace EbonianMod.Content.Items.Weapons.Melee;

public class TerrorFlail : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Melee/TerrorFlail";
    public override void SetDefaults()
    {
        Item.width = 22;
        Item.height = 20;
        Item.value = Item.sellPrice(silver: 5);
        Item.rare = ItemRarityID.LightRed;
        Item.noMelee = true;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.useAnimation = 40;
        Item.useTime = 40;
        Item.knockBack = 4f;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.damage = 35;
        Item.noUseGraphic = true;
        Item.shoot = ProjectileType<TerrortomaFlail>();
        Item.shootSpeed = 15.1f;
        Item.UseSound = SoundID.Item1;
        Item.DamageType = DamageClass.Melee;
        Item.crit = 9;
        Item.channel = true;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.BallOHurt).AddIngredient(ItemType<CorruptEyeMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
}