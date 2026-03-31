namespace LightingEssentials;

[System.Flags]
public enum WorldLightingFlags : ulong
{
    None = 0,
    DownedKingSlime = 1UL << 0,
    DownedEyeOfCthulhu = 1UL << 1,
    DownedEvilBoss = 1UL << 2,
    DownedQueenBee = 1UL << 3,
    DownedSkeletron = 1UL << 4,
    DownedDeerclops = 1UL << 5,
    HardModeUnlocked = 1UL << 6,
    DownedQueenSlime = 1UL << 7,
    DownedPlantera = 1UL << 8,
    DownedGolem = 1UL << 9,
    DownedFishron = 1UL << 10,
    DownedEmpressOfLight = 1UL << 11,
    DownedLunaticCultist = 1UL << 12,
    DownedMoonLord = 1UL << 13,
    BloodMoonActive = 1UL << 14,
    EclipseActive = 1UL << 15,
    FrostLegionActive = 1UL << 16,
}