using System;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageTechTelegraph : ModProjectile
{
	public override string Texture => Helper.Empty;
	public override void SetDefaults()
	{
		Projectile.height = 10;
		Projectile.width = 10;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
	}

	public override bool ShouldUpdatePosition() => false;
	public override bool? CanDamage() => false;

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = Assets.Extras.flameEye2.Value;
		Texture2D tex2 = Assets.Extras.crosslight.Value;
		Vector2 scale = new Vector2(1, 0.5f);
		
		Main.spriteBatch.End(out var ss);
		Main.spriteBatch.Begin(ss with { sortMode = SpriteSortMode.Immediate, effect = Effects.SpriteRotation.Value, samplerState = SamplerState.PointWrap });

		Vector2 position = Projectile.Center - Main.screenPosition;
		Vector4 color = Color.Red.ToVector4() with { W = 0 };
		
		
		for (int i = 0; i < 10; i++)
		{
			float alpha = MathF.Pow(1 - i / 10f, 0.5f) * MathHelper.Clamp(Projectile.ai[0] * 3, 0, 1f);
			
			position += new Vector2(7 + i, 0).RotatedBy(Projectile.velocity.ToRotation()) * MathF.Pow(Projectile.ai[0] + 0.1f, 2) * 3;
			
			Effects.SpriteRotation.Value.Parameters["scale"].SetValue(scale * (0.6f + i * 0.02f) * (i / 10f) * (1f - Projectile.ai[0]) * Projectile.ai[1]);
			Effects.SpriteRotation.Value.Parameters["rotation"].SetValue((float)Main.timeForVisualEffects * 0.1f + i * 1.2f);
			Effects.SpriteRotation.Value.Parameters["uColor"].SetValue(color * alpha * 0.5f);
			Main.spriteBatch.Draw(tex, position, null, Color.White, 0, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
			
			Effects.SpriteRotation.Value.Parameters["uColor"].SetValue(color * alpha * 2f);
			Main.spriteBatch.Draw(tex2, position, null, Color.White, 0, tex2.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		}
		
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(ss);
		return false;
	}
	public override void AI()
	{
		Projectile.ai[0] += 0.02f;
		if (Projectile.ai[0] > 1)
			Projectile.Kill();
	}
}