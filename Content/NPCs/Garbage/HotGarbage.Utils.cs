using System.Collections.Generic;
using ReLogic.Utilities;

namespace EbonianMod.Content.NPCs.Garbage;

public partial class HotGarbage : ModNPC
{
    public readonly List<Vector2> RedFrames = new List<Vector2>
    {
        new(0, 76*8), new(0, 76*10), new(0, 76*11), new(0, 76*12),

        new(80, 0), new(80, 76*1), new(80, 76*2),

        new(80*2, 0), new(80*2, 76*1), new(80*2, 76*2), new(80*2, 76*3)
    };
    public readonly List<Vector2> YellowFrames = new List<Vector2>
    {
        new(80, 76*3), new(80, 76*4), new(80, 76*5)
    };
    public readonly List<Vector2> GreenFrames = new List<Vector2>
    {
        new(80, 76*6), new(80, 76*7), new(80, 76*8), new(80, 76*9)
    };
    
    public readonly List<State> AttackPool = new List<State>() { 
        State.WarningForDash, State.WarningForBigDash,  State.SlamPreperation, State.MailBoxes, State.PipeBombAirstrike, State.MassiveLaser,
        
        State.OpenLid, State.OpenLid, State.OpenLid, State.OpenLid, State.OpenLid, State.OpenLid, 
    };
    public readonly List<State> OpenAttackPool = new List<State>()
    {
        State.SpewFire, State.SpewFire2, State.GiantFireball, State.TrashBags, State.SodaMissiles, State.SateliteLightning
    };
    
    public bool StruckDead;
    public bool PerformedFullMoveset;
    public Vector2 DisposablePosition;
    public SlotId LaserSoundSlot;
    
    void AmbientVFX() {
        if (RedFrames.Contains(new(NPC.frame.X, NPC.frame.Y)))
            Lighting.AddLight(NPC.Center, TorchID.Red);
        if (YellowFrames.Contains(new(NPC.frame.X, NPC.frame.Y)))
            Lighting.AddLight(NPC.Center, TorchID.Yellow);
        if (GreenFrames.Contains(new(NPC.frame.X, NPC.frame.Y)))
            Lighting.AddLight(NPC.Center, TorchID.Green);
        if (NPC.frame.X == 80 * 2 && NPC.frame.Y > 0)
        {
            Lighting.AddLight(NPC.Center, TorchID.Torch);
        }
    }
    
    void TargetingLogic() {
        NPC.TargetClosest(false);
        if (AIState != State.Death)
        {
            bool shouldntDespawn = false;
            foreach (Player p in Main.ActivePlayers)
                if (!p.dead)
                {
                    shouldntDespawn = true;
                    break;
                }
            if (!shouldntDespawn)
            {
                if (NPC.timeLeft > 10)
                {
                    NPC.timeLeft = 10;
                }
                NPC.active = false;
                return;
            }
        }
        NPC.timeLeft = 10;
    }
    
    void FacePlayer() => NPC.direction = player.Center.X > NPC.Center.X ? 1 : -1;

    void ResetTo(State state, State? stateAfterOpenLid = null)
    {
        NextAttack = state;
        if (stateAfterOpenLid != null)
            NextAttack2 = stateAfterOpenLid.Value;
        
        AIState = State.Idle;
        AITimer = 0;
        AITimer2 = 0;
        AITimer3 = 0;
        NPC.damage = 0;
        NPC.netUpdate = true;
    }  
    
    void JumpCheck()
    {
        Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY, 1, false, 0);

        if (AIState == State.Idle)
        {
            if (Helper.Raycast(NPC.Center, -Vector2.UnitY, NPC.height).RayLength < NPC.height - 1 && !Collision.CanHit(NPC, player))
            {
                if (!NPC.noTileCollide)
                {
                    NPC.noTileCollide = true;
                    NPC.netUpdate = true;
                }

                if (player.Center.Y < NPC.Center.Y)
                    NPC.Center -= Vector2.UnitY * 2;
                else
                    NPC.Center += Vector2.UnitY * 2;

                NPC.Center += new Vector2(Helper.FromAToB(NPC.Center, player.Center).X * 2, 0);
            }
            else if ((!Collision.CanHit(NPC, player) || !Collision.CanHitLine(NPC.TopLeft, 10, 10, player.position, player.width, player.height) || !Collision.CanHitLine(NPC.TopRight, 10, 10, player.position, player.width, player.height)) && player.Center.X.InRange(NPC.Center.X, NPC.width))
            {
                if (!NPC.noTileCollide)
                {
                    NPC.noTileCollide = true;
                    NPC.netUpdate = true;
                }

                if (player.Center.Y < NPC.Center.Y)
                    NPC.Center -= Vector2.UnitY * 2;
                else
                    NPC.Center += Vector2.UnitY * 2;
            }
            else if (NPC.noTileCollide)
            {
                NPC.noTileCollide = false;
                NPC.netUpdate = true;
            }
        }
    }
}