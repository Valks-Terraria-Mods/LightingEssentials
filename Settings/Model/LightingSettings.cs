using System;
using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ID;
using Terraria.ModLoader.Config;

namespace LightingEssentials;

[BackgroundColor(0, 0, 0, 100)]
public class LightingSettings : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ClientSide;

    [DefaultValue(true)] 
    [BackgroundColor(0, 0, 0, 100)]
    public bool ModEnabled;

    [Range(1f, 1.10f)]
    [Increment(0.025f)]
    [DefaultValue(1f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float UiScale;

    public List<LightingTileEffectEntry> TileEffectEntries;
    public List<LightingEventEffectEntry> EventEffectEntries;
    public List<LightingBossEffectEntry> BossEffectEntries;
    public List<LightingEntityEffectEntry> EntityEffectEntries;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "7, 7, 7, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color PlayerLight;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool PlayerLightEnabled;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossKingSlimeEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossKingSlimeEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossEyeofCthulhuEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossEyeofCthulhuEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossEvilBiomeEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossEvilBiomeEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossQueenBeeEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossQueenBeeEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossSkeletronEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossSkeletronEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossDeerclopsEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossDeerclopsEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossWallOfFleshEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossWallOfFleshEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossQueenSlimeEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossQueenSlimeEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossMechEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossMechEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossPlanteraEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.5f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossPlanteraEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossGolemEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossGolemEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossDukeFishronEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossDukeFishronEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossEmpressOfLightEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossEmpressOfLightEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossLunaticCultistEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossLunaticCultistEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BossMoonLordEffects;

    [Range(1f, 4f)]
    [Increment(0.05f)]
    [DefaultValue(1.4f)]
    [BackgroundColor(0, 0, 0, 100)]
    public float BossMoonLordEffectsMultiplier;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool BloodMoonEventEffects;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "255, 35, 35, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color BloodMoonEventColor;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool SolarEclipseEventEffects;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "255, 130, 35, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color SolarEclipseEventColor;

    [DefaultValue(true)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool FrostLegionEventEffects;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "90, 160, 255, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color FrostLegionEventColor;

    [DefaultValue(false)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool ProjectileLightEnabled;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 10, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color ProjectileLightColor;

    [DefaultValue(false)]
    [BackgroundColor(0, 0, 0, 100)]
    public bool EnemyLightEnabled;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "35, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color EnemyLightColor;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 30, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Grass;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 30, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Plants;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "7, 7, 7, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Containers;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "7, 7, 7, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Pots;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "20, 40, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Cactus;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "30, 16, 40, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color HallowedFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 18, 32, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color MushroomFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "18, 30, 8, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color HerbFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 24, 22, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color AquaticFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "34, 30, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color SunflowerFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "18, 12, 8, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color AshFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "5, 22, 8, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color BambooFlora;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "12, 12, 20, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color ExoticMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 20, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color JungleBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 0, 20, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color SnowBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "20, 20, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color DesertBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "40, 0, 40, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color CorruptionBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "40, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color CrimsonBiome;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "255, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LifeCrystal;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 0, 255, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color ManaCrystal;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 255, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LifeFruit;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color RedMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 0, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color PurpleMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 10, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LongMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LavaMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 10, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color GreenMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "10, 10, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color BrownMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 0, 10, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color BlueMoss;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color LunarOre;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Titanium;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Adamantite;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Orichalcum;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Mythril;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Palladium;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Cobalt;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "50, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Hellstone;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "0, 3, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Chlorophyte;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "255, 0, 0, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Meteorite;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Iron;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Lead;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Copper;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Tin;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Silver;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Gold;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Platinum;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Tungsten;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Amethyst;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Topaz;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Emerald;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color AmberGemspark;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Diamond;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Ruby;

    [ColorNoAlpha]
    [DefaultValue(typeof(Color), "3, 3, 3, 255")]
    [BackgroundColor(0, 0, 0, 100)]
    public Color Sapphire;

    /// <summary>
    /// Ensures dynamic settings collections are initialized and valid.
    /// </summary>
    public void EnsureDynamicEntries()
    {
        if (TileEffectEntries is null)
        {
            TileEffectEntries = LightingDynamicCatalogs.CreateDefaultTileEntries();
        }
        else
        {
            TileEffectEntries = SanitizeTileEntries(TileEffectEntries);
        }

        if (EventEffectEntries is null)
        {
            EventEffectEntries = LightingDynamicCatalogs.CreateDefaultEventEntries();
        }
        else
        {
            EventEffectEntries = SanitizeEventEntries(EventEffectEntries);
        }

        if (BossEffectEntries is null)
        {
            BossEffectEntries = LightingDynamicCatalogs.CreateDefaultBossEntries();
        }
        else
        {
            BossEffectEntries = SanitizeBossEntries(BossEffectEntries);
        }

        if (EntityEffectEntries is null)
        {
            EntityEffectEntries = LightingDynamicCatalogs.CreateDefaultEntityEntries(this);
        }
        else
        {
            EntityEffectEntries = SanitizeEntityEntries(EntityEffectEntries);
        }
    }

    /// <summary>
    /// Replaces all dynamic entries with built-in defaults.
    /// </summary>
    public void ResetDynamicEntriesToDefaults()
    {
        TileEffectEntries = LightingDynamicCatalogs.CreateDefaultTileEntries();
        EventEffectEntries = LightingDynamicCatalogs.CreateDefaultEventEntries();
        BossEffectEntries = LightingDynamicCatalogs.CreateDefaultBossEntries();
        EntityEffectEntries = LightingDynamicCatalogs.CreateDefaultEntityEntries(this);
    }

    private static List<LightingTileEffectEntry> SanitizeTileEntries(List<LightingTileEffectEntry> source)
    {
        List<LightingTileEffectEntry> sanitized = [];
        HashSet<int> seenTileIds = [];

        for (int i = 0; i < source.Count; i++)
        {
            LightingTileEffectEntry entry = source[i];
            if (entry is null || entry.TileIds is null || entry.TileIds.Count == 0)
                continue;

            List<int> tileIds = [];
            for (int j = 0; j < entry.TileIds.Count; j++)
            {
                int tileId = entry.TileIds[j];
                if (tileId < 0 || tileId >= TileLoader.TileCount)
                    continue;

                if (!seenTileIds.Add(tileId))
                    continue;

                tileIds.Add(tileId);
            }

            if (tileIds.Count == 0)
                continue;

            string name = string.IsNullOrWhiteSpace(entry.Name)
                ? LightingDynamicCatalogs.TryGetTileCatalogItem(tileIds[0], out LightingTileCatalogItem item)
                    ? item.DisplayName
                    : "Tile Group"
                : entry.Name.Trim();

            sanitized.Add(new LightingTileEffectEntry(name, tileIds, entry.Color, entry.Enabled));
        }

        return sanitized;
    }

    private static List<LightingEventEffectEntry> SanitizeEventEntries(List<LightingEventEffectEntry> source)
    {
        List<LightingEventEffectEntry> sanitized = [];
        HashSet<LightingEventId> seen = [];

        for (int i = 0; i < source.Count; i++)
        {
            LightingEventEffectEntry entry = source[i];
            if (entry is null)
                continue;

            List<LightingEventId> eventIds = [];
            if (entry.EventIds is not null)
            {
                for (int j = 0; j < entry.EventIds.Count; j++)
                {
                    LightingEventId eventId = entry.EventIds[j];
                    if (!LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out _))
                        continue;

                    if (!seen.Add(eventId))
                        continue;

                    eventIds.Add(eventId);
                }
            }

            if (eventIds.Count == 0 && LightingDynamicCatalogs.TryGetEventCatalogItem(entry.EventId, out _)
                && seen.Add(entry.EventId))
            {
                eventIds.Add(entry.EventId);
            }

            if (eventIds.Count == 0)
                continue;

            LightingEventId firstEventId = eventIds[0];
            string fallbackName = LightingDynamicCatalogs.TryGetEventCatalogItem(firstEventId, out LightingEventCatalogItem item)
                ? item.DisplayName
                : "Event Group";

            string name = string.IsNullOrWhiteSpace(entry.Name)
                ? fallbackName
                : entry.Name.Trim();

            sanitized.Add(new LightingEventEffectEntry(name, eventIds, entry.Enabled, entry.Color));
        }

        return sanitized;
    }

    private static List<LightingBossEffectEntry> SanitizeBossEntries(List<LightingBossEffectEntry> source)
    {
        List<LightingBossEffectEntry> sanitized = [];
        HashSet<LightingBossId> seen = [];

        for (int i = 0; i < source.Count; i++)
        {
            LightingBossEffectEntry entry = source[i];
            if (entry is null)
                continue;

            List<LightingBossId> bossIds = [];
            if (entry.BossIds is not null)
            {
                for (int j = 0; j < entry.BossIds.Count; j++)
                {
                    LightingBossId bossId = entry.BossIds[j];
                    if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out _))
                        continue;

                    if (!seen.Add(bossId))
                        continue;

                    bossIds.Add(bossId);
                }
            }

            if (bossIds.Count == 0 && LightingDynamicCatalogs.TryGetBossCatalogItem(entry.BossId, out _)
                && seen.Add(entry.BossId))
            {
                bossIds.Add(entry.BossId);
            }

            if (bossIds.Count == 0)
                continue;

            LightingBossId firstBossId = bossIds[0];
            string fallbackName = LightingDynamicCatalogs.TryGetBossCatalogItem(firstBossId, out LightingBossCatalogItem item)
                ? item.DisplayName
                : "Boss Group";

            string name = string.IsNullOrWhiteSpace(entry.Name)
                ? fallbackName
                : entry.Name.Trim();

            float multiplier = Math.Clamp(entry.Multiplier, 1f, 2f);
            sanitized.Add(new LightingBossEffectEntry(name, bossIds, entry.Enabled, multiplier));
        }

        return sanitized;
    }

    private static List<LightingEntityEffectEntry> SanitizeEntityEntries(List<LightingEntityEffectEntry> source)
    {
        List<LightingEntityEffectEntry> sanitized = [];
        HashSet<int> seenNpcIds = [];
        HashSet<int> seenProjectileIds = [];

        bool playerUsed = false;
        bool allEnemiesUsed = false;
        bool allProjectilesUsed = false;

        for (int i = 0; i < source.Count; i++)
        {
            LightingEntityEffectEntry entry = source[i];
            if (entry is null)
                continue;

            bool includePlayer = entry.IncludePlayer && !playerUsed;
            if (includePlayer)
                playerUsed = true;

            bool includeAllEnemies = entry.IncludeAllEnemies && !allEnemiesUsed;
            if (includeAllEnemies)
                allEnemiesUsed = true;

            bool includeAllProjectiles = entry.IncludeAllProjectiles && !allProjectilesUsed;
            if (includeAllProjectiles)
                allProjectilesUsed = true;

            List<int> npcIds = [];
            if (entry.NpcIds is not null)
            {
                for (int j = 0; j < entry.NpcIds.Count; j++)
                {
                    int npcId = entry.NpcIds[j];
                    if (npcId < 0 || npcId >= NPCID.Count)
                        continue;

                    if (!seenNpcIds.Add(npcId))
                        continue;

                    npcIds.Add(npcId);
                }
            }

            List<int> projectileIds = [];
            if (entry.ProjectileIds is not null)
            {
                for (int j = 0; j < entry.ProjectileIds.Count; j++)
                {
                    int projectileId = entry.ProjectileIds[j];
                    if (projectileId < 0 || projectileId >= ProjectileID.Count)
                        continue;

                    if (!seenProjectileIds.Add(projectileId))
                        continue;

                    projectileIds.Add(projectileId);
                }
            }

            if (!includePlayer && !includeAllEnemies && !includeAllProjectiles && npcIds.Count == 0 && projectileIds.Count == 0)
                continue;

            string fallbackName = ResolveEntityFallbackName(includePlayer, includeAllEnemies, includeAllProjectiles, npcIds, projectileIds);
            string name = string.IsNullOrWhiteSpace(entry.Name)
                ? fallbackName
                : entry.Name.Trim();

            sanitized.Add(new LightingEntityEffectEntry(
                name,
                entry.Enabled,
                entry.Color,
                includePlayer,
                includeAllEnemies,
                includeAllProjectiles,
                npcIds,
                projectileIds));
        }

        return sanitized;
    }

    private static string ResolveEntityFallbackName(bool includePlayer, bool includeAllEnemies, bool includeAllProjectiles, IReadOnlyList<int> npcIds, IReadOnlyList<int> projectileIds)
    {
        if (includePlayer)
            return "Player";

        if (includeAllEnemies)
            return "Enemies";

        if (includeAllProjectiles)
            return "Projectiles";

        if (npcIds.Count > 0 && LightingDynamicCatalogs.TryGetEntityCatalogItem($"entity:npc:{npcIds[0]}", out LightingEntityCatalogItem npcItem))
            return npcItem.DisplayName;

        if (projectileIds.Count > 0 && LightingDynamicCatalogs.TryGetEntityCatalogItem($"entity:projectile:{projectileIds[0]}", out LightingEntityCatalogItem projectileItem))
            return projectileItem.DisplayName;

        return "Entity Group";
    }

    /// <summary>
    /// Pushes current settings values into runtime systems and refreshes tile lighting.
    /// </summary>
    public void ApplyRuntimeChanges()
    {
        LightingEssentials.Config = this;
        LightRuntime.ApplyConfig(this);
        LightTiles.InitLight();
    }

    /// <summary>
    /// Called by tModLoader when config data is first loaded.
    /// </summary>
    public override void OnLoaded()
    {
        EnsureDynamicEntries();
        ApplyRuntimeChanges();
    }

    /// <summary>
    /// Called by tModLoader whenever config values change.
    /// </summary>
    public override void OnChanged()
    {
        EnsureDynamicEntries();
        ApplyRuntimeChanges();
    }
}
