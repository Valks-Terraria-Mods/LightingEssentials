using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.Catalog;

internal static class LightingSettingsCatalog
{
    // Central descriptor registry grouped by tab to keep UI composition data-driven.
    private static readonly IReadOnlyDictionary<LightingSettingsTab, IReadOnlyList<LightingSettingDescriptor>> _tabDescriptors =
        new Dictionary<LightingSettingsTab, IReadOnlyList<LightingSettingDescriptor>>
        {
            [LightingSettingsTab.BossEffects] =
            [
                new BoolSettingDescriptor("King Slime Effect", static s => s.BossKingSlimeEffects, static (s, v) => s.BossKingSlimeEffects = v),
                new FloatSettingDescriptor("King Slime Multiplier", 1f, 2f, 0.05f, static s => s.BossKingSlimeEffectsMultiplier, static (s, v) => s.BossKingSlimeEffectsMultiplier = v),

                new BoolSettingDescriptor("Eye of Cthulhu Effect", static s => s.BossEyeofCthulhuEffects, static (s, v) => s.BossEyeofCthulhuEffects = v),
                new FloatSettingDescriptor("Eye of Cthulhu Multiplier", 1f, 2f, 0.05f, static s => s.BossEyeofCthulhuEffectsMultiplier, static (s, v) => s.BossEyeofCthulhuEffectsMultiplier = v),

                new BoolSettingDescriptor("Evil Biome Boss Effect", static s => s.BossEvilBiomeEffects, static (s, v) => s.BossEvilBiomeEffects = v),
                new FloatSettingDescriptor("Evil Biome Multiplier", 1f, 2f, 0.05f, static s => s.BossEvilBiomeEffectsMultiplier, static (s, v) => s.BossEvilBiomeEffectsMultiplier = v),

                new BoolSettingDescriptor("Queen Bee Effect", static s => s.BossQueenBeeEffects, static (s, v) => s.BossQueenBeeEffects = v),
                new FloatSettingDescriptor("Queen Bee Multiplier", 1f, 2f, 0.05f, static s => s.BossQueenBeeEffectsMultiplier, static (s, v) => s.BossQueenBeeEffectsMultiplier = v),

                new BoolSettingDescriptor("Skeletron Effect", static s => s.BossSkeletronEffects, static (s, v) => s.BossSkeletronEffects = v),
                new FloatSettingDescriptor("Skeletron Multiplier", 1f, 2f, 0.05f, static s => s.BossSkeletronEffectsMultiplier, static (s, v) => s.BossSkeletronEffectsMultiplier = v),

                new BoolSettingDescriptor("Deerclops Effect", static s => s.BossDeerclopsEffects, static (s, v) => s.BossDeerclopsEffects = v),
                new FloatSettingDescriptor("Deerclops Multiplier", 1f, 2f, 0.05f, static s => s.BossDeerclopsEffectsMultiplier, static (s, v) => s.BossDeerclopsEffectsMultiplier = v),

                new BoolSettingDescriptor("Wall of Flesh Effect", static s => s.BossWallOfFleshEffects, static (s, v) => s.BossWallOfFleshEffects = v),
                new FloatSettingDescriptor("Wall of Flesh Multiplier", 1f, 2f, 0.05f, static s => s.BossWallOfFleshEffectsMultiplier, static (s, v) => s.BossWallOfFleshEffectsMultiplier = v),

                new BoolSettingDescriptor("Queen Slime Effect", static s => s.BossQueenSlimeEffects, static (s, v) => s.BossQueenSlimeEffects = v),
                new FloatSettingDescriptor("Queen Slime Multiplier", 1f, 2f, 0.05f, static s => s.BossQueenSlimeEffectsMultiplier, static (s, v) => s.BossQueenSlimeEffectsMultiplier = v),

                new BoolSettingDescriptor("Mech Boss Effect", static s => s.BossMechEffects, static (s, v) => s.BossMechEffects = v),
                new FloatSettingDescriptor("Mech Boss Multiplier", 1f, 2f, 0.05f, static s => s.BossMechEffectsMultiplier, static (s, v) => s.BossMechEffectsMultiplier = v),

                new BoolSettingDescriptor("Plantera Effect", static s => s.BossPlanteraEffects, static (s, v) => s.BossPlanteraEffects = v),
                new FloatSettingDescriptor("Plantera Multiplier", 1f, 2f, 0.05f, static s => s.BossPlanteraEffectsMultiplier, static (s, v) => s.BossPlanteraEffectsMultiplier = v),

                new BoolSettingDescriptor("Golem Effect", static s => s.BossGolemEffects, static (s, v) => s.BossGolemEffects = v),
                new FloatSettingDescriptor("Golem Multiplier", 1f, 2f, 0.05f, static s => s.BossGolemEffectsMultiplier, static (s, v) => s.BossGolemEffectsMultiplier = v),

                new BoolSettingDescriptor("Duke Fishron Effect", static s => s.BossDukeFishronEffects, static (s, v) => s.BossDukeFishronEffects = v),
                new FloatSettingDescriptor("Duke Fishron Multiplier", 1f, 2f, 0.05f, static s => s.BossDukeFishronEffectsMultiplier, static (s, v) => s.BossDukeFishronEffectsMultiplier = v),

                new BoolSettingDescriptor("Empress of Light Effect", static s => s.BossEmpressOfLightEffects, static (s, v) => s.BossEmpressOfLightEffects = v),
                new FloatSettingDescriptor("Empress of Light Multiplier", 1f, 2f, 0.05f, static s => s.BossEmpressOfLightEffectsMultiplier, static (s, v) => s.BossEmpressOfLightEffectsMultiplier = v),

                new BoolSettingDescriptor("Lunatic Cultist Effect", static s => s.BossLunaticCultistEffects, static (s, v) => s.BossLunaticCultistEffects = v),
                new FloatSettingDescriptor("Lunatic Cultist Multiplier", 1f, 2f, 0.05f, static s => s.BossLunaticCultistEffectsMultiplier, static (s, v) => s.BossLunaticCultistEffectsMultiplier = v),

                new BoolSettingDescriptor("Moon Lord Effect", static s => s.BossMoonLordEffects, static (s, v) => s.BossMoonLordEffects = v),
                new FloatSettingDescriptor("Moon Lord Multiplier", 1f, 2f, 0.05f, static s => s.BossMoonLordEffectsMultiplier, static (s, v) => s.BossMoonLordEffectsMultiplier = v),
            ],

            [LightingSettingsTab.Events] =
            [
                new BoolSettingDescriptor("Blood Moon Effect", static s => s.BloodMoonEventEffects, static (s, v) => s.BloodMoonEventEffects = v),
                new ColorSettingDescriptor("Blood Moon Color", static s => s.BloodMoonEventColor, static (s, v) => s.BloodMoonEventColor = v),

                new BoolSettingDescriptor("Solar Eclipse Effect", static s => s.SolarEclipseEventEffects, static (s, v) => s.SolarEclipseEventEffects = v),
                new ColorSettingDescriptor("Solar Eclipse Color", static s => s.SolarEclipseEventColor, static (s, v) => s.SolarEclipseEventColor = v),

                new BoolSettingDescriptor("Frost Legion Effect", static s => s.FrostLegionEventEffects, static (s, v) => s.FrostLegionEventEffects = v),
                new ColorSettingDescriptor("Frost Legion Color", static s => s.FrostLegionEventColor, static (s, v) => s.FrostLegionEventColor = v),
            ],

            [LightingSettingsTab.TileEffects] =
            [
                new ColorSettingDescriptor("Grass", static s => s.Grass, static (s, v) => s.Grass = v),
                new ColorSettingDescriptor("Plants", static s => s.Plants, static (s, v) => s.Plants = v),
                new ColorSettingDescriptor("Containers", static s => s.Containers, static (s, v) => s.Containers = v),
                new ColorSettingDescriptor("Pots", static s => s.Pots, static (s, v) => s.Pots = v),
                new ColorSettingDescriptor("Cactus", static s => s.Cactus, static (s, v) => s.Cactus = v),
                new ColorSettingDescriptor("Hallowed Flora", static s => s.HallowedFlora, static (s, v) => s.HallowedFlora = v),
                new ColorSettingDescriptor("Mushroom Flora", static s => s.MushroomFlora, static (s, v) => s.MushroomFlora = v),
                new ColorSettingDescriptor("Herb Flora", static s => s.HerbFlora, static (s, v) => s.HerbFlora = v),
                new ColorSettingDescriptor("Aquatic Flora", static s => s.AquaticFlora, static (s, v) => s.AquaticFlora = v),
                new ColorSettingDescriptor("Sunflower Flora", static s => s.SunflowerFlora, static (s, v) => s.SunflowerFlora = v),
                new ColorSettingDescriptor("Ash Flora", static s => s.AshFlora, static (s, v) => s.AshFlora = v),
                new ColorSettingDescriptor("Bamboo Flora", static s => s.BambooFlora, static (s, v) => s.BambooFlora = v),
                new ColorSettingDescriptor("Exotic Moss", static s => s.ExoticMoss, static (s, v) => s.ExoticMoss = v),
                new ColorSettingDescriptor("Jungle Biome", static s => s.JungleBiome, static (s, v) => s.JungleBiome = v),
                new ColorSettingDescriptor("Snow Biome", static s => s.SnowBiome, static (s, v) => s.SnowBiome = v),
                new ColorSettingDescriptor("Desert Biome", static s => s.DesertBiome, static (s, v) => s.DesertBiome = v),
                new ColorSettingDescriptor("Corruption Biome", static s => s.CorruptionBiome, static (s, v) => s.CorruptionBiome = v),
                new ColorSettingDescriptor("Crimson Biome", static s => s.CrimsonBiome, static (s, v) => s.CrimsonBiome = v),
                new ColorSettingDescriptor("Life Crystal", static s => s.LifeCrystal, static (s, v) => s.LifeCrystal = v),
                new ColorSettingDescriptor("Mana Crystal", static s => s.ManaCrystal, static (s, v) => s.ManaCrystal = v),
                new ColorSettingDescriptor("Life Fruit", static s => s.LifeFruit, static (s, v) => s.LifeFruit = v),
                new ColorSettingDescriptor("Red Moss", static s => s.RedMoss, static (s, v) => s.RedMoss = v),
                new ColorSettingDescriptor("Purple Moss", static s => s.PurpleMoss, static (s, v) => s.PurpleMoss = v),
                new ColorSettingDescriptor("Long Moss", static s => s.LongMoss, static (s, v) => s.LongMoss = v),
                new ColorSettingDescriptor("Lava Moss", static s => s.LavaMoss, static (s, v) => s.LavaMoss = v),
                new ColorSettingDescriptor("Green Moss", static s => s.GreenMoss, static (s, v) => s.GreenMoss = v),
                new ColorSettingDescriptor("Brown Moss", static s => s.BrownMoss, static (s, v) => s.BrownMoss = v),
                new ColorSettingDescriptor("Blue Moss", static s => s.BlueMoss, static (s, v) => s.BlueMoss = v),
                new ColorSettingDescriptor("Lunar Ore", static s => s.LunarOre, static (s, v) => s.LunarOre = v),
                new ColorSettingDescriptor("Titanium", static s => s.Titanium, static (s, v) => s.Titanium = v),
                new ColorSettingDescriptor("Adamantite", static s => s.Adamantite, static (s, v) => s.Adamantite = v),
                new ColorSettingDescriptor("Orichalcum", static s => s.Orichalcum, static (s, v) => s.Orichalcum = v),
                new ColorSettingDescriptor("Mythril", static s => s.Mythril, static (s, v) => s.Mythril = v),
                new ColorSettingDescriptor("Palladium", static s => s.Palladium, static (s, v) => s.Palladium = v),
                new ColorSettingDescriptor("Cobalt", static s => s.Cobalt, static (s, v) => s.Cobalt = v),
                new ColorSettingDescriptor("Hellstone", static s => s.Hellstone, static (s, v) => s.Hellstone = v),
                new ColorSettingDescriptor("Chlorophyte", static s => s.Chlorophyte, static (s, v) => s.Chlorophyte = v),
                new ColorSettingDescriptor("Meteorite", static s => s.Meteorite, static (s, v) => s.Meteorite = v),
                new ColorSettingDescriptor("Iron", static s => s.Iron, static (s, v) => s.Iron = v),
                new ColorSettingDescriptor("Lead", static s => s.Lead, static (s, v) => s.Lead = v),
                new ColorSettingDescriptor("Copper", static s => s.Copper, static (s, v) => s.Copper = v),
                new ColorSettingDescriptor("Tin", static s => s.Tin, static (s, v) => s.Tin = v),
                new ColorSettingDescriptor("Silver", static s => s.Silver, static (s, v) => s.Silver = v),
                new ColorSettingDescriptor("Gold", static s => s.Gold, static (s, v) => s.Gold = v),
                new ColorSettingDescriptor("Platinum", static s => s.Platinum, static (s, v) => s.Platinum = v),
                new ColorSettingDescriptor("Tungsten", static s => s.Tungsten, static (s, v) => s.Tungsten = v),
                new ColorSettingDescriptor("Amethyst", static s => s.Amethyst, static (s, v) => s.Amethyst = v),
                new ColorSettingDescriptor("Topaz", static s => s.Topaz, static (s, v) => s.Topaz = v),
                new ColorSettingDescriptor("Emerald", static s => s.Emerald, static (s, v) => s.Emerald = v),
                new ColorSettingDescriptor("Amber Gemspark", static s => s.AmberGemspark, static (s, v) => s.AmberGemspark = v),
                new ColorSettingDescriptor("Diamond", static s => s.Diamond, static (s, v) => s.Diamond = v),
                new ColorSettingDescriptor("Ruby", static s => s.Ruby, static (s, v) => s.Ruby = v),
                new ColorSettingDescriptor("Sapphire", static s => s.Sapphire, static (s, v) => s.Sapphire = v),
            ],

            [LightingSettingsTab.EntityLights] =
            [
                new BoolSettingDescriptor("Player Light Enabled", static s => s.PlayerLightEnabled, static (s, v) => s.PlayerLightEnabled = v),
                new ColorSettingDescriptor("Player Light Color", static s => s.PlayerLight, static (s, v) => s.PlayerLight = v),
                new BoolSettingDescriptor("Projectile Light Enabled", static s => s.ProjectileLightEnabled, static (s, v) => s.ProjectileLightEnabled = v),
                new ColorSettingDescriptor("Projectile Light Color", static s => s.ProjectileLightColor, static (s, v) => s.ProjectileLightColor = v),
                new BoolSettingDescriptor("Enemy Light Enabled", static s => s.EnemyLightEnabled, static (s, v) => s.EnemyLightEnabled = v),
                new ColorSettingDescriptor("Enemy Light Color", static s => s.EnemyLightColor, static (s, v) => s.EnemyLightColor = v),
            ],
        };

    /// <summary>
    /// Returns descriptor definitions for a specific settings tab.
    /// </summary>
    /// <param name="tab">Tab whose descriptors should be returned.</param>
    /// <returns>Ordered descriptor list for the requested tab.</returns>
    public static IReadOnlyList<LightingSettingDescriptor> GetTabDescriptors(LightingSettingsTab tab)
    {
        return _tabDescriptors[tab];
    }

    /// <summary>
    /// Returns display title text for a tab identifier.
    /// </summary>
    /// <param name="tab">Tab enum value.</param>
    /// <returns>User-facing title string.</returns>
    public static string GetTabTitle(LightingSettingsTab tab)
    {
        return tab switch
        {
            LightingSettingsTab.TileEffects => "Tile Effects",
            LightingSettingsTab.Events => "Events",
            LightingSettingsTab.EntityLights => "Entity Lights",
            LightingSettingsTab.BossEffects => "Boss Effects",
            _ => "Settings",
        };
    }
}
