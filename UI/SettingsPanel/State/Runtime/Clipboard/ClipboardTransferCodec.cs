using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime.Clipboard;

internal static class LightingSettingsPanelClipboardTransferCodec
{
    private const string TransferPrefix = "LESMOD1:";

    private static readonly FieldInfo[] CopyableSettingsFields = typeof(LightingSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);
    private static readonly Dictionary<string, FieldInfo> CopyableSettingsFieldMap = BuildFieldMap();
    private static readonly HashSet<string> NonScalarCopiedFields = ["TileEffectEntries", "EventEffectEntries", "BossEffectEntries", "UiScale"];
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    public static string BuildTransferCode(LightingSettings settings)
    {
        settings.EnsureDynamicEntries();
        LightingSettings defaults = LightingSettingsDefaults.CreateDefaults();

        LightingSettingsPanelTransferPayload payload = BuildPayload(settings, defaults);
        if (!payload.HasAnyChanges)
            return string.Empty;

        string payloadJson = JsonSerializer.Serialize(payload, SerializerOptions);
        byte[] compressedPayload = CompressUtf8(payloadJson);
        return TransferPrefix + Convert.ToBase64String(compressedPayload);
    }

    public static bool TryApplyTransferCode(string transferCode, LightingSettings settings, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(transferCode))
        {
            errorMessage = "No import text was provided.";
            return false;
        }

        string normalizedCode = RemoveWhitespace(transferCode);
        if (normalizedCode.StartsWith(TransferPrefix, StringComparison.OrdinalIgnoreCase))
            normalizedCode = normalizedCode[TransferPrefix.Length..];

        if (string.IsNullOrWhiteSpace(normalizedCode))
        {
            errorMessage = "The import text is missing encoded data.";
            return false;
        }

        LightingSettingsPanelTransferPayload payload;
        try
        {
            byte[] compressedPayload = Convert.FromBase64String(normalizedCode);
            string payloadJson = DecompressUtf8(compressedPayload);
            payload = JsonSerializer.Deserialize<LightingSettingsPanelTransferPayload>(payloadJson, SerializerOptions);
        }
        catch
        {
            errorMessage = "Failed to decode settings string. Paste the full export string and try again.";
            return false;
        }

        if (payload is null)
        {
            errorMessage = "Failed to read encoded settings payload.";
            return false;
        }

        if (payload.Version != 1)
        {
            errorMessage = $"Unsupported settings payload version: {payload.Version}.";
            return false;
        }

        if (!payload.HasAnyChanges)
        {
            errorMessage = "The import string did not include any modified settings.";
            return false;
        }

        ApplyPayload(payload, settings);
        settings.EnsureDynamicEntries();
        return true;
    }

    private static LightingSettingsPanelTransferPayload BuildPayload(LightingSettings settings, LightingSettings defaults)
    {
        LightingSettingsPanelTransferPayload payload = new();

        for (int i = 0; i < CopyableSettingsFields.Length; i++)
        {
            FieldInfo field = CopyableSettingsFields[i];
            if (NonScalarCopiedFields.Contains(field.Name))
                continue;

            object currentValue = field.GetValue(settings);
            object defaultValue = field.GetValue(defaults);
            if (LightingSettingsPanelClipboardFormatting.ValuesEqual(currentValue, defaultValue))
                continue;

            if (field.FieldType == typeof(bool) && currentValue is bool boolValue)
            {
                payload.BoolScalars ??= [];
                payload.BoolScalars[field.Name] = boolValue;
                continue;
            }

            if (field.FieldType == typeof(float) && currentValue is float floatValue)
            {
                payload.FloatScalars ??= [];
                payload.FloatScalars[field.Name] = floatValue;
                continue;
            }

            if (field.FieldType == typeof(Color) && currentValue is Color colorValue)
            {
                payload.ColorScalars ??= [];
                payload.ColorScalars[field.Name] = LightingSettingsPanelTransferColor.FromColor(colorValue);
            }
        }

        if (!AreTileEntryListsEquivalent(settings.TileEffectEntries, defaults.TileEffectEntries))
            payload.TileEntries = BuildTileEntriesPayload(settings.TileEffectEntries);

        if (!AreEventEntryListsEquivalent(settings.EventEffectEntries, defaults.EventEffectEntries))
            payload.EventEntries = BuildEventEntriesPayload(settings.EventEffectEntries);

        if (!AreBossEntryListsEquivalent(settings.BossEffectEntries, defaults.BossEffectEntries))
            payload.BossEntries = BuildBossEntriesPayload(settings.BossEffectEntries);

        return payload;
    }

    private static void ApplyPayload(LightingSettingsPanelTransferPayload payload, LightingSettings settings)
    {
        ApplyBoolScalars(payload, settings);
        ApplyFloatScalars(payload, settings);
        ApplyColorScalars(payload, settings);

        if (payload.TileEntries is not null)
            settings.TileEffectEntries = BuildTileEntriesFromPayload(payload.TileEntries);

        if (payload.EventEntries is not null)
            settings.EventEffectEntries = BuildEventEntriesFromPayload(payload.EventEntries);

        if (payload.BossEntries is not null)
            settings.BossEffectEntries = BuildBossEntriesFromPayload(payload.BossEntries);
    }

    private static void ApplyBoolScalars(LightingSettingsPanelTransferPayload payload, LightingSettings settings)
    {
        if (payload.BoolScalars is null)
            return;

        foreach ((string fieldName, bool fieldValue) in payload.BoolScalars)
        {
            if (!TryGetScalarField(fieldName, typeof(bool), out FieldInfo field))
                continue;

            field.SetValue(settings, fieldValue);
        }
    }

    private static void ApplyFloatScalars(LightingSettingsPanelTransferPayload payload, LightingSettings settings)
    {
        if (payload.FloatScalars is null)
            return;

        foreach ((string fieldName, float fieldValue) in payload.FloatScalars)
        {
            if (!TryGetScalarField(fieldName, typeof(float), out FieldInfo field))
                continue;

            field.SetValue(settings, fieldValue);
        }
    }

    private static void ApplyColorScalars(LightingSettingsPanelTransferPayload payload, LightingSettings settings)
    {
        if (payload.ColorScalars is null)
            return;

        foreach ((string fieldName, LightingSettingsPanelTransferColor fieldValue) in payload.ColorScalars)
        {
            if (!TryGetScalarField(fieldName, typeof(Color), out FieldInfo field))
                continue;

            field.SetValue(settings, fieldValue.ToColor());
        }
    }

    private static bool TryGetScalarField(string fieldName, Type fieldType, out FieldInfo field)
    {
        if (NonScalarCopiedFields.Contains(fieldName))
        {
            field = null;
            return false;
        }

        if (!CopyableSettingsFieldMap.TryGetValue(fieldName, out field))
            return false;

        return field.FieldType == fieldType;
    }

    private static Dictionary<string, FieldInfo> BuildFieldMap()
    {
        Dictionary<string, FieldInfo> map = new(StringComparer.Ordinal);
        for (int i = 0; i < CopyableSettingsFields.Length; i++)
        {
            FieldInfo field = CopyableSettingsFields[i];
            map[field.Name] = field;
        }

        return map;
    }

    private static List<LightingSettingsPanelTransferTileEntry> BuildTileEntriesPayload(IReadOnlyList<LightingTileEffectEntry> entries)
    {
        List<LightingSettingsPanelTransferTileEntry> payloadEntries = [];

        for (int i = 0; i < entries.Count; i++)
        {
            LightingTileEffectEntry entry = entries[i];
            if (entry is null)
                continue;

            payloadEntries.Add(new LightingSettingsPanelTransferTileEntry
            {
                Name = entry.Name ?? string.Empty,
                TileIds = entry.TileIds is null ? [] : [..entry.TileIds],
                Enabled = entry.Enabled,
                ColorValue = LightingSettingsPanelTransferColor.FromColor(entry.Color),
            });
        }

        return payloadEntries;
    }

    private static List<LightingSettingsPanelTransferEventEntry> BuildEventEntriesPayload(IReadOnlyList<LightingEventEffectEntry> entries)
    {
        List<LightingSettingsPanelTransferEventEntry> payloadEntries = [];

        for (int i = 0; i < entries.Count; i++)
        {
            LightingEventEffectEntry entry = entries[i];
            if (entry is null)
                continue;

            List<LightingEventId> eventIds = LightingSettingsPanelClipboardFormatting.GetEventIds(entry);
            List<int> eventIdValues = [];
            for (int j = 0; j < eventIds.Count; j++)
                eventIdValues.Add((int)eventIds[j]);

            payloadEntries.Add(new LightingSettingsPanelTransferEventEntry
            {
                Name = entry.Name ?? string.Empty,
                EventIds = eventIdValues,
                Enabled = entry.Enabled,
                ColorValue = LightingSettingsPanelTransferColor.FromColor(entry.Color),
            });
        }

        return payloadEntries;
    }

    private static List<LightingSettingsPanelTransferBossEntry> BuildBossEntriesPayload(IReadOnlyList<LightingBossEffectEntry> entries)
    {
        List<LightingSettingsPanelTransferBossEntry> payloadEntries = [];

        for (int i = 0; i < entries.Count; i++)
        {
            LightingBossEffectEntry entry = entries[i];
            if (entry is null)
                continue;

            List<LightingBossId> bossIds = LightingSettingsPanelClipboardFormatting.GetBossIds(entry);
            List<int> bossIdValues = [];
            for (int j = 0; j < bossIds.Count; j++)
                bossIdValues.Add((int)bossIds[j]);

            payloadEntries.Add(new LightingSettingsPanelTransferBossEntry
            {
                Name = entry.Name ?? string.Empty,
                BossIds = bossIdValues,
                Enabled = entry.Enabled,
                Multiplier = entry.Multiplier,
            });
        }

        return payloadEntries;
    }

    private static List<LightingTileEffectEntry> BuildTileEntriesFromPayload(IReadOnlyList<LightingSettingsPanelTransferTileEntry> payloadEntries)
    {
        List<LightingTileEffectEntry> entries = [];

        for (int i = 0; i < payloadEntries.Count; i++)
        {
            LightingSettingsPanelTransferTileEntry payloadEntry = payloadEntries[i];
            if (payloadEntry is null)
                continue;

            List<int> tileIds = payloadEntry.TileIds is null ? [] : [..payloadEntry.TileIds];
            entries.Add(new LightingTileEffectEntry(payloadEntry.Name ?? string.Empty, tileIds, payloadEntry.ColorValue.ToColor(), payloadEntry.Enabled));
        }

        return entries;
    }

    private static List<LightingEventEffectEntry> BuildEventEntriesFromPayload(IReadOnlyList<LightingSettingsPanelTransferEventEntry> payloadEntries)
    {
        List<LightingEventEffectEntry> entries = [];

        for (int i = 0; i < payloadEntries.Count; i++)
        {
            LightingSettingsPanelTransferEventEntry payloadEntry = payloadEntries[i];
            if (payloadEntry is null)
                continue;

            List<LightingEventId> eventIds = [];
            if (payloadEntry.EventIds is not null)
            {
                for (int j = 0; j < payloadEntry.EventIds.Count; j++)
                    eventIds.Add((LightingEventId)payloadEntry.EventIds[j]);
            }

            if (eventIds.Count == 0)
                eventIds.Add(LightingEventId.BloodMoon);

            entries.Add(new LightingEventEffectEntry(payloadEntry.Name ?? string.Empty, eventIds, payloadEntry.Enabled, payloadEntry.ColorValue.ToColor()));
        }

        return entries;
    }

    private static List<LightingBossEffectEntry> BuildBossEntriesFromPayload(IReadOnlyList<LightingSettingsPanelTransferBossEntry> payloadEntries)
    {
        List<LightingBossEffectEntry> entries = [];

        for (int i = 0; i < payloadEntries.Count; i++)
        {
            LightingSettingsPanelTransferBossEntry payloadEntry = payloadEntries[i];
            if (payloadEntry is null)
                continue;

            List<LightingBossId> bossIds = [];
            if (payloadEntry.BossIds is not null)
            {
                for (int j = 0; j < payloadEntry.BossIds.Count; j++)
                    bossIds.Add((LightingBossId)payloadEntry.BossIds[j]);
            }

            if (bossIds.Count == 0)
                bossIds.Add(LightingBossId.KingSlime);

            entries.Add(new LightingBossEffectEntry(payloadEntry.Name ?? string.Empty, bossIds, payloadEntry.Enabled, payloadEntry.Multiplier));
        }

        return entries;
    }

    private static bool AreTileEntryListsEquivalent(IReadOnlyList<LightingTileEffectEntry> left, IReadOnlyList<LightingTileEffectEntry> right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (left.Count != right.Count)
            return false;

        for (int i = 0; i < left.Count; i++)
        {
            if (!LightingSettingsPanelClipboardFormatting.AreTileEntryEquivalent(left[i], right[i]))
                return false;
        }

        return true;
    }

    private static bool AreEventEntryListsEquivalent(IReadOnlyList<LightingEventEffectEntry> left, IReadOnlyList<LightingEventEffectEntry> right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (left.Count != right.Count)
            return false;

        for (int i = 0; i < left.Count; i++)
        {
            if (!LightingSettingsPanelClipboardFormatting.AreEventEntryEquivalent(left[i], right[i]))
                return false;
        }

        return true;
    }

    private static bool AreBossEntryListsEquivalent(IReadOnlyList<LightingBossEffectEntry> left, IReadOnlyList<LightingBossEffectEntry> right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (left.Count != right.Count)
            return false;

        for (int i = 0; i < left.Count; i++)
        {
            if (!LightingSettingsPanelClipboardFormatting.AreBossEntryEquivalent(left[i], right[i]))
                return false;
        }

        return true;
    }

    private static string RemoveWhitespace(string value)
    {
        StringBuilder builder = new(value.Length);
        for (int i = 0; i < value.Length; i++)
        {
            char character = value[i];
            if (!char.IsWhiteSpace(character))
                builder.Append(character);
        }

        return builder.ToString();
    }

    private static byte[] CompressUtf8(string value)
    {
        byte[] input = Encoding.UTF8.GetBytes(value);

        using MemoryStream output = new();
        using (DeflateStream compressor = new(output, CompressionLevel.SmallestSize, leaveOpen: true))
            compressor.Write(input, 0, input.Length);

        return output.ToArray();
    }

    private static string DecompressUtf8(byte[] value)
    {
        using MemoryStream input = new(value);
        using DeflateStream decompressor = new(input, CompressionMode.Decompress);
        using StreamReader reader = new(decompressor, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}

internal sealed class LightingSettingsPanelTransferPayload
{
    public int Version { get; set; } = 1;

    public Dictionary<string, bool> BoolScalars { get; set; }

    public Dictionary<string, float> FloatScalars { get; set; }

    public Dictionary<string, LightingSettingsPanelTransferColor> ColorScalars { get; set; }

    public List<LightingSettingsPanelTransferTileEntry> TileEntries { get; set; }

    public List<LightingSettingsPanelTransferEventEntry> EventEntries { get; set; }

    public List<LightingSettingsPanelTransferBossEntry> BossEntries { get; set; }

    [JsonIgnore]
    public bool HasAnyChanges =>
        (BoolScalars is { Count: > 0 })
        || (FloatScalars is { Count: > 0 })
        || (ColorScalars is { Count: > 0 })
        || TileEntries is not null
        || EventEntries is not null
        || BossEntries is not null;
}

internal readonly record struct LightingSettingsPanelTransferColor(byte R, byte G, byte B, byte A)
{
    public static LightingSettingsPanelTransferColor FromColor(Color value)
    {
        return new LightingSettingsPanelTransferColor(value.R, value.G, value.B, value.A);
    }

    public Color ToColor()
    {
        return new Color(R, G, B, A);
    }
}

internal sealed class LightingSettingsPanelTransferTileEntry
{
    public string Name { get; set; } = string.Empty;

    public List<int> TileIds { get; set; } = [];

    public bool Enabled { get; set; } = true;

    public LightingSettingsPanelTransferColor ColorValue { get; set; } = new(255, 255, 255, 255);
}

internal sealed class LightingSettingsPanelTransferEventEntry
{
    public string Name { get; set; } = string.Empty;

    public List<int> EventIds { get; set; } = [];

    public bool Enabled { get; set; } = true;

    public LightingSettingsPanelTransferColor ColorValue { get; set; } = new(255, 255, 255, 255);
}

internal sealed class LightingSettingsPanelTransferBossEntry
{
    public string Name { get; set; } = string.Empty;

    public List<int> BossIds { get; set; } = [];

    public bool Enabled { get; set; } = true;

    public float Multiplier { get; set; } = 1.4f;
}
