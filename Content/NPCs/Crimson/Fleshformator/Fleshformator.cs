using EbonianMod.Common.Players;
using EbonianMod.Core.Systems.Verlets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.GameContent.Bestiary;

namespace EbonianMod.Content.NPCs.Crimson.Fleshformator;

public class Fleshformator : ModNPC
{
    public override string Texture => Helper.AssetPath + "NPCs/Crimson/Fleshformator/"+Name;
    public override void SetDefaults()
    {
        NPC.width = 58;
        NPC.height = 42;
        NPC.damage = 10;
        NPC.defense = 5;
        NPC.lifeMax = 800;
        NPC.HitSound = SoundID.NPCHit1;
        NPC.DeathSound = Sounds.fleshHit;
        NPC.value = 500f;
        NPC.aiStyle = -1;
        NPC.noGravity = true;
        NPC.noTileCollide = true;
        NPC.knockBackResist = 0f;
        NPC.behindTiles = true;
        NPC.buffImmune[BuffID.Ichor] = true;

    }
    public override float SpawnChance(NPCSpawnInfo spawnInfo)
    {
        if (Main.invasionType > 0) return 0;
        return spawnInfo.Player.ZoneCrimson && spawnInfo.Player.ZoneRockLayerHeight && Main.hardMode ? 0.2f : 0;
    }
    public override void ModifyNPCLoot(NPCLoot npcLoot)
    {
        npcLoot.Add(ItemDropRule.Common(ItemID.Ichor, 2, 1, 4));
    }
    public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
    {
        bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[] {
            BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.Misc.Types.Evil"),
            new FlavorTextBestiaryInfoElement("Mods.EbonianMod.NPCs.Fleshformator.Bestiary"),
        });
    }
    Verlet[] verlet = new Verlet[3];
    Vector2[] endPos = new Vector2[3];
    Vector2[] ogEndPos = new Vector2[3];
    Vector2[] startPos = new Vector2[3];

    float controlTimer;
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(controlTimer);
        for (int i = 0; i < 3; i++)
        {
            writer.WriteVector2(endPos[i]);
            writer.WriteVector2(ogEndPos[i]);
            writer.WriteVector2(startPos[i]);
        }
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        controlTimer = reader.ReadSingle();
        for (int i = 0; i < 3; i++)
        {
            endPos[i] = reader.ReadVector2();
            ogEndPos[i] = reader.ReadVector2();
            startPos[i] = reader.ReadVector2();
        }
    }
    public override void HitEffect(NPC.HitInfo hit)
    {
        if (Main.dedServ)
            return;
        if (NPC.life <= 0)
        {
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/Crimorrhage1").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/CrimsonGoreChunk9").Type);
            Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/CrimsonGoreChunk2").Type);
            for (int i = 0; i < 3; i++)
                Gore.NewGore(NPC.GetSource_Death(), NPC.Center, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/Bone2").Type);


            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < verlet[i].segments.Count; j++)
                    Gore.NewGore(NPC.GetSource_Death(), verlet[i].segments[j].pointA.position, Main.rand.NextVector2Unit(), Find<ModGore>("EbonianMod/CecitiorChainGore").Type);
            }
        }
    }
    public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        if (endPos[0] == Vector2.Zero)
            return false;
        if (!NPC.IsABestiaryIconDummy)
        {
            if (verlet[0] is null)
            {
                for (int i = -1; i < 2; i++)
                    verlet[i + 1] = new Verlet(NPC.Center + new Vector2(21 * i, -5), 12, 13, NPC.ai[3], stiffness: 30);
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (!Main.gamePaused)
                        verlet[i].Update(startPos[i], endPos[i]);
                    verlet[i].Draw(spriteBatch, new VerletDrawData(new VerletTextureData(Helper.AssetPath+"NPCs/Crimson/Fleshformator/Fleshformator_Hook0", _endTex: Helper.AssetPath+"NPCs/Crimson/Fleshformator/Fleshformator_Hook1")));
                }
            }
        }
        return true;
    }
    public float AIState
    {
        get => NPC.ai[0];
        set => NPC.ai[0] = value;
    }
    public float AITimer
    {
        get => NPC.ai[1];
        set => NPC.ai[1] = value;
    }
    public float AITimer2
    {
        get => NPC.ai[2];
        set => NPC.ai[2] = value;
    }
    bool currentControl;
    float alpha;
    Vector2 offset;
    public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        Player player = Main.player[NPC.target];
        Texture2D tex = Assets.ExtraSprites.UI.arrow.Value;
        if (player.whoAmI == Main.myPlayer)
        {
            offset = Vector2.Lerp(offset, Vector2.Zero, 0.1f);
            if (AITimer2 > 50)
            {
                if (NPC.rotation == MathHelper.ToRadians(90))
                {
                    spriteBatch.Draw(tex, player.Center + new Vector2(-80, 0) - screenPos + offset, null, Color.White * alpha, -MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(tex, player.Center + new Vector2(80, 0) - screenPos + offset, null, Color.White * alpha, MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
                }
            }
            else if (AITimer2 < 50 && AITimer2 > 0)
            {
                if (NPC.rotation == MathHelper.ToRadians(90))
                {
                    spriteBatch.Draw(tex, player.Center + new Vector2(80, 0) - screenPos + offset, null, Color.White * alpha, MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
                }
                else
                {
                    spriteBatch.Draw(tex, player.Center + new Vector2(-80, 0) - screenPos + offset, null, Color.White * alpha, -MathHelper.PiOver2, tex.Size() / 2, 1, SpriteEffects.None, 0);
                }
            }
        }
    }
    public override void AI()
    {
        if (endPos[0] == Vector2.Zero && MPUtils.NotMPClient)
        {
            int rand = Main.rand.Next(4);
            int attempts = 0;
            while (attempts < 25 && NPC.ai[3] == 0)
            {
                switch (rand)
                {
                    case 0:
                        if (Helper.Raycast(NPC.Center, Vector2.UnitY, 1500).RayLength < 1490)
                        {
                            NPC.ai[3] = 0.5f;
                            NPC.Center = Helper.Raycast(NPC.Center, Vector2.UnitY, 1500).Point - new Vector2(0, 14);
                        }
                        rand = Main.rand.Next(4);
                        break;
                    case 1:
                        if (Helper.Raycast(NPC.Center, Vector2.UnitX, 1500).RayLength < 1490)
                        {
                            NPC.ai[3] = 0.5f;
                            NPC.rotation = MathHelper.ToRadians(-90);
                            NPC.Center = Helper.Raycast(NPC.Center, Vector2.UnitX, 1500).Point - new Vector2(14, 0);
                        }
                        rand = Main.rand.Next(4);
                        break;
                    case 2:
                        if (Helper.Raycast(NPC.Center, -Vector2.UnitX, 1500).RayLength < 1490)
                        {
                            NPC.ai[3] = 0.5f;
                            NPC.rotation = MathHelper.ToRadians(90);
                            NPC.Center = Helper.Raycast(NPC.Center, -Vector2.UnitX, 1500).Point + new Vector2(14, 0);
                        }
                        rand = Main.rand.Next(4);
                        break;
                    case 3:
                        if (Helper.Raycast(NPC.Center, -Vector2.UnitY, 1500).RayLength < 1490)
                        {
                            NPC.ai[3] = -0.5f;
                            NPC.rotation = MathHelper.ToRadians(180);
                            NPC.Center = Helper.Raycast(NPC.Center, -Vector2.UnitY, 1500).Point + new Vector2(0, 14);
                        }
                        rand = Main.rand.Next(4);
                        break;
                }
                attempts++;
            }
            if (MathF.Abs(NPC.ai[3]) < 0.1f)
            {
                NPC.active = false;
                return;
            }
            for (int i = -1; i < 2; i++)
            {
                Vector2 pos = NPC.Center + new Vector2(40 * i, Main.rand.NextFloat(-85, -100)).RotatedBy(NPC.rotation);
                endPos[i + 1] = pos;
                ogEndPos[i + 1] = pos;
                startPos[i + 1] = NPC.Center + new Vector2(15 * i, 0).RotatedBy(NPC.rotation);
            }
            NPC.netUpdate = true;
        }
        NPC.damage = 0;
        Player player = Main.player[NPC.target];
        if (AITimer2 <= -50)
            NPC.TargetClosest(false);
        for (int i = 0; i < 3; i++)
        {
            if (endPos[i].Distance(player.Center) < 10 && AITimer2 <= -50)
            {
                NPC.netUpdate = true;
                NPC.target = player.whoAmI;
                AITimer = i;
                alpha = 1;
                player.GetModPlayer<MiscPlayer>().fleshformators++;
                AITimer2 = 100;
                return;
            }

            if (endPos[i].Distance(startPos[i]) < 159 && player.Center.Distance(startPos[i] + NPC.Center.FromAToB(startPos[i]) * 50) < 150 && AITimer2 <= 0)
                endPos[i] += endPos[i].FromAToB(player.Center).RotatedByRandom(0.5f) * 0.75f;
            else
            {
                if (AITimer2 > 0 && (int)AITimer == i)
                {
                    if (player.GetModPlayer<MiscPlayer>().fleshformators > 0)
                        endPos[i] += endPos[i].FromAToB(NPC.Center - new Vector2(0, 31).RotatedBy(NPC.rotation), false).RotatedByRandom(0.5f) / 30;
                }
                else
                    endPos[i] += endPos[i].FromAToB(ogEndPos[i], false).RotatedByRandom(0.5f) / 30;
            }
            for (int j = 0; j < 3; j++)
            {
                if (j == i) continue;
                if (endPos[i].Distance(endPos[j]) < 20 && AITimer2 <= 0)
                    endPos[i] += endPos[i].FromAToB(ogEndPos[i], true);
            }
        }
        if (AITimer2 > 0)
        {
            if (AITimer2 > 50)
            {
                if (NPC.rotation == MathHelper.ToRadians(90))
                    currentControl = player.controlLeft;
                else
                    currentControl = player.controlRight;
            }
            if ((int)AITimer2 == 50)
                alpha = 1;
            if (AITimer2 < 50)
            {
                if (NPC.rotation == MathHelper.ToRadians(90))
                    currentControl = player.controlRight;
                else
                    currentControl = player.controlLeft;
            }
            if (!player.controlLeft && !player.controlRight)
                controlTimer--;

            if (controlTimer < -30 && AITimer2 > 10 && alpha > 0)
            {
                if (AITimer2 < 49)
                    AITimer2++;
                alpha = MathHelper.Clamp(alpha + 0.02f, 0, 1);
            }
            if (currentControl && controlTimer <= 0)
            {
                offset += Main.rand.NextVector2Unit() * Main.rand.NextFloat(2, 20);
                controlTimer = 2;
                AITimer2 -= 5;
                alpha -= 0.1f;
                NPC.netUpdate = true;
            }
            if (AITimer2 <= 10)
            {
                NPC.netUpdate = true;
                if (NPC.rotation == MathHelper.ToRadians(90))
                    player.velocity = Vector2.UnitX * 15;
                else
                    player.velocity = -Vector2.UnitX * 15;
                player.GetModPlayer<MiscPlayer>().fleshformators--;
                AITimer2 -= 10;
            }
            if (AITimer2 > 1)
            {
                player.velocity.Y = -1;
                player.velocity.X = 0;
                player.Center += player.Center.FromAToB(endPos[(int)AITimer], false) / 2f;
            }
            if (player.GetModPlayer<MiscPlayer>().fleshformators > 1)
            {
                if (player.Center.Distance(endPos[(int)AITimer]) < 10)
                    endPos[(int)AITimer] += endPos[(int)AITimer].FromAToB(NPC.Center - new Vector2(0, 31).RotatedBy(NPC.rotation)) * 1.75f * player.GetModPlayer<MiscPlayer>().fleshformators * Main.rand.NextFloat(0.7f, 3f);
                endPos[(int)AITimer] += endPos[(int)AITimer].FromAToB(player.Center) * 0.75f * player.GetModPlayer<MiscPlayer>().fleshformators;
            }

            player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 15, 0);
            if (player.whoAmI == Main.myPlayer)
                NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
        }
        else
        {
            foreach (Player p in Main.ActivePlayers)
            {
                p.GetModPlayer<MiscPlayer>().fleshformators--;
            }
            AITimer2--;
            alpha = 0;
            controlTimer = 0;
        }
    }
}
