using EbonianMod.Content.Dusts;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageDrone : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Garbage/" + Name;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override bool? CanDamage()
    {
        return false;
    }
    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 20;
        Projectile.aiStyle = -1;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.hostile = true;
        Projectile.timeLeft = 400;
        Projectile.Opacity = 0;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        lightColor = Color.White * Projectile.Opacity;
        Texture2D tex = Assets.Projectiles.Garbage.GarbageDrone_Bloom.Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        var fadeMult = 1f / Projectile.oldPos.Count();
        for (int i = 0; i < Projectile.oldPos.Count(); i++)
        {
            float mult = (1 - i * fadeMult);
            Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Cyan * (mult * 0.8f) * glowAlpha, Projectile.rotation, tex.Size() / 2, Projectile.scale * 1.1f * Projectile.Opacity, SpriteEffects.None, 0);
        }

        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Cyan * (0.5f) * glowAlpha, Projectile.rotation, tex.Size() / 2, Projectile.scale * (1 + (MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1) * 0.5f) * Projectile.Opacity, SpriteEffects.None, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return true;
    }
    Vector2 startP;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(startP);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        startP = reader.ReadVector2();
    }

    private float glowAlpha = 1, rotationOffset;
    public override void AI()
    {
        if (startP == Vector2.Zero)
        {
            startP = Projectile.Center;
            rotationOffset = Main.rand.NextFloat(-0.1f, 0.1f);
            Projectile.netUpdate = true;
        }
        Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1, 0.025f);
        Projectile.ai[0]++;

        if (Projectile.ai[0] <= 230)
        {
            if (Projectile.ai[0] < 20)
                Projectile.velocity *= 1.025f;
            if (Projectile.ai[0] < 80 && Projectile.ai[0] > 20 && Projectile.ai[0] % 5 == 0)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity,
                    Helper.FromAToB(Projectile.Center, startP + new Vector2(Projectile.ai[1], -260), false)
                        .RotatedBy(MathF.Sin(Projectile.ai[0]) * Projectile.ai[2] * 10) * Projectile.ai[2] * 2, 0.2f);
            else
                Projectile.velocity *= 0.98f;
            if (Projectile.ai[0] > 90 && Projectile.ai[0] % 5 == 0)
                Projectile.velocity = Vector2.Lerp(Projectile.velocity,
                    Helper.FromAToB(Projectile.Center, startP + new Vector2(Projectile.ai[1], -350), true)
                        .RotatedBy(MathF.Sin(Projectile.ai[0]) * Projectile.ai[2] * 10) * Projectile.ai[2] * 200, 0.2f);

            if (Projectile.ai[0] is > 100 and < 200)
            {
                float progress = (Projectile.ai[0] - 100) / 100f;
                Vector2 target = Helper.Raycast(Projectile.Center, Vector2.UnitY, 900).Point;
                if ((int)Projectile.ai[0] % 2 == 0) 
                {
                    Dust.NewDustPerfect(target, DustType<LineDustFollowPoint>(), new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-6, -1)) * progress, newColor: Color.Cyan, Scale: 0.1f);
                    Dust.NewDustPerfect(target, DustType<SparkleDust>(), Vector2.Zero, newColor: Color.Cyan * Main.rand.NextFloat(0.5f, 1), Scale: 0.065f);
                    
                    Vector2 position = target + Main.rand.NextVector2CircularEdge(50, 50) * progress;
                    if (Projectile.ai[0] < 180)
                        Dust.NewDustPerfect(position, DustType<LineDustFollowPoint>(), position.DirectionTo(target) * Main.rand.NextFloat(1.5f, 2.5f), newColor: Color.Cyan * 0.5f, Scale: 0.1f).customData = target;
                }
                

                if ((int)Projectile.ai[0] % 5 == 0)
                    SoundEngine.PlaySound(SoundID.DD2_LightningAuraZap with { Pitch = progress * 2 },
                        Projectile.Center);

                Dust.NewDustPerfect(Projectile.Center, DustType<LineDustFollowPoint>(), Vector2.UnitY.RotatedByRandom(0.5f) * Main.rand.NextFloat(10, 20) * progress, newColor: Color.Cyan, Scale: 0.1f * progress).customData = target;
            }

            if (Projectile.owner == Main.myPlayer)
                if (Projectile.ai[0] == 200)
                {
                    Projectile.NewProjectile(null, Projectile.Center, Vector2.UnitY.RotatedByRandom(0.5f),
                        ProjectileType<GarbageLightning>(), Projectile.damage, 0);
                }
        }
        else if (Projectile.ai[0] < 260)
        {
            glowAlpha = MathHelper.Lerp(glowAlpha, 0, 0.1f);
        }
        else
        {
            Dust.NewDustPerfect(Projectile.Center + new Vector2(Main.rand.NextFloat(-20, 20), Main.rand.NextFloat(-5, 5)).RotatedBy(Projectile.rotation), DustType<GarbageFlameDust>(), Projectile.velocity.RotatedByRandom(0.2f) * 0.5f, newColor: Color.OrangeRed, Scale: 0.1f);
            
            Projectile.velocity.Y += .2f + Projectile.ai[2];
            Projectile.rotation = Utils.AngleLerp(Projectile.rotation, MathHelper.PiOver2 + rotationOffset, 0.05f);
            
            if (Projectile.Grounded()) {
                Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosionWSpriteHostile>(), 50, 0);
                Projectile.Kill();
            }
        }
    }
}
public class GarbageDroneF : ModProjectile
{
    public override string Texture => Helper.AssetPath+"Projectiles/Garbage/GarbageDrone";
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
    }
    public override bool? CanDamage()
    {
        return false;
    }
    public override void SetDefaults()
    {
        Projectile.width = 32;
        Projectile.height = 20;
        Projectile.aiStyle = -1;
        Projectile.friendly = true;
        Projectile.tileCollide = false;
        Projectile.hostile = false;
        Projectile.timeLeft = 400;
        Projectile.Opacity = 0;
    }
    public override bool PreDraw(ref Color lightColor)
    {
        lightColor = Color.White * Projectile.Opacity;
        Texture2D tex = Assets.Projectiles.Garbage.GarbageDrone_Bloom.Value;
        Main.spriteBatch.Reload(BlendState.Additive);
        var fadeMult = 1f / Projectile.oldPos.Count();
        for (int i = 0; i < Projectile.oldPos.Count(); i++)
        {
            float mult = (1 - i * fadeMult);
            Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, Color.Cyan * (Projectile.Opacity * mult * 0.8f), Projectile.rotation, tex.Size() / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0);
        }

        Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, Color.Cyan * (0.5f * Projectile.Opacity), Projectile.rotation, tex.Size() / 2, Projectile.scale * (1 + (MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1) * 0.5f), SpriteEffects.None, 0);
        Main.spriteBatch.Reload(BlendState.AlphaBlend);
        return true;
    }
    Vector2 startP;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(startP);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        startP = reader.ReadVector2();
    }
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];
        if (startP == Vector2.Zero)
        {
            startP = Projectile.Center;
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.ai[1] = Main.MouseWorld.X;
                Projectile.ai[2] = Main.MouseWorld.Y;
            }
            Projectile.netUpdate = true;
        }

        if (Main.myPlayer == player.whoAmI)
        {
            Projectile.ai[1] = Main.MouseWorld.X;
            Projectile.ai[2] = Main.MouseWorld.Y;
        }
        if ((int)Projectile.ai[0] % 15 == 0 || (int)Projectile.ai[0] == 80)
            Projectile.netUpdate = true;
        Vector2 pos = new Vector2(Projectile.ai[1], Projectile.ai[2]) - new Vector2(0, 200);
        Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1, 0.025f);
        Projectile.ai[0]++;
        if (Projectile.ai[0] < 80 && Projectile.timeLeft % 5 == 0)
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, Helper.FromAToB(Projectile.Center, pos, false) * 0.05f, 0.2f);
        if (Projectile.ai[0] > 80)
            Projectile.velocity *= 0.9f;
        if (Projectile.ai[0] >= 100 && Projectile.ai[0] % 5 == 0 && Projectile.ai[0] < 120 && Projectile.owner == Main.myPlayer)
        {
            Projectile.NewProjectileDirect(null, Projectile.Center, Vector2.UnitY, ProjectileType<GarbageLightningF>(), Projectile.damage, 0, Projectile.owner);
        }
        if (Projectile.ai[0] > 130)
        {
            Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 0, 0);
            Projectile.Kill();
        }
    }
}
public class GarbageLightningF : GarbageLightning
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
    }
}
public class GarbageLightning : ModProjectile
{
    public override string Texture => Helper.Empty;
    int MAX_TIME = 40;
    public override void SetDefaults()
    {
        Projectile.width = 25;
        Projectile.height = 25;
        Projectile.friendly = false;
        Projectile.hostile = true;
        Projectile.ignoreWater = true;
        Projectile.tileCollide = false;
        Projectile.timeLeft = 80;
        Projectile.hide = true;
        Projectile.penetrate = -1;
        Projectile.extraUpdates = 1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.localNPCHitCooldown = 15;
    }
    public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) => behindNPCs.Add(index);
    public override void OnSpawn(IEntitySource source)
    {
        end = Projectile.Center;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (!RunOnce || points.Count < 2) return false;
        float a = 0f;
        bool colliding = false;
        for (int i = 1; i < points.Count; i++)
        {
            colliding = Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), points[i], points[i - 1], Projectile.width, ref a);
            if (colliding) break;
        }
        return colliding;
    }
    bool RunOnce;
    List<Vector2> points = new List<Vector2>();
    List<float> alphas = new();
    Vector2 end;
    static SoundStyle sound => SoundID.DD2_LightningAuraZap.WithVolumeScale(0.5f);
    private float lightningAmplitude, lightningMovement, lightningDirection;
    public override void AI()
    {
        Projectile.direction = end.X > Projectile.Center.X ? 1 : -1;
        Projectile.rotation = Projectile.velocity.ToRotation();

        float progress = Utils.GetLerpValue(0, 80, Projectile.timeLeft);
        if (Projectile.timeLeft < 40)
            Projectile.scale = MathHelper.Lerp(0, 1, Projectile.timeLeft / 40f);

        int n = (int)MathHelper.Clamp((Projectile.ai[2] <= 0 ? 900 : Projectile.ai[2]) / 60, 5, 20);

        Vector2 start = Projectile.Center;
        end = Helper.Raycast(Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.UnitY), Projectile.ai[2] <= 0 ? 900 : Projectile.ai[2]).Point;
        
        if (!RunOnce)
        {
            if (Projectile.ai[2] <= 0)
                SoundEngine.PlaySound(SoundID.NPCDeath56, Projectile.Center);
            points.Clear();
            
            lightningAmplitude = Main.rand.NextFloat(30, 40);
            
            lightningMovement = Main.rand.NextFloat(-lightningAmplitude, lightningAmplitude).SafeDivision();
            lightningDirection = -MathF.Sign(lightningAmplitude);
            
            for (int i = 0; i < n; i++)
            {
                alphas.Add(1f);
            }
            RunOnce = true;
        }
        
        Vector2 dir = (end - start).RotatedBy(MathHelper.PiOver4).SafeNormalize(Vector2.UnitY);

        if (Projectile.ai[1] < n)
        {
            lightningMovement += Main.rand.NextFloat(lightningAmplitude) * 0.5f * lightningDirection;
            lightningMovement = MathHelper.Clamp(lightningMovement, -lightningAmplitude, lightningAmplitude);

            if (MathF.Abs(lightningMovement + lightningDirection * lightningAmplitude) > 40)
                lightningDirection = -lightningDirection;

            Vector2 velocity = new Vector2(dir.X * (Projectile.ai[1] < n / 5f ? 0 : lightningMovement), dir.Y).RotatedBy(Projectile.velocity.SafeNormalize(Vector2.UnitY).ToRotation() - MathHelper.PiOver2);
            Vector2 point = Vector2.SmoothStep(start, end, Projectile.ai[1] / (float)n) + velocity;

            Projectile.velocity.X = MathHelper.Lerp(Projectile.velocity.X, 0, Projectile.ai[1] / n);
            Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 1, Projectile.ai[1] / n);
            
            points.Add(point);
        }

        if ((int)Projectile.ai[1] == n - 1)
        {
            Helper.AddCameraModifier(new PunchCameraModifier(end, Vector2.UnitY, 3, 10, 40, 500));
            for (int i = 0; i < 25; i++)
            {
                Dust.NewDustPerfect(end, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(7.5f, 7.5f), newColor: Color.Cyan * Main.rand.NextFloat(0.5f, 1), Scale: 0.13f);
                Dust.NewDustPerfect(end, DustType<GarbageFlameDust>(), Main.rand.NextVector2Circular(6, 6), newColor: Color.Cyan * Main.rand.NextFloat(0.5f, 1), Scale: 0.1f);
                Dust.NewDustPerfect(end, DustID.Smoke, Main.rand.NextVector2Circular(3, 3));
            }

            Collision.HitTiles(end, Vector2.UnitY, 30, 16);
        }
        
        Projectile.ai[1]++;
        
        if (points.Count > 2 && Projectile.ai[1] > n)
        {
            Projectile.ai[0]++;
            
            if ((int)Projectile.ai[0] % 5 == 0 && Projectile.ai[2] <= 0) 
                SoundEngine.PlaySound(sound, Projectile.Center);
            
            if (Projectile.timeLeft < 40)
                for (int i = 0; i < MathHelper.Clamp(Projectile.ai[0] * 0.15f, 0, points.Count); i++)
                {
                    alphas[i] = MathHelper.Lerp(alphas[i], 0, 0.07f);
                    points[i] = new Vector2(MathHelper.Lerp(points[i].X, Projectile.Center.X, 0.01f), points[i].Y);
                }
        }
        points[0] = Projectile.Center;
        if (Projectile.ai[1] > n)
            points[points.Count - 1] = end;
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Texture2D tex = Assets.Extras.Ex1.Value;
        Texture2D tex2 = Assets.Extras.Extras2.spark_08.Value;
        float s = 0f;
        List<VertexPositionColorTexture> vertices = new();
        
        if (points.Count > 1)
        {
            for (int i = 1; i < points.Count; i++)
            {
                if (i < points.Count / 2)
                    s = MathHelper.SmoothStep(0, 1, i / (points.Count / 2f));
                else
                    s = MathHelper.SmoothStep(1, 0, (i - (points.Count / 2f)) / (points.Count / 2f));

                float alpha = Projectile.scale;

                Vector2 curPoint = points[i] - Main.screenPosition;
                Vector2 prevPoint = points[i - 1] - Main.screenPosition;
                float rot = Helper.FromAToB(curPoint, prevPoint).ToRotation();
                if (points.Count < 5)
                    rot = Projectile.velocity.ToRotation();
                
                float off = (float)Main.timeForVisualEffects * 0.1f + (float)(i - 1) / points.Count;
                vertices.Add(Helper.AsVertex(curPoint + new Vector2(Projectile.scale * 20, 0).RotatedBy(rot - MathHelper.PiOver2), Color.CornflowerBlue with { A = 0 } * alpha * 1.5f * alphas[i], new Vector2(off, 0)));
                vertices.Add(Helper.AsVertex(curPoint + new Vector2(Projectile.scale * 20, 0).RotatedBy(rot + MathHelper.PiOver2), Color.Cyan with { A = 0 } * alpha * 1.5f * alphas[i], new Vector2(off, 1)));
            }
        }
        if (vertices.Count >= 3)
        {
            Main.spriteBatch.End(out var ss);
            Main.spriteBatch.Begin(ss with { samplerState = SamplerState.PointWrap });
            Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, tex);
            Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, tex2);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(ss);
        }
        
        return false;
    }
}

