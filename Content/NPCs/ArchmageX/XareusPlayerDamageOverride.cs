using EbonianMod.Content.Buffs;

namespace EbonianMod.Content.NPCs.ArchmageX;

public class XareusPlayerDamageOverride : GlobalProjectile
{
	public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.friendly;

	public override bool? CanDamage(Projectile projectile)
	{
		if (Main.player[projectile.owner].HasBuff<Sheepened>())
			return false;
		
		return base.CanDamage(projectile);
	}
}