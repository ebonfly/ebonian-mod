using EbonianMod.Content.NPCs.ArchmageX;
using EbonianMod.Core.Systems.Boss;
using Terraria.Graphics.Effects;

namespace EbonianMod.Common.Detours;

public class MiscDetours : ModSystem
{
    public override void Load()
    {
        On_NPC.SetEventFlagCleared += EventClear;
        On_Main.Update += UpdateDeltaTime;
        On_FilterManager.CanCapture += (On_FilterManager.orig_CanCapture orig, FilterManager self) => true;
    }
    void UpdateDeltaTime(On_Main.orig_Update orig, Main self, GameTime gameTime)
    {
        float oldFrameRate = Main.frameRate;
        orig(self, gameTime);
        if (!Main.dedServ)
        {
            if (Main.FrameSkipMode == Terraria.Enums.FrameSkipMode.On) TimeSystem.deltaTime = 1;
            else
            {
                float averageFrameRate = (Main.frameRate + oldFrameRate) / 2f;
                TimeSystem.deltaTime = Clamp((float)(gameTime.TotalGameTime.TotalSeconds - gameTime.ElapsedGameTime.TotalSeconds) / (averageFrameRate), 0.2f, 1.1f);
            }
        }
    }
    void EventClear(On_NPC.orig_SetEventFlagCleared orig, ref bool eventFlag, int gameEventId)
    {
        if (gameEventId == 3 && !GetInstance<XareusSystem>().downedMartianXareus && GetInstance<XareusSystem>().downedXareus)
        {
            MPUtils.NewNPC(Main.player[0].Center, NPCType<ArchmageCutsceneMartian>(), false, -1);
            GetInstance<XareusSystem>().downedMartianXareus = true;
        }
        orig(ref eventFlag, gameEventId);
    }
}
