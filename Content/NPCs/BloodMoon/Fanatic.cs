using System;
using EbonianMod.Common.Globals;
using EbonianMod.Common.Misc;
using EbonianMod.Content.Dusts;
using EbonianMod.Content.Projectiles.Enemy.BloodMoon;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.BloodMoon;

public class Fanatic : CommonNPC
{
	public override string Texture => Helper.AssetPath + "NPCs/BloodMoon/" + Name;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[Type] = 17;
	}
	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (Main.invasionType > 0) return 0;
		return Main.bloodMoon && spawnInfo.Player.ZoneOverworldHeight ? 0.05f : 0;
	}
	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
			new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.BaphometWorship"),
			new FlavorTextBestiaryInfoElement(NPC.BestiaryKey()),
		});
	}

	public override void SetDefaults()
	{
		NPC.Size = new Vector2(38, 64);
		NPC.aiStyle = -1;
		NPC.damage = 30;
		NPC.defense = 4;
		NPC.lifeMax = 80;
		NPC.value = Item.buyPrice(0, 0, 15, 0);
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
	}

	public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier,
		ref Rectangle npcHitbox)
	{
		if ((States)AIState == States.Stab && AITimer is > 5 and < 20)
		{
			if (NPC.direction > 0)
				npcHitbox.X += npcHitbox.Width / 2;
			else
				npcHitbox.X -= npcHitbox.Width;
			
			npcHitbox.Width += npcHitbox.Width / 2;
		}
		return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = TextureAssets.Npc[Type].Value;
		Texture2D arm = Assets.NPCs.BloodMoon.Fanatic_Arm.Value;
		Texture2D head = Assets.NPCs.BloodMoon.Fanatic_Head.Value;
		
		Rectangle frame = new Rectangle(0, NPC.frame.Y, arm.Width, NPC.frame.Height);
		Vector2 offset = gfxOff + new Vector2(0, 2) - screenPos;
		spriteBatch.Draw(arm, NPC.Center + offset, frame, NPC.HunterPotionColor(drawColor), NPC.rotation, frame.Size() / 2 + new Vector2(20f * -NPC.spriteDirection, -1), NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		
		spriteBatch.Draw(tex, NPC.Center + offset, NPC.frame, NPC.HunterPotionColor(drawColor), NPC.rotation, NPC.Size / 2, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

		if (NPC.frame.Y > 66 * 14)
		{
			int frameY = NPC.frame.Y / 66 - 15;
			frame = new Rectangle(0, frameY * 34, head.Width, 34);
			Vector2 origin = frame.Size() * 0.5f + new Vector2(-2, 6);
			spriteBatch.Draw(head, NPC.Center + offset + new Vector2(-1 - 2* NPC.direction,3), frame, NPC.HunterPotionColor(drawColor), AITimer2, origin, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		return false;
	}
	
	public override bool CheckDead()
	{
		if (Main.dedServ)
			return base.CheckDead();
        
		if (Main.rand.NextBool(4))
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/FanaticHeadGoreAlt").Type, NPC.scale);
		else
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/FanaticHeadGore").Type, NPC.scale);

		for (int i = 0; i < 4; i++)
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/FanaticGore" + (i < 2 ? "1" : "2")).Type, NPC.scale);
		
		for (int i = 0; i < 4; i++)
			Gore.NewGore(NPC.GetSource_Death(), NPC.position, Main.rand.NextVector2Circular(1, 1), Find<ModGore>("EbonianMod/FanaticGore2").Type, NPC.scale);

		for (int i = 0; i < 50; i++)
			Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Blood, Main.rand.NextFloatDirection(), Main.rand.NextFloatDirection());
        
		return base.CheckDead();
	}

	enum States
	{
		Walk,
		Stab,
		FlameBreath,
		Worship,
		WorshipAggro
	}
	
	public override void AI()
	{
		Lighting.AddLight(NPC.Center, Color.Maroon.ToVector3()*0.6f);
		NPC.TargetClosest(false);

		Player player = Main.player[NPC.target];
		float distanceToPlayer = player.Distance(NPC.Center);
		switch ((States)AIState)
		{
			case States.Walk:
			{
				NPC.FaceTarget();
				NPC.damage = 0;
				
				if (MathF.Abs(player.Center.X - NPC.Center.X) > 40)
					NPC.GetGlobalNPC<FighterGlobalAI>().FighterAI(NPC, 5, 1);
				else 
					NPC.velocity.X *= 0;
				
				if (distanceToPlayer < 400)
					AITimer++;
				if (distanceToPlayer < 70)
					AITimer += 2;
				if (distanceToPlayer < 40)
					AITimer += 2;

				if (AITimer > 250 && distanceToPlayer < 200 && !player.dead) 
				{
					if (distanceToPlayer < 50)
					{
						NPC.velocity.X *= 0;
						SwitchState((int)States.Stab);
					}
					else if (distanceToPlayer > 70)
					{
						NPC.velocity.X *= 0;
						SwitchState((int)States.FlameBreath);
					}
					else
						AITimer = 100;
				}
			}
				break;

			case States.Stab:
			{
				AITimer++;
				if (AITimer > 22)
					NPC.damage = 0;
				else if (AITimer > 10)
					NPC.damage = 30;

				if ((int)AITimer == 15)
					SoundEngine.PlaySound(SoundID.Item1, NPC.Center);
				
				if (AITimer > 30) 
					SwitchState((int)States.Walk);
			} 
				break;

			case States.FlameBreath:
			{
				NPC.damage = 0;
				if (distanceToPlayer > 400)
					AITimer += 4;
				AITimer++;
				NPC.FaceTarget();
				float rotation = (player.Center - NPC.Center).ToRotation() + (NPC.direction == -1 ? Pi : 0);
				if (NPC.direction == 1)
					rotation = MathHelper.Clamp(rotation, -.5f, .5f);
				else
				{
					if (rotation < 5)
						rotation = MathHelper.Clamp(rotation, 0f, .5f);
					else
						rotation = MathHelper.Clamp(rotation, 6f, 6.5f);
				}

				AITimer2 = Utils.AngleLerp(AITimer2, rotation , 0.1f);

				if ((int)AITimer == 23)
					SoundEngine.PlaySound(Sounds.goat.WithPitchOffset(-1.5f).WithVolumeScale(0.7f), NPC.Center);
				
				if (AITimer > 30 && AITimer % 3 == 0)
				{
					Vector2 mouth = NPC.Center + new Vector2(0, 4);
					Vector2 vel = (player.Center - NPC.Center).SafeNormalize(Vector2.UnitY);
					float mult = 1f;
					if (AITimer > 200)
						mult = 1- (AITimer - 200) / 40f;
					for (int i = 0; i < 3; i++)
					{
						Dust.NewDustPerfect(mouth + Main.rand.NextVector2Circular(5, 2), DustType<GlowDust>(),
							vel.RotatedByRandom(0.4f) * Main.rand.NextFloat(2, 4)*mult, newColor: Color.Maroon, Scale:0.5f*mult);
					}

					if (AITimer > 55 && AITimer < 200)
					{
						if (AITimer % 12 == 0)
							SoundEngine.PlaySound(SoundID.Item34, NPC.Center);

						vel = vel.RotatedByRandom(0.35f) + new Vector2(0, 0.2f);
						MPUtils.NewProjectile(null, mouth, vel * Main.rand.NextFloat(4, 5),
							ProjectileType<FanaticFlame>(), 15,
							0);
					}
				}
				
				if (AITimer > 240) 
					SwitchState((int)States.Walk);
			}
				break;
		}

		NPC.spriteDirection = NPC.direction;
	}

	public override void FindFrame(int frameHeight)
	{
		if (MathF.Abs(NPC.velocity.Y) > 0.05f)
		{
			NPC.frame.Y = 4 * frameHeight;
			return; 
		}

		if (++NPC.frameCounter % 5 != 0)
			return;

		if (MathF.Abs(NPC.velocity.X) > 0.05f)
		{
			if ((NPC.frame.Y += frameHeight) > 3 * frameHeight)
				NPC.frame.Y = 0;
		}
		else if ((States)AIState == States.Stab)
		{
			if (NPC.frame.Y < 5 * frameHeight)
				NPC.frame.Y = 5 * frameHeight;

			if (NPC.frame.Y < 11 * frameHeight)
				NPC.frame.Y += frameHeight;
		}
		else if ((States)AIState == States.FlameBreath)
		{
			if (NPC.frame.Y < 11 * frameHeight)
				NPC.frame.Y = 11 * frameHeight;
			
			if ((NPC.frame.Y += frameHeight) > 16 * frameHeight)
				NPC.frame.Y = 15 * frameHeight;
		}
		else if ((States)AIState == States.Worship)
		{
			// idk
		}
		else NPC.frame.Y = 0;
	}
}