using System;
using System.Collections.Generic;

namespace EbonianMod.Common.Graphics.RenderTargets;

public class PixelationRendering : BaseCachedActionRenderTarget<PixelationRendering>
{
	public override int TargetAmount => 2;
	public override void PopulateTarget()
	{
		Main.graphics.GraphicsDevice.SetRenderTarget(Targets[0]);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);
		
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		DrawCache.InvokeAllAndClear();
		Main.spriteBatch.End();
		
		Main.graphics.GraphicsDevice.SetRenderTarget(Targets[1]);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);
		
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise, Effects.colorQuant.Value, Matrix.Identity);
		Effects.colorQuant.Value.Parameters["res"].SetValue(32);
		Main.spriteBatch.Draw(Targets[0], new Rectangle(0, 0, (int)(Main.screenWidth / (1 + Main.GameZoomTarget)), (int)(Main.screenHeight / (1 + Main.GameZoomTarget))), Color.White);
		Main.spriteBatch.End();
	}
	public override void DrawTarget()
	{
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
		Main.spriteBatch.Draw(Targets[1], new Rectangle(0, 0, (int)(Main.screenWidth * (1 + Main.GameZoomTarget)), (int)(Main.screenHeight * (1 + Main.GameZoomTarget))), Color.White);
		Main.spriteBatch.End();
	}
}