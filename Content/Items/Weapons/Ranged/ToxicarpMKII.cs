using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Friendly.Corruption;

namespace EbonianMod.Content.Items.Weapons.Ranged;

public class ToxicarpMKII : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/"+Name;
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 20;
        Item.useTime = 2;
        Item.useAnimation = 30;
        Item.shoot = ProjectileType<TinyFish>();
        Item.shootSpeed = 8f;
        Item.value = Item.buyPrice(0, 30, 0, 0);
        Item.rare = ItemRarityID.LightRed;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.UseSound = SoundID.Item11;
        Item.useAmmo = AmmoID.Bullet;
        Item.autoReuse = true;
    }
    public override Vector2? HoldoutOffset()
    {
        return new Vector2(-10, 0);
    }
    public override bool CanConsumeAmmo(Item ammo, Player player)
    {
        return (player.itemAnimation > 21);
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.Ebonkoi).AddIngredient(ItemType<CorruptEyeMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        position += new Vector2(16, -5).RotatedBy(velocity.ToRotation()) * player.direction;
        if (player.itemAnimation > 21)
            Projectile.NewProjectile(source, position, velocity.RotatedByRandom(MathHelper.ToRadians(10)), ProjectileType<TinyFish>(), damage, knockback, player.whoAmI);
        return false;
    }
}
