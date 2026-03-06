/*using System;
using System.Collections.Generic;

namespace EbonianMod.Common.Graphics.RenderTargets;

public class InvisibleMaskRendering : BaseCachedActionRenderTarget<InvisibleMaskRendering>
{
	public override int TargetAmount => 2;
	public static List<Action> AffectedDrawCache => ModContent.GetInstance<InvisibleMaskRendering>()._affectedDrawCache;
	internal List<Action> _affectedDrawCache = [];
	public override void Load()
	{
		base.Load();

		_affectedDrawCache ??= [];
	}

	public override void PopulateTarget()
	{
		Main.graphics.GraphicsDevice.SetRenderTarget(Targets[0]);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);
		
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		AffectedDrawCache.InvokeAllAndClear();
		Main.spriteBatch.End();

		Main.graphics.GraphicsDevice.SetRenderTarget(Targets[1]);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);
		
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawCache.InvokeAllAndClear();
		Main.spriteBatch.End();
	}

	public override void DrawTarget()
	{
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
		Effects.invisibleMask.Value.CurrentTechnique.Passes[0].Apply();
		Main.graphics.GraphicsDevice.Textures[1] = Targets[1];
		Main.spriteBatch.Draw(Targets[0], Helper.ScreenRect, Color.White);
		Main.spriteBatch.End();
	}
}*/ // Unused