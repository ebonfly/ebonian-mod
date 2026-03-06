namespace EbonianMod.Common.Graphics.RenderTargets;

public class XareusRendering : BaseCachedActionRenderTarget<XareusRendering>
{
	public override void PopulateTarget()
	{
		Main.graphics.GraphicsDevice.SetRenderTarget(Targets[0]);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);
		
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawCache.InvokeAllAndClear();
		Main.spriteBatch.End();
	}

	public override void DrawTarget()
	{
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		Main.graphics.GraphicsDevice.Textures[1] = Assets.Extras.darkShadowflameGradient.Value;
		Main.graphics.GraphicsDevice.Textures[2] = Assets.Extras.space_full.Value;
		Main.graphics.GraphicsDevice.Textures[3] = Assets.Extras.seamlessNoiseHighContrast.Value;
		Main.graphics.GraphicsDevice.Textures[4] = Assets.Extras.alphaGradient.Value;
		Effects.metaballGradientNoiseTex.Value.CurrentTechnique.Passes[0].Apply();
		Effects.metaballGradientNoiseTex.Value.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
		Effects.metaballGradientNoiseTex.Value.Parameters["offsetX"].SetValue(1f);
		Effects.metaballGradientNoiseTex.Value.Parameters["offsetY"].SetValue(1f);
		Main.spriteBatch.Draw(Targets[0], Vector2.Zero, Color.White);
		Main.spriteBatch.End();
	}
}