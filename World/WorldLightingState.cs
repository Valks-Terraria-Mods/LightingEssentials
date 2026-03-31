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

public readonly struct WorldLightingState : System.IEquatable<WorldLightingState>
{
    public const WorldLightingFlags EventMask =
        WorldLightingFlags.BloodMoonActive |
        WorldLightingFlags.EclipseActive |
        WorldLightingFlags.FrostLegionActive;

    public WorldLightingFlags Flags { get; }
    public int MechBossesDowned { get; }

    public bool DownedKingSlime => Has(WorldLightingFlags.DownedKingSlime);
    public bool DownedEyeOfCthulhu => Has(WorldLightingFlags.DownedEyeOfCthulhu);
    public bool DownedEvilBoss => Has(WorldLightingFlags.DownedEvilBoss);
    public bool DownedQueenBee => Has(WorldLightingFlags.DownedQueenBee);
    public bool DownedSkeletron => Has(WorldLightingFlags.DownedSkeletron);
    public bool DownedDeerclops => Has(WorldLightingFlags.DownedDeerclops);
    public bool HardModeUnlocked => Has(WorldLightingFlags.HardModeUnlocked);
    public bool DownedQueenSlime => Has(WorldLightingFlags.DownedQueenSlime);
    public bool DownedPlantera => Has(WorldLightingFlags.DownedPlantera);
    public bool DownedGolem => Has(WorldLightingFlags.DownedGolem);
    public bool DownedFishron => Has(WorldLightingFlags.DownedFishron);
    public bool DownedEmpressOfLight => Has(WorldLightingFlags.DownedEmpressOfLight);
    public bool DownedLunaticCultist => Has(WorldLightingFlags.DownedLunaticCultist);
    public bool DownedMoonLord => Has(WorldLightingFlags.DownedMoonLord);
    public bool BloodMoonActive => Has(WorldLightingFlags.BloodMoonActive);
    public bool EclipseActive => Has(WorldLightingFlags.EclipseActive);
    public bool FrostLegionActive => Has(WorldLightingFlags.FrostLegionActive);

    public WorldLightingState(WorldLightingFlags flags, int mechBossesDowned)
    {
        Flags = flags;
        MechBossesDowned = System.Math.Clamp(mechBossesDowned, 0, 3);
    }

    public bool Has(WorldLightingFlags flag)
    {
        return (Flags & flag) != 0;
    }

    public WorldLightingState WithEventFlags(WorldLightingFlags eventFlags)
    {
        return new WorldLightingState((Flags & ~EventMask) | (eventFlags & EventMask), MechBossesDowned);
    }

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

    public bool Equals(WorldLightingState other)
    {
        return Flags == other.Flags && MechBossesDowned == other.MechBossesDowned;
    }

    public override bool Equals(object obj)
    {
        return obj is WorldLightingState other && Equals(other);
    }

    public override int GetHashCode()
    {
        return System.HashCode.Combine((ulong)Flags, MechBossesDowned);
    }

    public static bool operator ==(WorldLightingState left, WorldLightingState right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(WorldLightingState left, WorldLightingState right)
    {
        return !left.Equals(right);
    }
}