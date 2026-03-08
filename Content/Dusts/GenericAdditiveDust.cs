using Terraria.Graphics.CameraModifiers;

namespace EbonianMod.Content.Dusts;

public class GenericAdditiveDust : ModDust
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
        dust.velocity *= 0.95f;
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        Texture2D tex = Assets.Extras.explosion.Value;
        if (d.customData is not null)
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, Color.White with { A = 0 } * d.scale * 5, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, d.color with { A = 0 } * (d.customData is not null ? ((int)d.customData == 2 ? d.scale * 10 : 1) : 1), 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
        return false;
    }
}
public class SparkleDust : ModDust
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
        dust.velocity *= 0.95f;
        if (dust.scale <= 0)
            dust.active = false;
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        Texture2D tex = Assets.Extras.crosslight.Value;
        if (d.customData is not null)
            Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, Color.White with { A = 0 } * d.scale * 5, 0, tex.Size() / 2, d.scale * 0.85f * 2, SpriteEffects.None, 0);
        Main.spriteBatch.Draw(tex, d.position - Main.screenPosition, null, d.color with { A = 0 } * (d.customData is not null ? ((int)d.customData == 2 ? d.scale * 10 : 1) : 1), 0, tex.Size() / 2, d.scale * 2, SpriteEffects.None, 0);
        return false;
    }
}
public class LineDustFollowPoint : ModDust
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
        dust.scale -= 0.0036f;
        dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;
        if (dust.customData is not null && dust.customData.GetType() == typeof(Vector2))
        {
            dust.velocity = Vector2.Lerp(dust.velocity, Helper.FromAToB(dust.position, (Vector2)dust.customData, false) / 25, 0.05f + dust.scale);
            if (dust.position.Distance((Vector2)dust.customData) < 100)
                dust.scale -= 0.01f;
        }
        else
            dust.velocity *= 0.98f;
        if (dust.scale <= 0)
            dust.active = false;
        dust.fadeIn = Lerp(dust.fadeIn, 1, 0.1f);
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        Texture2D tex = Assets.Extras.Extras2.trace_01.Value;

        for (float i = 0; i < Clamp(10 * d.fadeIn * d.scale * 5, 0, 5); i++)
            Main.spriteBatch.Draw(tex, d.position - d.velocity * i - Main.screenPosition, null, d.color with { A = 0 } * (d.scale * 10 * SmoothStep(1, 0, i / 10f)), d.rotation, tex.Size() / 2, new Vector2(1, Clamp(d.velocity.Length() * 0.25f, 0, 1.5f)) * d.scale * 2, SpriteEffects.None, 0);
        return false;
    }
}
public class IntenseDustFollowPoint : ModDust
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
        dust.rotation = dust.velocity.ToRotation() - MathHelper.PiOver2;
        if (dust.customData is not null && dust.customData.GetType() == typeof(Vector2) && dust.fadeIn >= 0.9f)
        {
            dust.velocity = Vector2.Lerp(dust.velocity, Helper.FromAToB(dust.position, (Vector2)dust.customData, false) / 5, 0.1f + dust.scale);
            if (dust.position.Distance((Vector2)dust.customData) < 30)
            {
                if (dust.frame.Y == 0)
                {
                    Helper.AddCameraModifier(new PunchCameraModifier(dust.position, dust.velocity.SafeNormalize(Vector2.Zero), 3, 6, 10, 1000));
                    dust.frame.Y = 1;
                }
                dust.velocity *= 0.7f;
                dust.scale -= 0.02f;
            }
        }
        else
            dust.velocity *= 0.98f;
        if (dust.scale <= 0)
            dust.active = false;
        dust.fadeIn = Lerp(dust.fadeIn, 1, 0.1f);
        return false;
    }

    public override bool PreDraw(Dust d)
    {
        Texture2D tex = Assets.Extras.crosslight.Value;
        Texture2D tex2 = Assets.Extras.flare.Value;

        for (float i = 0; i < Clamp(30 * d.scale * 10, 0, 30); i++)
            Main.spriteBatch.Draw(tex2, d.position + d.velocity * 2 - d.velocity * 0.15f * i - Main.screenPosition, null, d.color with { A = 0 } * (d.scale * 10 * SmoothStep(1, 0, i / 30f)) * d.fadeIn, d.rotation, tex2.Size() / 2, new Vector2(1, Clamp(d.velocity.Length() * 0.25f, 1, 3)) * d.scale, SpriteEffects.None, 0);

        Main.spriteBatch.Draw(tex, d.position + d.velocity * 2 - Main.screenPosition, null, d.color with { A = 0 } * (d.scale * 10) * d.fadeIn, d.rotation, tex.Size() / 2, new Vector2(1, Clamp(d.velocity.Length() * 0.25f, 0, 2)) * d.scale * 3, SpriteEffects.None, 0);
        return false;
    }
}
