using System;
using System.Collections.Generic;
using System.CommandLine.Help;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.Bestiary;
using Terraria.ModLoader.Utilities;

namespace EbonianMod.Content.NPCs.Underground;
public class Hookworm : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Underground/Hookworm/"+Name;
    public override void SetStaticDefaults()
    {
        NPCID.Sets.TrailCacheLength[Type] = 18;
        NPCID.Sets.TrailingMode[Type] = 3;

        NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
        {
            CustomTexturePath = Helper.AssetPath+"NPCs/Underground/Hookworm/Hookworm_Bestiary",
            PortraitPositionXOverride = -50,
            Direction = 1,
            Position = new Vector2(-90, 0)
        };
        NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, value);
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
        {
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Caverns,
                new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.GiantBeast"),
                new FlavorTextBestiaryInfoElement(NPC.BestiaryKey())
        });
    }
    public override void SetDefaults()
    {
        NPC.Size = new Vector2(28, 26);
        NPC.lifeMax = 60;
        NPC.defense = 10;
        NPC.damage = 40;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.value = Item.buyPrice(0, 10);
        NPC.knockBackResist = 0.1f;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = SoundID.NPCDeath1;
    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        return spawnInfo.Player.ZoneNormalCaverns && NPC.downedBoss2 ? 0.008f : 0;
    }

    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.WhoopieCushion, 45));
    }

    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Texture2D tex = TextureAssets.Npc[Type].Value;
        Texture2D backTex = Assets.NPCs.Underground.Hookworm.Hookworm_Back.Value;
        Texture2D fang = Assets.NPCs.Underground.Hookworm.Hookworm_Fang.Value;
        Texture2D mandible = Assets.NPCs.Underground.Hookworm.Hookworm_Mandible.Value;

        Texture2D[] segment = [Assets.NPCs.Underground.Hookworm.HookwormSegment0.Value, Assets.NPCs.Underground.Hookworm.HookwormSegment1.Value, Assets.NPCs.Underground.Hookworm.HookwormSegment2.Value, Assets.NPCs.Underground.Hookworm.HookwormSegment3.Value];
        Vector2 pos = NPC.Center;
        for (int i = 0; i < 4; i++)
        {
            Vector2 oldPos = pos;
            float width = tex.Width * 0.7f;
            if (i > 0) width = segment[i - 1].Width - 2;
            pos += new Vector2(-width, 0).RotatedBy(NPC.oldRot[2 + i * 3]);
            Main.EntitySpriteDraw(segment[i], pos - screenPos, null, drawColor, Helper.FromAToB(oldPos, pos).ToRotation(), segment[i].Size() / 2, NPC.scale, SpriteEffects.FlipHorizontally);
        }

        Main.EntitySpriteDraw(backTex, NPC.Center - screenPos, null, drawColor, NPC.rotation, backTex.Size() / 2, NPC.scale, SpriteEffects.None);

        for (int i = 0; i < 2; i++)
        {
            int dir = i == 1 ? -1 : 1;
            Main.EntitySpriteDraw(fang, NPC.Center - screenPos, null, drawColor, NPC.rotation + fangRotation * dir, new Vector2(-6, fang.Height * i - dir * 2), NPC.scale, i == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
            Main.EntitySpriteDraw(mandible, NPC.Center - screenPos, null, drawColor, NPC.rotation + mandibleRotation * dir, new Vector2(-2, fang.Height * -dir - dir * 8 + fang.Height + 6), NPC.scale, i == 1 ? SpriteEffects.FlipVertically : SpriteEffects.None);
        }
        Main.EntitySpriteDraw(tex, NPC.Center - screenPos, null, drawColor, NPC.rotation, tex.Size() / 2, NPC.scale, SpriteEffects.None);
        return false;
    }
    float fangRotation, mandibleRotation;
    public override bool ModifyCollisionData(Rectangle victimHitbox, ref int immunityCooldownSlot, ref MultipliableFloat damageMultiplier, ref Rectangle npcHitbox)
    {
        if (NPC.ai[1] > 15)
        {
            Vector2 _pos = NPC.Center + NPC.rotation.ToRotationVector2() * 36 - new Vector2(24, 24);
            int x = (int)_pos.X;
            int y = (int)_pos.Y;
            int width = 48;
            int height = 48;
            npcHitbox = new Rectangle(x, y, width, height);
        }
        return base.ModifyCollisionData(victimHitbox, ref immunityCooldownSlot, ref damageMultiplier, ref npcHitbox);
    }
    public override void AI()
    {
        Player player = Main.player[NPC.target];
        NPC.TargetClosest(false);
        float speed = 10f;
        float acceleration = 0.1f;

        NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.2f);

        //ripped example worm code
        int minTilePosX = (int)(NPC.Left.X / 16) - 1;
        int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
        int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
        int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

        if (minTilePosX < 0)
            minTilePosX = 0;
        if (maxTilePosX > Main.maxTilesX)
            maxTilePosX = Main.maxTilesX;
        if (minTilePosY < 0)
            minTilePosY = 0;
        if (maxTilePosY > Main.maxTilesY)
            maxTilePosY = Main.maxTilesY;

        bool collision = false;

        for (int i = minTilePosX; i < maxTilePosX; ++i)
        {
            for (int j = minTilePosY; j < maxTilePosY; ++j)
            {
                Tile tile = Main.tile[i, j];

                if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64)
                {
                    Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

                    if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16)
                    {
                        collision = true;

                        if (Main.rand.NextBool(100))
                            WorldGen.KillTile(i, j, fail: true, effectOnly: true, noItem: false);
                    }
                }
            }
        }

        float dirX = NPC.FromAToB(player).X;
        float dirY = NPC.FromAToB(player).Y;
        if (!collision)
        {
            NPC.velocity.Y += 0.11f;
            if (NPC.velocity.Y > speed)
                NPC.velocity.Y = speed;
            if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f)
            {
                if (NPC.velocity.X < 0.0f)
                    NPC.velocity.X -= acceleration * 1.1f;
                else
                    NPC.velocity.X += acceleration * 1.1f;
            }
            else if (NPC.velocity.Y == speed)
            {
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration;
            }
            else if (NPC.velocity.Y > 4)
            {
                if (NPC.velocity.X < 0)
                    NPC.velocity.X += acceleration * 0.9f;
                else
                    NPC.velocity.X -= acceleration * 0.9f;
            }
        }
        else
        {
            speed = 5f;
            if (NPC.velocity.Length() > speed)
                NPC.velocity *= 0.9f;
            float absDirX = Math.Abs(dirX);
            float absDirY = Math.Abs(dirY);
            float newSpeed = speed / (float)Math.Sqrt(dirX * dirX + dirY * dirY);
            dirX *= newSpeed;
            dirY *= newSpeed;

            if ((NPC.velocity.X > 0 && dirX > 0) || (NPC.velocity.X < 0 && dirX < 0) || (NPC.velocity.Y > 0 && dirY > 0) || (NPC.velocity.Y < 0 && dirY < 0))
            {
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration;

                if (NPC.velocity.Y < dirY)
                    NPC.velocity.Y += acceleration;
                else if (NPC.velocity.Y > dirY)
                    NPC.velocity.Y -= acceleration;

                if (Math.Abs(dirY) < speed * 0.2 && ((NPC.velocity.X > 0 && dirX < 0) || (NPC.velocity.X < 0 && dirX > 0)))
                {
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y += acceleration * 2f;
                    else
                        NPC.velocity.Y -= acceleration * 2f;
                }

                if (Math.Abs(dirX) < speed * 0.2 && ((NPC.velocity.Y > 0 && dirY < 0) || (NPC.velocity.Y < 0 && dirY > 0)))
                {
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
                    else
                        NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
                }
            }
            else if (absDirX > absDirY)
            {
                if (NPC.velocity.X < dirX)
                    NPC.velocity.X += acceleration * 1.1f;
                else if (NPC.velocity.X > dirX)
                    NPC.velocity.X -= acceleration * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                {
                    if (NPC.velocity.Y > 0)
                        NPC.velocity.Y += acceleration;
                    else
                        NPC.velocity.Y -= acceleration;
                }
            }
            else
            {
                if (NPC.velocity.Y < dirY)
                    NPC.velocity.Y += acceleration * 1.1f;
                else if (NPC.velocity.Y > dirY)
                    NPC.velocity.Y -= acceleration * 1.1f;

                if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5)
                {
                    if (NPC.velocity.X > 0)
                        NPC.velocity.X += acceleration;
                    else
                        NPC.velocity.X -= acceleration;
                }
            }
        }
        // ripped example worm code ends here

        if (NPC.ai[1] < 0 && collision)
            NPC.ai[1]++;
        if (player.Distance(NPC.Center) < 200 && player.Distance(NPC.Center) > 50 && NPC.ai[1] == 0)
        {
            NPC.velocity = NPC.FromAToB(player) * 10;
            NPC.ai[1]++;
        }
        if (NPC.ai[1] > 0)
        {
            NPC.ai[1]++;
            if (NPC.ai[1] < 15)
            {
                mandibleRotation = Utils.AngleLerp(mandibleRotation, 0.8f, 0.1f);
            }
            else if (NPC.ai[1] == 15)
                SoundEngine.PlaySound(Sounds.chomp0, NPC.Center);
            else if (NPC.ai[1] < 40)
            {
                mandibleRotation = Utils.AngleLerp(mandibleRotation, -0.25f, 0.3f);
            }
            else NPC.ai[1] = Main.rand.Next(-300, -100);
        }
        else
        {
            mandibleRotation = Utils.AngleLerp(mandibleRotation, MathF.Sin(Main.GlobalTimeWrappedHourly) * 0.1f, 0.3f);
            fangRotation = Utils.AngleLerp(fangRotation, MathF.Sin(Main.GlobalTimeWrappedHourly * 2) * 0.1f, 0.3f);
        }
    }
    public override bool CheckDead()
    {
        if (Main.dedServ)
            return true;
        for (int i = 0; i < 4;i++)
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(7,7), Find<ModGore>("EbonianMod/Hookworm" + i).Type, NPC.scale);
        
        for (int j = 0; j < 2;j++)
        for (int i = 4; i < 7;i++)
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Circular(7,7), Find<ModGore>("EbonianMod/Hookworm" + i).Type, NPC.scale);
        return true;
    }
}