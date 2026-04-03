using System.Collections.Generic;
using System.Reflection;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime.Clipboard;

internal static class LightingSettingsPanelClipboardExporter
{
    private static readonly FieldInfo[] CopyableSettingsFields = typeof(LightingSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);
    private static readonly HashSet<string> NonScalarCopiedFields = ["TileEffectEntries", "EventEffectEntries", "BossEffectEntries"];

    public static string BuildModifiedSettingsClipboardText(LightingSettings settings)
    {
        settings.EnsureDynamicEntries();
        LightingSettings defaults = LightingSettingsDefaults.CreateDefaults();

        List<string> sections = [];

        string scalarSection = BuildModifiedScalarSection(settings, defaults);
        if (!string.IsNullOrWhiteSpace(scalarSection))
            sections.Add(scalarSection);

        string tileSection = LightingSettingsPanelClipboardSections.BuildTileEntriesSection(settings.TileEffectEntries, defaults.TileEffectEntries);
        if (!string.IsNullOrWhiteSpace(tileSection))
            sections.Add(tileSection);

        string eventSection = LightingSettingsPanelClipboardSections.BuildEventEntriesSection(settings.EventEffectEntries, defaults.EventEffectEntries);
        if (!string.IsNullOrWhiteSpace(eventSection))
            sections.Add(eventSection);

        string bossSection = LightingSettingsPanelClipboardSections.BuildBossEntriesSection(settings.BossEffectEntries, defaults.BossEffectEntries);
        if (!string.IsNullOrWhiteSpace(bossSection))
            sections.Add(bossSection);

        return sections.Count == 0
            ? string.Empty
            : "LightingEssentials Modified Settings\n\n" + string.Join("\n\n", sections);
    }

    private static string BuildModifiedScalarSection(LightingSettings settings, LightingSettings defaults)
    {
        List<string> lines = [];

        for (int i = 0; i < CopyableSettingsFields.Length; i++)
        {
            FieldInfo field = CopyableSettingsFields[i];
            if (NonScalarCopiedFields.Contains(field.Name))
                continue;

            object currentValue = field.GetValue(settings);
            object defaultValue = field.GetValue(defaults);
            if (LightingSettingsPanelClipboardFormatting.ValuesEqual(currentValue, defaultValue))
                continue;

            lines.Add($"- {field.Name}: {LightingSettingsPanelClipboardFormatting.FormatValue(currentValue)} (default: {LightingSettingsPanelClipboardFormatting.FormatValue(defaultValue)})");
        }

        return lines.Count == 0
            ? string.Empty
            : "Scalar fields:\n" + string.Join("\n", lines);
    }
}
