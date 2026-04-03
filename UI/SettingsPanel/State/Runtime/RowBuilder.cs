using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Catalog;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Components.Rows;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.Styling;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal sealed class LightingSettingsPanelRowBuilder
{
    public void BuildRowsForTab(
        LightingSettingsPanelRuntimeState state,
        LightingSettings settings,
        LightingSettingsPanelRowCallbacks callbacks,
        LightingSettingsPanelConfigActionCallbacks configActionCallbacks)
    {
        if (state.SettingsList is null)
            return;

        state.SettingsList.Clear();

        bool usesDynamicEntries = LightingSettingsCatalog.UsesDynamicEntries(state.ActiveTab);
        state.SettingsList.ListPadding = state.ActiveTab == LightingSettingsTab.BossEffects ? state.Scale(2f) : state.Scale(8f);
        ApplyTabLayoutMode(state, usesDynamicEntries);

        if (usesDynamicEntries)
        {
            BuildDynamicRows(state, settings, callbacks);
            state.SettingsList.Recalculate();
            return;
        }

        IReadOnlyList<LightingSettingDescriptor> descriptors = LightingSettingsCatalog.GetTabDescriptors(state.ActiveTab);
        for (int i = 0; i < descriptors.Count; i++)
            state.SettingsList.Add(CreateDescriptorRow(descriptors[i], settings, state, callbacks));

        if (state.ActiveTab == LightingSettingsTab.Config)
            state.SettingsList.Add(CreateConfigActionsRow(state, configActionCallbacks));

        state.SettingsList.Recalculate();
    }

    private static void ApplyTabLayoutMode(LightingSettingsPanelRuntimeState state, bool usesDynamicEntries)
    {
        state.SettingsScrollPanel?.Height.Set(usesDynamicEntries ? -state.Scale(78f) : -state.Scale(76f), 1f);

        if (state.AddEntryButton is null)
            return;

        state.AddEntryButton.SetText(LightingSettingsCatalog.GetAddButtonLabel(state.ActiveTab));

        if (usesDynamicEntries)
        {
            if (state.AddEntryButton.Parent is null)
                state.ContentContainer?.Append(state.AddEntryButton);
        }
        else
        {
            state.AddEntryButton.Remove();
        }
    }

    private static UIElement CreateConfigActionsRow(LightingSettingsPanelRuntimeState state, LightingSettingsPanelConfigActionCallbacks configActions)
    {
        UIElement row = new();
        row.Width.Set(0f, 1f);
        row.Height.Set(state.Scale(68f), 0f);

        FlatTextButton modEnabledButton = new(string.Empty, state.ScaleText(0.68f))
        {
            HoverStyleEnabled = false,
        };
        modEnabledButton.Width.Set(-state.Scale(4f), 0.3334f);
        modEnabledButton.Height.Set(state.Scale(24f), 0f);
        modEnabledButton.OnLeftClick += (_, _) =>
        {
            configActions.ToggleModEnabled();
            RefreshConfigModEnabledButton(modEnabledButton);
        };
        row.Append(modEnabledButton);

        FlatTextButton resetAllButton = new("Reset to Defaults", state.ScaleText(0.66f));
        resetAllButton.Width.Set(-state.Scale(4f), 0.3333f);
        resetAllButton.Height.Set(state.Scale(24f), 0f);
        resetAllButton.Left.Set(0f, 0.3333f);
        resetAllButton.OnLeftClick += (_, _) =>
        {
            configActions.ResetAll();
            RefreshConfigModEnabledButton(modEnabledButton);
        };
        row.Append(resetAllButton);

        FlatTextButton copyModifiedButton = new("Copy Modified", state.ScaleText(0.66f));
        copyModifiedButton.Width.Set(-state.Scale(4f), 0.3333f);
        copyModifiedButton.Height.Set(state.Scale(24f), 0f);
        copyModifiedButton.Left.Set(0f, 0.6666f);
        copyModifiedButton.OnLeftClick += (_, _) => configActions.CopyModified();
        row.Append(copyModifiedButton);

        FlatTextButton importSettingsButton = new("Import Settings", state.ScaleText(0.66f));
        importSettingsButton.Width.Set(-state.Scale(4f), 0.5f);
        importSettingsButton.Height.Set(state.Scale(24f), 0f);
        importSettingsButton.Top.Set(state.Scale(40f), 0f);
        importSettingsButton.OnLeftClick += (_, _) => configActions.OpenImportPopup();
        row.Append(importSettingsButton);

        FlatTextButton exportSettingsButton = new("Export Settings", state.ScaleText(0.66f));
        exportSettingsButton.Width.Set(-state.Scale(4f), 0.5f);
        exportSettingsButton.Height.Set(state.Scale(24f), 0f);
        exportSettingsButton.Top.Set(state.Scale(40f), 0f);
        exportSettingsButton.Left.Set(0f, 0.5f);
        exportSettingsButton.OnLeftClick += (_, _) => configActions.ExportModified();
        row.Append(exportSettingsButton);

        RefreshConfigModEnabledButton(modEnabledButton);
        return row;
    }

    private static void RefreshConfigModEnabledButton(FlatTextButton modEnabledButton)
    {
        bool isEnabled = ModContent.GetInstance<LightingSettings>().ModEnabled;
        modEnabledButton.SetText(isEnabled ? "Mod: ON" : "Mod: OFF");
        modEnabledButton.BackgroundColor = isEnabled ? SettingsPanelTheme.Positive : SettingsPanelTheme.Negative;
    }

    private static void BuildDynamicRows(LightingSettingsPanelRuntimeState state, LightingSettings settings, LightingSettingsPanelRowCallbacks callbacks)
    {
        if (state.ActiveTab == LightingSettingsTab.TileEffects)
            BuildTileRows(state, settings, callbacks);
        else if (state.ActiveTab == LightingSettingsTab.Events)
            BuildEventRows(state, settings, callbacks);
        else if (state.ActiveTab == LightingSettingsTab.BossEffects)
            BuildBossRows(state, settings, callbacks);
    }

    private static void BuildTileRows(LightingSettingsPanelRuntimeState state, LightingSettings settings, LightingSettingsPanelRowCallbacks callbacks)
    {
        bool hasRows = false;
        for (int i = 0; i < settings.TileEffectEntries.Count; i++)
        {
            int entryIndex = i;
            LightingTileEffectEntry entry = settings.TileEffectEntries[i];
            if (entry is null || entry.TileIds is null || entry.TileIds.Count == 0)
                continue;

            string label = string.IsNullOrWhiteSpace(entry.Name) ? "Tile Group" : entry.Name;
            string displayLabel = LightingSettingsPanelEntryCatalogService.FormatGroupLabel(label, entry.TileIds.Count);
            Color defaultColor = LightingDynamicCatalogs.GetSuggestedTileColor(entry.TileIds[0]);

            LightingTileEffectEntry GetCurrentEntry()
            {
                return entryIndex >= 0 && entryIndex < settings.TileEffectEntries.Count
                    ? settings.TileEffectEntries[entryIndex]
                    : null;
            }

            ColorSettingDescriptor descriptor = new(
                displayLabel,
                _ => GetCurrentEntry()?.Color ?? defaultColor,
                (_, value) =>
                {
                    LightingTileEffectEntry current = GetCurrentEntry();
                    if (current is not null)
                        current.Color = value;
                },
                _ => defaultColor,
                _ => GetCurrentEntry()?.Enabled ?? true,
                (_, value) =>
                {
                    LightingTileEffectEntry current = GetCurrentEntry();
                    if (current is not null)
                        current.Enabled = value;
                });

            ColorSettingRow row = new(descriptor, settings, d => callbacks.OpenColorPicker(d, settings), state.UiScale, () => callbacks.EditTileEntry(entryIndex), () => callbacks.RemoveTileEntry(entryIndex), () => callbacks.ApplySettingsChange(settings));
            state.SettingsList.Add(row);
            hasRows = true;
        }

        if (!hasRows)
            state.SettingsList.Add(new UIText("No tile entries. Add one below.", state.ScaleText(0.74f)));
    }

    private static void BuildEventRows(LightingSettingsPanelRuntimeState state, LightingSettings settings, LightingSettingsPanelRowCallbacks callbacks)
    {
        bool hasRows = false;
        for (int i = 0; i < settings.EventEffectEntries.Count; i++)
        {
            int entryIndex = i;
            LightingEventEffectEntry entry = settings.EventEffectEntries[i];
            if (entry is null)
                continue;

            LightingEventId eventId = entry.EventIds is { Count: > 0 } ? entry.EventIds[0] : entry.EventId;
            if (!LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out LightingEventCatalogItem item))
                continue;

            string label = string.IsNullOrWhiteSpace(entry.Name) ? $"{item.DisplayName} Group" : entry.Name;
            int eventCount = entry.EventIds is null || entry.EventIds.Count == 0 ? 1 : entry.EventIds.Count;
            string displayLabel = LightingSettingsPanelEntryCatalogService.FormatGroupLabel(label, eventCount);

            LightingEventEffectEntry GetCurrentEntry()
            {
                return entryIndex >= 0 && entryIndex < settings.EventEffectEntries.Count
                    ? settings.EventEffectEntries[entryIndex]
                    : null;
            }

            ColorSettingDescriptor descriptor = new(
                displayLabel,
                _ => GetCurrentEntry()?.Color ?? item.DefaultColor,
                (_, value) =>
                {
                    LightingEventEffectEntry current = GetCurrentEntry();
                    if (current is not null)
                        current.Color = value;
                },
                _ => item.DefaultColor,
                _ => GetCurrentEntry()?.Enabled ?? true,
                (_, value) =>
                {
                    LightingEventEffectEntry current = GetCurrentEntry();
                    if (current is not null)
                        current.Enabled = value;
                });

            state.SettingsList.Add(new ColorSettingRow(descriptor, settings, d => callbacks.OpenColorPicker(d, settings), state.UiScale, () => callbacks.EditEventEntry(entryIndex), () => callbacks.RemoveEventEntry(entryIndex), () => callbacks.ApplySettingsChange(settings)));
            hasRows = true;
        }

        if (!hasRows)
            state.SettingsList.Add(new UIText("No event entries. Add one below.", state.ScaleText(0.74f)));
    }

    private static void BuildBossRows(LightingSettingsPanelRuntimeState state, LightingSettings settings, LightingSettingsPanelRowCallbacks callbacks)
    {
        bool hasRows = false;
        for (int i = 0; i < settings.BossEffectEntries.Count; i++)
        {
            int entryIndex = i;
            LightingBossEffectEntry entry = settings.BossEffectEntries[i];
            if (entry is null)
                continue;

            LightingBossId bossId = entry.BossIds is { Count: > 0 } ? entry.BossIds[0] : entry.BossId;
            if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out LightingBossCatalogItem item))
                continue;

            string label = string.IsNullOrWhiteSpace(entry.Name) ? $"{item.DisplayName} Group" : entry.Name;
            int bossCount = entry.BossIds is null || entry.BossIds.Count == 0 ? 1 : entry.BossIds.Count;
            string displayLabel = LightingSettingsPanelEntryCatalogService.FormatGroupLabel(label, bossCount);

            LightingBossEffectEntry GetCurrentEntry()
            {
                return entryIndex >= 0 && entryIndex < settings.BossEffectEntries.Count
                    ? settings.BossEffectEntries[entryIndex]
                    : null;
            }

            FloatSettingDescriptor descriptor = new(
                displayLabel,
                1f,
                2f,
                0.05f,
                _ => GetCurrentEntry()?.Multiplier ?? item.DefaultMultiplier,
                (_, value) =>
                {
                    LightingBossEffectEntry current = GetCurrentEntry();
                    if (current is not null)
                        current.Multiplier = Math.Clamp(value, 1f, 2f);
                },
                _ => GetCurrentEntry()?.Enabled ?? true,
                (_, value) =>
                {
                    LightingBossEffectEntry current = GetCurrentEntry();
                    if (current is not null)
                        current.Enabled = value;
                });

            state.SettingsList.Add(new FloatSettingRow(descriptor, settings, () => callbacks.ApplySettingsChange(settings), state.UiScale, () => callbacks.EditBossEntry(entryIndex), () => callbacks.RemoveBossEntry(entryIndex)));
            hasRows = true;
        }

        if (!hasRows)
            state.SettingsList.Add(new UIText("No boss entries. Add one below.", state.ScaleText(0.74f)));
    }

    private static UIElement CreateDescriptorRow(LightingSettingDescriptor descriptor, LightingSettings settings, LightingSettingsPanelRuntimeState state, LightingSettingsPanelRowCallbacks callbacks)
    {
        return descriptor switch
        {
            BoolSettingDescriptor boolDescriptor => new BoolSettingRow(boolDescriptor, settings, () => callbacks.ApplySettingsChange(settings), state.UiScale),
            FloatSettingDescriptor floatDescriptor => new FloatSettingRow(floatDescriptor, settings, () => callbacks.ApplySettingsChange(settings), state.UiScale),
            ColorSettingDescriptor colorDescriptor => new ColorSettingRow(colorDescriptor, settings, d => callbacks.OpenColorPicker(d, settings), state.UiScale, onSettingChanged: () => callbacks.ApplySettingsChange(settings)),
            _ => new UIText("Unsupported setting"),
        };
    }
}
