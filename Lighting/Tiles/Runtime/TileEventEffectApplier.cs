using System.Collections.Generic;

namespace LightingEssentials;

internal static class TileEventEffectApplier
{
    private const float EventTintBlend = 0.82f;
    private const float MinimumTintIntensity = 0.12f;

    public static void Apply(LightingSettings config, in WorldLightingState state)
    {
        List<LightingEventEffectEntry> entries = config.EventEffectEntries;
        for (int i = 0; i < entries.Count; i++)
        {
            LightingEventEffectEntry entry = entries[i];
            if (entry is null || !entry.Enabled)
                continue;

            bool triggered = false;
            if (entry.EventIds is null || entry.EventIds.Count == 0)
            {
                WorldLightingFlags legacyEventFlag = LightingDynamicCatalogs.GetEventFlag(entry.EventId);
                triggered = legacyEventFlag != WorldLightingFlags.None && state.Has(legacyEventFlag);
            }
            else
            {
                for (int j = 0; j < entry.EventIds.Count; j++)
                {
                    WorldLightingFlags eventFlag = LightingDynamicCatalogs.GetEventFlag(entry.EventIds[j]);
                    if (eventFlag == WorldLightingFlags.None || !state.Has(eventFlag))
                        continue;

                    triggered = true;
                    break;
                }
            }

            if (!triggered)
                continue;

            TileLightStore.TintAll(entry.Color.ToVector3(), EventTintBlend, MinimumTintIntensity);
        }
    }
}