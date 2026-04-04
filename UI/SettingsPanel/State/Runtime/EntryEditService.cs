using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal sealed class LightingSettingsPanelEntryEditService
{
    private readonly LightingSettingsPanelEntryCatalogService _catalogService;
    private readonly Action<string, IReadOnlyList<CatalogPickerOption>, Action<IReadOnlyList<CatalogPickerOption>, string>, IReadOnlyCollection<string>, string, string> _openCatalogPicker;
    private readonly Action<string, IReadOnlyList<CatalogPickerOption>, IReadOnlyList<CatalogPickerOption>, Action<IReadOnlyList<CatalogPickerOption>, IReadOnlyList<CatalogPickerOption>, string>, IReadOnlyCollection<string>, IReadOnlyCollection<string>, string, string> _openBossGroupPicker;
    private readonly Action<LightingSettings> _applySettingsChange;
    private readonly Action _rebuildRows;

    public LightingSettingsPanelEntryEditService(
        LightingSettingsPanelEntryCatalogService catalogService,
        Action<string, IReadOnlyList<CatalogPickerOption>, Action<IReadOnlyList<CatalogPickerOption>, string>, IReadOnlyCollection<string>, string, string> openCatalogPicker,
        Action<string, IReadOnlyList<CatalogPickerOption>, IReadOnlyList<CatalogPickerOption>, Action<IReadOnlyList<CatalogPickerOption>, IReadOnlyList<CatalogPickerOption>, string>, IReadOnlyCollection<string>, IReadOnlyCollection<string>, string, string> openBossGroupPicker,
        Action<LightingSettings> applySettingsChange,
        Action rebuildRows)
    {
        _catalogService = catalogService;
        _openCatalogPicker = openCatalogPicker;
        _openBossGroupPicker = openBossGroupPicker;
        _applySettingsChange = applySettingsChange;
        _rebuildRows = rebuildRows;
    }

    public void EditTileEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.TileEffectEntries.Count)
            return;

        LightingTileEffectEntry entry = settings.TileEffectEntries[index];
        if (entry is null || entry.TileIds is null || entry.TileIds.Count == 0)
            return;

        List<CatalogPickerOption> options = _catalogService.BuildCatalogOptionsForTab(LightingSettingsTab.TileEffects, settings, index);
        List<string> selectedKeys = [];
        for (int i = 0; i < entry.TileIds.Count; i++)
            selectedKeys.Add($"tile:{entry.TileIds[i]}");

        _openCatalogPicker(
            "Edit Tile Group",
            options,
            (selectedOptions, groupName) =>
            {
                List<int> tileIds = LightingSettingsPanelEntrySelectionParser.ParseSelectedTileIds(selectedOptions);
                if (tileIds.Count == 0 || index < 0 || index >= settings.TileEffectEntries.Count)
                    return;

                LightingTileEffectEntry currentEntry = settings.TileEffectEntries[index];
                if (currentEntry is null)
                    return;

                string fallback = string.IsNullOrWhiteSpace(currentEntry.Name) ? "Tile Group" : currentEntry.Name;
                string resolved = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, fallback);
                settings.TileEffectEntries[index] = new LightingTileEffectEntry(resolved, tileIds, currentEntry.Color, currentEntry.Enabled);
                _applySettingsChange(settings);
                _rebuildRows();
            },
            selectedKeys,
            entry.Name,
            "Save Group");
    }

    public void EditEventEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.EventEffectEntries.Count)
            return;

        LightingEventEffectEntry entry = settings.EventEffectEntries[index];
        if (entry is null)
            return;

        List<CatalogPickerOption> options = _catalogService.BuildCatalogOptionsForTab(LightingSettingsTab.Events, settings, index);
        List<string> selectedKeys = [];

        if (entry.EventIds is null || entry.EventIds.Count == 0)
            selectedKeys.Add($"event:{(int)entry.EventId}");
        else
            for (int i = 0; i < entry.EventIds.Count; i++)
                selectedKeys.Add($"event:{(int)entry.EventIds[i]}");

        _openCatalogPicker(
            "Edit Event Group",
            options,
            (selectedOptions, groupName) =>
            {
                List<LightingEventId> eventIds = LightingSettingsPanelEntrySelectionParser.ParseSelectedEventIds(selectedOptions);
                if (eventIds.Count == 0 || index < 0 || index >= settings.EventEffectEntries.Count)
                    return;

                LightingEventEffectEntry currentEntry = settings.EventEffectEntries[index];
                if (currentEntry is null)
                    return;

                string fallback = string.IsNullOrWhiteSpace(currentEntry.Name) ? "Event Group" : currentEntry.Name;
                string resolved = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, fallback);
                settings.EventEffectEntries[index] = new LightingEventEffectEntry(resolved, eventIds, currentEntry.Enabled, currentEntry.Color);
                _applySettingsChange(settings);
                _rebuildRows();
            },
            selectedKeys,
            entry.Name,
            "Save Group");
    }

    public void EditBossEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.BossEffectEntries.Count)
            return;

        LightingBossEffectEntry entry = settings.BossEffectEntries[index];
        if (entry is null)
            return;

        List<CatalogPickerOption> bossOptions = _catalogService.BuildCatalogOptionsForTab(LightingSettingsTab.BossEffects, settings, index);
        List<CatalogPickerOption> targetTileGroupOptions = _catalogService.BuildBossTargetTileGroupOptions();
        List<LightingBossId> currentBossIds = entry.BossIds is { Count: > 0 } ? [..entry.BossIds] : [entry.BossId];

        List<string> selectedBossKeys = [];
        for (int i = 0; i < currentBossIds.Count; i++)
            selectedBossKeys.Add($"boss:{(int)currentBossIds[i]}");

        List<string> selectedTargetTileGroupKeys = LightingDynamicCatalogs.ResolveBossTargetTileGroupKeys(currentBossIds, entry.TargetTileGroupKeys);

        _openBossGroupPicker(
            "Edit Boss Group",
            bossOptions,
            targetTileGroupOptions,
            (selectedBossOptions, selectedTargetTileGroupOptions, groupName) =>
            {
                List<LightingBossId> bossIds = LightingSettingsPanelEntrySelectionParser.ParseSelectedBossIds(selectedBossOptions);
                List<string> targetTileGroupKeys = LightingSettingsPanelEntrySelectionParser.ParseSelectedBossTargetTileGroupKeys(selectedTargetTileGroupOptions);
                if (bossIds.Count == 0 || targetTileGroupKeys.Count == 0 || index < 0 || index >= settings.BossEffectEntries.Count)
                    return;

                LightingBossEffectEntry currentEntry = settings.BossEffectEntries[index];
                if (currentEntry is null)
                    return;

                string fallback = string.IsNullOrWhiteSpace(currentEntry.Name) ? "Boss Group" : currentEntry.Name;
                string resolved = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedBossOptions, fallback);
                settings.BossEffectEntries[index] = new LightingBossEffectEntry(resolved, bossIds, currentEntry.Enabled, currentEntry.Multiplier, targetTileGroupKeys);
                _applySettingsChange(settings);
                _rebuildRows();
            },
            selectedBossKeys,
            selectedTargetTileGroupKeys,
            entry.Name,
            "Save Group");
    }

    public void EditEntityEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.EntityEffectEntries.Count)
            return;

        LightingEntityEffectEntry entry = settings.EntityEffectEntries[index];
        if (entry is null)
            return;

        List<CatalogPickerOption> options = _catalogService.BuildCatalogOptionsForTab(LightingSettingsTab.EntityLights, settings, index);
        List<string> selectedKeys = LightingSettingsPanelEntrySelectionParser.BuildSelectedEntityKeys(entry);

        _openCatalogPicker(
            "Edit Entity Group",
            options,
            (selectedOptions, groupName) =>
            {
                LightingSettingsPanelEntrySelectionParser.EntitySelection selection = LightingSettingsPanelEntrySelectionParser.ParseSelectedEntitySelection(selectedOptions);
                if (!selection.HasAny || index < 0 || index >= settings.EntityEffectEntries.Count)
                    return;

                LightingEntityEffectEntry currentEntry = settings.EntityEffectEntries[index];
                if (currentEntry is null)
                    return;

                string fallback = string.IsNullOrWhiteSpace(currentEntry.Name) ? "Entity Group" : currentEntry.Name;
                string resolved = LightingSettingsPanelEntryCatalogService.ResolveGroupName(groupName, selectedOptions, fallback);
                settings.EntityEffectEntries[index] = new LightingEntityEffectEntry(
                    resolved,
                    currentEntry.Enabled,
                    currentEntry.Color,
                    selection.IncludePlayer,
                    selection.IncludeAllEnemies,
                    selection.IncludeAllProjectiles,
                    selection.NpcIds,
                    selection.ProjectileIds);

                _applySettingsChange(settings);
                _rebuildRows();
            },
            selectedKeys,
            entry.Name,
            "Save Group");
    }

    public void RemoveTileEntry(int index)
    {
        LightingSettingsPanelEntryRemoval.RemoveTileEntry(index, _applySettingsChange, _rebuildRows);
    }

    public void RemoveEventEntry(int index)
    {
        LightingSettingsPanelEntryRemoval.RemoveEventEntry(index, _applySettingsChange, _rebuildRows);
    }

    public void RemoveBossEntry(int index)
    {
        LightingSettingsPanelEntryRemoval.RemoveBossEntry(index, _applySettingsChange, _rebuildRows);
    }

    public void RemoveEntityEntry(int index)
    {
        LightingSettingsPanelEntryRemoval.RemoveEntityEntry(index, _applySettingsChange, _rebuildRows);
    }
}
