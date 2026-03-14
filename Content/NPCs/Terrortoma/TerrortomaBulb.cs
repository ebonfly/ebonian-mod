using EbonianMod.Content.Items.Misc;
using EbonianMod.Content.Projectiles.Friendly.Corruption;
using ReLogic.Utilities;

namespace EbonianMod.Content.NPCs.Terrortoma;

public class TerrortomaBulb : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Terrortoma/"+Name;
    public override void SetStaticDefaults()
    {
        Main.npcFrameCount[Type] = 4;
        var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            Hide = true
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0 || Main.CurrentFrameFlags.AnyActiveBossNPC) return 0;
        return (spawnInfo.SpawnTileType == TileID.CorruptGrass && spawnInfo.Player.ZoneCorrupt && spawnInfo.Player.ZoneOverworldHeight && Main.hardMode && !NPC.AnyNPCs(Type) && !NPC.AnyNPCs(NPCType<Terrortoma>())) ? (GetInstance<DownedBossSystem>().downedTerrortoma ? 0.09f : 0.1f) : 0;
    }
    public override void SetDefaults()
    {
        NPC.Size = new Vector2(116, 104);
        NPC.damage = 0;
        NPC.defense = 3;
        NPC.lifeMax = 1000;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = Sounds.terrortomaFlesh;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0f;
        NPC.chaseable = false;
        NPC.rarity = 1;
    }
    public override void OnSpawn(IEntitySource source)
    {
        int atts = 0;
        Point p = NPC.Center.ToTileCoordinates();
        int i = (p.X > 0 && p.X < Main.maxTilesX) ? p.X : 0;
        int j = (p.Y > 0 && p.Y < Main.maxTilesY) ? p.Y : 0;

        while (Main.tile[i, j].HasTile && ++atts < 300)
            NPC.Center -= Vector2.UnitY * 16;
        NPC.Center = Helper.Raycast(NPC.Center, Vector2.UnitY, 1500).Point - new Vector2(0, 35);
    }
    public override void DrawBehind(int index)
    {
        NPC.behindTiles = true;
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (NPC.life <= 0)
        {
            MPUtils.NewNPC(NPC.Center + new Vector2(0, 40), NPCType<Terrortoma>(), true);
            MPUtils.NewProjectile(NPC.GetSource_Death(), NPC.Center, Vector2.Zero, ProjectileType<OstertagiExplosion>(), 0, 0, 0);
            for (int i = 0; i < 5; i++)
                MPUtils.NewProjectile(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, 3), ProjectileType<OstertagiWorm>(), 20, 0, 0);
            if (Main.dedServ)
                return;
            SoundEngine.PlaySound(Sounds.cecitiorDie, NPC.Center);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/Terrortoma2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/VileSlimeGore").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/VileSlimeGore2").Type, NPC.scale);
            for (int i = 0; i < 5; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/VileSlimeGore4").Type, NPC.scale);
            for (int i = 0; i < 3; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/EbonCrawlerGore2").Type, NPC.scale);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(5, 10), Find<ModGore>("EbonianMod/EbonCrawlerGore1").Type, NPC.scale);
        }
    }
    public override void FindFrame(int frameHeight)
    {
        if (NPC.frameCounter++ % 5 == 0)
        {
            if (NPC.frame.Y < frameHeight * 3)
                NPC.frame.Y += frameHeight;
            else
                NPC.frame.Y = 0;
        }
    }
    SlotId cachedSound;
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        spriteBatch.Draw(TextureAssets.Npc[Type].Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, NPC.scale, SpriteEffects.None, 0);
        return false;
    }
    public override void AI()
    {
        NPC.dontTakeDamage = true;
        foreach (Player player in Main.ActivePlayers)
        {
            if (player.Distance(NPC.Center) < 200)
            {
                if (player.whoAmI == Main.myPlayer && Main.MouseWorld.Distance(NPC.Center) < NPC.Size.Length() * 1.5f)
                {
                    NPC.dontTakeDamage = false;
                }
            }
        }
        NPC.chaseable = false;
        NPC.timeLeft = 10;
        NPC.despawnEncouraged = false;
        if (NPC.AnyNPCs(NPCType<Terrortoma>())) NPC.active = false;
        SoundStyle selected = Sounds.flesh0;
        switch (Main.rand.Next(3))
        {
            case 0:
                selected = Sounds.flesh1;
                break;
            case 1:
                selected = Sounds.flesh2;
                break;
        }
        if (!cachedSound.IsValid || !SoundEngine.TryGetActiveSound(cachedSound, out var activeSound) || !activeSound.IsPlaying)
        {
            cachedSound = SoundEngine.PlaySound(selected, NPC.Center);
        }
    }
}
