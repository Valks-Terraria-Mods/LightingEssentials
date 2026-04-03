using System;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.Styling;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Rows;

internal sealed class ColorSettingRow : UIPanel
{
    private readonly LightingSettings _settings;
    private readonly ColorSettingDescriptor _descriptor;
    private readonly Action<ColorSettingDescriptor> _onPickRequested;

    private readonly UIPanel _previewPanel;

    /// <summary>
    /// Creates a compact color row with preview swatch and popup trigger button.
    /// </summary>
    /// <param name="descriptor">Descriptor that provides color get/set behavior.</param>
    /// <param name="settings">Mutable settings instance.</param>
    /// <param name="onPickRequested">Callback that opens the popup editor for this color descriptor.</param>
    /// <param name="uiScale">Current panel UI scale factor.</param>
    public ColorSettingRow(ColorSettingDescriptor descriptor, LightingSettings settings, Action<ColorSettingDescriptor> onPickRequested, float uiScale)
    {
        _descriptor = descriptor;
        _settings = settings;
        _onPickRequested = onPickRequested;

        float rowHeight = SettingsPanelScale.Pixels(38f, uiScale);
        float rowPadding = SettingsPanelScale.Pixels(8f, uiScale);
        float labelScale = SettingsPanelScale.Text(0.82f, uiScale);
        float pickTextScale = SettingsPanelScale.Text(0.74f, uiScale);
        float pickWidth = SettingsPanelScale.Pixels(56f, uiScale);
        float pickHeight = SettingsPanelScale.Pixels(22f, uiScale);
        float previewOffset = SettingsPanelScale.Pixels(60f, uiScale);
        float previewSize = SettingsPanelScale.Pixels(18f, uiScale);

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

        FlatTextButton pickButton = new("Pick", pickTextScale)
        {
            HAlign = 1f,
            VAlign = 0.5f,
        };
        pickButton.Width.Set(pickWidth, 0f);
        pickButton.Height.Set(pickHeight, 0f);
        pickButton.OnLeftClick += OnPickPressed;
        Append(pickButton);

        _previewPanel = new UIPanel
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Left = StyleDimension.FromPixels(-previewOffset),
        };
        _previewPanel.Width.Set(previewSize, 0f);
        _previewPanel.Height.Set(previewSize, 0f);
        _previewPanel.BorderColor = Color.Transparent;
        _previewPanel.SetPadding(0f);
        Append(_previewPanel);
    }

    /// <summary>
    /// Keeps the swatch preview synced with underlying setting changes.
    /// </summary>
    /// <param name="gameTime">Current frame timing data.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _previewPanel.BackgroundColor = _descriptor.Getter(_settings);
    }

    /// <summary>
    /// Opens the popup color picker for this row.
    /// </summary>
    private void OnPickPressed(UIMouseEvent evt, UIElement listeningElement)
    {
        _onPickRequested(_descriptor);
    }
}
