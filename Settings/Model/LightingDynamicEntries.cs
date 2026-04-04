using System.Collections.Generic;
using Terraria.ModLoader.Config;

namespace LightingEssentials;

public enum LightingEventId
{
    Party,
    LanternNight,
    Rain,
    Sandstorm,
    WindyDay,
    Thunderstorm,
    Starfall,
    BloodMoon,
    GoblinArmy,
    SlimeRain,
    OldOnesArmy,
    TorchGod,
    FrostLegion,
    SolarEclipse,
    PirateInvasion,
    PumpkinMoon,
    FrostMoon,
    MartianMadness,
    LunarEvents,
}

public enum LightingBossId
{
    KingSlime,
    EyeOfCthulhu,
    EaterOfWorlds,
    BrainOfCthulhu,
    EvilBiomeBoss,
    QueenBee,
    Skeletron,
    Deerclops,
    WallOfFlesh,
    QueenSlime,
    MechBosses,
    Twins,
    Destroyer,
    SkeletronPrime,
    Plantera,
    Golem,
    DukeFishron,
    EmpressOfLight,
    LunaticCultist,
    MoonLord,
    DarkMage,
    Ogre,
    Betsy,
    FlyingDutchman,
    MourningWood,
    Pumpking,
    Everscream,
    SantaNk1,
    IceQueen,
    MartianSaucer,
    SolarPillar,
    NebulaPillar,
    VortexPillar,
    StardustPillar,
}

public sealed class LightingTileEffectEntry
{
    public string Name;
    public List<int> TileIds;
    public bool Enabled;

    [ColorNoAlpha]
    public Color Color;

    public LightingTileEffectEntry()
    {
        Name = string.Empty;
        TileIds = [];
        Enabled = true;
        Color = Color.White;
    }

    public LightingTileEffectEntry(string name, IEnumerable<int> tileIds, Color color, bool enabled = true)
    {
        Name = name;
        TileIds = [..tileIds];
        Enabled = enabled;
        Color = color;
    }

    public LightingTileEffectEntry Clone()
    {
        return new LightingTileEffectEntry(Name, TileIds, Color, Enabled);
    }
}

public sealed class LightingEventEffectEntry
{
    // Legacy fallback value for pre-group configs.
    public LightingEventId EventId;
    public string Name;
    public List<LightingEventId> EventIds;
    public bool Enabled;

    [ColorNoAlpha]
    public Color Color;

    public LightingEventEffectEntry()
    {
        EventId = LightingEventId.BloodMoon;
        Name = string.Empty;
        EventIds = [LightingEventId.BloodMoon];
        Enabled = true;
        Color = Color.White;
    }

    public LightingEventEffectEntry(string name, IEnumerable<LightingEventId> eventIds, bool enabled, Color color)
    {
        Name = name;
        EventIds = [..eventIds];
        EventId = EventIds.Count > 0 ? EventIds[0] : LightingEventId.BloodMoon;
        Enabled = enabled;
        Color = color;
    }

    public LightingEventEffectEntry(LightingEventId eventId, bool enabled, Color color)
        : this(string.Empty, [eventId], enabled, color)
    {
        EventId = eventId;
    }

    public LightingEventEffectEntry Clone()
    {
        return new LightingEventEffectEntry(Name, EventIds, Enabled, Color)
        {
            EventId = EventId,
        };
    }
}

public sealed class LightingBossEffectEntry
{
    // Legacy fallback value for pre-group configs.
    public LightingBossId BossId;
    public string Name;
    public List<LightingBossId> BossIds;
    public List<string> TargetTileGroupKeys;
    public bool Enabled;
    public float Multiplier;

    public LightingBossEffectEntry()
    {
        BossId = LightingBossId.KingSlime;
        Name = string.Empty;
        BossIds = [LightingBossId.KingSlime];
        TargetTileGroupKeys = [];
        Enabled = true;
        Multiplier = 1.4f;
    }

    public LightingBossEffectEntry(string name, IEnumerable<LightingBossId> bossIds, bool enabled, float multiplier, IEnumerable<string> targetTileGroupKeys = null)
    {
        Name = name;
        BossIds = [..bossIds];
        BossId = BossIds.Count > 0 ? BossIds[0] : LightingBossId.KingSlime;
        TargetTileGroupKeys = targetTileGroupKeys is null ? [] : [..targetTileGroupKeys];
        Enabled = enabled;
        Multiplier = multiplier;
    }

    public LightingBossEffectEntry(LightingBossId bossId, bool enabled, float multiplier)
        : this(string.Empty, [bossId], enabled, multiplier)
    {
        BossId = bossId;
    }

    public LightingBossEffectEntry Clone()
    {
        return new LightingBossEffectEntry(Name, BossIds, Enabled, Multiplier, TargetTileGroupKeys)
        {
            BossId = BossId,
        };
    }
}

public sealed class LightingEntityEffectEntry
{
    public string Name;
    public bool Enabled;

    [ColorNoAlpha]
    public Color Color;

    public bool IncludePlayer;
    public bool IncludeAllEnemies;
    public bool IncludeAllProjectiles;
    public List<int> NpcIds;
    public List<int> ProjectileIds;

    public LightingEntityEffectEntry()
    {
        Name = string.Empty;
        Enabled = true;
        Color = Color.White;
        IncludePlayer = false;
        IncludeAllEnemies = false;
        IncludeAllProjectiles = false;
        NpcIds = [];
        ProjectileIds = [];
    }

    public LightingEntityEffectEntry(
        string name,
        bool enabled,
        Color color,
        bool includePlayer,
        bool includeAllEnemies,
        bool includeAllProjectiles,
        IEnumerable<int> npcIds,
        IEnumerable<int> projectileIds)
    {
        Name = name;
        Enabled = enabled;
        Color = color;
        IncludePlayer = includePlayer;
        IncludeAllEnemies = includeAllEnemies;
        IncludeAllProjectiles = includeAllProjectiles;
        NpcIds = npcIds is null ? [] : [..npcIds];
        ProjectileIds = projectileIds is null ? [] : [..projectileIds];
    }

    public LightingEntityEffectEntry Clone()
    {
        return new LightingEntityEffectEntry(
            Name,
            Enabled,
            Color,
            IncludePlayer,
            IncludeAllEnemies,
            IncludeAllProjectiles,
            NpcIds,
            ProjectileIds);
    }
}
