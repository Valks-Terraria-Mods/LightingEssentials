using System;

namespace LightingEssentials;

internal readonly record struct TileLightEntry(int TileId, Func<LightingSettings, Color> SelectColor);
internal readonly record struct TileLightGroupEntry(int[] TileIds, Func<LightingSettings, Color> SelectColor);

internal static class TileLightBaseEntries
{
    public static readonly TileLightEntry[] SingleTileEntries =
    [
        new(TileID.Sapphire, static c => c.Sapphire),
        new(TileID.Ruby, static c => c.Ruby),
        new(TileID.Diamond, static c => c.Diamond),
        new(TileID.AmberGemspark, static c => c.AmberGemspark),
        new(TileID.Emerald, static c => c.Emerald),
        new(TileID.Topaz, static c => c.Topaz),
        new(TileID.Amethyst, static c => c.Amethyst),
        new(TileID.Iron, static c => c.Iron),
        new(TileID.Lead, static c => c.Lead),
        new(TileID.Copper, static c => c.Copper),
        new(TileID.Tin, static c => c.Tin),
        new(TileID.Silver, static c => c.Silver),
        new(TileID.Gold, static c => c.Gold),
        new(TileID.Platinum, static c => c.Platinum),
        new(TileID.Tungsten, static c => c.Tungsten),
        new(TileID.Meteorite, static c => c.Meteorite),
        new(TileID.Chlorophyte, static c => c.Chlorophyte),
        new(TileID.Hellstone, static c => c.Hellstone),
        new(TileID.Cobalt, static c => c.Cobalt),
        new(TileID.Palladium, static c => c.Palladium),
        new(TileID.Mythril, static c => c.Mythril),
        new(TileID.Orichalcum, static c => c.Orichalcum),
        new(TileID.Adamantite, static c => c.Adamantite),
        new(TileID.Titanium, static c => c.Titanium),
        new(TileID.LunarOre, static c => c.LunarOre),

        new(TileID.Grass, static c => c.Grass),
        new(TileID.CrimsonGrass, static c => c.CrimsonBiome),
        new(TileID.CrimsonJungleGrass, static c => c.CrimsonBiome),
        new(TileID.CrimsonPlants, static c => c.CrimsonBiome),
        new(TileID.CrimsonThorns, static c => c.CrimsonBiome),
        new(TileID.CrimsonVines, static c => c.CrimsonBiome),
        new(TileID.CorruptGrass, static c => c.CorruptionBiome),
        new(TileID.Sunflower, static c => c.SunflowerFlora),
        new(TileID.Pots, static c => c.Pots),
        new(TileID.FossilOre, static c => c.DesertBiome),
        new(TileID.IceBlock, static c => c.SnowBiome),
        new(TileID.BlueMoss, static c => c.BlueMoss),
        new(TileID.BrownMoss, static c => c.BrownMoss),
        new(TileID.GreenMoss, static c => c.GreenMoss),
        new(TileID.LavaMoss, static c => c.LavaMoss),
        new(TileID.LongMoss, static c => c.LongMoss),
        new(TileID.PurpleMoss, static c => c.PurpleMoss),
        new(TileID.RedMoss, static c => c.RedMoss),
        new(TileID.LifeFruit, static c => c.LifeFruit),
        new(TileID.Heart, static c => c.LifeCrystal),
        new(TileID.ManaCrystal, static c => c.ManaCrystal),
        new(TileID.JungleGrass, static c => c.JungleBiome),
        new(TileID.JunglePlants, static c => c.JungleBiome),
        new(TileID.JunglePlants2, static c => c.JungleBiome),
        new(TileID.JungleThorns, static c => c.JungleBiome),
        new(TileID.JungleVines, static c => c.JungleBiome),
        new(TileID.LargePiles, static c => c.Containers),
        new(TileID.LargePiles2, static c => c.Containers),
        new(TileID.Containers, static c => c.Containers),
        new(TileID.Containers2, static c => c.Containers),
        new(TileID.Plants, static c => c.Plants),
        new(TileID.Plants2, static c => c.Plants),
        new(TileID.Cactus, static c => c.Cactus),
    ];

    public static readonly TileLightGroupEntry[] GroupEntries =
    [
        new(TileLightGroups.CorruptionFloraTiles, static c => c.CorruptionBiome),
        new(TileLightGroups.HallowedFloraTiles, static c => c.HallowedFlora),
        new(TileLightGroups.MushroomFloraTiles, static c => c.MushroomFlora),
        new(TileLightGroups.HerbFloraTiles, static c => c.HerbFlora),
        new(TileLightGroups.AquaticFloraTiles, static c => c.AquaticFlora),
        new(TileLightGroups.AshFloraTiles, static c => c.AshFlora),
        new(TileLightGroups.BambooFloraTiles, static c => c.BambooFlora),
        new(TileLightGroups.ExoticMossTiles, static c => c.ExoticMoss),
        new(TileLightGroups.JungleRareFloraTiles, static c => c.JungleBiome),
    ];
}