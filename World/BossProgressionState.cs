namespace LightingEssentials;

public readonly record struct BossProgressionState(
    bool DownedBoss1,
    bool DownedBoss2,
    bool DownedBoss3,
    bool DownedPlantera,
    int MechBossesDowned,
    bool DownedMoonLord)
{
    public static BossProgressionState Capture()
    {
        int mechBossesDowned = 0;
        if (NPC.downedMechBoss1)
            mechBossesDowned++;
        if (NPC.downedMechBoss2)
            mechBossesDowned++;
        if (NPC.downedMechBoss3)
            mechBossesDowned++;

        return new BossProgressionState(
            NPC.downedBoss1,
            NPC.downedBoss2,
            NPC.downedBoss3,
            NPC.downedPlantBoss,
            mechBossesDowned,
            NPC.downedMoonlord);
    }
}
