using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Crimson;

namespace EbonianMod.Content.Items.Weapons.Magic;

public class CrimCannon : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/CrimCannon";
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Magic;
        Item.damage = 3;
        Item.useTime = 50;
        Item.shootSpeed = 1;
        Item.shoot = ProjectileType<CrimCannonProjectile>();
        Item.rare = ItemRarityID.Green;
        Item.useStyle = 5;
        Item.value = Item.buyPrice(0, 5, 0, 0);
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.channel = true;
        Item.mana = 25;
        Item.crit = 30;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.CrimtaneBar, 20).AddIngredient(ItemID.Vertebrae, 20).AddTile(TileID.Anvils).Register();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        velocity.Normalize();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, -20);
        return false;
    }
}

public class CrimCannonProjectile : HeldProjectileGun
{
    Vector2 Scale = new Vector2(0.4f, 0);
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/CrimCannonReload";
    public override bool? CanDamage() => false;
    public override void SetDefaults()
    {
        base.SetDefaults();
        ItemType = ItemType<CrimCannon>();
        RotationSpeed = 0.08f;
        Projectile.Size = new Vector2(56, 38);
        HoldOffset.X = 25;
    }
    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(50);
        Player player = Main.player[Projectile.owner];
        player.CheckMana(player.HeldItem.mana, true);

        if (player.whoAmI == Main.myPlayer)
            Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation();

        Projectile.frameCounter = (int)(-15 * AttackDelayMultiplier);
    }
    public override void AI()
    {
        base.AI();

        Player player = Main.player[Projectile.owner];

        if (!Main.player[Projectile.owner].channel || !player.CheckMana(player.HeldItem.mana)) Projectile.Kill();

        player.heldProj = Projectile.whoAmI;

        Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);

        if(Projectile.frameCounter++ > 4 * AttackDelayMultiplier)
        {
            Projectile.frame++;
            Projectile.frameCounter = 0;
        }
        if (Projectile.frame > 5)
        {
            Scale = new Vector2(0.65f, 1.6f);
            RecoilRotation = -0.15f * player.direction;

            player.CheckMana(player.HeldItem.mana, true, true);

            Vector2 shotPoint = Projectile.Center + Projectile.rotation.ToRotationVector2() * 5;
            if (Main.myPlayer == player.whoAmI)
            {
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), shotPoint, Projectile.rotation.ToRotationVector2() * 1.2f, ProjectileType<GoryJaw>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(shotPoint, DustID.Blood, (Projectile.rotation + Main.rand.NextFloat(PiOver2, PiOver4)).ToRotationVector2() * Main.rand.NextFloat(3, 8), Scale: 1.5f).noGravity = true;
                    Dust.NewDustPerfect(shotPoint, DustID.Blood, (Projectile.rotation + Main.rand.NextFloat(-PiOver2, -PiOver4)).ToRotationVector2() * Main.rand.NextFloat(3, 8), Scale: 1.5f).noGravity = true;
                }
            }

            SoundEngine.PlaySound(SoundID.NPCHit9.WithPitchOffset(Main.rand.NextFloat(-1f, -0.5f)), player.Center);

            Projectile.frame = 0;
            Projectile.frameCounter = (int)(-15 * AttackDelayMultiplier);
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), lightColor, Projectile.rotation, Projectile.Size / 2, Scale, Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}