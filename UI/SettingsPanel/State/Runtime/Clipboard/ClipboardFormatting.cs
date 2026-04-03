using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime.Clipboard;

internal static class LightingSettingsPanelClipboardFormatting
{
    public static bool AreTileEntryEquivalent(LightingTileEffectEntry left, LightingTileEffectEntry right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (!string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            return false;

        if (left.Enabled != right.Enabled || left.Color != right.Color)
            return false;

        return SequenceEquals(left.TileIds, right.TileIds);
    }

    public static bool AreEventEntryEquivalent(LightingEventEffectEntry left, LightingEventEffectEntry right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (!string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            return false;

        if (left.Enabled != right.Enabled || left.Color != right.Color)
            return false;

        return SequenceEquals(GetEventIds(left), GetEventIds(right));
    }

    public static bool AreBossEntryEquivalent(LightingBossEffectEntry left, LightingBossEffectEntry right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (!string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            return false;

        if (left.Enabled != right.Enabled || MathF.Abs(left.Multiplier - right.Multiplier) > 0.0001f)
            return false;

        return SequenceEquals(GetBossIds(left), GetBossIds(right));
    }

    public static List<LightingEventId> GetEventIds(LightingEventEffectEntry entry)
    {
        return entry.EventIds is { Count: > 0 }
            ? entry.EventIds
            : [entry.EventId];
    }

    public static List<LightingBossId> GetBossIds(LightingBossEffectEntry entry)
    {
        return entry.BossIds is { Count: > 0 }
            ? entry.BossIds
            : [entry.BossId];
    }

    public static string JoinFormattedMembers<T>(IReadOnlyList<T> values, Func<T, string> formatter)
    {
        StringBuilder builder = new();
        for (int i = 0; i < values.Count; i++)
        {
            if (i > 0)
                builder.Append(", ");

            builder.Append(formatter(values[i]));
        }

        return builder.ToString();
    }

    public static bool ValuesEqual(object left, object right)
    {
        if (left is null || right is null)
            return left is null && right is null;

        if (left is float leftFloat && right is float rightFloat)
            return MathF.Abs(leftFloat - rightFloat) <= 0.0001f;

        return left.Equals(right);
    }

    public static string FormatValue(object value)
    {
        return value switch
        {
            null => "<null>",
            bool boolValue => boolValue ? "true" : "false",
            float floatValue => floatValue.ToString("0.###", CultureInfo.InvariantCulture),
            Color color => FormatColor(color),
            _ => value.ToString(),
        };
    }

    public static string FormatColor(Color color)
    {
        return $"{color.R},{color.G},{color.B},{color.A}";
    }

    public static string FormatTileMember(int tileId)
    {
        if (TileID.Search.TryGetName(tileId, out string tileName) && !string.IsNullOrWhiteSpace(tileName))
            return $"{tileName}({tileId})";

        return $"Tile{tileId}";
    }

    public static string FormatEventMember(LightingEventId eventId)
    {
        return LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out LightingEventCatalogItem item)
            ? item.DisplayName
            : eventId.ToString();
    }

    public static string FormatBossMember(LightingBossId bossId)
    {
        return LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out LightingBossCatalogItem item)
            ? item.DisplayName
            : bossId.ToString();
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
