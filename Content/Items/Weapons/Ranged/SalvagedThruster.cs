using EbonianMod.Content.Projectiles.Bases;

namespace EbonianMod.Content.Items.Weapons.Ranged;

public class SalvagedThruster : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Ranged/SalvagedThruster";
    public override void SetDefaults()
    {
        Item.DamageType = DamageClass.Ranged;
        Item.damage = 4;
        Item.useTime = 100;
        Item.shoot = ProjectileType<SalvagedThrusterProjectile>();
        Item.shootSpeed = 1f;
        Item.rare = ItemRarityID.Green;
        Item.useStyle = 5;
        Item.value = Item.buyPrice(0, 1, 0, 0);
        Item.autoReuse = false;
        Item.noUseGraphic = true;
        Item.noMelee = true;
        Item.channel = true;
        Item.useAmmo = AmmoID.Gel;
        Item.crit = 0;
    }

    public override bool CanConsumeAmmo(Item ammo, Player player) => false;
    public override bool? CanAutoReuseItem(Player player) => false;
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        velocity.Normalize();
        Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
        return false;
    }
}
public class SalvagedThrusterProjectile : HeldProjectileGun
{
    Vector2 Scale;
    float RayLength = 0, Charge;
    bool CanAttack, IsReady;
    public override string Texture => Helper.AssetPath+"Items/Weapons/Ranged/SalvagedThrusterProjectile";
    public override bool? CanDamage() => CanAttack;
    public override void SetDefaults()
    {
        base.SetDefaults();
        ItemType = ItemType<SalvagedThruster>();
        Projectile.Size = new Vector2(44, 30);
        Projectile.penetrate = -1;
        Projectile.usesLocalNPCImmunity = true;
        Projectile.frame = 0;
        Projectile.ArmorPenetration = 100;
    }
    public override void OnSpawn(IEntitySource source)
    {
        CalculateAttackSpeedParameters(100);
        RotationSpeed = AttackSpeedMultiplier / 40;
        Projectile.rotation = (Main.MouseWorld - Main.player[Projectile.owner].Center).ToRotation();
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        float a = 0f;
        return CanAttack && IsReady ? Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), Projectile.Center, Projectile.Center + Projectile.rotation.ToRotationVector2() * RayLength, 50, ref a) : false;
    }
    public override void AI()
    {
        Player player = Main.player[Projectile.owner];

        player.heldProj = Projectile.whoAmI;
        Projectile.localNPCHitCooldown = (int)Clamp(4 * AttackDelayMultiplier, 2, 4);

        IsReady = Scale.X > 0.9f;
        Main.NewText(IsReady);
        if (Projectile.timeLeft % 10 == 0)
        {
            if(IsReady) CanAttack = Projectile.UseAmmo(AmmoID.Gel);

            if (!CanAttack)
            {
                Dust.NewDustPerfect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 47, DustID.Smoke, Projectile.rotation.ToRotationVector2().RotatedByRandom(PiOver4 / 3) * Main.rand.NextFloat(0.1f, 3));
                Dust.NewDustPerfect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 47, DustID.Smoke, Projectile.rotation.ToRotationVector2().RotatedByRandom(PiOver4 / 3) * Main.rand.NextFloat(0.1f, 3));
                Projectile.frame = 0;
                return;
            }
            SoundEngine.PlaySound(SoundID.Item34 with { Volume = Projectile.ai[0] / 250 }, Projectile.Center);
        }

        if (CanAttack && IsReady)
        {
            Projectile.ai[0] = Lerp(Projectile.ai[0], 225, 0.05f);
            Charge = Projectile.ai[0];
            RayLength = Helper.Raycast(Projectile.Center, Projectile.rotation.ToRotationVector2(), Charge).RayLength;

            Dust.NewDustPerfect(Projectile.Center + Projectile.rotation.ToRotationVector2() * 47, DustID.Smoke, (Projectile.rotation).ToRotationVector2().RotatedByRandom(Pi / 4) * Main.rand.NextFloat(0.1f, 2), Scale: Main.rand.NextFloat(1.1f, 2));
            for (int i = 0; i < Charge / 12; i++)
                for (int u = 0; u < i / 4 + 1; u++)
                    Dust.NewDustPerfect(player.MountedCenter + Projectile.rotation.ToRotationVector2() * (47 + ((RayLength - 47) / 20) * i), DustID.Torch, (Main.rand.NextFloat(0, Pi * 2)).ToRotationVector2() * Main.rand.NextFloat(1.2f, 2) * Clamp(i * Charge / 1800, 0.5f, 5), Scale: Main.rand.NextFloat(1.9f, 2.5f)).noGravity = true;

            Projectile.frame = 1;
        }
        else
            Scale = Vector2.Lerp(Scale, Vector2.One, 0.2f);

        if (!player.channel)
            Projectile.Kill();

        base.AI();
    }
    public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
    {
        target.AddBuff(BuffID.OnFire, 210);
    }

    public override bool PreDraw(ref Color lightColor)
    {
        Player player = Main.player[Projectile.owner];
        Main.EntitySpriteDraw(TextureAssets.Projectile[Type].Value, Projectile.Center + player.GFX() + Main.rand.NextVector2Circular(Charge * 0.01f, Charge * 0.01f) - Main.screenPosition, new Rectangle(0, Projectile.height * Projectile.frame, Projectile.width, Projectile.height), lightColor, Projectile.rotation, new Vector2(Projectile.width / 2 - 20, Projectile.height / 2 - 4 * player.direction), Scale, player.direction == 1 ? SpriteEffects.None : SpriteEffects.FlipVertically);
        return false;
    }
}

