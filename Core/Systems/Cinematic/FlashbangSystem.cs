using Terraria.Graphics.Effects;

namespace EbonianMod.Core.Systems.Cinematic;

public class FlashbangSystem : ModSystem
{
	public RenderTarget2D FlashTarget;
	public float FlashImageAlpha, FlashOverlayAlpha;
	public bool ShouldCaptureTarget;
	public override void Load()
	{
		Main.QueueMainThreadAction(() => FlashTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight));
		
		Main.OnResolutionChanged += (_) => Main.QueueMainThreadAction(() => FlashTarget = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight));
		
		On_Main.Draw += (orig, self, time) =>
		{
			orig(self, time);
			
			Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
			if (FlashImageAlpha > 0 && FlashTarget is not null)
				Main.spriteBatch.Draw(FlashTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * FlashImageAlpha);

			if (FlashOverlayAlpha > 0)
				Main.spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * FlashOverlayAlpha);
			Main.spriteBatch.End();
		};

		On_FilterManager.EndCapture += (orig, self, texture, target1, target2, color) =>
		{
			if (ShouldCaptureTarget)
			{
				var old = Main.graphics.GraphicsDevice.GetRenderTargets();
				Main.graphics.GraphicsDevice.SetRenderTarget(FlashTarget);
				Main.graphics.GraphicsDevice.Clear(Color.Black);
				Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
				Main.spriteBatch.Draw(Main.screenTarget, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White);
				Main.spriteBatch.End();
				Main.graphics.GraphicsDevice.SetRenderTargets(old);
				ShouldCaptureTarget = false;
			}

			orig(self, texture, target1, target2, color);
		};
	}

	public override void PostUpdateEverything()
	{
		FlashImageAlpha = MathHelper.Lerp(FlashImageAlpha, 0, 0.01f);
		FlashOverlayAlpha = MathHelper.Lerp(FlashOverlayAlpha, 0, 0.005f);

		if (FlashImageAlpha < 0.01f)
			FlashImageAlpha = 0;
		if (FlashOverlayAlpha < 0.01f)
			FlashOverlayAlpha = 0;
	}

	public static void InitFlashbang()
	{
		FlashbangSystem instance = ModContent.GetInstance<FlashbangSystem>();
		instance.ShouldCaptureTarget = true;
		instance.FlashImageAlpha = 1;
		instance.FlashOverlayAlpha = 1;
	}
}