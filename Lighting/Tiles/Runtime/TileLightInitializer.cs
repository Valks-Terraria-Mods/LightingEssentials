using System.Collections.Generic;

namespace LightingEssentials;

internal static class TileLightInitializer
{
    public static void Initialize()
    {
        LightingSettings config = LightingEssentials.Config;
        if (config is null)
            return;

        TileLightStore.Reset(TileLoader.TileCount);

        if (!LightRuntime.ModEnabled)
            return;

        ApplyBaseLighting(config);

        WorldLightingState state = LightWorldSystem.GetCurrentState();
        TileBossEffectApplier.Apply(config, state);
        TileEventEffectApplier.Apply(config, state);
    }

    private static void ApplyBaseLighting(LightingSettings config)
    {
        List<LightingTileEffectEntry> entries = config.TileEffectEntries;
        for (int i = 0; i < entries.Count; i++)
        {
            LightingTileEffectEntry entry = entries[i];
            if (entry is null || !entry.Enabled || entry.TileIds is null || entry.TileIds.Count == 0)
                continue;

            Vector3 color = entry.Color.ToVector3();
            for (int j = 0; j < entry.TileIds.Count; j++)
            {
                int tileId = entry.TileIds[j];
                if (tileId < 0 || tileId >= TileLoader.TileCount)
                    continue;

                TileLightStore.SetColor(tileId, color);
            }
        }
    }
}