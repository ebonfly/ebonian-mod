using System.Collections.Generic;
using System.Linq;

namespace EbonianMod.Core.Systems.Verlets;

public class SpawnableVerlet
{
    public Verlet verlet;
    public VerletDrawData drawData;
    public Vector2 velocity;
    public int timeLeft, maxTime;
    public SpawnableVerlet(Verlet v, VerletDrawData vdd, Vector2 vel, int _timeLeft = 120)
    {
        verlet = v;
        drawData = vdd;
        velocity = vel;
        timeLeft = _timeLeft;
        maxTime = _timeLeft;
    }
}
public class S_VerletSystem : ModSystem
{
    public override void Load()
    {
        On_Main.DrawPlayers_AfterProjectiles += (orig, self) =>
        {
            if (verlets.Any())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                
                for (int i = 0; i < verlets.Count; i++)
                {
                    if (verlets[i].timeLeft > 0 && verlets[i].verlet is not null)
                    {
                        float alpha = Clamp(Lerp(0, 2, (float)verlets[i].timeLeft / verlets[i].maxTime), 0, 1);
                        VerletDrawData verletDrawData = verlets[i].drawData;
                        verletDrawData.color = Lighting.GetColor(verlets[i].verlet.lastP.position.ToTileCoordinates()) * alpha;
                        verlets[i].verlet.Draw(Main.spriteBatch, verletDrawData);
                    }
                }
                Main.spriteBatch.End();
            }
            
            orig(self);
        };
    }

    public static List<SpawnableVerlet> verlets = new(100);
    public override void PostUpdateEverything()
    {
        if (Main.dedServ)
        {
            verlets.Clear();
            return;
        }
        for (int i = 0; i < verlets.Count; i++)
        {
            if (verlets[i].timeLeft > -1)
            {
                if (verlets[i].verlet is not null)
                {
                    if (verlets[i].verlet.gravity > 5)
                        verlets[i].velocity = Vector2.Lerp(verlets[i].velocity, new Vector2(verlets[i].velocity.X * 0.99f, 10), 0.05f);
                    verlets[i].verlet.gravity = Lerp(verlets[i].verlet.gravity, 10, 0.05f);
                    for (int j = 0; j < verlets[i].verlet.points.Count; j++)
                    {
                        UnifiedRandom rand = new UnifiedRandom(i + j);
                        verlets[i].verlet.points[j].collide = true;
                        if (verlets[i].verlet.points[j].colLength < verlets[i].verlet.gravity)
                            verlets[i].verlet.points[j].colLength = verlets[i].verlet.gravity * 2;
                        Vector2 velocity = verlets[i].velocity;
                        if (Helper.Raycast(verlets[i].verlet.points[j].position, velocity.SafeNormalize(Vector2.UnitY), verlets[i].verlet.points[j].colLength * 2).RayLength >= verlets[i].verlet.points[j].colLength * 1.8f || !Collision.SolidCollision(verlets[i].verlet.points[j].position, (int)verlets[i].verlet.points[j].colLength, (int)verlets[i].verlet.points[j].colLength))
                            verlets[i].verlet.points[j].position += verlets[i].velocity * (rand.NextFloat(0.75f, 1f) * (j / (float)verlets[i].verlet.points.Count));

                        verlets[i].verlet.points[j].locked = false;
                    }
                    verlets[i].verlet.Update(verlets[i].verlet.startPos, verlets[i].verlet.endPos);
                }
                verlets[i].timeLeft--;
            }
            else
            {
                verlets.RemoveAt(i);
            }
        }
    }
}