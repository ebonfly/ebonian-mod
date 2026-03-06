using EbonianMod.Content.Dusts;
using System.IO;
using System.Linq;

namespace EbonianMod.Content.Projectiles.ArchmageX;

public class XCloud : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetStaticDefaults()
    {
        FinalDrawSystem.ProjectileTypeList.Add(Type);
    }
    public override void SetDefaults()
    {
        Projectile.height = 120;
        Projectile.width = 200;
        Projectile.hostile = true;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 386;
    }
    public override bool? CanDamage() => false;
    public override bool ShouldUpdatePosition() => false;
    public override bool PreDraw(ref Color lightColor)
    {
        return false;
    }
    public override void OnSpawn(IEntitySource source)
    {
        MPUtils.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
        for (int i = 0; i < 20; i++)
        {
            Vector2 vel = Main.rand.NextVector2Circular(.5f, .5f);
            float s = Main.rand.NextFloat(3, 5);
            //Dust.NewDustPerfect(Projectile.Center, DustType<XGoopDust2Dark>(), vel, 0, Color.White * 0.15f, Scale: s);
            Dust.NewDustPerfect(Projectile.Center, DustType<XGoopDust2>(), vel, 0, Color.White * 0.3f, Scale: s).customData = 1;
            //Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), DustType<XGoopDust2Dark>(), vel, 0, Color.White, Scale: s * 1.1f);
            //Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), DustType<XGoopDust2>(), vel, 0, Color.White, Scale: s);

            Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), DustType<SparkleDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.Indigo, Scale: Main.rand.NextFloat(0.1f, .15f));
        }
    }
    public override void SendExtraAI(BinaryWriter writer)
    {
        writer.Write(Projectile.localAI[0]);
    }
    public override void ReceiveExtraAI(BinaryReader reader)
    {
        Projectile.localAI[0] = reader.ReadSingle();
    }
    Vector2 savedDir;
    public override void AI()
    {
        Lighting.AddLight(Projectile.Center, TorchID.Purple);
        Vector2 vel = Main.rand.NextVector2Circular(2, 2);
        float s = Main.rand.NextFloat(1.5f, 4.25f);
        if (Projectile.timeLeft > 30 && Projectile.timeLeft % 2 == 0)
            Dust.NewDustPerfect(Projectile.Center, DustType<XGoopDust2>(), vel, 0, Color.White * 0.1f, Scale: s).customData = 1;
        //Dust.NewDustPerfect(Projectile.Center, DustType<XGoopDust2Dark>(), vel, 0, Color.White * 0.15f, Scale: s * 1.1f);
        //Dust.NewDustPerfect(Projectile.Center, DustType<XGoopDust2>(), vel, 0, Color.White * 0.15f, Scale: s);

        if (Projectile.ai[0] > 40)
        {
            Projectile.localAI[0] = MathHelper.Lerp(Projectile.localAI[0], 1, 0.2f);
            if (Projectile.ai[0] % 6 == 0 && Projectile.ai[0] < 70)
            {
                Vector2 pos = Projectile.Center + Main.rand.NextVector2CircularEdge(150, 150) * Main.rand.NextFloat(1, 2);
                Dust.NewDustPerfect(pos, DustType<LineDustFollowPoint>(), Helper.FromAToB(pos, Projectile.Center) * Main.rand.NextFloat(1, 2), 0, Color.Lerp(Color.Purple, Color.Indigo, Projectile.localAI[0]), Scale: Main.rand.NextFloat(0.15f, .4f)).customData = Projectile.Center;
            }
        }
        else
            Projectile.localAI[0] = MathHelper.Lerp(Projectile.localAI[0], 0, 0.2f);

        if (Projectile.timeLeft % 8 == 0)
        {
            MPUtils.NewProjectile(null, Projectile.Center, Main.rand.NextVector2Unit() * Main.rand.NextFloat(2.5f, 4), ProjectileType<XCloudVFXExtra>(), 0, 0);
        }

        if (Projectile.timeLeft % 6 == 0)
            Dust.NewDustPerfect(Main.rand.NextVector2FromRectangle(Projectile.getRect()), DustType<SparkleDust>(), Main.rand.NextVector2Circular(2, 2), 0, Color.Indigo, Scale: Main.rand.NextFloat(0.1f, .15f));
        if (Projectile.timeLeft <= 375)
            Projectile.ai[0]++;
        if (Projectile.ai[0] <= 20 && savedDir == Vector2.Zero && Projectile.timeLeft >= 80)
        {
            savedDir = Helper.FromAToB(Projectile.Center, Main.player[(int)Projectile.ai[2]].Center);
            if (Projectile.ai[1] != 1)
                MPUtils.NewProjectile(null, Projectile.Center, savedDir, ProjectileType<XTelegraphLine>(), 0, 0);
            else
                MPUtils.NewProjectile(null, Projectile.Center, savedDir, ProjectileType<SheepeningOrb>(), 20, 0, Main.myPlayer, Projectile.ai[2]);
        }
        if (Projectile.ai[0] > ((int)Projectile.ai[1] != 1 ? 40 : 100) && Projectile.timeLeft >= 30)
        {
            if ((int)Projectile.ai[1] != 1)
                MPUtils.NewProjectile(null, Projectile.Center, savedDir, ProjectileType<XLightningBolt>(), 20, 0);
            MPUtils.NewProjectile(null, Projectile.Center, Vector2.Zero, ProjectileType<XExplosion>(), 0, 0);
            savedDir = Vector2.Zero;
            Projectile.ai[0] = 0;
        }
    }
}
public class XCloudVFXExtra : ModProjectile
{
    public override string Texture => Helper.Empty;
    public override void SetDefaults()
    {
        Projectile.height = 5;
        Projectile.width = 5;
        Projectile.hostile = false;
        Projectile.friendly = false;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.timeLeft = 60;
    }
    int seed;
    public override void OnSpawn(IEntitySource source)
    {
        seed = Main.rand.Next(10000);
    }
    public override void AI()
    {
        UnifiedRandom rand = new UnifiedRandom(seed);
        Projectile.velocity = Projectile.velocity.RotatedBy(MathHelper.ToRadians(rand.NextFloat(1.5f, 3.5f)) * (rand.NextFloatDirection() > 0 ? 1 : -1));
        //Dust.NewDustPerfect(Projectile.Center, DustType<XGoopDust2Dark>(), Vector2.Zero, Scale: Projectile.timeLeft * 0.015f);
        if (Projectile.timeLeft % 2 == 0)
            Dust.NewDustPerfect(Projectile.Center, DustType<XGoopDust2>(), Projectile.velocity * MathHelper.Lerp(0.05f, 0, (float)Projectile.timeLeft / 60), 0, Color.White * 0.7f, Projectile.timeLeft * 0.015f).customData = 1;
    }
}
