namespace LightingEssentials;

public readonly struct WorldLightingState : System.IEquatable<WorldLightingState>
{
    public const WorldLightingFlags EventMask =
        WorldLightingFlags.PartyActive |
        WorldLightingFlags.LanternNightActive |
        WorldLightingFlags.RainActive |
        WorldLightingFlags.SandstormActive |
        WorldLightingFlags.WindyDayActive |
        WorldLightingFlags.ThunderstormActive |
        WorldLightingFlags.StarfallActive |
        WorldLightingFlags.BloodMoonActive |
        WorldLightingFlags.GoblinArmyActive |
        WorldLightingFlags.SlimeRainActive |
        WorldLightingFlags.OldOnesArmyActive |
        WorldLightingFlags.TorchGodActive |
        WorldLightingFlags.EclipseActive |
        WorldLightingFlags.FrostLegionActive |
        WorldLightingFlags.PirateInvasionActive |
        WorldLightingFlags.PumpkinMoonActive |
        WorldLightingFlags.FrostMoonActive |
        WorldLightingFlags.MartianMadnessActive |
        WorldLightingFlags.LunarEventsActive;

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