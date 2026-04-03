using System;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal readonly record struct LightingSettingsPanelRowCallbacks(
    Action<LightingSettings> ApplySettingsChange,
    Action<ColorSettingDescriptor, LightingSettings> OpenColorPicker,
    Action<int> EditTileEntry,
    Action<int> RemoveTileEntry,
    Action<int> EditEventEntry,
    Action<int> RemoveEventEntry,
    Action<int> EditBossEntry,
    Action<int> RemoveBossEntry);
