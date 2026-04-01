using EbonianMod.Content.NPCs.Aureus;
using EbonianMod.Content.NPCs.Void;
using EbonianMod.Content.Skies;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace EbonianMod.Core.Systems.Misc;

public class FilterSystem : ModSystem 
{
	public override void Load()
	{
		Filters.Scene["Asteroid"] = new Filter(new ScreenShaderData("FilterMiniTower").UseColor(0f, 0f, 0f).UseOpacity(0f), EffectPriority.VeryHigh);
		SkyManager.Instance["Asteroid"] = new RiverOfStarlightSky();

		Filters.Scene["EbonianMod:CorruptTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.68f, .56f, .73f).UseOpacity(0.35f), EffectPriority.Medium);
		SkyManager.Instance["EbonianMod:CorruptTint"] = new BasicTint();

		Filters.Scene["EbonianMod:CrimsonTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.75f, 0f, 0f).UseOpacity(0.35f), EffectPriority.Medium);
		SkyManager.Instance["EbonianMod:CrimsonTint"] = new BasicTint();

		Filters.Scene["EbonianMod:Conglomerate"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(0, 0, 0f).UseOpacity(0), EffectPriority.Medium);
		SkyManager.Instance["EbonianMod:Conglomerate"] = new ConglomerateSky();
		Filters.Scene["EbonianMod:Aureus"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.1f, .1f, .5f).UseOpacity(0.45f), EffectPriority.Medium);
		SkyManager.Instance["EbonianMod:Aureus"] = new AureusSky();
		
		Filters.Scene["EbonianMod:Void"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(.2f, .1f, .5f).UseOpacity(0.45f), EffectPriority.Medium);
		SkyManager.Instance["EbonianMod:Void"] = new VoidSky();

		Filters.Scene["EbonianMod:HellTint"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(2.55f, .97f, .31f).UseOpacity(0.1f), EffectPriority.Medium);
		SkyManager.Instance["EbonianMod:HellTint"] = new BasicTint();
		Filters.Scene["EbonianMod:HellTint2"] = new Filter(new BasicScreenTint("FilterMiniTower").UseColor(0.03f, 0f, .18f).UseOpacity(0.425f), EffectPriority.Medium);
		SkyManager.Instance["EbonianMod:HellTint2"] = new BasicTint();
		Filters.Scene["EbonianMod:ScreenFlash"] = new Filter(new ScreenShaderData(Request<Effect>("EbonianMod/Assets/Effects/ScreenFlash"), "Flash"), EffectPriority.VeryHigh);
	}
}