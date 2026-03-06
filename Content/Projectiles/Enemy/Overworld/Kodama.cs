using EbonianMod.Content.Dusts;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using EbonianMod.Common.Graphics;
using Terraria.GameContent.Drawing;

namespace EbonianMod.Content.Projectiles.Enemy.Overworld;
public class Kodama : ModProjectile
{
    public override string Texture => Helper.AssetPath + "Projectiles/Enemy/Overworld/" + Name;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.TrailingMode[Type] = 3;
        ProjectileID.Sets.TrailCacheLength[Type] = 20;
    }
    public override void SetDefaults()
    {
        Projectile.Size = new Vector2(22, 26);
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.aiStyle = -1;
    }
    public override Color? GetAlpha(Color drawColor) => Color.White;
    public override bool PreDraw(ref Color drawColor)
    {
        PixelationRendering.DrawCache.Add(() =>
        {
            List<VertexPositionColorTexture> vertices = new();
            for (int i = 0; i < Projectile.oldPos.Length; i++)
            {
                if (Projectile.oldPos[i] == Vector2.Zero) continue;
                Vector2 oldPos = Projectile.oldPos[i] + new Vector2(0, MathF.Sin(i * 0.5f) * 2).RotatedBy(Projectile.rotation);
                float mult = 1 - 1f / Projectile.oldPos.Length * i;
                float rotOffset = oldPos.FromAToB(Projectile.position).ToRotation();
                if (i > 0)
                {
                    Vector2 oldPos2 = Projectile.oldPos[i - 1] + new Vector2(0, MathF.Sin(i * 0.5f) * 2).RotatedBy(Projectile.rotation);
                    rotOffset = oldPos2.FromAToB(oldPos).ToRotation();
                }
                rotOffset += MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + Projectile.whoAmI * 10) * SmoothStep(1, 0, mult);
                Vector2 off = i <= 1 ? Projectile.rotation.ToRotationVector2() * Projectile.velocity.Length() * 0.5f : Vector2.Zero;
                Vector2 pos = oldPos + Projectile.Size / 2 + new Vector2(0, -4).RotatedBy(Projectile.rotation) - rotOffset.ToRotationVector2() * 10 + off - Main.screenPosition;
                vertices.Add(Helper.AsVertex(pos + new Vector2(13 * Clamp(mult * 2, 0, 1), 0).RotatedBy(PiOver2 + rotOffset), Color.White * alpha * (i < 3 ? 0 : 1), new Vector2((float)i / Projectile.oldPos.Length * 3 - Main.GlobalTimeWrappedHourly * 1.5f, 0)));
                vertices.Add(Helper.AsVertex(pos + new Vector2(13 * Clamp(mult * 2, 0, 1), 0).RotatedBy(-PiOver2 + rotOffset), Color.White * alpha * (i < 3 ? 0 : 1), new Vector2((float)i / Projectile.oldPos.Length * 3 - Main.GlobalTimeWrappedHourly * 1.5f, 1)));
            }
            if (vertices.Count > 2)
                Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip, Assets.ExtraSprites.Overworld.KodamaTrail.Value, false, true);
        }
        );
        FinalDrawSystem.DrawCache.Add(() =>
        Main.spriteBatch.Draw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, Color.White * alpha, Projectile.rotation + (Projectile.direction == -1 ? Pi : 0), Projectile.Size / 2, Projectile.scale, Projectile.direction == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0)
        );
        return false;
    }
    public float alpha = 1;
    Vector2 initVel, initPos;
    public override void AI()
    {
        if (Projectile.ai[2] == 0)
        {
            initVel = Projectile.velocity;
            initPos = Projectile.Center;
            Projectile.ai[2] = Main.rand.Next(ushort.MaxValue);
        }
        else
            Projectile.SineMovement(initPos, initVel, .1f, .6f);
        for (int i = 0; i < Projectile.oldPos.Length; i += 2)
            Lighting.AddLight(Projectile.oldPos[i] + Projectile.Size / 2, 197f / 255f * (1 - 1f / Projectile.oldPos.Length * i), 226f / 255f * (1 - 1f / Projectile.oldPos.Length * i), 105f / 255f * (1 - 1f / Projectile.oldPos.Length * i));

        Lighting.AddLight(Projectile.Center + Projectile.velocity, 197f / 255f, 226f / 255f, 105f / 255f);
        Projectile.direction = Projectile.spriteDirection = -1;
        Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation(), 0.2f);

        Projectile.ai[0] += 0.5f;
        if (Projectile.ai[0] > 100)
            alpha = Lerp(alpha, 0, 0.1f);
        if (Projectile.ai[0] > 110)
            Projectile.Kill();
    }
}

public class KodamaF : Kodama
{
    public override string Texture => GetModProjectile(ProjectileType<Kodama>()).Texture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.DamageType = DamageClass.Magic;
        Projectile.friendly = true;
        Projectile.hostile = false;
        Projectile.usesLocalNPCImmunity = true;
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings
        {
            PositionInWorld = Projectile.Center + Projectile.velocity,
            MovementVector = Vector2.One
        });
        for (int i = 0; i < 10; i++)
        {
            Vector2 _v = Main.rand.NextVector2Circular(5, 5);
            float scale = Main.rand.NextFloat(0.01f, 0.15f);
            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustType<SparkleDust>(), _v, 0, Color.LawnGreen, scale);
            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustType<SparkleDust>(), _v, 0, Color.White, scale * 0.9f);
            _v = Main.rand.NextVector2Circular(5, 5);
            Dust.NewDustPerfect(Projectile.Center + Projectile.velocity, DustID.WoodFurniture, _v).noGravity = true;
        }
    }
    Vector2 pos, vel;
    Vector2 mousePos;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.WriteVector2(vel);
        writer.WriteVector2(pos);
        writer.WriteVector2(mousePos);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        vel = reader.ReadVector2();
        pos = reader.ReadVector2();
        mousePos = reader.ReadVector2();
    }
    public override void AI()
    {
        if (vel == Vector2.Zero)
        {
            SoundEngine.PlaySound(SoundID.Item117 with { Pitch = 0.5f, PitchVariance = 0.2f }, Projectile.Center);
            for (int i = 0; i < 3; i++)
            {
                Vector2 _v = Main.rand.NextVector2Circular(5, 5);
                float scale = Main.rand.NextFloat(0.01f, 0.15f);
                Dust.NewDustPerfect(Projectile.Center, DustType<SparkleDust>(), _v, 0, Color.LawnGreen, scale);
                Dust.NewDustPerfect(Projectile.Center, DustType<SparkleDust>(), _v, 0, Color.White, scale * 0.9f);
            }
            ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.TrueNightsEdge, new ParticleOrchestraSettings
            {
                PositionInWorld = Projectile.Center + Projectile.velocity,
                MovementVector = Vector2.One
            });
            alpha = 0;
            float rand = Main.rand.NextFloat(-0.5f, 0.5f);
            for (int i = 0; i < Projectile.oldPos.Length; i++)
                Projectile.oldPos[i] = Projectile.position - Projectile.velocity.RotatedBy(rand) * i * 0.4f;
            if (Main.myPlayer == Projectile.owner)
                mousePos = Main.MouseWorld;
            Projectile.netUpdate = true;
        }
        for (int i = 0; i < Projectile.oldPos.Length; i += 2)
            Lighting.AddLight(Projectile.oldPos[i] + Projectile.Size / 2, 197f / 255f * (1 - 1f / Projectile.oldPos.Length * i), 226f / 255f * (1 - 1f / Projectile.oldPos.Length * i), 105f / 255f * (1 - 1f / Projectile.oldPos.Length * i));

        Lighting.AddLight(Projectile.Center + Projectile.velocity, 197f / 255f, 226f / 255f, 105f / 255f);
        Projectile.direction = Projectile.spriteDirection = -1;
        Projectile.rotation = Projectile.rotation.AngleLerp(Projectile.velocity.ToRotation(), 0.2f);


        Projectile.ai[0] += 0.5f;

        if (Projectile.ai[0] > 100)
            alpha = Lerp(alpha, 0, 0.1f);

        pos += vel * Projectile.velocity.Length();
        Projectile.velocity = Vector2.Lerp(Projectile.velocity, vel * 20, 0.02f);
        if (Projectile.ai[0] <= 20)
        {
            alpha = Lerp(alpha, 1, 0.1f);
            if (Projectile.ai[0] < 5)
                vel = Helper.FromAToB(Projectile.Center - new Vector2(0, 150), mousePos);
            Projectile.netUpdate = true;
        }
        if (Projectile.ai[0] > 110)
            Projectile.Kill();
    }
}