using System;
using System.Collections.Generic;

namespace LightingEssentials;

internal static class TileBossEffectApplier
{
    private const float MinBossMultiplier = 1f;
    private const float MaxBossMultiplier = 2f;

    public static void Apply(LightingSettings config, in WorldLightingState state)
    {
        List<LightingBossEffectEntry> entries = config.BossEffectEntries;
        for (int i = 0; i < entries.Count; i++)
        {
            LightingBossEffectEntry entry = entries[i];
            if (entry is null || !entry.Enabled)
                continue;

            if (entry.BossIds is null || entry.BossIds.Count == 0)
            {
                TryApplyBossEffect(entry.BossId, entry.Multiplier, state);
                continue;
            }

            for (int j = 0; j < entry.BossIds.Count; j++)
            {
                TryApplyBossEffect(entry.BossIds[j], entry.Multiplier, state);
            }
        }
    }

    private static void TryApplyBossEffect(LightingBossId bossId, float configuredMultiplier, in WorldLightingState state)
    {
        if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out _))
            return;

        if (!LightingDynamicCatalogs.IsBossTriggered(bossId, state))
            return;

        int[][] targetTileGroups = LightingDynamicCatalogs.GetBossTargetTileGroups(bossId);
        if (targetTileGroups.Length == 0)
            return;

        if (LightingDynamicCatalogs.UsesProgressiveMultiplier(bossId))
        {
            float mechMaxMultiplier = ClampBossMultiplier(configuredMultiplier);
            float mechProgress = state.MechBossesDowned / 3f;
            float mechMultiplier = 1f + ((mechMaxMultiplier - 1f) * mechProgress);
            BrightenGroups(targetTileGroups, mechMultiplier);
            return;
        }

        BrightenGroups(targetTileGroups, ClampBossMultiplier(configuredMultiplier));
    }

    private static void BrightenGroups(int[][] targetTileGroups, float multiplier)
    {
        for (int i = 0; i < targetTileGroups.Length; i++)
        {
            TileLightStore.Brighten(targetTileGroups[i], multiplier);
        }
    }

    private static float ClampBossMultiplier(float multiplier)
    {
        return Math.Clamp(multiplier, MinBossMultiplier, MaxBossMultiplier);
    }
}