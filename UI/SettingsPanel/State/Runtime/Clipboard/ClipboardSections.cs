using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime.Clipboard;

internal static class LightingSettingsPanelClipboardSections
{
    public static string BuildTileEntriesSection(IReadOnlyList<LightingTileEffectEntry> currentEntries, IReadOnlyList<LightingTileEffectEntry> defaultEntries)
    {
        List<string> blocks = [];

        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {
            LightingTileEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingTileEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;
            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                blocks.Add(BuildTileSingleValueBlock("Removed", defaultEntry));
                continue;
            }

            if (defaultEntry is null)
            {
                blocks.Add(BuildTileSingleValueBlock("Added", currentEntry));
                continue;
            }

            if (LightingSettingsPanelClipboardFormatting.AreTileEntryEquivalent(currentEntry, defaultEntry))
                continue;

            blocks.Add(BuildTileModifiedBlock(currentEntry, defaultEntry));
        }

        return BuildSection("__Tile Groups__", blocks);
    }

    public static string BuildEventEntriesSection(IReadOnlyList<LightingEventEffectEntry> currentEntries, IReadOnlyList<LightingEventEffectEntry> defaultEntries)
    {
        List<string> blocks = [];

        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {
            LightingEventEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingEventEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;
            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                blocks.Add(BuildEventSingleValueBlock("Removed", defaultEntry));
                continue;
            }

            if (defaultEntry is null)
            {
                blocks.Add(BuildEventSingleValueBlock("Added", currentEntry));
                continue;
            }

            if (LightingSettingsPanelClipboardFormatting.AreEventEntryEquivalent(currentEntry, defaultEntry))
                continue;

            blocks.Add(BuildEventModifiedBlock(currentEntry, defaultEntry));
        }

        return BuildSection("__Event Groups__", blocks);
    }

    public static string BuildBossEntriesSection(IReadOnlyList<LightingBossEffectEntry> currentEntries, IReadOnlyList<LightingBossEffectEntry> defaultEntries)
    {
        List<string> blocks = [];

        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {
            LightingBossEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingBossEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;
            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                blocks.Add(BuildBossSingleValueBlock("Removed", defaultEntry));
                continue;
            }

            if (defaultEntry is null)
            {
                blocks.Add(BuildBossSingleValueBlock("Added", currentEntry));
                continue;
            }

            if (LightingSettingsPanelClipboardFormatting.AreBossEntryEquivalent(currentEntry, defaultEntry))
                continue;

            blocks.Add(BuildBossModifiedBlock(currentEntry, defaultEntry));
        }

        return BuildSection("__Boss Groups__", blocks);
    }

    private static string BuildSection(string title, List<string> blocks)
    {
        if (blocks.Count == 0)
            return string.Empty;

        StringBuilder builder = new();
        builder.AppendLine(title);

        for (int i = 0; i < blocks.Count; i++)
            builder.AppendLine(blocks[i]);

        return builder.ToString().TrimEnd();
    }

    private static string BuildTileModifiedBlock(LightingTileEffectEntry currentEntry, LightingTileEffectEntry defaultEntry)
    {
        List<string> differences = [];
        string currentName = GetGroupName(currentEntry.Name, "Tile Group");
        string defaultName = GetGroupName(defaultEntry.Name, "Tile Group");
        if (!string.Equals(currentName, defaultName, StringComparison.Ordinal))
            differences.Add(BuildDifference("Name", currentName, defaultName));

        if (currentEntry.Enabled != defaultEntry.Enabled)
            differences.Add(BuildDifference("Enabled", LightingSettingsPanelClipboardFormatting.FormatValue(currentEntry.Enabled), LightingSettingsPanelClipboardFormatting.FormatValue(defaultEntry.Enabled)));

        if (currentEntry.Color != defaultEntry.Color)
            differences.Add(BuildDifference("Color", LightingSettingsPanelClipboardFormatting.FormatColor(currentEntry.Color), LightingSettingsPanelClipboardFormatting.FormatColor(defaultEntry.Color)));

        if (!SequenceEquals(currentEntry.TileIds, defaultEntry.TileIds))
            differences.Add(BuildDifference("Members", JoinTileMembers(currentEntry.TileIds), JoinTileMembers(defaultEntry.TileIds)));

        return BuildModifiedBlock(currentName, differences);
    }

    private static string BuildEventModifiedBlock(LightingEventEffectEntry currentEntry, LightingEventEffectEntry defaultEntry)
    {
        List<string> differences = [];
        string currentName = GetGroupName(currentEntry.Name, "Event Group");
        string defaultName = GetGroupName(defaultEntry.Name, "Event Group");
        if (!string.Equals(currentName, defaultName, StringComparison.Ordinal))
            differences.Add(BuildDifference("Name", currentName, defaultName));

        if (currentEntry.Enabled != defaultEntry.Enabled)
            differences.Add(BuildDifference("Enabled", LightingSettingsPanelClipboardFormatting.FormatValue(currentEntry.Enabled), LightingSettingsPanelClipboardFormatting.FormatValue(defaultEntry.Enabled)));

        if (currentEntry.Color != defaultEntry.Color)
            differences.Add(BuildDifference("Color", LightingSettingsPanelClipboardFormatting.FormatColor(currentEntry.Color), LightingSettingsPanelClipboardFormatting.FormatColor(defaultEntry.Color)));

        List<LightingEventId> currentIds = LightingSettingsPanelClipboardFormatting.GetEventIds(currentEntry);
        List<LightingEventId> defaultIds = LightingSettingsPanelClipboardFormatting.GetEventIds(defaultEntry);
        if (!SequenceEquals(currentIds, defaultIds))
            differences.Add(BuildDifference("Members", JoinEventMembers(currentIds), JoinEventMembers(defaultIds)));

        return BuildModifiedBlock(currentName, differences);
    }

    private static string BuildBossModifiedBlock(LightingBossEffectEntry currentEntry, LightingBossEffectEntry defaultEntry)
    {
        List<string> differences = [];
        string currentName = GetGroupName(currentEntry.Name, "Boss Group");
        string defaultName = GetGroupName(defaultEntry.Name, "Boss Group");
        if (!string.Equals(currentName, defaultName, StringComparison.Ordinal))
            differences.Add(BuildDifference("Name", currentName, defaultName));

        if (currentEntry.Enabled != defaultEntry.Enabled)
            differences.Add(BuildDifference("Enabled", LightingSettingsPanelClipboardFormatting.FormatValue(currentEntry.Enabled), LightingSettingsPanelClipboardFormatting.FormatValue(defaultEntry.Enabled)));

        if (MathF.Abs(currentEntry.Multiplier - defaultEntry.Multiplier) > 0.0001f)
            differences.Add(BuildDifference(
                "Multiplier",
                currentEntry.Multiplier.ToString("0.00", CultureInfo.InvariantCulture),
                defaultEntry.Multiplier.ToString("0.00", CultureInfo.InvariantCulture)));

        List<LightingBossId> currentIds = LightingSettingsPanelClipboardFormatting.GetBossIds(currentEntry);
        List<LightingBossId> defaultIds = LightingSettingsPanelClipboardFormatting.GetBossIds(defaultEntry);
        if (!SequenceEquals(currentIds, defaultIds))
            differences.Add(BuildDifference("Members", JoinBossMembers(currentIds), JoinBossMembers(defaultIds)));

        return BuildModifiedBlock(currentName, differences);
    }

    private static string BuildTileSingleValueBlock(string status, LightingTileEffectEntry entry)
    {
        string name = GetGroupName(entry.Name, "Tile Group");
        string primaryLine = $"Enabled: {LightingSettingsPanelClipboardFormatting.FormatValue(entry.Enabled)} | Color: {LightingSettingsPanelClipboardFormatting.FormatColor(entry.Color)}";

        string members = JoinTileMembers(entry.TileIds);
        string membersLine = string.IsNullOrWhiteSpace(members) ? null : $"Members: {members}";

        return BuildSingleValueBlock(name, status, primaryLine, membersLine);
    }

    private static string BuildEventSingleValueBlock(string status, LightingEventEffectEntry entry)
    {
        string name = GetGroupName(entry.Name, "Event Group");
        string primaryLine = $"Enabled: {LightingSettingsPanelClipboardFormatting.FormatValue(entry.Enabled)} | Color: {LightingSettingsPanelClipboardFormatting.FormatColor(entry.Color)}";

        string members = JoinEventMembers(LightingSettingsPanelClipboardFormatting.GetEventIds(entry));
        string membersLine = string.IsNullOrWhiteSpace(members) ? null : $"Members: {members}";

        return BuildSingleValueBlock(name, status, primaryLine, membersLine);
    }

    private static string BuildBossSingleValueBlock(string status, LightingBossEffectEntry entry)
    {
        string name = GetGroupName(entry.Name, "Boss Group");
        string primaryLine = $"Enabled: {LightingSettingsPanelClipboardFormatting.FormatValue(entry.Enabled)} | Multiplier: {entry.Multiplier.ToString("0.00", CultureInfo.InvariantCulture)}";

        string members = JoinBossMembers(LightingSettingsPanelClipboardFormatting.GetBossIds(entry));
        string membersLine = string.IsNullOrWhiteSpace(members) ? null : $"Members: {members}";

        return BuildSingleValueBlock(name, status, primaryLine, membersLine);
    }

    private static string BuildModifiedBlock(string name, List<string> differences)
    {
        if (differences.Count == 0)
            return $"**{name}:** `Modified`";

        if (differences.Count == 1)
            return $"**{name}:** `Modified` {differences[0]}";

        StringBuilder builder = new();
        builder.AppendLine($"**{name}:** `Modified`");
        for (int i = 0; i < differences.Count; i++)
            builder.AppendLine($"    {differences[i]}");

        return builder.ToString().TrimEnd();
    }

    private static string BuildSingleValueBlock(string name, string status, string primaryLine, string secondaryLine)
    {
        StringBuilder builder = new();
        builder.AppendLine($"**{name}:** `{status}`");

        if (!string.IsNullOrWhiteSpace(primaryLine))
            builder.AppendLine($"    {primaryLine}");

        if (!string.IsNullOrWhiteSpace(secondaryLine))
            builder.AppendLine($"    {secondaryLine}");

        return builder.ToString().TrimEnd();
    }

    private static string BuildDifference(string field, string modifiedValue, string defaultValue)
    {
        return $"{field}: {modifiedValue} *(default: {defaultValue})*";
    }

    private static string JoinTileMembers(IReadOnlyList<int> tileIds)
    {
        if (tileIds is null || tileIds.Count == 0)
            return string.Empty;

        return LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(tileIds, static tileId => LightingSettingsPanelClipboardFormatting.FormatTileMember(tileId));
    }

    private static string JoinEventMembers(IReadOnlyList<LightingEventId> eventIds)
    {
        if (eventIds is null || eventIds.Count == 0)
            return string.Empty;

        return LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(eventIds, static eventId => LightingSettingsPanelClipboardFormatting.FormatEventMember(eventId));
    }

    private static string JoinBossMembers(IReadOnlyList<LightingBossId> bossIds)
    {
        if (bossIds is null || bossIds.Count == 0)
            return string.Empty;

        return LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(bossIds, static bossId => LightingSettingsPanelClipboardFormatting.FormatBossMember(bossId));
    }

    private static string GetGroupName(string name, string fallback)
    {
        return string.IsNullOrWhiteSpace(name) ? fallback : name.Trim();
    }

    private static bool SequenceEquals<T>(IReadOnlyList<T> left, IReadOnlyList<T> right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (left.Count != right.Count)
            return false;

        for (int i = 0; i < left.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(left[i], right[i]))
                return false;
        }

        return true;
    }
}
