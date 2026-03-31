namespace LightingEssentials;

public class LightBossKillGlobalNPC : GlobalNPC
{
    public override void OnKill(NPC npc)
    {
        LightWorldSystem.NotifyTrackedBossKill(npc);
    }
}