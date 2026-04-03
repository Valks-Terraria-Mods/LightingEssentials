using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime.Clipboard;

internal static class LightingSettingsPanelClipboardSections
{
    public static string BuildTileEntriesSection(IReadOnlyList<LightingTileEffectEntry> currentEntries, IReadOnlyList<LightingTileEffectEntry> defaultEntries)
    {
        StringBuilder builder = new();
        builder.AppendLine("Tile groups:");

        bool hasModifiedEntries = false;
        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {
            LightingTileEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingTileEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;
            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                AppendTileDiffLine(builder, "Removed", defaultEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (defaultEntry is null)
            {
                AppendTileDiffLine(builder, "Added", currentEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (LightingSettingsPanelClipboardFormatting.AreTileEntryEquivalent(currentEntry, defaultEntry))
                continue;

            AppendTileDiffLine(builder, "Modified", currentEntry, defaultEntry);
            hasModifiedEntries = true;
        }

        return hasModifiedEntries ? builder.ToString().TrimEnd() : string.Empty;
    }

    public static string BuildEventEntriesSection(IReadOnlyList<LightingEventEffectEntry> currentEntries, IReadOnlyList<LightingEventEffectEntry> defaultEntries)
    {
        StringBuilder builder = new();
        builder.AppendLine("Event groups:");

        bool hasModifiedEntries = false;
        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {
            LightingEventEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingEventEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;
            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                AppendEventDiffLine(builder, "Removed", defaultEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (defaultEntry is null)
            {
                AppendEventDiffLine(builder, "Added", currentEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (LightingSettingsPanelClipboardFormatting.AreEventEntryEquivalent(currentEntry, defaultEntry))
                continue;

            AppendEventDiffLine(builder, "Modified", currentEntry, defaultEntry);
            hasModifiedEntries = true;
        }

        return hasModifiedEntries ? builder.ToString().TrimEnd() : string.Empty;
    }

    public static string BuildBossEntriesSection(IReadOnlyList<LightingBossEffectEntry> currentEntries, IReadOnlyList<LightingBossEffectEntry> defaultEntries)
    {
        StringBuilder builder = new();
        builder.AppendLine("Boss groups:");

        bool hasModifiedEntries = false;
        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {
            LightingBossEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingBossEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;
            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                AppendBossDiffLine(builder, "Removed", defaultEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (defaultEntry is null)
            {
                AppendBossDiffLine(builder, "Added", currentEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (LightingSettingsPanelClipboardFormatting.AreBossEntryEquivalent(currentEntry, defaultEntry))
                continue;

            AppendBossDiffLine(builder, "Modified", currentEntry, defaultEntry);
            hasModifiedEntries = true;
        }

        return hasModifiedEntries ? builder.ToString().TrimEnd() : string.Empty;
    }

    private static void AppendTileDiffLine(StringBuilder builder, string tag, LightingTileEffectEntry entry, LightingTileEffectEntry defaultEntry)
    {
        int memberCount = entry.TileIds?.Count ?? 0;
        string name = string.IsNullOrWhiteSpace(entry.Name) ? "Tile Group" : entry.Name.Trim();
        builder.AppendLine($"- [{tag}] {name} | Enabled={entry.Enabled} | Color={LightingSettingsPanelClipboardFormatting.FormatColor(entry.Color)} | Count={memberCount}");

        if (memberCount > 0)
            builder.AppendLine($"  Members: {LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(entry.TileIds, static tileId => LightingSettingsPanelClipboardFormatting.FormatTileMember(tileId))}");

        if (defaultEntry is null || tag != "Modified")
            return;

        int defaultCount = defaultEntry.TileIds?.Count ?? 0;
        builder.AppendLine($"  Default -> Enabled={defaultEntry.Enabled} | Color={LightingSettingsPanelClipboardFormatting.FormatColor(defaultEntry.Color)} | Count={defaultCount}");
        if (defaultCount > 0)
            builder.AppendLine($"  Default Members: {LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(defaultEntry.TileIds, static tileId => LightingSettingsPanelClipboardFormatting.FormatTileMember(tileId))}");
    }

    private static void AppendEventDiffLine(StringBuilder builder, string tag, LightingEventEffectEntry entry, LightingEventEffectEntry defaultEntry)
    {
        List<LightingEventId> eventIds = LightingSettingsPanelClipboardFormatting.GetEventIds(entry);
        string name = string.IsNullOrWhiteSpace(entry.Name) ? "Event Group" : entry.Name.Trim();
        builder.AppendLine($"- [{tag}] {name} | Enabled={entry.Enabled} | Color={LightingSettingsPanelClipboardFormatting.FormatColor(entry.Color)} | Count={eventIds.Count}");
        if (eventIds.Count > 0)
            builder.AppendLine($"  Members: {LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(eventIds, static eventId => LightingSettingsPanelClipboardFormatting.FormatEventMember(eventId))}");

        if (defaultEntry is null || tag != "Modified")
            return;

        List<LightingEventId> defaultEventIds = LightingSettingsPanelClipboardFormatting.GetEventIds(defaultEntry);
        builder.AppendLine($"  Default -> Enabled={defaultEntry.Enabled} | Color={LightingSettingsPanelClipboardFormatting.FormatColor(defaultEntry.Color)} | Count={defaultEventIds.Count}");
        if (defaultEventIds.Count > 0)
            builder.AppendLine($"  Default Members: {LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(defaultEventIds, static eventId => LightingSettingsPanelClipboardFormatting.FormatEventMember(eventId))}");
    }

    private static void AppendBossDiffLine(StringBuilder builder, string tag, LightingBossEffectEntry entry, LightingBossEffectEntry defaultEntry)
    {
        List<LightingBossId> bossIds = LightingSettingsPanelClipboardFormatting.GetBossIds(entry);
        string name = string.IsNullOrWhiteSpace(entry.Name) ? "Boss Group" : entry.Name.Trim();
        builder.AppendLine($"- [{tag}] {name} | Enabled={entry.Enabled} | Multiplier={entry.Multiplier.ToString("0.00", CultureInfo.InvariantCulture)} | Count={bossIds.Count}");
        if (bossIds.Count > 0)
            builder.AppendLine($"  Members: {LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(bossIds, static bossId => LightingSettingsPanelClipboardFormatting.FormatBossMember(bossId))}");

        if (defaultEntry is null || tag != "Modified")
            return;

        List<LightingBossId> defaultBossIds = LightingSettingsPanelClipboardFormatting.GetBossIds(defaultEntry);
        builder.AppendLine($"  Default -> Enabled={defaultEntry.Enabled} | Multiplier={defaultEntry.Multiplier.ToString("0.00", CultureInfo.InvariantCulture)} | Count={defaultBossIds.Count}");
        if (defaultBossIds.Count > 0)
            builder.AppendLine($"  Default Members: {LightingSettingsPanelClipboardFormatting.JoinFormattedMembers(defaultBossIds, static bossId => LightingSettingsPanelClipboardFormatting.FormatBossMember(bossId))}");
    }
}
