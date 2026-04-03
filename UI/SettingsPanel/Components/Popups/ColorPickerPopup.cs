using System;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Styling;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Popups;

internal sealed class ColorPickerPopup : UIPanel
{
    private readonly Action<Color> _onColorChanged;
    private readonly Action _onClose;
    private readonly Color _defaultColor;
    private readonly float _uiScale;

    private readonly UIPanel _previewPanel;
    private readonly ByteSlider _redSlider;
    private readonly ByteSlider _greenSlider;
    private readonly ByteSlider _blueSlider;
    private readonly ByteSlider _alphaSlider;

    /// <summary>
    /// Creates an RGBA popup editor for a specific color setting.
    /// </summary>
    /// <param name="title">Friendly setting name shown in popup header.</param>
    /// <param name="initialColor">Current setting color.</param>
    /// <param name="defaultColor">Default setting color used by reset action.</param>
    /// <param name="onColorChanged">Callback invoked whenever slider changes produce a new color.</param>
    /// <param name="onClose">Callback invoked when popup close is requested.</param>
    /// <param name="uiScale">Current panel UI scale factor.</param>
    public ColorPickerPopup(string title, Color initialColor, Color defaultColor, Action<Color> onColorChanged, Action onClose, float uiScale)
    {
        _onColorChanged = onColorChanged;
        _onClose = onClose;
        _defaultColor = defaultColor;
        _uiScale = uiScale;

        Width.Set(Scale(352f), 0f);
        Height.Set(Scale(230f), 0f);
        HAlign = 0.5f;
        VAlign = 0.5f;
        SetPadding(Scale(12f));

        BackgroundColor = new Color(12, 12, 12, 245);
        BorderColor = Color.Transparent;

        UIText header = new($"Color Picker: {title}", ScaleText(0.86f));
        Append(header);

        FlatTextButton closeButton = new("Close", ScaleText(0.72f))
        {
            HAlign = 1f,
        };
        closeButton.Width.Set(Scale(54f), 0f);
        closeButton.Height.Set(Scale(22f), 0f);
        closeButton.Top.Set(-Scale(36f), 1f);
        closeButton.Left.Set(Scale(5), 0f);
        closeButton.OnLeftClick += (_, _) => _onClose();
        Append(closeButton);

        // Footer actions are intentionally placed at the bottom edge for quick thumb travel.
        FlatTextButton resetButton = new("Reset to Defaults", ScaleText(0.72f))
        {
            HAlign = 1f,
        };
        resetButton.Width.Set(Scale(122f), 0f);
        resetButton.Height.Set(Scale(22f), 0f);
        resetButton.Top.Set(-Scale(36f), 1f);
        resetButton.Left.Set(-Scale(55f), 0f);
        resetButton.OnLeftClick += (_, _) =>
        {
            SetColor(_defaultColor, notify: true);
        };
        Append(resetButton);

        _previewPanel = new UIPanel
        {
            HAlign = 1f,
            Top = StyleDimension.FromPixels(Scale(30f)),
            BackgroundColor = initialColor,
            BorderColor = Color.Transparent,
        };
        _previewPanel.Width.Set(Scale(50f), 0f);
        _previewPanel.Height.Set(Scale(50f), 0f);
        _previewPanel.SetPadding(0f);
        Append(_previewPanel);

        _redSlider = CreateSlider("R", initialColor.R, Scale(34f));
        _greenSlider = CreateSlider("G", initialColor.G, Scale(66f));
        _blueSlider = CreateSlider("B", initialColor.B, Scale(98f));
        _alphaSlider = CreateSlider("A", initialColor.A, Scale(130f));

        _redSlider.ValueChanged += _ => OnChannelChanged();
        _greenSlider.ValueChanged += _ => OnChannelChanged();
        _blueSlider.ValueChanged += _ => OnChannelChanged();
        _alphaSlider.ValueChanged += _ => OnChannelChanged();

        SetColor(initialColor, notify: false);
    }

    /// <summary>
    /// Marks UI interaction only while cursor is over popup bounds.
    /// </summary>
    /// <param name="spriteBatch">Current sprite batch.</param>
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (ContainsPoint(Main.MouseScreen))
            Main.LocalPlayer.mouseInterface = true;
    }

    /// <summary>
    /// Creates one RGBA channel slider line.
    /// </summary>
    /// <param name="label">Channel label.</param>
    /// <param name="initialValue">Initial channel value.</param>
    /// <param name="top">Top offset inside popup.</param>
    /// <returns>Configured slider instance.</returns>
    private ByteSlider CreateSlider(string label, byte initialValue, float top)
    {
        ByteSlider slider = new(label, initialValue, _uiScale);
        slider.Width.Set(-Scale(72f), 1f);
        slider.Top.Set(top, 0f);
        Append(slider);
        return slider;
    }

    /// <summary>
    /// Converts baseline pixel constants to the popup's active UI scale.
    /// </summary>
    private float Scale(float baselinePixels)
    {
        return SettingsPanelScale.Pixels(baselinePixels, _uiScale);
    }

    /// <summary>
    /// Converts baseline text scales to the popup's active UI scale.
    /// </summary>
    private float ScaleText(float baselineTextScale)
    {
        return SettingsPanelScale.Text(baselineTextScale, _uiScale);
    }

    /// <summary>
    /// Recomputes output color after any channel slider changes.
    /// </summary>
    private void OnChannelChanged()
    {
        ApplyColor(notify: true);
    }

    /// <summary>
    /// Sets all slider channel values from a single <see cref="Color"/> value.
    /// </summary>
    /// <param name="color">Color to push into channel sliders.</param>
    /// <param name="notify">Whether to emit change callback after update.</param>
    private void SetColor(Color color, bool notify)
    {
        _redSlider.SetValue(color.R, notify: false);
        _greenSlider.SetValue(color.G, notify: false);
        _blueSlider.SetValue(color.B, notify: false);
        _alphaSlider.SetValue(color.A, notify: false);
        ApplyColor(notify);
    }

    /// <summary>
    /// Builds the output color from channel sliders and updates preview/callback.
    /// </summary>
    /// <param name="notify">Whether to emit value changed callback.</param>
    private void ApplyColor(bool notify)
    {
        Color selectedColor = new(_redSlider.Value, _greenSlider.Value, _blueSlider.Value, _alphaSlider.Value);
        _previewPanel.BackgroundColor = selectedColor;

        if (notify)
            _onColorChanged(selectedColor);
    }
}
