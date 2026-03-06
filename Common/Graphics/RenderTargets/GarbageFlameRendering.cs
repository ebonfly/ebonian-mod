using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Graphics.Effects;

namespace EbonianMod.Common.Graphics.RenderTargets;

public class GarbageFlameRendering : BaseCachedActionRenderTarget<GarbageFlameRendering>
{
	public override int TargetAmount => 2;

	public override void PopulateTarget()
	{
		Main.graphics.GraphicsDevice.SetRenderTarget(Targets[0]);
		Main.graphics.GraphicsDevice.Clear(Color.Transparent);
		
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
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
	    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null);
        Main.graphics.GraphicsDevice.Textures[1] = Assets.Extras.coherentNoise.Value;
        Effects.displacementMap.Value.CurrentTechnique.Passes[0].Apply();
        Effects.displacementMap.Value.Parameters["uIntensity"].SetValue(3f);
        Effects.displacementMap.Value.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.75f);
        Effects.displacementMap.Value.Parameters["offsetY"].SetValue(Main.GlobalTimeWrappedHourly * 0.25f);
        Effects.displacementMap.Value.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
        Effects.displacementMap.Value.Parameters["offset"].SetValue(0.0075f);
        Effects.displacementMap.Value.Parameters["alpha"].SetValue(0.1f);
        Main.spriteBatch.Draw(Targets[1], new Rectangle(0, 0, (int)(Main.screenWidth * (1 + Main.GameZoomTarget)), (int)(Main.screenHeight * (1 + Main.GameZoomTarget))), Color.White);
        Main.graphics.GraphicsDevice.Textures[1] = Assets.Extras.swirlyNoise.Value;
        Effects.displacementMap.Value.Parameters["offsetY"].SetValue(Main.GlobalTimeWrappedHourly * 0.34f);
        Main.spriteBatch.Draw(Targets[1], new Rectangle(0, 0, (int)(Main.screenWidth * (1 + Main.GameZoomTarget)), (int)(Main.screenHeight * (1 + Main.GameZoomTarget))), Color.White);

        Main.graphics.GraphicsDevice.Textures[1] = Assets.Extras.coherentNoise.Value;
        Effects.displacementMap.Value.Parameters["offsetY"].SetValue(0);
        Effects.displacementMap.Value.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
        Effects.displacementMap.Value.Parameters["offset"].SetValue(0.0025f);
        Effects.displacementMap.Value.Parameters["alpha"].SetValue(0.1f);
        Main.spriteBatch.Draw(Targets[1], new Rectangle(0, 0, (int)(Main.screenWidth * (1 + Main.GameZoomTarget)), (int)(Main.screenHeight * (1 + Main.GameZoomTarget))), Color.White);
        Main.graphics.GraphicsDevice.Textures[1] = Assets.Extras.swirlyNoise.Value;
        Effects.displacementMap.Value.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.74f);
        Main.spriteBatch.Draw(Targets[1], new Rectangle(0, 0, (int)(Main.screenWidth * (1 + Main.GameZoomTarget)), (int)(Main.screenHeight * (1 + Main.GameZoomTarget))), Color.White);
        Main.spriteBatch.End();
    }
}