using System;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.Styling;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Rows;

internal sealed class FloatSettingRow : UIPanel
{
    private readonly LightingSettings _settings;
    private readonly FloatSettingDescriptor _descriptor;
    private readonly Action _onSettingChanged;

    private readonly UIElement _sliderTrack;
    private readonly UIText _valueText;
    private bool _dragging;

    private const float KnobWidth = 14f;
    private const float KnobHeight = 14f;

    private static readonly Color TrackBorderColor = new(70, 86, 112, 255);
    private static readonly Color TrackBackgroundColor = new(18, 22, 30, 255);
    private static readonly Color FillColor = new(120, 176, 255, 255);
    private static readonly Color KnobFaceColor = new(24, 32, 44, 255);
    private static readonly Color KnobBorderColor = new(146, 190, 255, 255);
    private static readonly Color KnobGripColor = new(186, 216, 255, 255);

    /// <summary>
    /// Creates a slider-backed float row used for multiplier tuning.
    /// </summary>
    /// <param name="descriptor">Descriptor defining range, step, and get/set behavior.</param>
    /// <param name="settings">Mutable settings instance.</param>
    /// <param name="onSettingChanged">Callback invoked after value updates.</param>
    public FloatSettingRow(FloatSettingDescriptor descriptor, LightingSettings settings, Action onSettingChanged)
    {
        _descriptor = descriptor;
        _settings = settings;
        _onSettingChanged = onSettingChanged;

        Width.Set(0f, 1f);
        Height.Set(52f, 0f);
        SetPadding(8f);

        BackgroundColor = SettingsPanelTheme.RowBackground;
        BorderColor = SettingsPanelTheme.RowBorder;

        UIText label = new(descriptor.Label, 0.82f)
        {
            Top = StyleDimension.FromPixels(2f),
        };
        Append(label);

        _valueText = new UIText(string.Empty, 0.76f)
        {
            HAlign = 1f,
            Top = StyleDimension.FromPixels(2f),
        };
        Append(_valueText);

        _sliderTrack = new UIElement();
        _sliderTrack.Width.Set(0f, 1f);
        _sliderTrack.Height.Set(10f, 0f);
        _sliderTrack.Top.Set(30f, 0f);
        _sliderTrack.OnLeftMouseDown += OnSliderMouseDown;
        _sliderTrack.OnLeftClick += OnSliderMouseDown;
        _sliderTrack.OnLeftMouseUp += OnSliderMouseUp;
        Append(_sliderTrack);

        RefreshVisualState();
    }

    /// <summary>
    /// Draws the row and then renders a flat slider with constrained knob travel.
    /// </summary>
    /// <param name="spriteBatch">Current sprite batch.</param>
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        DrawSlider(spriteBatch);
    }

    /// <summary>
    /// Continues slider dragging while the left mouse button is held.
    /// </summary>
    /// <param name="gameTime">Current frame timing data.</param>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!_dragging)
            return;

        if (!Main.mouseLeft)
        {
            _dragging = false;
            return;
        }

        UpdateValueFromMouse();
    }

    /// <summary>
    /// Starts dragging and immediately snaps to the current mouse position.
    /// </summary>
    private void OnSliderMouseDown(UIMouseEvent evt, UIElement listeningElement)
    {
        _dragging = true;
        UpdateValueFromMouse();
    }

    /// <summary>
    /// Ends active drag interaction.
    /// </summary>
    private void OnSliderMouseUp(UIMouseEvent evt, UIElement listeningElement)
    {
        _dragging = false;
    }

    /// <summary>
    /// Converts mouse X position to a clamped/stepped float value.
    /// </summary>
    private void UpdateValueFromMouse()
    {
        CalculatedStyle dimensions = _sliderTrack.GetDimensions();
        if (dimensions.Width <= 0f)
            return;

        float sliderMinX = dimensions.X + 1f;
        float sliderWidth = Math.Max(1f, dimensions.Width - 2f);
        float travelWidth = Math.Max(1f, sliderWidth - KnobWidth);

        float knobLeft = Main.MouseScreen.X - (KnobWidth * 0.5f);
        float t = (knobLeft - sliderMinX) / travelWidth;
        t = MathHelper.Clamp(t, 0f, 1f);

        float rawValue = _descriptor.Min + ((_descriptor.Max - _descriptor.Min) * t);
        float snappedValue = MathF.Round(rawValue / _descriptor.Step) * _descriptor.Step;
        float finalValue = Math.Clamp(snappedValue, _descriptor.Min, _descriptor.Max);

        _descriptor.Setter(_settings, finalValue);
        _onSettingChanged();
        RefreshVisualState();
    }

    /// <summary>
    /// Syncs slider fill/knob visuals and value text to current setting value.
    /// </summary>
    private void RefreshVisualState()
    {
        float value = _descriptor.Getter(_settings);
        _valueText.SetText(value.ToString("0.00"));
    }

    /// <summary>
    /// Draws the float slider track, fill, and knob with exact edge clamping.
    /// </summary>
    /// <param name="spriteBatch">Current sprite batch.</param>
    private void DrawSlider(SpriteBatch spriteBatch)
    {
        CalculatedStyle dimensions = _sliderTrack.GetDimensions();
        if (dimensions.Width <= 2f || dimensions.Height <= 2f)
            return;

        Rectangle outer = new((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height);
        Rectangle inner = new(outer.X + 1, outer.Y + 1, Math.Max(1, outer.Width - 2), Math.Max(1, outer.Height - 2));

        float min = _descriptor.Min;
        float max = _descriptor.Max;
        float value = _descriptor.Getter(_settings);

        float t = max <= min
            ? 0f
            : MathHelper.Clamp((value - min) / (max - min), 0f, 1f);

        int knobPixelWidth = (int)KnobWidth;
        int knobPixelHeight = (int)KnobHeight;
        int travelWidth = Math.Max(0, inner.Width - knobPixelWidth);
        int knobLeft = inner.X + (int)MathF.Round(travelWidth * t);
        int knobTop = inner.Y + ((inner.Height - knobPixelHeight) / 2);

        int fillWidth = Math.Clamp((knobLeft + knobPixelWidth) - inner.X, 0, inner.Width);
        Rectangle fillRect = new(inner.X, inner.Y, fillWidth, inner.Height);

        Rectangle knobRect = new(knobLeft, knobTop, knobPixelWidth, knobPixelHeight);
        Rectangle knobInner = new(knobRect.X + 1, knobRect.Y + 1, Math.Max(1, knobRect.Width - 2), Math.Max(1, knobRect.Height - 2));

        Texture2D pixel = TextureAssets.MagicPixel.Value;
        spriteBatch.Draw(pixel, outer, TrackBorderColor);
        spriteBatch.Draw(pixel, inner, TrackBackgroundColor);
        spriteBatch.Draw(pixel, fillRect, FillColor);

        spriteBatch.Draw(pixel, knobRect, KnobBorderColor);
        spriteBatch.Draw(pixel, knobInner, KnobFaceColor);

        int gripHeight = Math.Max(4, knobInner.Height - 6);
        int gripTop = knobInner.Y + ((knobInner.Height - gripHeight) / 2);
        for (int i = 0; i < 3; i++)
        {
            int gripX = knobInner.X + 4 + (i * 2);
            spriteBatch.Draw(pixel, new Rectangle(gripX, gripTop, 1, gripHeight), KnobGripColor);
        }
    }
}
