using System;

namespace LightingEssentials;

internal static class TileBossEffectApplier
{
    private const float MinBossMultiplier = 1f;
    private const float MaxBossMultiplier = 4f;

    public static void Apply(Config config, in WorldLightingState state)
    {
        TryBrighten(config.BossKingSlimeEffects, state.DownedKingSlime, config.BossKingSlimeEffectsMultiplier, TileLightGroups.SurfaceGrowthTiles);
        TryBrighten(config.BossEyeofCthulhuEffects, state.DownedEyeOfCthulhu, config.BossEyeofCthulhuEffectsMultiplier, TileLightGroups.SurfaceGrowthTiles, TileLightGroups.HerbFloraTiles);
        TryBrighten(config.BossEvilBiomeEffects, state.DownedEvilBoss, config.BossEvilBiomeEffectsMultiplier, TileLightGroups.EvilBiomeTiles, TileLightGroups.CorruptionFloraTiles);
        TryBrighten(config.BossQueenBeeEffects, state.DownedQueenBee, config.BossQueenBeeEffectsMultiplier, TileLightGroups.JungleTiles, TileLightGroups.JungleRareFloraTiles);
        TryBrighten(config.BossSkeletronEffects, state.DownedSkeletron, config.BossSkeletronEffectsMultiplier, TileLightGroups.UnderworldOreTiles);
        TryBrighten(config.BossDeerclopsEffects, state.DownedDeerclops, config.BossDeerclopsEffectsMultiplier, TileLightGroups.SnowTiles);
        TryBrighten(config.BossWallOfFleshEffects, state.HardModeUnlocked, config.BossWallOfFleshEffectsMultiplier, TileLightGroups.UnderworldOreTiles, TileLightGroups.AshFloraTiles);
        TryBrighten(config.BossQueenSlimeEffects, state.DownedQueenSlime, config.BossQueenSlimeEffectsMultiplier, TileLightGroups.HardmodeProgressionTiles, TileLightGroups.HallowedFloraTiles, TileLightGroups.MushroomFloraTiles);

        if (config.BossMechEffects && state.MechBossesDowned > 0)
        {
            float mechMaxMultiplier = ClampBossMultiplier(config.BossMechEffectsMultiplier);
            float mechProgress = state.MechBossesDowned / 3f;
            float mechMultiplier = 1f + ((mechMaxMultiplier - 1f) * mechProgress);
            TileLightStore.Brighten(TileLightGroups.HardmodeOreTiles, mechMultiplier);
        }

        TryBrighten(config.BossPlanteraEffects, state.DownedPlantera, config.BossPlanteraEffectsMultiplier, TileLightGroups.JungleTiles, TileLightGroups.JungleRareFloraTiles, TileLightGroups.HerbFloraTiles);
        TryBrighten(config.BossGolemEffects, state.DownedGolem, config.BossGolemEffectsMultiplier, TileLightGroups.ChlorophyteTiles);
        TryBrighten(config.BossDukeFishronEffects, state.DownedFishron, config.BossDukeFishronEffectsMultiplier, TileLightGroups.GemTiles, TileLightGroups.AquaticFloraTiles);
        TryBrighten(config.BossEmpressOfLightEffects, state.DownedEmpressOfLight, config.BossEmpressOfLightEffectsMultiplier, TileLightGroups.CrystalTiles, TileLightGroups.HallowedFloraTiles);
        TryBrighten(config.BossLunaticCultistEffects, state.DownedLunaticCultist, config.BossLunaticCultistEffectsMultiplier, TileLightGroups.LunarOreTiles);
        TryBrighten(config.BossMoonLordEffects, state.DownedMoonLord, config.BossMoonLordEffectsMultiplier, TileLightGroups.LunarOreTiles, TileLightGroups.ExoticMossTiles);
    }

    private static void TryBrighten(bool effectEnabled, bool triggerActive, float multiplier, params int[][] targetTileGroups)
    {
        if (!effectEnabled || !triggerActive)
            return;

        float clampedMultiplier = ClampBossMultiplier(multiplier);
        for (int i = 0; i < targetTileGroups.Length; i++)
        {
            TileLightStore.Brighten(targetTileGroups[i], clampedMultiplier);
        }
    }

    private static float ClampBossMultiplier(float multiplier)
    {
        return Math.Clamp(multiplier, MinBossMultiplier, MaxBossMultiplier);
    }
}