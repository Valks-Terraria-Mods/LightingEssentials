using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime.Clipboard;

internal static class LightingSettingsPanelClipboardExporter
{
    private static readonly FieldInfo[] CopyableSettingsFields = typeof(LightingSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);
    private static readonly HashSet<string> NonScalarCopiedFields = ["TileEffectEntries", "EventEffectEntries", "BossEffectEntries", "EntityEffectEntries"];

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

        string entitySection = LightingSettingsPanelClipboardSections.BuildEntityEntriesSection(settings.EntityEffectEntries, defaults.EntityEffectEntries);
        if (!string.IsNullOrWhiteSpace(entitySection))
            sections.Add(entitySection);

        if (sections.Count == 0)
            return string.Empty;

        return string.Join("\n\n", sections);
    }

    private static string BuildModifiedScalarSection(LightingSettings settings, LightingSettings defaults)
    {
        List<(string Name, string Modified, string Default)> rows = [];

        for (int i = 0; i < CopyableSettingsFields.Length; i++)
        {
            FieldInfo field = CopyableSettingsFields[i];
            if (NonScalarCopiedFields.Contains(field.Name))
                continue;

            if (field.Name.Equals("UiScale", System.StringComparison.OrdinalIgnoreCase))
                continue;

            object currentValue = field.GetValue(settings);
            object defaultValue = field.GetValue(defaults);
            if (LightingSettingsPanelClipboardFormatting.ValuesEqual(currentValue, defaultValue))
                continue;

            rows.Add((
                field.Name,
                LightingSettingsPanelClipboardFormatting.FormatValue(currentValue),
                LightingSettingsPanelClipboardFormatting.FormatValue(defaultValue)));
        }

        if (rows.Count == 0)
            return string.Empty;

        StringBuilder builder = new();
        builder.AppendLine("__Scalar Settings__");

        for (int i = 0; i < rows.Count; i++)
        {
            (string name, string modified, string defaultValue) = rows[i];
            builder.AppendLine($"**{name}:** `Modified` {modified} *(default: {defaultValue})*");
        }

        return builder.ToString().TrimEnd();
    }
}
