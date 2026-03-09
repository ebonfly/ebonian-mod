using StructureHelper.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EbonianMod.Content.Items.Weapons.Magic;
public class BotanistHeadStaff : ModItem
{
    public override string Texture => Helper.AssetPath + "Items/Weapons/Magic/BotanistHeadStaff";
    public override void SetStaticDefaults()
    {
        Item.staff[Type] = true;
    }
    public override void SetDefaults()
    {
        Item.damage = 13;
        Item.width = 40;
        Item.height = 40;
        Item.mana = 5;
        Item.DamageType = DamageClass.Magic;
        Item.useTime = 50;
        Item.useAnimation = 50;
        Item.useStyle = ItemUseStyleID.Shoot;
        Item.knockBack = 10;
        Item.rare = ItemRarityID.Green;
        Item.UseSound = SoundID.Item8;
        Item.noMelee = true;
        Item.value = Item.buyPrice(0, 20, 0, 0);
        Item.autoReuse = true;
        Item.shoot = ProjectileType<BotanistHeadProjectile>();
        Item.shootSpeed = 8f;
    }
    public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
    {
        float dist = 300 - Helper.Raycast(Main.MouseWorld, Vector2.UnitY, 300).RayLength;
        Vector2 target = Helper.Raycast(player.Center, player.FromAToB(Main.MouseWorld), player.Distance(Main.MouseWorld)).Point - new Vector2(0, dist);
        Vector2 mouse = Vector2.Lerp(Main.MouseWorld, target, 0.5f) - player.RotatedRelativePoint(player.MountedCenter, reverseRotation: true);
        player.itemRotation = MathF.Atan2(mouse.Y * player.direction, mouse.X * player.direction);
        NetMessage.SendData(MessageID.PlayerControls, number: player.whoAmI);
        NetMessage.SendData(MessageID.ShotAnimationAndSound, number: player.whoAmI);


        for (float i = 0; i < 1f; i += 0.025f)
        {
            float iSq = MathF.Pow(i, 2);
            Vector2 pos = Vector2.Lerp(Vector2.Lerp(position - new Vector2(0, 30), Main.MouseWorld, iSq), Vector2.Lerp(Main.MouseWorld, target, iSq), i);
            Dust.NewDustPerfect(pos + Main.rand.NextVector2Circular(2, 2), DustID.GrassBlades, Main.rand.NextVector2Circular(2, 2), Alpha: (int)Clamp((1f - i) * 320, 0, 225), Scale: SmoothStep(0, 1, Clamp(i * 5, 0, 0.8f)));
        }

        for (int i = 0; i < 30; i++)
            Dust.NewDustPerfect(target + Main.rand.NextVector2Circular(20, 20), DustID.GrassBlades, Main.rand.NextVector2Circular(16, 16)).noGravity = true;

        for (int i = 0; i < 4; i++)
            Projectile.NewProjectile(null, target, new Vector2(i - 4, velocity.Length() * Main.rand.NextFloat(-0.5f, 1.5f)), type, damage, knockback, player.whoAmI);

        Projectile.NewProjectile(null, target, new Vector2(0, velocity.Length()), type, damage, knockback, player.whoAmI);
        return false;
    }
}

public class BotanistHeadProjectile : ModProjectile
{
    public override string Texture => Helper.AssetPath+"NPCs/Overworld/Botanist/BotanistHead";
    public override void SetDefaults()
    {
        Projectile.width = 22;
        Projectile.height = 22;
        Projectile.aiStyle = 14;
        AIType = ProjectileID.StickyGlowstick;
        Projectile.friendly = true;
        Projectile.tileCollide = true;
        Projectile.hostile = false;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.timeLeft = 240;
    }
    public override bool OnTileCollide(Vector2 oldVelocity)
    {
        if (Projectile.localAI[2] == 0)
        {
            Projectile.timeLeft = 60;
            Collision.HitTiles(Projectile.Center - new Vector2(25, 0), Projectile.velocity, 50, 20);
            SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
            Projectile.Center += new Vector2(0, 20);
            Projectile.localAI[2] = 1;
        }
        return base.OnTileCollide(oldVelocity);
    }
    float savedP;
    public override void AI()
    {
        if (Projectile.localAI[2] == 0 && Projectile.Grounded())
        {
            Projectile.timeLeft = 60;
            Collision.HitTiles(Projectile.Center - new Vector2(25, 0), Projectile.velocity, 50, 20);
            SoundEngine.PlaySound(SoundID.Item70, Projectile.Center);
            Projectile.Center += new Vector2(0, 20);
            Projectile.localAI[2] = 1;
        }

        Projectile.velocity.X = Lerp(Projectile.velocity.X, 0, 0.025f);
        Player p = Main.player[Projectile.owner];
        if (savedP == 0)
            savedP = p.Center.Y;
        if (Projectile.velocity.Length() < 25f && Projectile.velocity.Y > 0)
            Projectile.velocity.Y *= 1.01f;
        if (Projectile.localAI[2] == 1)
        {
            Projectile.velocity.Y = 0;
            Projectile.Opacity = Lerp(0, 1, InOutCirc.Invoke(Projectile.timeLeft / 60f));
        }
    }
}
