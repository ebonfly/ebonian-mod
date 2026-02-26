using EbonianMod.Content.NPCs.Garbage.Projectiles;
using EbonianMod.Content.Projectiles.VFXProjectiles;

namespace EbonianMod.Content.NPCs.Garbage;

public partial class HotGarbage : ModNPC
{
	void DoDash()
	{
		AITimer++;
		if (AIState == State.WarningForDash)
		{
			AnimationStyle = AnimationStyles.BoostWarning;
			FacePlayer();
            
			NPC.velocity.X *= 0.99f;
			if (AITimer == 20)
			{
				SoundEngine.PlaySound(SoundID.Zombie66, NPC.Center);
				MPUtils.NewProjectile(NPC.InheritSource(NPC), NPC.Center, Vector2.Zero, ProjectileType<CircleTelegraph>(), 0, 0);
			}
			if (AITimer >= 55)
			{
				NPC.velocity.X = 0;
				AITimer = 0;
				AITimer2 = 0;
				AITimer3 = 0;
				AIState = State.Dash;
			}
		}
		else
		{
			const int DashInterval = 65;
			NPC.damage = 60;
			
			if (NPC.velocity.Length() > 2f && AITimer3 < DashInterval-10)
				AnimationStyle = AnimationStyles.Boost;
			else
				AnimationStyle = AnimationStyles.BoostWarning;
            
			if ((int)AITimer3 == 7)
				SoundEngine.PlaySound(Sounds.exolDash, NPC.Center);
			
			if (AITimer3 < 22)
			{
				if (NPC.Grounded() && NPC.collideX)
					NPC.velocity.Y = -5.75f;
                
				NPC.velocity.X = Lerp(NPC.velocity.X, 20f * NPC.direction, 0.15f);
			}
			else
			{
				NPC.velocity *= 0.96f;
				if (NPC.velocity.Length() < 4f)
					FacePlayer();
                
				if (AITimer3 < 40 && AITimer3 % 2 == 0 && NPC.Grounded())
				{
					MPUtils.NewProjectile(NPC.GetSource_FromThis(), NPC.Center + new Vector2(Main.rand.NextFloat(-4, 4), NPC.height / 2f + 6), new Vector2(-NPC.direction * Main.rand.NextFloat(1, 3), Main.rand.NextFloat(-5, -1)), ProjectileType<GarbageDashFlames>(), 15, 0);
				}
			}
			
			if (++AITimer3 >= DashInterval)
			{
				AITimer3 = 0;
				NPC.netUpdate = true;
			}
			
			if (AITimer >= DashInterval * 3)
			{
				NPC.velocity = Vector2.Zero;
				ResetTo(State.OpenLid, State.SpewFire);
			}
		}
	}

	void DoSlam()
	{
		
	}
}