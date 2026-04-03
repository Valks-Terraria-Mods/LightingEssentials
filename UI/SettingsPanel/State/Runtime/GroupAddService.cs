using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal sealed class LightingSettingsPanelGroupAddService
{
    public bool TryAddSelectedGroup(LightingSettingsTab tab, LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        if (selectedOptions is null || selectedOptions.Count == 0)
            return false;

        return tab switch
        {
            LightingSettingsTab.TileEffects => TryAddTileGroup(settings, selectedOptions, groupName),
            LightingSettingsTab.Events => TryAddEventGroup(settings, selectedOptions, groupName),
            LightingSettingsTab.BossEffects => TryAddBossGroup(settings, selectedOptions, groupName),
            _ => false,
        };
    }

    private static bool TryAddTileGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        HashSet<int> used = [];
        for (int i = 0; i < settings.TileEffectEntries.Count; i++)
        {
            LightingTileEffectEntry entry = settings.TileEffectEntries[i];
            if (entry?.TileIds is null)
                continue;

            for (int j = 0; j < entry.TileIds.Count; j++)
                used.Add(entry.TileIds[j]);
        }

        List<int> tileIds = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(selectedOptions[i].Key, "tile", out int tileId) || !used.Add(tileId))
                continue;

            tileIds.Add(tileId);
        }

        if (tileIds.Count == 0)
            return false;

        string resolvedName = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, "Tile Group");
        Color suggestedColor = LightingDynamicCatalogs.GetSuggestedTileColor(tileIds[0]);
        settings.TileEffectEntries.Add(new LightingTileEffectEntry(resolvedName, tileIds, suggestedColor, enabled: true));
        return true;
    }

    private static bool TryAddEventGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        HashSet<LightingEventId> used = [];
        for (int i = 0; i < settings.EventEffectEntries.Count; i++)
        {
            LightingEventEffectEntry entry = settings.EventEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.EventIds is null || entry.EventIds.Count == 0)
            {
                used.Add(entry.EventId);
                continue;
            }

            for (int j = 0; j < entry.EventIds.Count; j++)
                used.Add(entry.EventIds[j]);
        }

        List<LightingEventId> eventIds = [];
        Color defaultColor = Color.White;
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "event", out int eventIdRaw))
                continue;

            LightingEventId eventId = (LightingEventId)eventIdRaw;
            if (!LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out LightingEventCatalogItem item) || !used.Add(eventId))
                continue;

            if (eventIds.Count == 0)
                defaultColor = item.DefaultColor;

            eventIds.Add(eventId);
        }

        if (eventIds.Count == 0)
            return false;

        string resolvedName = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, "Event Group");
        settings.EventEffectEntries.Add(new LightingEventEffectEntry(resolvedName, eventIds, true, defaultColor));
        return true;
    }

    private static bool TryAddBossGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        HashSet<LightingBossId> used = [];
        for (int i = 0; i < settings.BossEffectEntries.Count; i++)
        {
            LightingBossEffectEntry entry = settings.BossEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.BossIds is null || entry.BossIds.Count == 0)
            {
                used.Add(entry.BossId);
                continue;
            }

            for (int j = 0; j < entry.BossIds.Count; j++)
                used.Add(entry.BossIds[j]);
        }

        List<LightingBossId> bossIds = [];
        float defaultMultiplier = 1.4f;
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!LightingSettingsPanelEntryCatalogService.TryParseOptionKey(option.Key, "boss", out int bossIdRaw))
                continue;

            LightingBossId bossId = (LightingBossId)bossIdRaw;
            if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out LightingBossCatalogItem item) || !used.Add(bossId))
                continue;

            if (bossIds.Count == 0)
                defaultMultiplier = item.DefaultMultiplier;

            bossIds.Add(bossId);
        }

        if (bossIds.Count == 0)
            return false;

        string resolvedName = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, "Boss Group");
        settings.BossEffectEntries.Add(new LightingBossEffectEntry(resolvedName, bossIds, true, Math.Clamp(defaultMultiplier, 1f, 2f)));
        return true;
    }
}
