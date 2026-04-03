using System;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal static class LightingSettingsPanelEntryRemoval
{
    public static void RemoveTileEntry(int index, Action<LightingSettings> applySettingsChange, Action rebuildRows)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();
        if (index < 0 || index >= settings.TileEffectEntries.Count)
            return;

        settings.TileEffectEntries.RemoveAt(index);
        applySettingsChange(settings);
        rebuildRows();
    }

    public static void RemoveEventEntry(int index, Action<LightingSettings> applySettingsChange, Action rebuildRows)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();
        if (index < 0 || index >= settings.EventEffectEntries.Count)
            return;

        settings.EventEffectEntries.RemoveAt(index);
        applySettingsChange(settings);
        rebuildRows();
    }

    public static void RemoveBossEntry(int index, Action<LightingSettings> applySettingsChange, Action rebuildRows)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();
        if (index < 0 || index >= settings.BossEffectEntries.Count)
            return;

        settings.BossEffectEntries.RemoveAt(index);
        applySettingsChange(settings);
        rebuildRows();
    }

    public static void RemoveEntityEntry(int index, Action<LightingSettings> applySettingsChange, Action rebuildRows)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();
        if (index < 0 || index >= settings.EntityEffectEntries.Count)
            return;

        settings.EntityEffectEntries.RemoveAt(index);
        applySettingsChange(settings);
        rebuildRows();
    }
}
