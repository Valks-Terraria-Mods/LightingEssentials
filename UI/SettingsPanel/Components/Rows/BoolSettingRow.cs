using System;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.Styling;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Rows;

internal sealed class BoolSettingRow : UIPanel
{
    private readonly LightingSettings _settings;
    private readonly BoolSettingDescriptor _descriptor;
    private readonly Action _onSettingChanged;

    private readonly FlatTextButton _toggleButton;

    /// <summary>
    /// Creates a compact boolean setting row with label and toggle action.
    /// </summary>
    /// <param name="descriptor">Descriptor defining getter/setter and label text.</param>
    /// <param name="settings">Mutable settings instance.</param>
    /// <param name="onSettingChanged">Callback invoked after the value changes.</param>
    public BoolSettingRow(BoolSettingDescriptor descriptor, LightingSettings settings, Action onSettingChanged)
    {
        _descriptor = descriptor;
        _settings = settings;
        _onSettingChanged = onSettingChanged;

        Width.Set(0f, 1f);
        Height.Set(36f, 0f);
        SetPadding(8f);

        BackgroundColor = SettingsPanelTheme.RowBackground;
        BorderColor = SettingsPanelTheme.RowBorder;

        UIText label = new(descriptor.Label, 0.84f)
        {
            VAlign = 0.5f,
        };
        Append(label);

        _toggleButton = new FlatTextButton(string.Empty)
        {
            HAlign = 1f,
            VAlign = 0.5f,
        };
        _toggleButton.Width.Set(66f, 0f);
        _toggleButton.Height.Set(22f, 0f);
        _toggleButton.HoverStyleEnabled = false;
        _toggleButton.OnLeftClick += OnTogglePressed;
        Append(_toggleButton);

        RefreshVisualState();
    }

    /// <summary>
    /// Handles button clicks by flipping the underlying boolean value.
    /// </summary>
    private void OnTogglePressed(UIMouseEvent evt, UIElement listeningElement)
    {
        bool nextValue = !_descriptor.Getter(_settings);
        _descriptor.Setter(_settings, nextValue);
        _onSettingChanged();
        RefreshVisualState();
    }

    /// <summary>
    /// Updates toggle text/color to reflect current setting value.
    /// </summary>
    private void RefreshVisualState()
    {
        bool enabled = _descriptor.Getter(_settings);
        _toggleButton.SetText(enabled ? "ON" : "OFF");
        _toggleButton.BackgroundColor = enabled ? SettingsPanelTheme.Positive : SettingsPanelTheme.Negative;
    }
}
