using EbonianMod.Common.Players;
using EbonianMod.Content.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Content.Items.Armor.Cecitoma;
[AutoloadEquip(EquipType.Head)]
public class Cecihead : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Armor/Cecitoma/Cecihead";

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.defense = 10;
        Item.rare = ItemRarityID.LightRed;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemType<CecitiorMaterial>(), 12).AddIngredient(ItemType<CecitiorClawMaterial>(), 1).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool IsArmorSet(Item head, Item body, Item legs)
    {
        return body.type == ItemType<Cecibody>() && legs.type == ItemType<Cecilegs>();
    }
    public const float DmgIncrease = 0.05f, SpeedIncrease = 0.01f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DmgIncrease * 100, SpeedIncrease * 100);
    public override void UpdateEquip(Player player)
    {
        player.meleeDamage += DmgIncrease;
        player.meleeSpeed += SpeedIncrease;
    }
    public override void UpdateArmorSet(Player player)
    {
        player.setBonus = this.GetLocalization("SetBonus").Value;
        player.GetModPlayer<SetBonusPlayer>().cecitior = true;
    }
}
[AutoloadEquip(EquipType.Body)]
public class Cecibody : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Armor/Cecitoma/Cecibody";
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.defense = 18;
        Item.rare = ItemRarityID.LightRed;
    }
    public const float DmgIncrease = 0.08f, SpeedIncrease = 0.025f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DmgIncrease * 100, SpeedIncrease * 100);
    public override void UpdateEquip(Player player)
    {
        player.meleeDamage += DmgIncrease;
        player.meleeSpeed += SpeedIncrease;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemType<CecitiorMaterial>(), 16).AddIngredient(ItemType<CecitiorClawMaterial>(), 2).AddTile(TileID.MythrilAnvil).Register();
    }
}
[AutoloadEquip(EquipType.Legs)]
public class Cecilegs : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Armor/Cecitoma/Cecilegs";
    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = 10000;
        Item.defense = 12;
        Item.rare = ItemRarityID.LightRed;
    }
    public const float DmgIncrease = 0.02f, CritIncrease = 0.09f;
    public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(DmgIncrease * 100, CritIncrease * 100);
    public override void UpdateEquip(Player player)
    {
        player.meleeDamage += DmgIncrease;
        player.meleeCrit += CritIncrease;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemType<CecitiorMaterial>(), 12).AddIngredient(ItemType<CecitiorClawMaterial>(), 1).AddTile(TileID.MythrilAnvil).Register();
    }
}
