using System.IO;

namespace LightingEssentials;

public class LightWorldSystem : ModSystem
{
    public static WorldLightingState CurrentState => WorldLightingStateController.CurrentState;

    public override void OnWorldLoad()
    {
        WorldLightingStateController.OnWorldLoad();
    }

    public override void ClearWorld()
    {
        WorldLightingStateController.ClearWorld();
    }

    public override void OnWorldUnload()
    {
        WorldLightingStateController.ClearWorld();
    }

    public override void NetReceive(BinaryReader reader)
    {
        WorldLightingStateController.NetReceive();
    }

    public override void NetSend(BinaryWriter writer)
    {
    }

    public override void PreUpdateTime()
    {
        WorldLightingStateController.PreUpdateTime();
    }

    public static WorldLightingState GetCurrentState()
    {
        return CurrentState;
    }

    internal static void NotifyTrackedBossKill(NPC npc)
    {
        WorldLightingStateController.NotifyTrackedBossKill(npc);
    }
}