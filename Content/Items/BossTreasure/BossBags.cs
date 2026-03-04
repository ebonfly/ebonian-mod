using EbonianMod.Content.Items.Accessories;
using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Items.Misc;
using EbonianMod.Content.Items.Weapons.Magic;
using EbonianMod.Content.Items.Weapons.Melee;
using EbonianMod.Content.Items.Weapons.Ranged;
using EbonianMod.Content.Items.Weapons.Summoner;
using EbonianMod.Content.NPCs.ArchmageX;
using EbonianMod.Content.NPCs.Cecitior;
using EbonianMod.Content.NPCs.Garbage;
using EbonianMod.Content.NPCs.Terrortoma;
using System.Collections.Generic;

namespace EbonianMod.Content.Items.BossTreasure;

public abstract class BossBags : ModItem
{
    public override void SetStaticDefaults()
    {
        ItemID.Sets.BossBag[Type] = true;

        Item.ResearchUnlockCount = 3;
    }

    public override void SetDefaults()
    {
        Item.maxStack = Item.CommonMaxStack;
        Item.consumable = true;
        Item.width = 16;
        Item.height = 16;
        Item.rare = ItemRarityID.Purple;
        Item.expert = true;
    }

    public override bool CanRightClick()
    {
        return true;
    }
}
public class CecitiorBag : BossBags
{
    public override string Texture => Helper.AssetPath + "Items/BossTreasure/CecitiorBag";
    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        itemLoot.Add(ItemDropRule.Common(ItemType<CecitiorMaterial>(), 1, 40, 60));
        itemLoot.Add(ItemDropRule.Common(ItemType<CecitiorClawMaterial>(), 1, 10, 15));
        itemLoot.Add(ItemDropRule.Common(ItemType<BrainAcc>(), 1));
        itemLoot.Add(ItemDropRule.Common(ItemType<SelfStab>(), 1));
        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<Cecitior>()));
    }
}
public class TerrortomaBag : BossBags
{
    public override string Texture => Helper.AssetPath + "Items/BossTreasure/TerrortomaBag";
    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        itemLoot.Add(ItemDropRule.Common(ItemType<CorruptEyeMaterial>(), 1, 40, 60));
        itemLoot.Add(ItemDropRule.Common(ItemType<TerrortomaMaterial>(), 1, 10, 15));
        itemLoot.Add(ItemDropRule.Common(ItemType<EbonianHeart>(), 1));
        itemLoot.Add(ItemDropRule.Common(ItemType<Ostertagi>(), 1));
        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<Terrortoma>()));
    }
}

public class GarbageBagI : BossBags
{
    public override string Texture => Helper.AssetPath + "Items/BossTreasure/GarbageBagI";
    public override void SetStaticDefaults()
    {
        ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        ItemID.Sets.BossBag[Type] = true;
    }
    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        itemLoot.Add(ItemDropRule.Common(ItemType<HotShield>(), 1));
        itemLoot.Add(ItemDropRule.Common(ItemType<PipebombI>(), 1, 20, 100));
        List<int> drops = new List<int> {
        ItemType<Chainsword>(),
        ItemType<DoomsdayRemote>(),
        ItemType<MailboxStaff>(),
        ItemType<SalvagedThruster>(),
        ItemType<GarbageFlail>(),
        ItemType<NastySnapper>(),
        };
        itemLoot.Add(new FromOptionsWithoutRepeatsDropRule(3, drops.ToArray()));
        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<HotGarbage>()));
    }
}

public class ArchmageBag : BossBags
{
    public override string Texture => Helper.AssetPath + "Items/BossTreasure/ArchmageBag";
    public override void SetStaticDefaults()
    {
        ItemID.Sets.PreHardmodeLikeBossBag[Type] = true;
        ItemID.Sets.BossBag[Type] = true;
    }
    public override void ModifyItemLoot(ItemLoot itemLoot)
    {
        itemLoot.Add(ItemDropRule.Common(ItemType<XTentacleAcc>(), 1));
        List<int> drops = new List<int>
        {
            ItemType<ArchmageXTome>(),
            ItemType<StaffofXWeapon>(),
            ItemType<XareusPotion>(),
            ItemType<PhantasmalGreatsword>(),
        };
        itemLoot.Add(new FromOptionsWithoutRepeatsDropRule(2, drops.ToArray()));

        itemLoot.Add(ItemDropRule.CoinsBasedOnNPCValue(NPCType<ArchmageX>()));
    }
}
