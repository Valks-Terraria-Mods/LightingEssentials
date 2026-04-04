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

        if (!SequenceEquals(GetBossIds(left), GetBossIds(right)))
            return false;

        return SequenceEquals(GetBossTargetTileGroupKeys(left), GetBossTargetTileGroupKeys(right));
    }

    public static bool AreEntityEntryEquivalent(LightingEntityEffectEntry left, LightingEntityEffectEntry right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (!string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            return false;

        if (left.Enabled != right.Enabled || left.Color != right.Color)
            return false;

        if (left.IncludePlayer != right.IncludePlayer
            || left.IncludeAllEnemies != right.IncludeAllEnemies
            || left.IncludeAllProjectiles != right.IncludeAllProjectiles)
        {
            return false;
        }

        if (!SequenceEquals(left.NpcIds, right.NpcIds))
            return false;

        return SequenceEquals(left.ProjectileIds, right.ProjectileIds);
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

    public static List<string> GetBossTargetTileGroupKeys(LightingBossEffectEntry entry)
    {
        if (entry is null)
            return [];

        List<LightingBossId> bossIds = GetBossIds(entry);
        return LightingDynamicCatalogs.ResolveBossTargetTileGroupKeys(bossIds, entry.TargetTileGroupKeys);
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
        return $"{color.R}, {color.G}, {color.B}";
    }

    public static string FormatTileMember(int tileId)
    {
        if (TileID.Search.TryGetName(tileId, out string tileName) && !string.IsNullOrWhiteSpace(tileName))
            return tileName;

        return "Unknown Tile";
    }

    public static string EscapeMarkdownTableCell(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "-";

        return value
            .Replace("|", "\\|")
            .Replace("\r", " ")
            .Replace("\n", " ");
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

    public static string FormatBossTargetTileGroupMember(string groupKey)
    {
        return LightingDynamicCatalogs.TryGetBossTargetTileGroupCatalogItem(groupKey, out LightingBossTargetTileGroupCatalogItem item)
            ? item.DisplayName
            : groupKey;
    }

    public static List<string> GetEntityMemberKeys(LightingEntityEffectEntry entry)
    {
        List<string> memberKeys = [];
        if (entry is null)
            return memberKeys;

        if (entry.IncludePlayer)
            memberKeys.Add("entity:player");

        if (entry.IncludeAllEnemies)
            memberKeys.Add("entity:npc-all");

        if (entry.IncludeAllProjectiles)
            memberKeys.Add("entity:projectile-all");

        if (entry.NpcIds is not null)
            for (int i = 0; i < entry.NpcIds.Count; i++)
                memberKeys.Add($"entity:npc:{entry.NpcIds[i]}");

        if (entry.ProjectileIds is not null)
            for (int i = 0; i < entry.ProjectileIds.Count; i++)
                memberKeys.Add($"entity:projectile:{entry.ProjectileIds[i]}");

        return memberKeys;
    }

    public static string FormatEntityMember(string key)
    {
        if (LightingDynamicCatalogs.TryGetEntityCatalogItem(key, out LightingEntityCatalogItem item))
            return item.DisplayName;

        return key;
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
