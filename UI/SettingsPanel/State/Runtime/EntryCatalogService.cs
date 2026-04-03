using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal sealed class LightingSettingsPanelEntryCatalogService
{
    private readonly LightingSettingsPanelGroupAddService _groupAddService = new();

    public List<CatalogPickerOption> BuildCatalogOptionsForTab(LightingSettingsTab tab, LightingSettings settings, int ignoreEntryIndex = -1)
    {
        List<CatalogPickerOption> options = [];

        if (tab == LightingSettingsTab.TileEffects)
            AddTileOptions(settings, options, ignoreEntryIndex);
        else if (tab == LightingSettingsTab.Events)
            AddEventOptions(settings, options, ignoreEntryIndex);
        else if (tab == LightingSettingsTab.BossEffects)
            AddBossOptions(settings, options, ignoreEntryIndex);

        options.Sort(static (a, b) => string.Compare(a.Label, b.Label, StringComparison.OrdinalIgnoreCase));
        return options;
    }

    public bool TryAddSelectedGroup(LightingSettingsTab tab, LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        return _groupAddService.TryAddSelectedGroup(tab, settings, selectedOptions, groupName);
    }

    public static bool TryParseOptionKey(string key, string expectedPrefix, out int value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(key))
            return false;

        string[] parts = key.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2 && parts[0] == expectedPrefix && int.TryParse(parts[1], out value);
    }

    public static string ResolveGroupName(string groupName, IReadOnlyList<CatalogPickerOption> selectedOptions, string multiSelectionFallback)
    {
        if (!string.IsNullOrWhiteSpace(groupName))
            return groupName.Trim();

        return selectedOptions.Count == 1 ? selectedOptions[0].Label : multiSelectionFallback;
    }

    public static string FormatGroupLabel(string baseLabel, int groupCount)
    {
        return groupCount > 1 ? $"{baseLabel} ({groupCount})" : baseLabel;
    }

    private static void AddTileOptions(LightingSettings settings, List<CatalogPickerOption> options, int ignoreEntryIndex)
    {
        HashSet<int> usedTileIds = [];
        for (int i = 0; i < settings.TileEffectEntries.Count; i++)
        {
            if (i == ignoreEntryIndex)
                continue;

            LightingTileEffectEntry entry = settings.TileEffectEntries[i];
            if (entry?.TileIds is null)
                continue;

            for (int j = 0; j < entry.TileIds.Count; j++)
                usedTileIds.Add(entry.TileIds[j]);
        }

        IReadOnlyList<LightingTileCatalogItem> tiles = LightingDynamicCatalogs.GetTileCatalogItems();
        for (int i = 0; i < tiles.Count; i++)
        {
            LightingTileCatalogItem tile = tiles[i];
            if (!usedTileIds.Contains(tile.TileId))
                options.Add(new CatalogPickerOption($"tile:{tile.TileId}", tile.DisplayName));
        }
    }

    private static void AddEventOptions(LightingSettings settings, List<CatalogPickerOption> options, int ignoreEntryIndex)
    {
        HashSet<LightingEventId> usedEvents = [];
        for (int i = 0; i < settings.EventEffectEntries.Count; i++)
        {
            if (i == ignoreEntryIndex)
                continue;

            LightingEventEffectEntry entry = settings.EventEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.EventIds is null || entry.EventIds.Count == 0)
            {
                usedEvents.Add(entry.EventId);
                continue;
            }

            for (int j = 0; j < entry.EventIds.Count; j++)
                usedEvents.Add(entry.EventIds[j]);
        }

        IReadOnlyList<LightingEventCatalogItem> events = LightingDynamicCatalogs.GetEventCatalogItems();
        for (int i = 0; i < events.Count; i++)
        {
            LightingEventCatalogItem eventItem = events[i];
            if (!usedEvents.Contains(eventItem.EventId))
                options.Add(new CatalogPickerOption($"event:{(int)eventItem.EventId}", eventItem.DisplayName));
        }
    }

    private static void AddBossOptions(LightingSettings settings, List<CatalogPickerOption> options, int ignoreEntryIndex)
    {
        HashSet<LightingBossId> usedBosses = [];
        for (int i = 0; i < settings.BossEffectEntries.Count; i++)
        {
            if (i == ignoreEntryIndex)
                continue;

            LightingBossEffectEntry entry = settings.BossEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.BossIds is null || entry.BossIds.Count == 0)
            {
                usedBosses.Add(entry.BossId);
                continue;
            }

            for (int j = 0; j < entry.BossIds.Count; j++)
                usedBosses.Add(entry.BossIds[j]);
        }

        IReadOnlyList<LightingBossCatalogItem> bosses = LightingDynamicCatalogs.GetBossCatalogItems();
        for (int i = 0; i < bosses.Count; i++)
        {
            LightingBossCatalogItem bossItem = bosses[i];
            if (!usedBosses.Contains(bossItem.BossId))
                options.Add(new CatalogPickerOption($"boss:{(int)bossItem.BossId}", bossItem.DisplayName));
        }
    }

}
