using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.State.Runtime.Clipboard;
using ReLogic.OS;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal static class LightingSettingsPanelSettingsActions
{
    private static readonly Color FeedbackColor = new(85, 85, 85);

    private const string LimeTag = "94ff94";

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
        ShowFeedback($"All settings were {Highlight("reset to their defaults")}.");
    }

    public static void CopyModifiedSettings()
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        string clipboardText = LightingSettingsPanelClipboardExporter.BuildModifiedSettingsClipboardText(settings);
        if (string.IsNullOrWhiteSpace(clipboardText))
        {
            ShowFeedback($"No settings were modified, {Highlight("nothing was copied")} to clipboard.");
            return;
        }

        Platform.Get<IClipboard>().Value = clipboardText;
        ShowFeedback($"{Highlight("Copied modified settings")} to clipboard.");
    }

    public static void ExportModifiedSettings()
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        string transferCode = LightingSettingsPanelClipboardTransferCodec.BuildTransferCode(settings);
        if (string.IsNullOrWhiteSpace(transferCode))
        {
            ShowFeedback($"No settings were modified, {Highlight("nothing was exported")}.");
            return;
        }

        Platform.Get<IClipboard>().Value = transferCode;
        ShowFeedback($"{Highlight("Copied settings")} to clipboard.");
    }

    public static bool ImportModifiedSettings(string transferCode, Action<LightingSettings> applySettingsChange, Action rebuildRows)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (!LightingSettingsPanelClipboardTransferCodec.TryApplyTransferCode(transferCode, settings, out string errorMessage))
        {
            ShowFeedback($"{Highlight("Import failed")}: {Highlight(errorMessage)}");
            return false;
        }

        applySettingsChange(settings);
        rebuildRows();
        ShowFeedback($"{Highlight("Settings successfully imported")}");
        return true;
    }

    private static string Highlight(string text)
    {
        return $"[c/{LimeTag}:{text}]";
    }

    private static void ShowFeedback(string message)
    {
        Main.NewText(message, FeedbackColor);
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
