namespace LightingEssentials;

internal static class WorldLightingStateCapture
{
    public static WorldLightingFlags CaptureEventFlags()
    {
        WorldLightingFlags flags = WorldLightingFlags.None;

        if (Main.bloodMoon)
            flags |= WorldLightingFlags.BloodMoonActive;

        if (Main.eclipse)
            flags |= WorldLightingFlags.EclipseActive;

        if (Main.invasionType == InvasionID.SnowLegion)
            flags |= WorldLightingFlags.FrostLegionActive;

        return flags;
    }

    public static WorldLightingState Capture()
    {
        int mechBossesDowned = 0;
        if (NPC.downedMechBoss1)
            mechBossesDowned++;
        if (NPC.downedMechBoss2)
            mechBossesDowned++;
        if (NPC.downedMechBoss3)
            mechBossesDowned++;

        WorldLightingFlags flags = CaptureProgressionFlags() | CaptureEventFlags();
        return new WorldLightingState(flags, mechBossesDowned);
    }

    private static WorldLightingFlags CaptureProgressionFlags()
    {
        WorldLightingFlags flags = WorldLightingFlags.None;

        if (NPC.downedSlimeKing)
            flags |= WorldLightingFlags.DownedKingSlime;

        if (NPC.downedBoss1)
            flags |= WorldLightingFlags.DownedEyeOfCthulhu;

        if (NPC.downedBoss2)
            flags |= WorldLightingFlags.DownedEvilBoss;

        if (NPC.downedQueenBee)
            flags |= WorldLightingFlags.DownedQueenBee;

        if (NPC.downedBoss3)
            flags |= WorldLightingFlags.DownedSkeletron;

        if (NPC.downedDeerclops)
            flags |= WorldLightingFlags.DownedDeerclops;

        if (Main.hardMode)
            flags |= WorldLightingFlags.HardModeUnlocked;

        if (NPC.downedQueenSlime)
            flags |= WorldLightingFlags.DownedQueenSlime;

        if (NPC.downedPlantBoss)
            flags |= WorldLightingFlags.DownedPlantera;

        if (NPC.downedGolemBoss)
            flags |= WorldLightingFlags.DownedGolem;

        if (NPC.downedFishron)
            flags |= WorldLightingFlags.DownedFishron;

        if (NPC.downedEmpressOfLight)
            flags |= WorldLightingFlags.DownedEmpressOfLight;

        if (NPC.downedAncientCultist)
            flags |= WorldLightingFlags.DownedLunaticCultist;

        if (NPC.downedMoonlord)
            flags |= WorldLightingFlags.DownedMoonLord;

        return flags;
    }
}