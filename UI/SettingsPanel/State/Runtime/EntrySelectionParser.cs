using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal static class LightingSettingsPanelEntrySelectionParser
{
    public static List<int> ParseSelectedTileIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<int> tileIds = [];
        HashSet<int> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "tile", out int tileId) || !seen.Add(tileId))
                continue;

            tileIds.Add(tileId);
        }

        return tileIds;
    }

    public static List<LightingEventId> ParseSelectedEventIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<LightingEventId> eventIds = [];
        HashSet<LightingEventId> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "event", out int eventIdRaw))
                continue;

            LightingEventId eventId = (LightingEventId)eventIdRaw;
            if (!LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out _) || !seen.Add(eventId))
                continue;

            eventIds.Add(eventId);
        }

        return eventIds;
    }

    public static List<LightingBossId> ParseSelectedBossIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<LightingBossId> bossIds = [];
        HashSet<LightingBossId> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "boss", out int bossIdRaw))
                continue;

            LightingBossId bossId = (LightingBossId)bossIdRaw;
            if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out _) || !seen.Add(bossId))
                continue;

            bossIds.Add(bossId);
        }

        return bossIds;
    }
}
