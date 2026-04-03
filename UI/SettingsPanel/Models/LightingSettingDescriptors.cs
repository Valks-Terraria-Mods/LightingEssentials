using System;

namespace LightingEssentials.UI.SettingsPanel.Models;

internal abstract record LightingSettingDescriptor(string Label);

internal sealed record BoolSettingDescriptor(
    string Label,
    Func<LightingSettings, bool> Getter,
    Action<LightingSettings, bool> Setter)
    : LightingSettingDescriptor(Label);

internal sealed record FloatSettingDescriptor(
    string Label,
    float Min,
    float Max,
    float Step,
    Func<LightingSettings, float> Getter,
    Action<LightingSettings, float> Setter,
    Func<LightingSettings, bool> EnabledGetter = null,
    Action<LightingSettings, bool> EnabledSetter = null)
    : LightingSettingDescriptor(Label);

internal sealed record ColorSettingDescriptor(
    string Label,
    Func<LightingSettings, Color> Getter,
    Action<LightingSettings, Color> Setter,
    Func<LightingSettings, Color> DefaultGetter = null,
    Func<LightingSettings, bool> EnabledGetter = null,
    Action<LightingSettings, bool> EnabledSetter = null)
    : LightingSettingDescriptor(Label);
