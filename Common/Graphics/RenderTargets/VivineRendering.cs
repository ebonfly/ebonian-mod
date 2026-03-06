using EbonianMod.Content.Dusts;

namespace EbonianMod.Common.Graphics.RenderTargets;

public class VivineRendering : BaseCachedActionRenderTarget<VivineRendering>
{
	public override void PopulateTarget()
	{
		Main.graphics.GraphicsDevice.SetRenderTarget(Targets[0]);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);

		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		JunglePinkDust.DrawAll(Main.spriteBatch);
		Main.spriteBatch.End();
	}

	public override void DrawTarget()
	{
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		Main.graphics.GraphicsDevice.Textures[1] = Assets.Extras.jungleDustColor.Value;
		Effects.metaballGradient.Value.CurrentTechnique.Passes[0].Apply();
		Main.spriteBatch.Draw(Targets[0], Vector2.Zero, Color.White);
		Main.spriteBatch.End();
	}
}