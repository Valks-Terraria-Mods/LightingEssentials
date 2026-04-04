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
                TryApplyBossEffect(entry, entry.BossId, entry.Multiplier, state);
                continue;
            }

            for (int j = 0; j < entry.BossIds.Count; j++)
            {
                TryApplyBossEffect(entry, entry.BossIds[j], entry.Multiplier, state);
            }
        }
    }

    private static void TryApplyBossEffect(LightingBossEffectEntry entry, LightingBossId bossId, float configuredMultiplier, in WorldLightingState state)
    {
        if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out _))
            return;

        if (!LightingDynamicCatalogs.IsBossTriggered(bossId, state))
            return;

        IReadOnlyList<string> targetTileGroupKeys = entry.TargetTileGroupKeys;
        if (targetTileGroupKeys is null || targetTileGroupKeys.Count == 0)
            targetTileGroupKeys = LightingDynamicCatalogs.GetDefaultBossTargetTileGroupKeys(bossId);

        if (targetTileGroupKeys.Count == 0)
            return;

        if (LightingDynamicCatalogs.UsesProgressiveMultiplier(bossId))
        {
            float mechMaxMultiplier = ClampBossMultiplier(configuredMultiplier);
            float mechProgress = state.MechBossesDowned / 3f;
            float mechMultiplier = 1f + ((mechMaxMultiplier - 1f) * mechProgress);
            BrightenGroups(targetTileGroupKeys, mechMultiplier);
            return;
        }

        BrightenGroups(targetTileGroupKeys, ClampBossMultiplier(configuredMultiplier));
    }

    private static void BrightenGroups(IReadOnlyList<string> targetTileGroupKeys, float multiplier)
    {
        for (int i = 0; i < targetTileGroupKeys.Count; i++)
        {
            if (!LightingDynamicCatalogs.TryGetBossTargetTileIds(targetTileGroupKeys[i], out int[] tileIds))
                continue;

            TileLightStore.Brighten(tileIds, multiplier);
        }
    }

    private static float ClampBossMultiplier(float multiplier)
    {
        return Math.Clamp(multiplier, MinBossMultiplier, MaxBossMultiplier);
    }
}