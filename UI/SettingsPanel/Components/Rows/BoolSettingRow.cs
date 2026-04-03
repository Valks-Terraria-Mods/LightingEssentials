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
    /// <param name="uiScale">Current panel UI scale factor.</param>
    /// <param name="onRemoveRequested">Optional callback to remove this setting group from dynamic tabs.</param>
    public BoolSettingRow(BoolSettingDescriptor descriptor, LightingSettings settings, Action onSettingChanged, float uiScale, Action onRemoveRequested = null)
    {
        _descriptor = descriptor;
        _settings = settings;
        _onSettingChanged = onSettingChanged;

        float rowHeight = SettingsPanelScale.Pixels(36f, uiScale);
        float rowPadding = SettingsPanelScale.Pixels(8f, uiScale);
        float labelScale = SettingsPanelScale.Text(0.84f, uiScale);
        float toggleTextScale = SettingsPanelScale.Text(0.8f, uiScale);
        float toggleWidth = SettingsPanelScale.Pixels(66f, uiScale);
        float toggleHeight = SettingsPanelScale.Pixels(22f, uiScale);
        float removeButtonWidth = SettingsPanelScale.Pixels(22f, uiScale);
        float removeButtonGap = SettingsPanelScale.Pixels(6f, uiScale);
        float removeOffset = onRemoveRequested is null ? 0f : removeButtonWidth + removeButtonGap;

        Width.Set(0f, 1f);
        Height.Set(rowHeight, 0f);
        SetPadding(rowPadding);

        BackgroundColor = SettingsPanelTheme.RowBackground;
        BorderColor = SettingsPanelTheme.RowBorder;

        UIText label = new(descriptor.Label, labelScale)
        {
            VAlign = 0.5f,
        };
        Append(label);

        _toggleButton = new FlatTextButton(string.Empty, toggleTextScale)
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Left = StyleDimension.FromPixels(-removeOffset),
        };
        _toggleButton.Width.Set(toggleWidth, 0f);
        _toggleButton.Height.Set(toggleHeight, 0f);
        _toggleButton.HoverStyleEnabled = false;
        _toggleButton.OnLeftClick += OnTogglePressed;
        Append(_toggleButton);

        if (onRemoveRequested is not null)
        {
            FlatTextButton removeButton = new("-", toggleTextScale)
            {
                HAlign = 1f,
                VAlign = 0.5f,
            };
            removeButton.Width.Set(removeButtonWidth, 0f);
            removeButton.Height.Set(toggleHeight, 0f);
            removeButton.BackgroundColor = SettingsPanelTheme.Negative;
            removeButton.HoverStyleEnabled = false;
            removeButton.OnLeftClick += (_, _) => onRemoveRequested();
            Append(removeButton);
        }

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
