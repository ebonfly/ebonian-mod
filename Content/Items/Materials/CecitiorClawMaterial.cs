namespace EbonianMod.Content.Items.Materials;

public class CecitiorClawMaterial : ModItem
{
	public override string Texture => Helper.AssetPath + "Items/Materials/" + Name;
	public override void SetDefaults()
	{
		Item.CloneDefaults(ItemID.SoulofNight);
	}
}