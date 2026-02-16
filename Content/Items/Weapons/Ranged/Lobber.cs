using EbonianMod.Content.Items.Materials;
using EbonianMod.Content.Projectiles.Bases;
using EbonianMod.Content.Projectiles.Friendly.Corruption;
using EbonianMod.Content.Projectiles.Friendly.Crimson;

namespace EbonianMod.Content.Items.Weapons.Ranged;

public class Lobber : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/Lobber";
    public override void SetDefaults()
    {
        Item.damage = 80;
        Item.value = Item.buyPrice(0, 37, 0, 0);
        Item.useTime = 20;
        Item.DamageType = DamageClass.Ranged;
        Item.useStyle = 1;
        Item.knockBack = 10;
        Item.value = 1000;
        Item.rare = ItemRarityID.Red;
        Item.shootSpeed = 1;
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.channel = true;
        Item.noMelee = true;
        Item.useAmmo = AmmoID.Rocket;
        Item.crit = 25;
    }
    public override bool AltFunctionUse(Player player)
    {
        return true;
    }
    public override bool CanUseItem(Player player)
    {
        Item.shoot = player.altFunctionUse == 2 ? ProjectileType<LobberProjectile2>() : ProjectileType<LobberProjectile>();
        return base.CanUseItem(player);
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        velocity.Normalize();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
    public override bool? CanAutoReuseItem(Player player) => false;
    public override void AddRecipes()
    {
        CreateRecipe().AddIngredient(ItemID.GrenadeLauncher).AddIngredient(ItemType<CecitiorMaterial>(), 10).AddIngredient(ItemType<TerrortomaMaterial>(), 10).AddTile(TileID.MythrilAnvil).Register();
    }
    public override bool CanConsumeAmmo(Item ammo, Player player) => false;
}


public class LobberProjectile : HeldProjectileGun
{
    Vector2 Scale = new Vector2(0, 0);
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/LobberAttack";
    public override bool? CanDamage() => false;
    public override void SetDefaults()
    {
        base.SetDefaults();
        ItemType = ItemType<Lobber>();
        RotationSpeed = 0.2f;
        AimingOffset = 25;
        Projectile.Size = new Vector2(56, 48);
        Projectile.frame = 1;
    }
    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(20);
        Player player = Main.player[Projectile.owner];
        Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation() + player.direction * PiOver2;
        Projectile.frameCounter = (int)(-10 * AttackDelayMultiplier);
    }

    public override void AI()
    {
        base.AI();

        Player player = Main.player[Projectile.owner];

        player.heldProj = Projectile.whoAmI;

        if (Projectile.frameCounter++ > (int)(5 * AttackDelayMultiplier))
        {
            if (Projectile.frame == 1)
            {
                Scale = new Vector2(0.65f, 1.6f);
                if (player.whoAmI == Main.myPlayer)
                {
                    RecoilRotation = -0.2f * player.direction;
                    Vector2 spawnPosition = Projectile.Center + new Vector2(30, -25 * player.direction).RotatedBy(Projectile.rotation);
                    if (player.whoAmI == Main.myPlayer)
                        Projectile.NewProjectileDirect(Projectile.InheritSource(Projectile), spawnPosition, Projectile.rotation.ToRotationVector2() * 16, ProjectileType<CrimsonBall>(), (int)(Projectile.damage * 0.8f), Projectile.knockBack, Projectile.owner);
                    for (int i = 0; i < 14; i++)
                        Dust.NewDustPerfect(spawnPosition, DustID.Blood, (Projectile.rotation + Main.rand.NextFloat(-PiOver4, PiOver4)).ToRotationVector2() * Main.rand.NextFloat(2, 8), Scale: 1.5f).noGravity = true;
                }
                SoundEngine.PlaySound(SoundID.NPCDeath13.WithPitchOffset(Main.rand.NextFloat(0, 0.3f)), player.Center);
                Projectile.UseAmmo(AmmoID.Rocket);
            }
            Projectile.frameCounter = 0;
            Projectile.frame++;
            if (Projectile.frame > 3)
                Projectile.frame = 0;
        }

        Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.14f);

        if (!Main.player[Projectile.owner].channel)
            Projectile.Kill();

    }
    public override bool PreDraw(ref Color lightColor)
    {
        Main.EntitySpriteDraw(Helper.GetTexture(Texture).Value, Projectile.Center - new Vector2(0, 10) - Main.screenPosition, new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), lightColor, Projectile.rotation, new Vector2(Projectile.Size.X / 2 - 15, Projectile.Size.Y / 2), Scale, Main.player[Projectile.owner].direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}

public class LobberProjectile2 : HeldProjectileGun
{
    Vector2 Scale = new Vector2(0, 0);
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/LobberCharge";
    public override bool? CanDamage() => false;
    public override void SetDefaults()
    {
        base.SetDefaults();
        ItemType = ItemType<Lobber>();
        RotationSpeed = 0.13f;
        Projectile.Size = new Vector2(58, 50);
    }
    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(20);
        Player player = Main.player[Projectile.owner];
        Projectile.rotation = (Main.MouseWorld - player.Center).ToRotation() + player.direction * PiOver2;
        Projectile.ai[2] = 1;
    }
    public override void AI()
    {
        base.AI();

        Player player = Main.player[Projectile.owner];

        player.heldProj = Projectile.whoAmI;

        if (Projectile.frame < 6)
        {
            if (Projectile.ai[0] < 119)
            {
                Projectile.ai[0] += AttackSpeedMultiplier;
                if (Projectile.frameCounter++ > 20 * AttackDelayMultiplier)
                {
                    Projectile.frameCounter = 0;
                    Projectile.frame++;
                }
            }
            else if(Projectile.ai[0] != 1000)
            {
                SoundEngine.PlaySound(SoundID.MaxMana.WithPitchOffset(-0.3f), Projectile.Center);
                Projectile.ai[0] = 1000;
                Projectile.ai[1] = 1;
            }
            float charge = Clamp(Projectile.ai[0], 5, 120);
            float scaleNoise = charge / 82;
            Scale = Vector2.Lerp(Scale, new Vector2(Main.rand.NextFloat(2f - scaleNoise, scaleNoise), Main.rand.NextFloat(2f - scaleNoise, scaleNoise)), scaleNoise / 10);
            if (!Main.mouseRight && player.whoAmI == Main.myPlayer)
            {
                Projectile.frame = 6;
                RecoilRotation = -charge / 240f * player.direction;
                Vector2 spawnPosition = Projectile.Center + new Vector2(30, -25 * player.direction).RotatedBy(Projectile.rotation);
                for (int u = 0; u < 14; u++)
                    Dust.NewDustPerfect(spawnPosition, DustID.Blood, (Projectile.rotation + Main.rand.NextFloat(-PiOver4, PiOver4)).ToRotationVector2() * Main.rand.NextFloat(2, 8), Scale: 1.5f).noGravity = true;
                for (int i = 0; i < charge / 15; i++)
                {
                    Scale = new Vector2(0.55f, 1.7f);
                    SoundEngine.PlaySound(SoundID.NPCDeath13, player.Center);
                    if (player.whoAmI == Main.myPlayer)
                        Projectile.NewProjectile(Projectile.InheritSource(Projectile), spawnPosition, Main.rand.NextFloat(Projectile.rotation - (PiOver2 * 20 / charge), Projectile.rotation + (PiOver2 * 20 / charge)).ToRotationVector2() * Main.rand.NextFloat(charge / 5, charge / 8), ProjectileType<CorruptionBalls>(), Projectile.damage * 2, Projectile.knockBack, Projectile.owner);
                    Projectile.UseAmmo(AmmoID.Rocket);
                }
            }
        }
        else
        {
            Projectile.frameCounter++;
            if (Projectile.frameCounter > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;
                if (Projectile.frame > 8)
                    Projectile.Kill();
            }
        }
        Scale = Vector2.Lerp(Scale, new Vector2(1, 1), 0.17f);
        if (Projectile.ai[1] > 0)
        {
            Projectile.ai[1] -= 0.04f;
            Projectile.ai[2] += 0.01f;
        }
    }
    public override bool PreDraw(ref Color lightColor)
    {
        int Direction = Main.player[Projectile.owner].direction;
        Main.EntitySpriteDraw(Helper.GetTexture(Texture).Value, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * Projectile.height, Projectile.width, Projectile.height), lightColor, Projectile.rotation, new Vector2(Projectile.Size.X / 2 - 17, Projectile.Size.Y / 2 + 9 * Direction), Scale, Main.player[Projectile.owner].direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        Main.EntitySpriteDraw(Assets.Items.Weapons.Ranged.LobberFlash.Value, Projectile.Center + new Vector2(17, -9 * Direction).RotatedBy(Projectile.rotation) - Main.screenPosition, new Rectangle(0, 0, Projectile.width, Projectile.height), Color.White * Projectile.ai[1], Projectile.rotation, new Vector2(Projectile.Size.X / 2, Projectile.Size.Y / 2), new Vector2(Projectile.ai[2], Projectile.ai[2] * 1.1f) * Scale, Main.player[Projectile.owner].direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}