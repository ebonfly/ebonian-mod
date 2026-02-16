using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Crimson;
using Terraria.GameContent;

namespace EbonianMod.Content.Items.Weapons.Magic;

public class Latcher : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/Latcher";
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Magic;
        Item.damage = 80;
        Item.useTime = 50;
        Item.shoot = ProjectileType<LatcherProjectile>();
        Item.rare = ItemRarityID.Green;
        Item.shootSpeed = 1f;
        Item.useStyle = 5;
        Item.value = Item.buyPrice(0, 5, 0, 0);
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.channel = true;
        Item.mana = 50;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.Vertebrae, 20).AddIngredient(ItemID.Hook).AddTile(TileID.Anvils).Register();
    }
    public override bool CanUseItem(Player player)
    {
        return player.ownedProjectileCounts[ProjectileType<LatcherTongue>()] < 1;
    }
    public override bool? CanAutoReuseItem(Player player) => false;

    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        velocity.Normalize();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
}
public class LatcherProjectile : HeldProjectileGun
{
    Vector2 Scale = new Vector2(0, 0);
    Projectile ChildProjectile;
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/Latcher";
    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(50);
        Player player = Main.player[Projectile.owner];
        Projectile.rotation = Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation() + player.direction * Pi;
    }
    public override bool? CanDamage() => false;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.Size = new Vector2(60, 38);
        ItemType = ItemType<Latcher>();
        HoldOffset = new Vector2(25, -4);
        AimingOffset = 2;
    }
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        if (player.ownedProjectileCounts[ProjectileType<LatcherTongue>()] < 1 && Projectile.ai[1] == 1) Projectile.Kill();

        Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);

        base.AI();

        Projectile.ai[0]++;
        if (Projectile.ai[1] == 1)
        {
            RotationSpeed = 0.03f;
            Vector2 position = Projectile.Center + new Vector2(14, 4 * Projectile.direction).RotatedBy(Projectile.rotation);
            ChildProjectile.ai[0] = position.X;
            ChildProjectile.ai[1] = position.Y;
        }
        else
        {
            RotationSpeed = Min(0.1f * AttackSpeedMultiplier, 1);
            if (player.whoAmI == Main.myPlayer && !player.channel && Projectile.ai[0] > 45 * AttackDelayMultiplier)
            {
                Projectile.ai[1] = 1;
                Scale = new Vector2(0.65f, 1.2f);

                if (Helper.Raycast(player.Center, Projectile.rotation.ToRotationVector2(), 96).Success) 
                    Projectile.Kill();
                else 
                    ChildProjectile = Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), Projectile.Center, Projectile.rotation.ToRotationVector2() * 27, ProjectileType<LatcherTongue>(), 1, Projectile.knockBack, Projectile.owner, ai2: Projectile.rotation);

                SoundEngine.PlaySound(SoundID.NPCHit8.WithPitchOffset(Main.rand.NextFloat(-0.4f, 0.4f)), player.Center);
            }
        }
    }
    public override void OnKill(int timeLeft)
    {
        for (int i = 0; i < 20; i++) Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Blood, Scale: 1.5f);
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, Projectile.Size / 2, Scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}
