using System;
using System.Collections.Generic;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using Microsoft.Build.Evaluation;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class LaserDrone : ModProjectile
{
	public override string Texture => Helper.AssetPath + "Projectiles/Garbage/GarbageDrone";
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Type] = 10;
		ProjectileID.Sets.TrailingMode[Type] = 2;
	}

	public override bool? CanDamage() => false;
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
		Projectile.scale = 0.25f;
	}

	public override void OnKill(int timeLeft)
	{
		Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ProjectileType<FlameExplosionWSprite>(), 0, 0);
	}
	
	public override bool PreDraw(ref Color lightColor)
	{
		lightColor = Color.White * Projectile.Opacity;
		Texture2D tex = Assets.Projectiles.Garbage.GarbageDrone_Bloom.Value;
		Main.spriteBatch.Reload(BlendState.Additive);

		UnifiedRandom rand = new UnifiedRandom(Projectile.projUUID);
		Color color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * rand.NextFloat(0.5f, 1) + Projectile.whoAmI * rand.NextFloat(0.2f, 0.5f)) % 1f, 1, 0.85f);
		
		var fadeMult = 1f / Projectile.oldPos.Length;
		for (int i = 0; i < Projectile.oldPos.Length; i++)
		{
			float mult = (1 - i * fadeMult);
			Main.spriteBatch.Draw(tex, Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition, null, color * Projectile.scale* (Projectile.Opacity * mult * 0.8f), Projectile.rotation, tex.Size() / 2, Projectile.scale * 1.1f, SpriteEffects.None, 0);
		}

		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, color * Projectile.scale* (0.5f * Projectile.Opacity), Projectile.rotation, tex.Size() / 2, Projectile.scale * (1 + (MathF.Sin(Main.GlobalTimeWrappedHourly * 3f) + 1) * 0.5f), SpriteEffects.None, 0);
		Main.spriteBatch.Reload(BlendState.AlphaBlend);

		lightColor *= Projectile.scale;
		lightColor.A = (byte)(255 * Projectile.Opacity);
		tex = Assets.Projectiles.Garbage.GarbageDrone.Value;
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() / 2, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void AI()
	{
		Projectile.Opacity = MathHelper.Lerp(Projectile.Opacity, 1, 0.05f);
		Projectile.scale = MathHelper.Lerp(Projectile.scale, 1, 0.015f);
		
		if (Projectile.velocity.Length() > 0f)
			Projectile.rotation = Projectile.velocity.ToRotation();

		if (MathF.Sign(Projectile.velocity.X) < 0)
			Projectile.rotation += MathHelper.Pi;
		
		if (Projectile.ai[0] < 70)
			Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, 0, 0.025f);

		Projectile.ai[0]++;
		if (Projectile.ai[0] is > 60 and < 70)
			Projectile.velocity.X *= 0.98f;

		if (Projectile.ai[0] > 70)
		{
			Projectile.velocity.Y = MathHelper.Lerp(Projectile.velocity.Y, -10, 0.005f);
			Projectile.velocity.X *= 1.01f;
		}

		if (Projectile.ai[0] is > 50 and < 80 && (int)Projectile.ai[0] % 6 == 0)
		{
			SoundEngine.PlaySound(SoundID.Item91 with {MaxInstances = -1}, Projectile.Center);
			Vector2 velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedByRandom(0.1f);
			MPUtils.NewProjectile(null, Projectile.Center + velocity + Projectile.velocity * 3, velocity, ModContent.ProjectileType<LaserDroneLaser>(), Projectile.damage, 0);
		}
	}
}

public class LaserDroneLaser : ModProjectile
{
	public override string Texture => Helper.Empty;
	
	public override void SetStaticDefaults()
	{
		ProjectileID.Sets.TrailCacheLength[Type] = 10;
		ProjectileID.Sets.TrailingMode[Type] = 2;
	}
	
	public override void SetDefaults()
	{
		Projectile.Size = Vector2.One * 5;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = true;
		Projectile.hostile = true;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 400;
		Projectile.extraUpdates = 2;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.Center += Projectile.velocity;
		Projectile.velocity = Vector2.Zero;
		if (Projectile.timeLeft > 30)
			Projectile.timeLeft = 30;
		return false;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		List<VertexPositionColorTexture> vertices = new List<VertexPositionColorTexture>();

		for (int i = 0; i < Projectile.oldPos.Length; i++)
		{
			if (Projectile.oldPos[i] == Vector2.Zero) continue;
			Vector2 basePosition = Projectile.oldPos[i] + Projectile.Size / 2 - Main.screenPosition;
			float mult = 1f - i / (float)Projectile.oldPos.Length;
			for (int j = -1; j < 2; j += 2)
			{
				Vector2 position = basePosition + new Vector2(5f, 0).RotatedBy(Projectile.rotation + MathHelper.PiOver2 * j);
				Color color = Main.hslToRgb((Main.GlobalTimeWrappedHourly * 0.8f + i * new UnifiedRandom(Projectile.whoAmI).NextFloat(0.02f, 0.15f)) % 1f, 1, 0.75f) * mult * MathF.Sin(mult * MathF.PI) * 4;
				vertices.Add(Helper.AsVertex(position, color, new Vector2(mult + Main.GlobalTimeWrappedHourly*5, j < 0 ? 0 : j)));
			}	
		}

		if (vertices.Count > 2)
		{
			Main.spriteBatch.End(out var ss);
			Main.spriteBatch.Begin(ss with {samplerState = SamplerState.PointWrap});
			Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip,
				Assets.Extras.Tentacle.Value);
			Helper.DrawTexturedPrimitives(vertices.ToArray(), PrimitiveType.TriangleStrip,
				Assets.Extras.LintyTrail.Value);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(ss);
		}

		return false;
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation();
		if (Projectile.velocity.Length() < 15f)
			Projectile.velocity *= 1.03f;
	}
}