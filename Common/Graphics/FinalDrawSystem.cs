using EbonianMod.Common.Graphics;
using EbonianMod.Core.Systems.Verlets;
using EbonianMod.Content.Dusts;
using EbonianMod.Core.Systems.Cinematic;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.Graphics.Effects;

namespace EbonianMod.Common.Graphics;

public class FinalDrawSystem : ModSystem
{
    public static List<Action> DrawCache = [];
    public static List<int> ProjectileTypeList = new();
    
    public override void Load()
    {
        DrawCache ??= [];
        
        On_FilterManager.EndCapture += FilterManager_EndCapture;
    }
    void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
    {
        orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        foreach (Projectile projectile in Main.ActiveProjectiles)
        {
            if (projectile.active && (ProjectileTypeList.Contains(projectile.type)))
            {
                Color color = Color.White;
                projectile.ModProjectile.PreDraw(ref color);
            }
        }
        DrawCache.InvokeAllAndClear();
        Main.spriteBatch.End();
    }
}
