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
        for (int i = 0; i < TileLightBaseEntries.SingleTileEntries.Length; i++)
        {
            TileLightEntry entry = TileLightBaseEntries.SingleTileEntries[i];
            TileLightStore.SetColor(entry.TileId, entry.SelectColor(config).ToVector3());
        }

        for (int i = 0; i < TileLightBaseEntries.GroupEntries.Length; i++)
        {
            TileLightGroupEntry groupEntry = TileLightBaseEntries.GroupEntries[i];
            Vector3 groupColor = groupEntry.SelectColor(config).ToVector3();
            for (int j = 0; j < groupEntry.TileIds.Length; j++)
            {
                TileLightStore.SetColor(groupEntry.TileIds[j], groupColor);
            }
        }
    }
}