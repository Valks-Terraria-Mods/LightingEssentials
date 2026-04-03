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
    private readonly Action _onSettingChanged;

    private readonly UIPanel _previewPanel;
    private readonly FlatTextButton _toggleButton;

    /// <summary>
    /// Creates a compact color row with preview swatch and popup trigger button.
    /// </summary>
    /// <param name="descriptor">Descriptor that provides color get/set behavior.</param>
    /// <param name="settings">Mutable settings instance.</param>
    /// <param name="onPickRequested">Callback that opens the popup editor for this color descriptor.</param>
    /// <param name="uiScale">Current panel UI scale factor.</param>
    /// <param name="onEditRequested">Optional callback to edit grouped catalog members for this row.</param>
    /// <param name="onRemoveRequested">Optional callback to remove this setting row from dynamic tabs.</param>
    public ColorSettingRow(ColorSettingDescriptor descriptor, LightingSettings settings, Action<ColorSettingDescriptor> onPickRequested, float uiScale, Action onEditRequested = null, Action onRemoveRequested = null, Action onSettingChanged = null)
    {
        _descriptor = descriptor;
        _settings = settings;
        _onPickRequested = onPickRequested;
        _onSettingChanged = onSettingChanged;

        float rowHeight = SettingsPanelScale.Pixels(38f, uiScale);
        float rowPadding = SettingsPanelScale.Pixels(8f, uiScale);
        float labelScale = SettingsPanelScale.Text(0.82f, uiScale);
        float pickTextScale = SettingsPanelScale.Text(0.74f, uiScale);
        float pickWidth = SettingsPanelScale.Pixels(54f, uiScale);
        float pickHeight = SettingsPanelScale.Pixels(22f, uiScale);
        float editWidth = SettingsPanelScale.Pixels(54f, uiScale);
        float previewOffset = SettingsPanelScale.Pixels(60f, uiScale);
        float previewSize = SettingsPanelScale.Pixels(18f, uiScale);
        float toggleButtonWidth = SettingsPanelScale.Pixels(54f, uiScale);
        float removeButtonWidth = SettingsPanelScale.Pixels(24f, uiScale);
        float removeButtonGap = SettingsPanelScale.Pixels(8f, uiScale);
        bool hasToggle = descriptor.EnabledGetter is not null && descriptor.EnabledSetter is not null;
        float toggleOffset = hasToggle ? toggleButtonWidth + removeButtonGap : 0f;
        float removeOffset = onRemoveRequested is null ? 0f : removeButtonWidth + removeButtonGap;
        float editOffset = onEditRequested is null ? 0f : editWidth + removeButtonGap;

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
            Left = StyleDimension.FromPixels(-(removeOffset + toggleOffset)),
        };
        pickButton.Width.Set(pickWidth, 0f);
        pickButton.Height.Set(pickHeight, 0f);
        pickButton.OnLeftClick += OnPickPressed;
        Append(pickButton);

        if (onEditRequested is not null)
        {
            FlatTextButton editButton = new("Edit", pickTextScale)
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(-(removeOffset + toggleOffset + pickWidth + removeButtonGap)),
                HoverStyleEnabled = false,
            };
            editButton.Width.Set(editWidth, 0f);
            editButton.Height.Set(pickHeight, 0f);
            editButton.OnLeftClick += (_, _) => onEditRequested();
            Append(editButton);
        }

        if (hasToggle)
        {
            _toggleButton = new FlatTextButton(string.Empty, pickTextScale)
            {
                HAlign = 1f,
                VAlign = 0.5f,
                Left = StyleDimension.FromPixels(-removeOffset),
                HoverStyleEnabled = false,
            };
            _toggleButton.Width.Set(toggleButtonWidth, 0f);
            _toggleButton.Height.Set(pickHeight, 0f);
            _toggleButton.OnLeftClick += OnTogglePressed;
            Append(_toggleButton);
        }

        if (onRemoveRequested is not null)
        {
            FlatTextButton removeButton = new("-", pickTextScale)
            {
                HAlign = 1f,
                VAlign = 0.5f,
            };
            removeButton.Width.Set(removeButtonWidth, 0f);
            removeButton.Height.Set(pickHeight, 0f);
            removeButton.BackgroundColor = SettingsPanelTheme.Negative;
            removeButton.HoverStyleEnabled = false;
            removeButton.OnLeftClick += (_, _) => onRemoveRequested();
            Append(removeButton);
        }

        _previewPanel = new UIPanel
        {
            HAlign = 1f,
            VAlign = 0.5f,
            Left = StyleDimension.FromPixels(-(previewOffset + removeOffset + toggleOffset + editOffset)),
        };
        _previewPanel.Width.Set(previewSize, 0f);
        _previewPanel.Height.Set(previewSize, 0f);
        _previewPanel.BorderColor = Color.Transparent;
        _previewPanel.SetPadding(0f);
        Append(_previewPanel);

        RefreshToggleState();
    }

    /// <summary>
    /// Keeps the swatch preview synced with underlying setting changes.
    /// </summary>
    /// <param name="gameTime">Current frame timing data.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _previewPanel.BackgroundColor = _descriptor.Getter(_settings);
        RefreshToggleState();
    }

    /// <summary>
    /// Opens the popup color picker for this row.
    /// </summary>
    private void OnPickPressed(UIMouseEvent evt, UIElement listeningElement)
    {
        _onPickRequested(_descriptor);
    }

    private void OnTogglePressed(UIMouseEvent evt, UIElement listeningElement)
    {
        if (_descriptor.EnabledGetter is null || _descriptor.EnabledSetter is null)
            return;

        bool nextValue = !_descriptor.EnabledGetter(_settings);
        _descriptor.EnabledSetter(_settings, nextValue);

        if (_onSettingChanged is null)
            _settings.ApplyRuntimeChanges();
        else
            _onSettingChanged();

        RefreshToggleState();
    }

    private void RefreshToggleState()
    {
        if (_toggleButton is null || _descriptor.EnabledGetter is null)
            return;

        bool enabled = _descriptor.EnabledGetter(_settings);
        _toggleButton.SetText(enabled ? "ON" : "OFF");
        _toggleButton.BackgroundColor = enabled ? SettingsPanelTheme.Positive : SettingsPanelTheme.Negative;
    }
}
