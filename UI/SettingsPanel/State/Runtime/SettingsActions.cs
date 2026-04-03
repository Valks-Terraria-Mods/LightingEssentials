using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.State.Runtime.Clipboard;
using ReLogic.OS;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal static class LightingSettingsPanelSettingsActions
{
    public static void ToggleModEnabled(Action<LightingSettings> applySettingsChange)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.ModEnabled = !settings.ModEnabled;
        applySettingsChange(settings);
    }

    public static void ResetAllSettings(Action<LightingSettings> applySettingsChange, Action rebuildRows)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        LightingSettingsDefaults.ApplyDefaults(settings);
        applySettingsChange(settings);
        rebuildRows();
    }

    public static void CopyModifiedSettings()
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        string clipboardText = LightingSettingsPanelClipboardExporter.BuildModifiedSettingsClipboardText(settings);
        if (string.IsNullOrWhiteSpace(clipboardText))
        {
            Main.NewText("No settings were modified, nothing was copied to clipboard.");
            return;
        }

        Platform.Get<IClipboard>().Value = clipboardText;
        Main.NewText("Copied modified settings to clipboard");
    }

    public static void ApplySettingsChange(LightingSettings settings, LightingSettingsPanelRuntimeState state, int persistDebounceFrames)
    {
        settings.EnsureDynamicEntries();
        settings.ApplyRuntimeChanges();
        state.PendingConfigPersist = true;
        state.PersistCountdownFrames = persistDebounceFrames;
    }

    public static void PersistPendingSettingsChanges(LightingSettingsPanelRuntimeState state)
    {
        if (!state.PendingConfigPersist)
            return;

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();
        settings.SaveChanges();

        state.PendingConfigPersist = false;
        state.PersistCountdownFrames = 0;
    }

    public static bool TryApplyCatalogSelection(
        LightingSettingsPanelEntryCatalogService catalogService,
        LightingSettingsTab activeTab,
        IReadOnlyList<CatalogPickerOption> selectedOptions,
        string groupName,
        Action<LightingSettings> applySettingsChange,
        Action rebuildRows)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (!catalogService.TryAddSelectedGroup(activeTab, settings, selectedOptions, groupName))
            return false;

        applySettingsChange(settings);
        rebuildRows();
        return true;
    }
}
