using Terraria.Graphics.Effects;

namespace EbonianMod.Core.Systems.Cinematic;

public class OverlaySystem : ModSystem
{
    public static float FlashAlpha, DarkAlpha;

    public override void Load()
    {
        On_FilterManager.EndCapture += (orig, self, texture, target1, target2, color) =>
        {
            orig(self, texture, target1, target2, color);

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            if (FlashAlpha > 0)
                Main.spriteBatch.Draw(Assets.Extras.Line.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * OverlaySystem.FlashAlpha * 2);

            if (DarkAlpha > 0)
                Main.spriteBatch.Draw(Assets.Extras.Line.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * OverlaySystem.DarkAlpha);
            
            Main.spriteBatch.End();
        };
    }

    public override void PostUpdateEverything()
    {
        if (FlashAlpha > 0)
            FlashAlpha -= 0.01f;

        if (!Main.gameInactive)
            DarkAlpha = Lerp(DarkAlpha, 0, 0.1f);
        if (DarkAlpha < .05f)
            DarkAlpha = 0;
    }
}