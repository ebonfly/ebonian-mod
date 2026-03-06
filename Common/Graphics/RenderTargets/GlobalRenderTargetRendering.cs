using Terraria.Graphics.Effects;

namespace EbonianMod.Common.Graphics.RenderTargets;

public class GlobalRenderTargetRendering : ModSystem
{
	public delegate void Render();
	public static event Render PopulateTargets;
	public static event Render DrawTargets;
	
	public override void Load()
	{
		On_Main.DrawPlayers_AfterProjectiles += (orig, self) => 
		{
			orig(self);
			
			if (Main.gameMenu) return;
			
			RenderingUtils.PreserveMainTarget(false);
			PopulateTargets?.Invoke();
			
			RenderingUtils.PreserveMainTarget(true);
			DrawTargets?.Invoke();
		};
	}
}