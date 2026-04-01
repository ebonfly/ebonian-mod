using System;
using EbonianMod.Common.Misc;
using EbonianMod.Content.Projectiles.ArchmageX;
using EbonianMod.Content.Projectiles.Cecitior;
using EbonianMod.Content.Projectiles.Conglomerate;
using ReLogic.Utilities;

namespace EbonianMod.Content.NPCs.Void;

[AutoloadBossHead]
public class RAVAGER : CommonNPC
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
		NPC.Size = new Vector2(408, 445);
		NPC.lifeMax = ushort.MaxValue / 15;
		NPC.defense = 30;
		NPC.boss = true;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.HitSound = SoundID.DD2_BetsyHurt with { Volume = 2, Pitch = -3 };

		NPC.DeathSound = SoundID.ForceRoar;
		
		NPC.knockBackResist = 0f;
		NPC.aiStyle = -1;
	}
	
	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = TextureAssets.Npc[Type].Value;
		Texture2D bloom = Assets.Extras.explosion.Value;
		Texture2D vortex = Assets.NPCs.Void.RavagedPortal.Value;

		for (int i = 0; i < 5f; i++)
			spriteBatch.Draw(bloom, NPC.Center + Main.rand.NextVector2Circular(50, 50) - screenPos, null, Color.Black * 0.15f, Main.GlobalTimeWrappedHourly, bloom.Size() / 2f, 15 + MathF.Sin(Main.GlobalTimeWrappedHourly * 0.1f) * 2, SpriteEffects.None, 0);
		
		for (int j = NPC.oldPos.Length - 1; j >= 0; j--)
		{
			float alpha = 1f - j / (float)NPC.oldPos.Length;
			
			for (int i = 0; i < 5f; i++)
				spriteBatch.Draw(vortex, NPC.oldPos[j] + NPC.Size / 2f + Main.rand.NextVector2Circular(50, 50) - screenPos, null, Main.hslToRgb(alpha, 1, 0.8f) with { A = 0 } * 0.02f, 5 * Main.GlobalTimeWrappedHourly + j, vortex.Size() / 2f, 3 * alpha, SpriteEffects.None, 0);
		}
		
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
			}
		}

		return false;
	}

	private SlotId cachedSound;
	void Ambience()
	{
		if (!Main.dedServ)
		{
			if (SoundEngine.TryGetActiveSound(cachedSound, out var _activeSound))
			{
				_activeSound.Pitch = MathHelper.Clamp(Lerp(-2, 0, NPC.velocity.Length() / 20), -1, 1);
				_activeSound.Position = NPC.Center;
			}
			else
			{
				cachedSound = SoundEngine.PlaySound(Sounds.CeaselessIdle.WithVolumeScale(0.35f), NPC.Center, (_) => NPC.AnyNPCs(Type));
			}
		}
	}
	
	public override void AI()
	{
		if (!Main.dedServ)
		{
			Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/conglomerate");
		}
		
		Ambience();

		Player player = Main.player[NPC.target];
		NPC.TargetClosest(false);
		
		AITimer++;
		NPC.rotation = NPC.velocity.X * 0.01f;
		
		NPC.velocity = (player.Center - NPC.Center) * 0.1f;

		Vector2 pos = NPC.Center + Main.rand.NextVector2Circular(1500, 1500);
		if (AITimer % 15 == 0)
		{
			SoundEngine.PlaySound(Aureus.Aureus.hurtSound with { MaxInstances = -1, Volume = 5 }, NPC.Center);
			MPUtils.NewProjectile(null, pos, (player.Center - pos).SafeNormalize(Vector2.UnitX),
				ModContent.ProjectileType<VoidBlade>(), 30, 0);
		}

		if (AITimer % 200 == 0)
		{
			SoundEngine.PlaySound(Sounds.chargedBeamImpactOnly, NPC.Center);
			MPUtils.NewProjectile(null, NPC.Center, Vector2.UnitY.RotatedByRandom(0.1f),
				ModContent.ProjectileType<CBeam>(), 30, 0, ai2: NPC.whoAmI);
		}
	}
}
public class VoidSpawnItem : ModItem
{
	public override string Texture => Helper.AssetPath + "NPCs/Void/"+Name;
	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 20;
		Item.maxStack = 1;
		Item.value = 1000;
		Item.rare = ItemRarityID.Blue;
		Item.useAnimation = 30;
		Item.useTime = 30;
		Item.noUseGraphic = true;
		Item.useStyle = ItemUseStyleID.HoldUp;
		Item.consumable = false;
		Item.useTurn = false;
	}
	public override bool? UseItem(Player player)
	{
		NPC.SpawnBoss((int)player.Center.X, (int)player.Center.Y - 900, NPCType<Ceaseless>(), player.whoAmI);
		return true;
	}
}