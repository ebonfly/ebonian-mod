using EbonianMod.Common.Graphics;
using EbonianMod.Core.Systems.Verlets;
using EbonianMod.Content.Dusts;
using EbonianMod.Core.Systems.Cinematic;
using System;
using System.Linq;
using Terraria.Graphics.Effects;

namespace EbonianMod.Common.Detours;

public class DrawDetours : ModSystem
{
    public override void Load()
    {
        On_FilterManager.EndCapture += FilterManager_EndCapture;
        On_Main.DrawNPC += DrawNPC;
        On_Main.DrawPlayers_AfterProjectiles += PreDraw;
    }
    public static void DrawAdditiveDusts(SpriteBatch sb, GraphicsDevice gd)
    {
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (Main.dust.Any())
        {
            foreach (Dust d in Main.dust)
            {
                FireDust.DrawAll(sb, d);
                GenericAdditiveDust.DrawAll(sb, d);
                SparkleDust.DrawAll(sb, d);
                IntenseDustFollowPoint.DrawAll(sb, d);
                LineDustFollowPoint.DrawAll(sb, d);
                SmokeDustAkaFireDustButNoGlow2.DrawAll(sb, d);
                BlurDust.DrawAll(sb, d);
            }
        }
        sb.End();
    }
    public static void DrawGenericPostScreen(SpriteBatch sb, GraphicsDevice gd)
    {

        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (OverlaySystem.FlashAlpha > 0)
            Main.spriteBatch.Draw(Assets.Extras.Line.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * OverlaySystem.FlashAlpha * 2);
        sb.End();
    }
    void DrawNPC(Terraria.On_Main.orig_DrawNPC orig, global::Terraria.Main self, int iNPCIndex, bool behindTiles)
    {
        SpriteBatch sb = Main.spriteBatch;
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.Additive, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        SmokeDustAkaFireDustButNoGlow.DrawAll(Main.spriteBatch);
        sb.End();
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        orig(self, iNPCIndex, behindTiles);
    }

    public static void DrawVerlets(SpriteBatch sb)
    {
        for (int i = 0; i < S_VerletSystem.verlets.Count; i++)
        {
            if (S_VerletSystem.verlets[i].timeLeft > 0 && S_VerletSystem.verlets[i].verlet is not null)
            {
                float alpha = Clamp(Lerp(0, 2, (float)S_VerletSystem.verlets[i].timeLeft / S_VerletSystem.verlets[i].maxTime), 0, 1);
                VerletDrawData verletDrawData = S_VerletSystem.verlets[i].drawData;
                verletDrawData.color = Lighting.GetColor(S_VerletSystem.verlets[i].verlet.lastP.position.ToTileCoordinates()) * alpha;
                S_VerletSystem.verlets[i].verlet.Draw(sb, verletDrawData);
            }
        }
    }
    void FilterManager_EndCapture(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
    {
        orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

        if (OverlaySystem.FlashAlpha > 0)
        {
            Main.spriteBatch.Draw(Assets.Extras.Line.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.White * OverlaySystem.FlashAlpha * 2);
        }

        if (OverlaySystem.DarkAlpha > 0)
        {
            Main.spriteBatch.Draw(Assets.Extras.Line.Value, new Rectangle(0, 0, Main.screenWidth, Main.screenHeight), Color.Black * OverlaySystem.DarkAlpha);
        }
        Main.spriteBatch.End();

        if (!Main.gameMenu)
        {
            RTHandler.jungleDustTarget.RequestAndPrepare(Main.dust.Any());
            if (RTHandler.jungleDustTarget.IsReady)
            {
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                Main.graphics.GraphicsDevice.Textures[1] = Assets.Extras.jungleDustColor.Value;
                Effects.metaballGradient.Value.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.Draw(RTHandler.jungleDustTarget.GetTarget(), Vector2.Zero, Color.White);
                Main.spriteBatch.End();
            }

            DrawGarbageFlames();
            DrawInvisMasks(Main.spriteBatch, Main.graphics.GraphicsDevice);
            DrawXareusGoop(Main.spriteBatch, Main.graphics.GraphicsDevice);
            DrawPixelatedContent(Main.spriteBatch);
        }

        Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        foreach (Projectile projectile in Main.ActiveProjectiles)
        {
            if (projectile.active && (EbonianMod.projectileFinalDrawList.Contains(projectile.type)))
            {
                Color color = Color.White;
                projectile.ModProjectile.PreDraw(ref color);
            }
        }
        EbonianMod.finalDrawCache.InvokeAllAndClear();
        Main.spriteBatch.End();
    }

    void PreDraw(On_Main.orig_DrawPlayers_AfterProjectiles orig, Main self)
    {
        GraphicsDevice gd = Main.instance.GraphicsDevice;
        SpriteBatch sb = Main.spriteBatch;
        if (S_VerletSystem.verlets.Any())
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            DrawVerlets(sb);
            sb.End();
        }

        if (Main.dust.Any())
            foreach (Dust d in Main.dust)
            {
                XGoopDust.DrawAll(sb, d);
                ColoredFireDust.DrawAll(sb, d);
            }

        sb.Begin(SpriteSortMode.Deferred, RenderingUtils.Subtractive, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        ReiSmoke.DrawAll(sb);
        sb.End();

        orig(self);

        if (!Main.gameMenu && gd.GetRenderTargets().Contains(Main.screenTarget))
        {

            if (EbonianMod.blurDrawCache.Any())
                DrawBlurredContent(sb, gd);
        }

        DrawAdditiveDusts(sb, gd);

        DrawGenericPostScreen(sb, gd);
    }
    void DrawPixelatedContent(SpriteBatch sb)
    {
        bool anyProj = false;
        foreach (Projectile p in Main.ActiveProjectiles)
            if (PixelationTarget.pixelatedProjectiles.Contains(p.type))
                anyProj = true;
        RTHandler.pixelationTarget.RequestAndPrepare(EbonianMod.pixelationDrawCache.Any() || EbonianMod.primitivePixelationDrawCache.Any() || anyProj);
        if (RTHandler.pixelationTarget.IsReady)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise);
            sb.Draw(RTHandler.pixelationTarget.GetTarget(), new Rectangle(0, 0, (int)(Main.screenWidth * (1 + Main.GameZoomTarget)), (int)(Main.screenHeight * (1 + Main.GameZoomTarget))), Color.White);
            sb.End();
        }
    }
    public static void DrawInvisMasks(SpriteBatch sb, GraphicsDevice gd)
    {
        RTHandler.invisTarget.RequestAndPrepare(EbonianMod.affectedByInvisibleMaskCache.Any() || EbonianMod.invisibleMaskCache.Any());
        if (RTHandler.invisTarget.IsReady)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            Effects.invisibleMask.Value.CurrentTechnique.Passes[0].Apply();
            gd.Textures[1] = RTHandler.invisTarget.GetTarget();
            sb.Draw(RTHandler.invisTarget._target2, Helper.ScreenRect, Color.White);
            sb.End();
            gd.Textures[1] = null;
        }
    }
    public static void DrawGarbageFlames()
    {
        RTHandler.garbageTarget.RequestAndPrepare(EbonianMod.garbageFlameCache.Any());
        if (RTHandler.garbageTarget.IsReady)
        {
            EbonianMod.primitivePixelationDrawCache.Add(() =>
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null);

                for (int i = 0; i < 3; i++)
                    DrawGarbageFlame(RTHandler.garbageTarget, Main.spriteBatch, Main.graphics.GraphicsDevice);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
            });
        }
    }
    public static void DrawGarbageFlame(GarbageTarget target, SpriteBatch sb, GraphicsDevice gd)
    {
        gd.Textures[1] = Assets.Extras.coherentNoise.Value;
        Effects.displacementMap.Value.CurrentTechnique.Passes[0].Apply();
        Effects.displacementMap.Value.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.75f);
        Effects.displacementMap.Value.Parameters["offsetY"].SetValue(Main.GlobalTimeWrappedHourly * 0.25f);
        Effects.displacementMap.Value.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
        Effects.displacementMap.Value.Parameters["offset"].SetValue(0.0075f);
        Effects.displacementMap.Value.Parameters["alpha"].SetValue(0.1f);
        sb.Draw(target.GetTarget(), Vector2.Zero, Color.White * 0.25f);
        gd.Textures[1] = Assets.Extras.swirlyNoise.Value;
        Effects.displacementMap.Value.Parameters["offsetY"].SetValue(Main.GlobalTimeWrappedHourly * 0.34f);
        sb.Draw(target.GetTarget(), Vector2.Zero, Color.White * 0.25f);

        gd.Textures[1] = Assets.Extras.coherentNoise.Value;
        Effects.displacementMap.Value.Parameters["offsetY"].SetValue(0);
        Effects.displacementMap.Value.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.5f);
        Effects.displacementMap.Value.Parameters["offset"].SetValue(0.0025f);
        Effects.displacementMap.Value.Parameters["alpha"].SetValue(0.1f);
        sb.Draw(target.GetTarget(), Vector2.Zero, Color.White * 0.25f);
        gd.Textures[1] = Assets.Extras.swirlyNoise.Value;
        Effects.displacementMap.Value.Parameters["offsetX"].SetValue(Main.GlobalTimeWrappedHourly * 0.74f);
        sb.Draw(target.GetTarget(), Vector2.Zero, Color.White * 0.25f);
    }
    public static void DrawXareusGoop(SpriteBatch sb, GraphicsDevice gd)
    {
        RTHandler.xareusTarget.RequestAndPrepare(EbonianMod.xareusGoopCache.Any());
        if (RTHandler.xareusTarget.IsReady)
        {
            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            gd.Textures[1] = Assets.Extras.darkShadowflameGradient.Value;
            gd.Textures[2] = Assets.Extras.space_full.Value;
            gd.Textures[3] = Assets.Extras.seamlessNoiseHighContrast.Value;
            gd.Textures[4] = Assets.Extras.alphaGradient.Value;
            Effects.metaballGradientNoiseTex.Value.CurrentTechnique.Passes[0].Apply();
            Effects.metaballGradientNoiseTex.Value.Parameters["uTime"].SetValue(Main.GlobalTimeWrappedHourly * 0.2f);
            Effects.metaballGradientNoiseTex.Value.Parameters["offsetX"].SetValue(1f);
            Effects.metaballGradientNoiseTex.Value.Parameters["offsetY"].SetValue(1f);
            sb.Draw(RTHandler.xareusTarget.GetTarget(), Vector2.Zero, Color.White);
            sb.End();
        }
    }
    public static void DrawBlurredContent(SpriteBatch sb, GraphicsDevice gd)
    {
        gd.SetRenderTarget(EbonianMod.Instance.blurrender);
        gd.Clear(Color.Transparent);
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
        sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
        sb.End();
        gd.SetRenderTarget(Main.screenTargetSwap);
        gd.Clear(Color.Transparent);
        sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        EbonianMod.blurDrawCache.InvokeAllAndClear();
        sb.End();
        gd.SetRenderTarget(Main.screenTarget);
        gd.Clear(Color.Transparent);
        sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        Effects.Test2.Value.CurrentTechnique.Passes[0].Apply();
        Effects.Test2.Value.Parameters["tex0"].SetValue(Main.screenTargetSwap);
        Effects.Test2.Value.Parameters["i"].SetValue(0.02f);
        sb.Draw(EbonianMod.Instance.blurrender, Vector2.Zero, Color.White);
        sb.End();
    }
}
