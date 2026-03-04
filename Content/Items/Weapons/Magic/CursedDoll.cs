using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Corruption;

namespace EbonianMod.Content.Items.Weapons.Magic;

internal class CursedDoll : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/CursedDoll";
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Magic;
        Item.damage = 270;
        Item.useTime = 75;
        Item.shoot = ProjectileType<CursedDollProjectile>();
        Item.shootSpeed = 1;
        Item.rare = ItemRarityID.Green;
        Item.useStyle = 1;
        Item.value = Item.buyPrice(0, 13, 50, 0);
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.channel = true;
        Item.mana = 25;
        Item.crit = 1;
    }
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.GuideVoodooDoll).AddIngredient(ItemType<CorruptEyeMaterial>(), 20).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        velocity = Vector2.Zero;
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
}
public class CursedDollProjectile : HeldProjectileGun
{
    Vector2 Scale = new Vector2(0, 0);
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/CursedDoll";
    public override bool? CanDamage() => false;
    public override void SetDefaults()
    {
        base.SetDefaults();
        ItemType = ItemType<CursedDoll>();
        RotationSpeed = 0.08f;
        Projectile.Size = new Vector2(56, 38);
        Projectile.penetrate = -1;
    }
    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(75);
        Player player = Main.player[Projectile.owner];
        player.CheckMana(-player.HeldItem.mana, true);
        Projectile.rotation = Helper.FromAToB(player.Center, Main.MouseWorld).ToRotation();
        Projectile.frame = 5;
        Projectile.netUpdate = true;
    }
    public override void AI()
    {
        base.AI();

        Player player = Main.player[Projectile.owner];

        player.heldProj = Projectile.whoAmI;

        if (!Main.player[Projectile.owner].channel || !player.CheckMana(player.HeldItem.mana)) Projectile.Kill();

        Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);

        if (Projectile.ai[0]++ >= 75 * AttackDelayMultiplier)
        {
            Scale = new Vector2(0.65f, 1.6f);
            SoundEngine.PlaySound(SoundID.NPCHit28.WithPitchOffset(Main.rand.NextFloat(0.2f, 0.5f)) with { Volume = 0.1f }, player.MountedCenter);
            SoundEngine.PlaySound(SoundID.NPCHit4.WithPitchOffset(Main.rand.NextFloat(-0.5f, -0.3f)) with { Volume = 0.12f }, player.MountedCenter);
            player.CheckMana(player.HeldItem.mana, true, true);

            if (Main.myPlayer == player.whoAmI)
            {
                Vector2 mousePosition = Main.MouseWorld;
                Vector2 spawnPosition = player.MountedCenter + new Vector2(20, -10 * player.direction).RotatedBy(Projectile.rotation);
                for (int i = 0; i < 25; i++)
                {
                    Dust.NewDustPerfect(mousePosition, DustID.CursedTorch, Main.rand.NextFloat(0, Pi * 2).ToRotationVector2() * Main.rand.NextFloat(0.3f, 6), Scale: Main.rand.NextFloat(1.2f, 1.9f)).noGravity = true;
                    Dust.NewDustPerfect(spawnPosition, DustID.CursedTorch, Main.rand.NextFloat(0, Pi * 2).ToRotationVector2() * Main.rand.NextFloat(2, 6), Scale: 1.5f).noGravity = true;
                }
                Projectile.NewProjectile(Projectile.InheritSource(Projectile), mousePosition, Vector2.Zero, ProjectileType<DollPin>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                Projectile.ai[0] = 0;
            }
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Main.player[Projectile.owner];
        Main.EntitySpriteDraw(Helper.GetTexture(Texture).Value, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, new Vector2(Projectile.width / 2 - 30, Projectile.height / 2 + 10 * player.direction), Scale, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}
