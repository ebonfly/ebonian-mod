using EbonianMod.Content.Projectiles.Friendly.Generic;

namespace EbonianMod.Content.Items.Weapons.Magic
{
    public class WardingStar : ModItem
    {
        public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/WardingStar";
        public override void SetDefaults()
        {
            Item.damage = 19;
            Item.DamageType = DamageClass.Magic;
            Item.mana = 7;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.noUseGraphic = true;
            Item.knockBack = 4f;
            Item.width = 30;
            Item.height = 30;
            Item.useTime = 15;
            Item.useAnimation = 15;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.autoReuse = false;
            Item.channel = true;
            Item.value = Item.buyPrice(0, 0, 0, 1);
            Item.rare = ItemRarityID.Green;
            Item.shoot = ProjectileType<WardingStarP>();
        }
        public override bool CanShoot(Player player)
        {
            return player.ownedProjectileCounts[ProjectileType<WardingStarP>()] < 1 && player.statMana > 5;
        }
    }
}
