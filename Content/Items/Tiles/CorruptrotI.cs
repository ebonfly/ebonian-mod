using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Tiles;

namespace EbonianMod.Content.Items.Tiles;

public class CorruptrotI : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 16;
        Item.height = 16;
        Item.maxStack = Item.CommonMaxStack;
        Item.useTurn = true;
        Item.autoReuse = true;
        Item.useAnimation = 15;
        Item.rare = ItemRarityID.Green;
        Item.useTime = 10;
        Item.useStyle = 1;
        Item.consumable = true;
        Item.createTile = TileType<CorruptrotTile>();
    }
    public override void AddRecipes()
    {
        CreateRecipe(4).AddIngredient<CorruptEyeMaterial>().AddTile(TileID.HeavyWorkBench).Register();
    }
}
