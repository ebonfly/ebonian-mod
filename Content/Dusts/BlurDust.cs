namespace EbonianMod.Content.Dusts;

public class BlurDust : ModDust
{
    public override string Texture => Helper.Empty;
    public override void OnSpawn(Dust dust)
    {
        dust.alpha = 255;
        dust.noLight = true;
        dust.noGravity = true;
        //if (dust.scale <= 1f && dust.scale >= 0.8f)
        //  dust.scale = 0.25f;
        base.OnSpawn(dust);

    }
    public override bool Update(Dust dust)
    {
        dust.position += dust.velocity;
        dust.scale -= 0.005f;
        dust.rotation = dust.velocity.ToRotation();
        dust.velocity *= 0.95f;
        dust.fadeIn = Lerp(dust.fadeIn, 1, 0.1f);
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        BlurRendering.DrawCache.Add(() =>
        {
            Texture2D tex = Assets.Extras.Extras2.fire_01_normal.Value;
            for (float i = 0; i < 30 * d.fadeIn; i++)
            {
                Main.spriteBatch.Draw(tex, d.position - d.velocity * d.scale * i * 2 - Main.screenPosition, null, d.color * d.scale * SmoothStep(2, 0, i / 30f), 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
            }
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, d.color * d.scale * d.fadeIn, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
        });
        return false;
    }
}
