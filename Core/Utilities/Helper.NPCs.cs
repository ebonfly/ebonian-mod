namespace EbonianMod.Core.Utilities;

public static partial class Helper
{
	public static string BestiaryKey(this NPC npc) => "Mods.EbonianMod.NPCs." + npc.ModNPC.Name + ".Bestiary";
	public static Color HunterPotionColor(this NPC npc, Color defaultColor)
	{
		return Main.LocalPlayer.HasBuff(BuffID.Hunter) && !npc.IsABestiaryIconDummy ? HunterPotionColor(npc) : defaultColor;
	}
	public static Color HunterPotionColor(this NPC npc)
	{
		return Color.Lerp(Color.OrangeRed * 0.5f, Color.Transparent, Clamp(Utils.GetLerpValue(npc.Size.Length(), 0, Main.LocalPlayer.Distance(npc.Center)), 0, 1));
	}
	/// <summary>
	/// Laggy grounded check, only use this for stuff like death animations where you absolutely dont want the npc to not be able to detect the ground
	/// </summary>
	public static bool Grounded(this Entity entity, float offset = .5f, float offsetX = 1f, bool skipBasicCollideCheck = false)
	{
		if (entity is NPC npc && !skipBasicCollideCheck && npc.collideY && !npc.noTileCollide)
			return true;
		
		if ((!Collision.CanHitLine(new Vector2(entity.Center.X, entity.Center.Y), 1, 1
			     , new Vector2(entity.Center.X, entity.Center.Y + entity.height * offset), 1, 1)
		     || Collision.FindCollisionDirection(out int dir, entity.Center, 1, entity.height / 2)))
			return true;
		for (int i = 0; i < entity.width * offsetX; i += (int)(1 / (offsetX == 0 ? 1 : offsetX))) //full sprite check
		{
			bool isGrounded = Raycast(entity.BottomLeft + Vector2.UnitX * i, Vector2.UnitY, 2).Success;
			if (isGrounded)
				return true;
		}
		return false; //give up
	}
	public static void SpawnGore(this NPC NPC, string gore, int amount = 1, int type = -1, Vector2 vel = default, float scale = 1f)
	{
		if (Main.dedServ) return;

		var position = NPC.Center;
		if (type != -1)
		{
			gore += type;
		}
		for (int i = 0; i < amount; i++)
		{
			Gore.NewGore(NPC.GetSource_OnHit(NPC), position + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), vel, Find<ModGore>(gore).Type, scale);
		}
	}
	public static void SpawnGore(this NPC NPC, int gore, int amount = 1, Vector2 vel = default, float scale = 1f)
	{
		var position = NPC.Center;
		for (int i = 0; i < amount; i++)
		{
			Gore.NewGore(NPC.GetSource_OnHit(NPC), position + new Vector2(Main.rand.Next(-20, 20), Main.rand.Next(-20, 20)), vel, gore, scale);
		}
	}
}