using System;
using EbonianMod.Common.Misc;
using EbonianMod.Common.UI.Titledrop;
using EbonianMod.Content.Dusts;
using EbonianMod.Content.NPCs.Garbage.Projectiles;
using EbonianMod.Content.Projectiles.ArchmageX;
using EbonianMod.Content.Projectiles.Terrortoma;
using EbonianMod.Content.Projectiles.VFXProjectiles;
using ReLogic.Utilities;

namespace EbonianMod.Content.NPCs.Void;

[AutoloadBossHead]
public class Ceaseless : CommonNPC
{
	public override string Texture => Helper.AssetPath + "NPCs/Void/" + Name;

	public override void SetStaticDefaults()
	{
		NPCID.Sets.TrailingMode[Type] = 3;
		NPCID.Sets.TrailCacheLength[Type] = 10;

		NPCID.Sets.MustAlwaysDraw[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.Size = new Vector2(104);
		NPC.lifeMax = ushort.MaxValue / 5;
		NPC.defense = 30;
		NPC.boss = true;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.HitSound = SoundID.DD2_BetsyHurt with { Volume = 2, Pitch = -3 };
		NPC.knockBackResist = 0f;
		NPC.aiStyle = -1;
	}
	
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = TextureAssets.Npc[Type].Value;
		Texture2D bloom = Assets.Extras.explosion.Value;
		Texture2D vortex = Assets.Extras.vortex.Value;
		Texture2D rune = Assets.Extras.rune_alt.Value;

		for (int i = 0; i < 5f; i++)
			spriteBatch.Draw(bloom, NPC.Center + Main.rand.NextVector2Circular(50, 50) - screenPos, null, Color.Black * 0.15f, Main.GlobalTimeWrappedHourly, bloom.Size() / 2f, 15 + MathF.Sin(Main.GlobalTimeWrappedHourly * 0.1f) * 2, SpriteEffects.None, 0);
		
		for (int j = NPC.oldPos.Length - 1; j >= 0; j--)
		{
			float alpha = 1f - j / (float)NPC.oldPos.Length;
			
			for (int i = 0; i < 5f; i++)
				spriteBatch.Draw(vortex, NPC.oldPos[j] + NPC.Size / 2f + Main.rand.NextVector2Circular(50, 50) - screenPos, null, Main.hslToRgb(alpha, 1, 0.8f) with { A = 0 } * 0.02f, 5 * Main.GlobalTimeWrappedHourly + j, vortex.Size() / 2f, 3 * alpha, SpriteEffects.None, 0);
		}
		
		for (int i = -5; i < 5f; i++)
			spriteBatch.Draw(rune, NPC.Center + Main.rand.NextVector2Circular(15, 15) - screenPos, null, Color.Black * 0.1f, i * 0.03f, rune.Size() / 2f, 3, SpriteEffects.None, 0);
		
		spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, Color.Black, NPC.rotation, tex.Size() / 2f, NPC.scale * new Vector2(MathF.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly)), 1), SpriteEffects.None, 0);
		spriteBatch.Draw(tex, NPC.Center - screenPos, NPC.frame, Color.Black, NPC.rotation, tex.Size() / 2f, NPC.scale * new Vector2(MathF.Abs(MathF.Sin(Main.GlobalTimeWrappedHourly + MathHelper.PiOver2)), 1), SpriteEffects.None, 0);
		
		for (int j = NPC.oldPos.Length - 1; j >= 0; j--)
		{
			float alpha = 1f - j / (float)NPC.oldPos.Length;
			Vector2 offset = Main.rand.NextVector2Circular(30, 30) * (1 - alpha);
			
			for (int i = 0; i < 5f; i++)
			{
				spriteBatch.Draw(tex, NPC.oldPos[j] + NPC.Size / 2f + offset + Main.rand.NextVector2Circular(5, 5) * (3 - alpha) - screenPos, NPC.frame, Color.Black * 0.2f * alpha, NPC.rotation, tex.Size() / 2f, NPC.scale + MathF.Sin(Main.GlobalTimeWrappedHourly + i * 3) * 0.3f, SpriteEffects.None, 0);
				spriteBatch.Draw(tex, NPC.oldPos[j] + NPC.Size / 2f + offset + Main.rand.NextVector2Circular(5, 5) * (3 - alpha) - screenPos, NPC.frame, Main.hslToRgb(i / 5f, 1f, 0.5f) with { A = 0 } * alpha, NPC.rotation, tex.Size() / 2f, NPC.scale + MathF.Sin(Main.GlobalTimeWrappedHourly + i * 3) * 0.3f, SpriteEffects.None, 0);

				if (enrageAlpha > 0 && j == 0)
				{
					spriteBatch.Draw(tex, NPC.oldPos[j] + NPC.Size / 2f + offset + Main.rand.NextVector2Circular(50, 50) * (3 - alpha) - screenPos, NPC.frame, Main.hslToRgb(i / 5f, 1f, 0.5f) with { A = 0 } * alpha * MathF.Sin(enrageAlpha * MathF.PI), NPC.rotation, tex.Size() / 2f, NPC.scale + enrageAlpha * 4, SpriteEffects.None, 0);
				}
			}
		}

		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		
	}

	void LineDustRing(float radius) 
	{
		Vector2 pos = NPC.Center + Main.rand.NextVector2CircularEdge(radius, radius) * Main.rand.NextFloat(0.7f, 1.4f);
		Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), pos.DirectionTo(NPC.Center), 0, Main.hslToRgb(Main.rand.NextFloat(), 1, 0.8f) with { A = 0 } * 0.1f, Main.rand.NextFloat(0.15f, 0.25f)).customData = NPC;
	}

	private SlotId cachedSound;
	private SlotId cachedSoundSuck;
	void Ambience()
	{
		for (int i = 0; i < 2; i++)
			LineDustRing(700);
		
		if (!Main.dedServ)
		{
			if (SoundEngine.TryGetActiveSound(cachedSound, out var _activeSound))
			{
				_activeSound.Pitch = MathHelper.Clamp(Lerp(-1, 1, NPC.velocity.Length() / 20), -1, 1);
				_activeSound.Position = NPC.Center;
			}
			else
			{
				cachedSound = SoundEngine.PlaySound(Sounds.CeaselessIdle.WithVolumeScale(0.35f), NPC.Center, (_) => NPC.AnyNPCs(Type));
			}
			
			if (SoundEngine.TryGetActiveSound(cachedSoundSuck, out var activeSound))
			{
				activeSound.Position = NPC.Center;
				activeSound.Volume = MathF.Sin(enrageAlpha * MathF.PI);
			}
			else
			{
				cachedSoundSuck = SoundEngine.PlaySound(Sounds.CeaselessSuck.WithVolumeScale(0.35f), NPC.Center, (_) => NPC.AnyNPCs(Type));
			}
		}
	}

	private float enrageAlpha;

	public override void AI()
	{
		if (!Main.dedServ)
		{
			if (AIState > 0)
				Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/void");
			else
				Music = 0;
		}
		
		Ambience();
		
		Player player = Main.player[NPC.target];
		NPC.TargetClosest(false);

		AITimer++;
		NPC.rotation = NPC.velocity.X * 0.01f;

		if (Main.mouseRight)
		{
			NPC.velocity = (Main.MouseWorld - NPC.Center) * 0.05f;
			AITimer = 0;
			AIState = 0;
			enrageAlpha = 0;
			AITimer2 = 0;
		}

		NPC.velocity *= 0.9f;


		if (AITimer < 0)
		{
			enrageAlpha = 0;
			NPC.velocity = (player.Center - new Vector2(0, 200) - NPC.Center) * 0.15f;
			return;
		}
		
		switch (AIState)
		{
			case 0:
				if ((int)AITimer == 2)
				{
					SoundEngine.PlaySound(Sounds.voidtitledrop);
					TitledropSystem.SetStyle(VoidTitledrop.Instance);
				}

				if (AITimer > 300)
				{
					AIState = 1;
					enrageAlpha = 0;
					Reset();
					AITimer = -50;
				}

				break;
			
			case 1:
				NPC.velocity *= 0.9f;
				if (AITimer < 35)
				{
					if ((int)AITimer == 1)
					{
						SoundEngine.PlaySound(Sounds.CeaselessRoar, NPC.Center);
						SoundEngine.PlaySound(Sounds.CeaselessSuckShort.WithVolumeScale(1.8f).WithPitchOffset(0.2f), NPC.Center);
					}

					enrageAlpha = Lerp(enrageAlpha, 1, 0.15f);
					for (int i = 0; i < 20; i++)
						LineDustRing(500);
					
				}
				
				if (AITimer is >= 35 and <= 300 && AITimer % 16 == 0)  
				{
					for (int i = 0; i < 8; i++)
						MPUtils.NewProjectile(null, NPC.Center + new Vector2(1500).RotatedBy(Helper.CircleDividedEqually(i, 8) + AITimer * 0.02f), Vector2.Zero, ProjectileType<VoidOrb>(), 20, 0, -1, NPC.Center.X, NPC.Center.Y);
					
				}
					
				if (AITimer is >= 60 and <= 400 && AITimer % 4 == 0)
				{
					SoundEngine.PlaySound(SoundID.Item72.WithPitchOffset(0.5f).WithVolumeScale(0.5f), NPC.Center);
					MPUtils.NewProjectile(null, NPC.Center, new Vector2(15,0).RotatedBy(AITimer * 0.1f), ProjectileType<VoidLaser>(), 20, 0);
				}

				if (AITimer > 460)
				{
					AIState = 2;
					enrageAlpha = 0;
					Reset();
					AITimer = -50;
				}
				break;
			
			case 2:
				if (AITimer is > 10 and <= 30)
				{
					if ((int)AITimer == 11)
						SoundEngine.PlaySound(Sounds.CeaselessRoar, NPC.Center);
					enrageAlpha = Lerp(0, 1, (AITimer - 10f) / 20f);
					for (int i = 0; i < 30; i++)
						LineDustRing(300);
				}

				if (AITimer > 40)
					AITimer2 += Lerp(1, 5, (AITimer - 40) / (40 * 14));


				if (AITimer is > 40 and < 40 + 40 * 15)
				{
					if ((int)AITimer == 41)
					{
						NPC.localAI[0] = 0;
						AITimer3 = Main.rand.NextFloat(TwoPi);
						MPUtils.NewProjectile(null, NPC.Center, new Vector2(1, 0).RotatedBy(AITimer3), ProjectileType<XTelegraphLine>(), 0, 0);
					}

					if (AITimer2 < 30 && AITimer > 40)
						enrageAlpha = Lerp(0, 1, AITimer2 / 30f);

					if (AITimer2 is > 25 and < 40)  
					{
						if (NPC.localAI[0] == 0)
						{
							SoundEngine.PlaySound(Sounds.DarkLightning with { Pitch = Lerp(0.2f, -0.6f,(AITimer - 40) / (40 * 14)), Volume = 0.4f, MaxInstances = -1 }, NPC.Center);
							NPC.localAI[0] = 1;
						}
						MPUtils.NewProjectile(null, NPC.Center, new Vector2(1, 0).RotatedBy(AITimer3 + Main.rand.NextFloat(-0.2f, 0.2f)), ProjectileType<XLightningBolt>(), 20, 0, ai1: 1);
					}

					if (AITimer2 > 40)
					{
						NPC.localAI[0] = 0;
						AITimer3 = Main.rand.NextFloat(TwoPi);
						MPUtils.NewProjectile(null, NPC.Center, new Vector2(1, 0).RotatedBy(AITimer3), ProjectileType<XTelegraphLine>(), 0, 0);
						AITimer2 = 0;
					}
				}

				if ((int)AITimer == 40 + 40 * 15)
					for (int i = 0; i < 10; i++)
						MPUtils.NewProjectile(null, NPC.Center, new Vector2(1, 0).RotatedBy(Helper.CircleDividedEqually(i, 10f)), ProjectileType<XTelegraphLine>(), 0, 0);
				
				if ((int)AITimer == 40 + 40 * 16 - 10) 
				{
					for (int i = 0; i < 10; i++) 
					{
						for (int j = 0; j < 3; j++)
							MPUtils.NewProjectile(null, NPC.Center, new Vector2(1, 0).RotatedBy(Helper.CircleDividedEqually(i, 10f) + Main.rand.NextFloat(-0.2f, 0.2f)), ProjectileType<XLightningBolt>(), 20, 0, ai1: 1);
						
						MPUtils.NewProjectile(null, NPC.Center, new Vector2(1, 0).RotatedBy(Helper.CircleDividedEqually(i, 10f)), ProjectileType<XSineLaser>(), 20, 0);
					}
					
					SoundEngine.PlaySound(Sounds.BigDarkLightning, NPC.Center);
				}

				if (AITimer > 40 * 18)
				{
					AIState = 3;
					enrageAlpha = 0;
					Reset();
					AITimer = -50;
				}
				break;
			
			case 3:
				if (AITimer is > 10 and <= 60)
				{
					if ((int)AITimer == 11)
						SoundEngine.PlaySound(Sounds.CeaselessRoar, NPC.Center);
					enrageAlpha = Lerp(0, 1, (AITimer - 10f) / 50f);
					for (int i = 0; i < 20; i++)
						LineDustRing(500);
				}
				
				if (AITimer is > 60 and < 400 && (int)AITimer % 10 == 0) 
				{
					MPUtils.NewProjectile(null, NPC.Center, new Vector2(0, Main.rand.NextFloat(5, 10)).RotatedBy((AITimer - 60) * 0.03f), ProjectileType<VoidPortal>(), 20, 0);
				
					SoundEngine.PlaySound(Sounds.reiFail2.WithPitchOffset(-0.5f), NPC.Center);	
				}
				
				if (AITimer is > 210 and < 540 && (int)AITimer % 20 == 0)
					SoundEngine.PlaySound(Sounds.DarkLightning, NPC.Center);
				
				if (AITimer is >= 250 and < 610 && (int)(AITimer - 150) % 100 == 0)
					SoundEngine.PlaySound(Sounds.BigDarkLightning, NPC.Center);
				
				if ((int)AITimer % 100 == 0 && AITimer is > 60 and < 420)
				{
					float amt = 4 * (1 + MathHelper.Clamp(AITimer2, 0, 3));
					for (int i = 0; i < amt; i++)
						MPUtils.NewProjectile(null, NPC.Center, new Vector2(0, Main.rand.NextFloat(5, 10)).RotatedBy(Helper.CircleDividedEqually(i, amt)), ProjectileType<VoidPortal>(), 20, 0);

					AITimer2++;
				}

				if (AITimer > 720)
				{
					AIState = 4;
					enrageAlpha = 0;
					Reset();
					AITimer = -50;
				}
				
				break;
			
			case 4:
				AITimer2++;
				
				if ((int)AITimer2 == 2)
				{
					SoundEngine.PlaySound(Sounds.CeaselessRoar.WithPitchOffset(1), NPC.Center);
					for (int i = 0; i < 4; i++)
						MPUtils.NewProjectile(null, NPC.Center, new Vector2(5).RotatedBy(Helper.CircleDividedEqually(i, 4)), ProjectileType<VoidMissile>(), 20, 0);
				}
				
				if (AITimer2 > 60)
					NPC.velocity = (player.Center - new Vector2(0, 200) - NPC.Center) * 0.15f;

				if (AITimer2 > 100)
					AITimer2 = 0;

				if (AITimer > 350)
				{
					AIState = 5;
					enrageAlpha = 0;
					Reset();
					AITimer = -50;
				}
				break;
			
			case 5:
				if (AITimer < 400)
				{
					int time = 400 - (int)AITimer;
					
        			int fac = 12;
        			if (time < 140)
        			    fac = 1;
        			else if (time < 205)
        			    fac = 3;
        			else if (time < 275)
        			    fac = 5;
        			else if (time < 300)
        			    fac = 7;
        			else if (time < 320)
        			    fac = 8;
        			else if (time < 360)
        			    fac = 9;
					
					
        			if (AITimer % fac == 0 && time > 35)
        			{
        			    MPUtils.NewProjectile(null, NPC.Center - new Vector2(0, 6), Main.rand.NextVector2Unit() * Main.rand.NextFloat(0.5f, 2), ProjectileType<XCloudVFXExtra>(), 0, 0);
        			}
					
        			if (fac < 9 && AITimer % fac == 0 && time > 5)
        			    for (int i = 0; i < 2; i++)
        			    {
        			        Vector2 vel = Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, MathHelper.Clamp(10 - fac, 3, 10));
        			        MPUtils.NewProjectile(null, NPC.Center - new Vector2(0, 6) - vel * 2, vel, ProjectileType<XAnimeSlash>(), 0, 0, -1, 0, Main.rand.NextFloat(-0.1f, 0.1f), Main.rand.NextFloat(0.1f, 0.3f));
        			    }
					
        			if ((int)time == 399)
        			{
        			    SoundEngine.PlaySound(Sounds.buildup, NPC.Center);
        			}
        			if ((int)time == 130)
        			    SoundEngine.PlaySound(Sounds.BeamWindUp.WithPitchOffset(-0.5f), NPC.Center);
					
        			if ((int)time == 40)
        			{
        			    MPUtils.NewProjectile(null, NPC.Center, Vector2.Zero, ProjectileType<ArchmageChargeUp>(), 0, 0);
        			}
				}

				if ((int)AITimer == 390)
				{
					enrageAlpha = 0.5f;
					SoundEngine.PlaySound(Sounds.CeaselessRoar.WithVolumeScale(2).WithPitchOffset(-0.5f), NPC.Center);
					SoundEngine.PlaySound(Sounds.CeaselessRoar.WithVolumeScale(2), NPC.Center);
					SoundEngine.PlaySound(Sounds.CeaselessRoar.WithVolumeScale(2).WithPitchOffset(0.5f), NPC.Center);
				}
				
				if ((int)AITimer is >= 390 and <= 400 && (int)AITimer % 2 == 0)
					MPUtils.NewProjectile(null, NPC.Center, -Vector2.UnitY, ProjectileType<TBeam>(), 100, 0, ai1: 3);


				if (AITimer > 560)
					enrageAlpha = Lerp(enrageAlpha, 0, 0.1f);
				
				if (AITimer > 600)
				{
					AIState = 1;
					enrageAlpha = 0;
					Reset();
					AITimer = -50;
				}
				break;
		}
	}
}

public class VoidLaser : ModProjectile
{
	public override string Texture => Helper.AssetPath + "NPCs/Void/" + Name;
	public override void SetDefaults()
	{
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.Size = new Vector2(6, 6);
		Projectile.aiStyle = -1;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = TextureAssets.Projectile[Type].Value;
		float max = Projectile.localAI[0];
		for (int i = 0; i < Projectile.localAI[0]; i++)
		{
			Main.spriteBatch.Draw(tex, Projectile.Center - Projectile.velocity * i - Main.screenPosition, null, Color.White with { A = 0 } * (1f - i / Projectile.localAI[0]), Projectile.rotation, tex.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);	
		}
		return false;
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation();
		Projectile.localAI[0] = MathHelper.Lerp(Projectile.localAI[0], 15, 0.05f);
	}
}

public class VoidMissile : ModProjectile
{
	public override string Texture => Helper.AssetPath + "NPCs/Void/" + Name;
	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 5;
	}

	public override void SetDefaults()
	{
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.Size = new Vector2(50, 50);
		Projectile.aiStyle = -1;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		lightColor = Color.White;
		Texture2D tex = TextureAssets.Projectile[Type].Value;
		Rectangle frame = tex.Frame(1, 5, 0, Projectile.frame);
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, frame, Color.Black, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		
		for (int i = 0; i < 5; i++)
		{
			Main.spriteBatch.Draw(tex, Projectile.Center + Main.rand.NextVector2Circular(15, 15) - Main.screenPosition, frame, Color.White with { A = 0 }, Projectile.rotation, frame.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}

	public override void AI()
	{
		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

		Projectile.ai[0]++;
		if (Projectile.ai[0] < 30)
			Projectile.velocity *= 0.97f;
		
		if (Projectile.ai[0] is > 60 and < 120)
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, (Main.player[Projectile.owner].Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 20, 0.1f);

		if (Projectile.ai[0] > 150)
		{
			MPUtils.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<XExplosion>(), 20, 0);
			
			Projectile.Kill();
		}
	}
}

public class VoidOrb : ModProjectile
{
	public override string Texture => Helper.AssetPath + "NPCs/Void/" + Name;
	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 8;
	}

	public override void SetDefaults()
	{
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.Size = new Vector2(70, 82);
		Projectile.aiStyle = -1;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		lightColor = Color.White;
		Texture2D tex = TextureAssets.Projectile[Type].Value;
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, tex.Frame(1, 8, 0, Projectile.frame), Color.Black, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, SpriteEffects.None, 0);
		
		for (int i = 0; i < 5; i++)
		{
			Main.spriteBatch.Draw(tex, Projectile.Center + Main.rand.NextVector2Circular(15, 15) - Main.screenPosition, tex.Frame(1, 8, 0, Projectile.frame), Color.White with { A = 0 }, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}

	public override void AI()
	{
		bool forward = (int)Projectile.localAI[0] % 2 == 0;
		if (Projectile.frameCounter++ % 5 == 0)
		{
			Projectile.frame += forward ? 1 : -1;
			if ((Projectile.frame < 1 && !forward) || (Projectile.frame > 6 && forward))
				Projectile.localAI[0]++;
		}

		Vector2 target = new Vector2(Projectile.ai[0], Projectile.ai[1]);
		if (target != Vector2.Zero)
		{
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(target) * 15, 0.02f);

			if (Projectile.Distance(target) < 60)
			{
				if (Projectile.whoAmI % 4 == 0)
					SoundEngine.PlaySound(Sounds.xSpirit, Projectile.Center);
				
				for (int i = 0; i < 15; i++)
					Dust.NewDustPerfect(Projectile.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(15, 15), newColor: Color.Violet with { A = 0 }, Scale: 0.1f);
				
				Projectile.Kill();
			}
		}
	}
}

public class VoidPortal : ModProjectile
{
	public override string Texture => Helper.AssetPath + "NPCs/Void/" + Name;
	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = 6;
	}

	public override void SetDefaults()
	{
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.Size = new Vector2(86, 88);
		Projectile.aiStyle = -1;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = TextureAssets.Projectile[Type].Value;
		for (int i = 0; i < 5; i++)
		{
			Main.spriteBatch.Draw(tex, Projectile.Center + Main.rand.NextVector2Circular(15, 15) * Projectile.scale - Main.screenPosition, tex.Frame(1, 6, 0, Projectile.frame), Color.White with { A = 0 }, Projectile.rotation, Projectile.Size / 2f, Projectile.scale, SpriteEffects.None, 0);
		}

		return false;
	}

	public override void AI()
	{
		if (++Projectile.frameCounter % 5 == 0 && ++Projectile.frame > 5)
			Projectile.frame = 0;

		if (Projectile.ai[0]++ > 40)
		{
			Projectile.velocity *= 0.9f;


			if ((int)Projectile.ai[0] == 120)
			{
				Projectile.velocity = Vector2.Zero;
				Projectile.ai[1] = Main.player[Projectile.owner].Center.X + Main.player[Projectile.owner].velocity.X * 3;
				Projectile.ai[2] = Main.player[Projectile.owner].Center.Y + Main.player[Projectile.owner].velocity.Y * 3;
				
				Vector2 target = new Vector2(Projectile.ai[1], Projectile.ai[2]);
				MPUtils.NewProjectile(null, Projectile.Center, Projectile.DirectionTo(target), ProjectileType<XTelegraphLine>(), 0, 0);
			}

			if (Projectile.ai[0] > 130)
				Projectile.scale *= 0.9f;
			
			if ((int)Projectile.ai[0] == 150)
			{	
				Vector2 target = new Vector2(Projectile.ai[1], Projectile.ai[2]);
				MPUtils.NewProjectile(null, Projectile.Center, Projectile.DirectionTo(target), ProjectileType<XLightningBolt>(), 20, 0, ai1: 1);
				
				for (int i = 0; i < 25; i++)
					Dust.NewDustPerfect(Projectile.Center, DustType<LineDustFollowPoint>(), Main.rand.NextVector2Circular(15, 15), newColor: Color.Violet with { A = 0 }, Scale: 0.1f);

				Projectile.Kill();
			}
		}
	}
}

public class VoidBlade : ModProjectile
{
	public override string Texture => Helper.AssetPath + "NPCs/Void/" + Name;

	public override void SetDefaults()
	{
		Projectile.CloneDefaults(ProjectileID.BulletDeadeye);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return base.PreDraw(ref lightColor);
	}

	public override void AI()
	{
		base.AI();
	}
}