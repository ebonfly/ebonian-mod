using EbonianMod.Content.Dusts;

namespace EbonianMod.Content.NPCs.Garbage.Projectiles;

public class GarbageDashFlames : ModProjectile
{
	public override string Texture => Helper.Empty;

	public override void SetDefaults()
	{
		Projectile.width = 40;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.tileCollide = false;
		Projectile.hostile = true;
		Projectile.timeLeft = 100;
	}

	public override bool ShouldUpdatePosition() => false;

	public override void AI()
	{
		Projectile.ai[0]++;
		Dust.NewDustPerfect(Projectile.Top + new Vector2(Main.rand.NextFloat(-25, 25f), Main.rand.NextFloat(-4f, 2)), DustID.SolarFlare, Projectile.scale * new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-4, -1)), Scale: Projectile.scale / 2f).customData = 1;
		
		Dust.NewDustPerfect(Projectile.Top + new Vector2(Main.rand.NextFloat(-25, 25f), Main.rand.NextFloat(-8f, -2f) * Projectile.scale), ModContent.DustType<LineDustFollowPoint>(), Projectile.scale * new Vector2(Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(-4, -1)), Scale: Main.rand.NextFloat(0.03f, 0.1f), newColor: Color.OrangeRed);
		Projectile.scale = MathHelper.Clamp(MathHelper.SmoothStep(0, 2, Projectile.timeLeft / 100f), 0.1f, 2);
	}
}