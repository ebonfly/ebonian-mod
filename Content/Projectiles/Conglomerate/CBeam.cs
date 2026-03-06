using EbonianMod.Content.Dusts;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using EbonianMod.Core.Systems.Cinematic;
using System;
using System.Collections.Generic;
using EbonianMod.Common.Graphics;

namespace EbonianMod.Content.Projectiles.Conglomerate;

public class CBeam : ModProjectile
{
    public override void SetDefaults()
    {
        Projectile.width = 1;
        Projectile.height = 1;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 135;
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 10;
    }

    public override string Texture => Helper.Empty;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 7000;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
    }
    int damage;
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float a = 0f;
        if (Type == ProjectileType<CBeamSmall>() && Projectile.timeLeft < 80)
            return false;
        return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.velocity * Projectile.localAI[0], 60 * Projectile.ai[1], ref a) && Projectile.scale > 0.5f;
    }
    public bool RunOnce = true;
    public float flareAlpha = 1;
    public override bool PreAI()
    {
        NPC owner = Main.npc[(int)Projectile.ai[2]];

        if (owner.active)// && owner.type == ModContent.NPCType<NPCs.Conglomerate.Conglomerate>())
        {
            Projectile.velocity = (owner.rotation + PiOver2).ToRotationVector2();
            Projectile.Center = owner.Center;
        }
        return base.PreAI();
    }
    public Vector2 startVel, offsetShake;
    public override void AI()
    {
        if (RunOnce)
        {
            Projectile.velocity.Normalize();
            startVel = Projectile.velocity;
            Projectile.rotation = Projectile.velocity.ToRotation();
            RunOnce = false;
        }
        if (Projectile.timeLeft == 131)
        {
            Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<BlurScream>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
            CameraSystem.ScreenShakeAmount = 15;
        }
        if (Projectile.timeLeft % 2 == 0 && Projectile.timeLeft > 5)
        {
            if (CameraSystem.ScreenShakeAmount < 13)
                CameraSystem.ScreenShakeAmount = 13;
            if (Projectile.timeLeft % 8 == 0)
            {
                offsetShake = Main.rand.NextVector2Circular(50, 50);
                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Vector2.Zero, ModContent.ProjectileType<BlurScream>(), 0, 0, -1, 0, 0, Projectile.whoAmI);

                Projectile.NewProjectile(null, Projectile.Center + Projectile.velocity * 30, Projectile.velocity, ModContent.ProjectileType<ConglomerateScream>(), 0, 0, -1, 0, 0, Projectile.whoAmI);
            }
        }
        NPC owner = Main.npc[(int)Projectile.ai[2]];

        if (owner.active)// && owner.type == ModContent.NPCType<NPCs.Conglomerate.Conglomerate>())
        {
            Projectile.velocity = (owner.rotation + PiOver2 + Projectile.ai[0]).ToRotationVector2();
            Projectile.Center = owner.Center;
        }
        if (Projectile.timeLeft < 90)
            Projectile.ai[1] = Lerp(Projectile.ai[1], 0, 0.05f);
        for (int i = 0; i < 5; i++)
        {
            float fac = Main.rand.NextFloat(0.2f, 1);

            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(100, 1000), DustType<SparkleDust>(), Projectile.velocity.RotatedByRandom(PiOver4 / fac) * Main.rand.NextFloat(25, 50), newColor: Color.Lerp(Color.Maroon, Color.LawnGreen, Main.rand.NextFloat()) * 5 * fac, Scale: Main.rand.NextFloat(0.1f, 0.2f) * fac);
        }
        flareAlpha = Lerp(flareAlpha, 0, 0.05f);
        Projectile.localAI[0] = SmoothStep(Projectile.localAI[0], 1348, 0.35f);
        startSize = Lerp(startSize, 0, 0.01f);
        //Projectile.velocity = -Projectile.velocity.RotatedBy(ToRadians(Projectile.ai[1]));
        Projectile.rotation = Projectile.velocity.ToRotation();
        float progress = Utils.GetLerpValue(0, 135, Projectile.timeLeft);
        if (Projectile.timeLeft < 135)
            Projectile.scale = Clamp((float)Math.Sin(progress * Math.PI) * 2 * (Projectile.scale + 0.5f), 0, 1);
    }
    public float visual1, visual2, visual3, startSize = 2f, coloredFlareAlpha = 1f, flareScaleMult = 1f, shakeFac = 10,
        additionalAlphaOffset = 1f;
    int seed = 0;
    public override bool PreDraw(ref Color lightColor)
    {
        if (seed == 0)
            seed = Main.rand.Next(129471124);
        visual1 -= 0.035f * 1.4f;
        visual2 -= 0.02425f * 1.4f;
        visual3 -= 0.0446f * 1.4f;

        Texture2D texture = Assets.Extras.FlamesSeamless.Value;
        Texture2D texture2 = Assets.Extras.LintyTrail.Value;
        Texture2D texture3 = Assets.Extras.FlamesSeamless.Value;
        Texture2D tex = Assets.Extras.explosion.Value;

        float progress = Utils.GetLerpValue(0, 135, Projectile.timeLeft);
        float i_progress = Clamp(SmoothStep(1, 0.2f, progress) * 50, 0, 1 / Clamp(startSize, 1, 2)) * (1 + Projectile.ai[1]);

        float alpha = (0.35f + MathF.Sin(Main.GlobalTimeWrappedHourly * 6 + Projectile.whoAmI) * 0.1f) * Projectile.scale;

        Texture2D tex2 = Assets.Extras.crosslight.Value;
        if (Projectile.timeLeft < 134)
        {
            DrawVertices(Projectile.Center, Projectile.velocity.ToRotation(), texture, texture2, i_progress, 5f * additionalAlphaOffset, (Type == ProjectileType<CBeamSmall>() ? 0.05f : 0.0025f), 1, visual1 + new UnifiedRandom(seed).NextFloat());
            DrawVertices(Projectile.Center, Projectile.velocity.ToRotation(), texture3, texture, i_progress, 5f * additionalAlphaOffset, (Type == ProjectileType<CBeamSmall>() ? 0.05f : 0.0025f), 1, visual2 + new UnifiedRandom(seed).NextFloat());
        }
        return false;
    }
    void DrawVertices(Vector2 pos, float rotation, Texture2D texture, Texture2D texture2, float i_progress, float alphaOffset, float quality = 0.002f, float max = 1, float visOff = 0)
    {
        /*offsetShake = Vector2.Lerp(offsetShake, Vector2.Zero, 0.2f);
        SpritebatchParameters sbParams = Main.spriteBatch.Snapshot();
        List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();
        List<VertexPositionColorTexture> vertices2 = new List<VertexPositionColorTexture>();
        List<VertexPositionColorTexture> vertices3 = new List<VertexPositionColorTexture>();
        Vector2 start = pos + offsetShake + (shakeFac == 0 ? Vector2.Zero : Main.rand.NextVector2Circular(shakeFac, shakeFac)) + Projectile.velocity * -6 - Main.screenPosition;
        Vector2 off = (rotation.ToRotationVector2() * 1348);
        Vector2 end = start + off - Main.screenPosition;
        int maxOldPos = 0;
        for (int i = Projectile.oldPos.Length - 1; i > 0; i--)
        {
            if (Projectile.oldPos[i] == Vector2.Zero)
                continue;
            maxOldPos = i;
            break;
        }

        float s = 0.5f;
        for (float i = 1 - max; i < max; i += quality)
        {
            if (i < max / 2)
                s = Clamp(i * 5f, 0, 0.5f);
            else
                s = Clamp((-i + max) * 2, 0, 0.5f);

            int j = (int)MathF.Floor(InOutCirc.Invoke(i) * maxOldPos);
            Vector2 position = Vector2.Lerp(start, start + Utils.AngleLerp(Projectile.rotation, Projectile.oldRot[maxOldPos], i).ToRotationVector2() * 1348, i);
            float rot = Helper.FromAToB(start, start + Projectile.oldRot[maxOldPos].ToRotationVector2() * 1348).ToRotation();

            float _off = MathF.Abs(visOff + i);

            float sinFac = InOutCirc.Invoke((MathF.Sin(Main.GlobalTimeWrappedHourly * 16) + 1) * 0.5f);
            float a = SmoothStep(3 * s, 0.5f, i);
            float a2 = Clamp(Lerp(Projectile.scale * 2, Projectile.scale, i), 0, 1) * Lerp(1.5f, s * 0.7f, i);
            Color col = Color.Lerp(Color.White * s, Color.Red * (s * s * 4f * alphaOffset) * a, Clamp(i * 9, 0, 1)) * a2;
            float endSize = SmoothStep(420 + sinFac * 50, 600, InOutQuint.Invoke(i));

            vertices.Add(Helper.AsVertex(position + new Vector2(SmoothStep(Lerp(60 + sinFac * 10, 100 + sinFac * 50, i) * startSize, endSize, i * 3) * Clamp(startSize, 1, 2), 0).RotatedBy(rot + PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

            sinFac = InOutCirc.Invoke((MathF.Sin(Main.GlobalTimeWrappedHourly * 25) + 1) * 0.5f);
            col = Color.Lerp(Color.White * s * 2f, Color.Green * (s * s * 4f * alphaOffset) * a, Clamp(i * 9, 0, 1)) * a2;
            vertices.Add(Helper.AsVertex(position + new Vector2(SmoothStep(Lerp(60 + sinFac * 10, 100 + sinFac * 50, i) * startSize, endSize, i * 3) * Clamp(startSize, 1, 2), 0).RotatedBy(rot - PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));

            _off = MathF.Abs((visOff == visual1 ? visual2 : visual1) + i);

            sinFac = InOutCirc.Invoke((MathF.Sin(Main.GlobalTimeWrappedHourly * 10) + 1) * 0.5f);
            col = Color.White * (s * s * alphaOffset * 0.5f * SmoothStep(1, 0, i)) * a2;

            vertices2.Add(Helper.AsVertex(position + new Vector2(Lerp(Lerp(10 + sinFac * 4, 200 + sinFac * 90, i) * startSize, endSize, i * 2) * Clamp(startSize, 1, 2), 0).RotatedBy(rot + PiOver2) * i_progress, new Vector2(_off, 1), col * Projectile.scale));

            sinFac = InOutCirc.Invoke((MathF.Sin(Main.GlobalTimeWrappedHourly * 9) + 1) * 0.5f);
            vertices2.Add(Helper.AsVertex(position + new Vector2(Lerp(Lerp(10 + sinFac * 4, 200 + sinFac * 90, i) * startSize, endSize, i * 2) * Clamp(startSize, 1, 2), 0).RotatedBy(rot - PiOver2) * i_progress, new Vector2(_off, 0), col * Projectile.scale));

            _off = MathF.Abs(visual3 + i);
            col = Color.White;
            endSize = SmoothStep(500 + sinFac * 50, 690, InOutQuint.Invoke(i));
            //if (Type == ProjectileType<CBeamSmall>())
            //  endSize = Lerp(SmoothStep(420 + sinFac * 50, 600, InOutQuint.Invoke(i)), 20, InOutSine.Invoke(i)) * Projectile.scale;

            vertices3.Add(Helper.AsVertex(position + new Vector2(Lerp(Lerp(20, 280 + sinFac * 90, i) * startSize, endSize, i * 2) * Clamp(startSize, 1, 2), 0).RotatedBy(rot - PiOver2) * i_progress, new Vector2(_off, 0), col));
            vertices3.Add(Helper.AsVertex(position + new Vector2(Lerp(Lerp(20, 280 + sinFac * 90, i) * startSize, endSize, i * 2) * Clamp(startSize, 1, 2), 0).RotatedBy(rot + PiOver2) * i_progress, new Vector2(_off, 1), col));
        }

        Main.spriteBatch.End();
        Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
        if (vertices.Count >= 3 && vertices2.Count >= 3 && (Type != ProjectileType<CBeamSmall>() ? vertices3.Count >= 3 : true))
        {
            InvisibleMaskRendering.AffectedDrawCache.Add(() =>
            {
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.swirlyNoise.Value, false);
                Helper.DrawTexturedPrimitives(vertices2.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.vein.Value, false);
                for (int i = 0; i < 2; i++)
                {
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
                    Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture2, false);
                }
            });
            InvisibleMaskRendering.DrawCache.Add(() =>
            {
                Helper.DrawTexturedPrimitives(vertices3.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.laserMask.Value, false);
                Helper.DrawTexturedPrimitives(vertices3.ToArray(), PrimitiveType.TriangleStrip, Assets.Extras.swirlyNoise.Value, false);
            }); for (int i = 0; i < 2; i++)
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, texture, false);
        }
        Main.spriteBatch.ApplySaved(sbParams);*/
    }
}
public class CBeamSmall : ModProjectile
{
    public override string Texture => Helper.Empty;
}
public class CFlareExplosion : ModProjectile
{
    public override string Texture => Helper.Placeholder;
    public override void SetDefaults()
    {
        Projectile.height = 1;
        Projectile.width = 1;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 200;
        Projectile.tileCollide = false;
    }
    public override bool ShouldUpdatePosition() => false;
    int seed;
    public override void OnSpawn(IEntitySource source)
    {
        seed = Main.rand.Next(int.MaxValue);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        if (seed == 0) return false;
        Texture2D tex = Assets.Extras.cone5.Value;
        UnifiedRandom rand = new UnifiedRandom(seed);
        float max = 40;
        Main.spriteBatch.Reload(BlendState.Additive);
        float ringScale = Lerp(1, 0, Clamp(vfxIncrement * 6.5f, 0, 1));
        if (ringScale > 0.01f)
        {
            for (float i = 0; i < max; i++)
            {
                float angle = Helper.CircleDividedEqually(i, max);
                float scale = rand.NextFloat(0.2f, 1.15f);
                Vector2 offset = new Vector2(Main.rand.NextFloat(50, 100) * ringScale * scale, 0).RotatedBy(angle);
                for (float j = 0; j < 2; j++)
                    Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, rand.NextFloat()) * ringScale, angle, tex.Size() / 2, new Vector2(Clamp(vfxIncrement * 6.5f, 0, 1), ringScale) * scale * 0.4f, SpriteEffects.None, 0);
            }
        }

        float alpha = Lerp(0.5f, 0, Projectile.ai[1]) * 2;
        for (float i = 0; i < max; i++)
        {
            float angle = Helper.CircleDividedEqually(i, max);
            float scale = rand.NextFloat(0.2f, 1.15f);
            Vector2 offset = new Vector2(Main.rand.NextFloat(50) * Projectile.ai[1] * scale, 0).RotatedBy(angle);
            for (float j = 0; j < 2; j++)
                Main.spriteBatch.Draw(tex, Projectile.Center + offset - Main.screenPosition, null, Color.Lerp(Color.Red, Color.LimeGreen, rand.NextFloat()) * alpha * 2, angle, tex.Size() / 2, new Vector2(Projectile.ai[1], alpha) * scale * 1.5f, SpriteEffects.None, 0);
        }
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return false;
    }
    float vfxIncrement;
    public override void AI()
    {
        vfxIncrement = Lerp(vfxIncrement, 0.2f, 0.18f);
        Projectile.ai[0] = Lerp(Projectile.ai[0], Projectile.ai[0] + vfxIncrement + (Projectile.ai[0] * 0.075f), 0.5f);
        Projectile.ai[1] = SmoothStep(0, 1.5f, Projectile.ai[0]);

        Projectile owner = Main.projectile[(int)Projectile.ai[2]];
        if (owner.active && owner.type == ModContent.ProjectileType<CBeam>())
        {
            Projectile.Center = owner.Center;
        }

        if (Projectile.timeLeft >= 190 && Projectile.timeLeft < 194)
        {
            UnifiedRandom rand = new UnifiedRandom(seed);
            float max = 10 + ((Projectile.timeLeft - 190) * 10);
            for (int i = ((Projectile.timeLeft - 190) * 10); i < max; i++)
            {
                float angle = Helper.CircleDividedEqually(i, max);
                float scale = rand.NextFloat(1f - (Projectile.timeLeft - 190) * 0.2f);
                Vector2 offset = new Vector2(Main.rand.NextFloat(50) * scale, 0).RotatedBy(angle);
                int jMax = rand.Next(3, 5);
                //for (int j = 0; j < jMax; j++)
                //  Dust.NewDustPerfect(Projectile.Center + offset * 0.5f, Main.rand.NextBool() ? DustID.IchorTorch : DustID.CursedTorch, Helper.FromAToB(Projectile.Center, Projectile.Center + offset).RotatedByRandom(PiOver4 * (j == 0 ? 0 : 1)) * (scale * 20)).noGravity = true;
            }
        }

        if (Projectile.ai[1] > 1.15f)
            Projectile.Kill();
    }
}
