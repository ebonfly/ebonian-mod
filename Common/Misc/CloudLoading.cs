namespace EbonianMod.Common.Misc;

public class CloudLoading : ICustomAutoload
{
	public static void Autoload(Mod mod)
	{
		CloudLoader.AddCloudFromTexture(mod, "EbonianMod/Assets/Images/ExtraSprites/Clouds/NourCloud", spawnChance: 0.05f, rareCloud: true);
	}
}