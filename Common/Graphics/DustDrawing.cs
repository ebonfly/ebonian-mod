using System.Linq;
using EbonianMod.Content.Dusts;

namespace EbonianMod.Common.Graphics;

public class DustDrawing : ModSystem
{
	public override void Load()
	{
		On_Main.DrawPlayers_AfterProjectiles += (orig, self) =>
		{
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, RenderingUtils.Subtractive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			ReiSmoke.DrawAll(Main.spriteBatch);
			Main.spriteBatch.End();
			
			orig(self);
		};
	}
}