using System;
using LightingEssentials.UI.SettingsPanel.Styling;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Common;

internal sealed class ByteSlider : UIElement
{
    private readonly UIElement _track;
    private readonly UIText _valueText;
    private readonly Color _fillColor;
    private readonly float _uiScale;

    private bool _dragging;

    private const float KnobWidthBase = 12f;
    private const float KnobHeightBase = 14f;

    private static readonly Color TrackBorderColor = new(70, 86, 112, 255);
    private static readonly Color TrackBackgroundColor = new(18, 22, 30, 255);
    private static readonly Color KnobFaceColor = new(24, 32, 44, 255);
    private static readonly Color KnobBorderColor = new(146, 190, 255, 255);
    private static readonly Color KnobGripColor = new(186, 216, 255, 255);

    /// <summary>
    /// Fired when the slider value changes due to user input.
    /// </summary>
    public event Action<byte> ValueChanged;

    /// <summary>
    /// Current channel value represented by this slider.
    /// </summary>
    public byte Value { get; private set; }

    /// <summary>
    /// Creates a byte slider row used by the RGBA popup.
    /// </summary>
    /// <param name="labelText">Channel label (R/G/B/A).</param>
    /// <param name="initialValue">Initial channel value.</param>
    /// <param name="uiScale">Current popup UI scale factor.</param>
    public ByteSlider(string labelText, byte initialValue, float uiScale)
    {
        _uiScale = uiScale;

        float sliderHeight = Scale(24f);
        float labelScale = ScaleText(0.78f);
        float valueScale = ScaleText(0.74f);
        float textTop = Scale(4f);
        float trackLeft = Scale(24f);
        float trackRightInset = Scale(60f);
        float trackTop = Scale(14f);
        float trackHeight = Scale(8f);

        Width.Set(0f, 1f);
        Height.Set(sliderHeight, 0f);

        _fillColor = ResolveFillColor(labelText);

        UIText label = new(labelText, labelScale)
        {
            Top = StyleDimension.FromPixels(textTop),
        };
        Append(label);

        _valueText = new UIText(string.Empty, valueScale)
        {
            HAlign = 1f,
            Top = StyleDimension.FromPixels(textTop),
        };
        Append(_valueText);

        _track = new UIElement();
        _track.Left.Set(trackLeft, 0f);
        _track.Width.Set(-trackRightInset, 1f);
        _track.Top.Set(trackTop, 0f);
        _track.Height.Set(trackHeight, 0f);
        _track.OnLeftMouseDown += OnTrackMouseDown;
        _track.OnLeftClick += OnTrackMouseDown;
        _track.OnLeftMouseUp += OnTrackMouseUp;
        Append(_track);

        SetValue(initialValue, notify: false);
    }

    /// <summary>
    /// Draws a flat modern slider with constrained knob travel.
    /// </summary>
    /// <param name="spriteBatch">Current sprite batch.</param>
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        DrawSlider(spriteBatch);
    }

    /// <summary>
    /// Continues drag updates while the left mouse button remains held.
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
    /// Sets slider value and optionally emits a value-changed callback.
    /// </summary>
    /// <param name="value">New channel value.</param>
    /// <param name="notify">Whether <see cref="ValueChanged"/> should be raised.</param>
    public void SetValue(byte value, bool notify)
    {
        if (Value == value)
        {
            UpdateVisuals();
            return;
        }

        Value = value;
        UpdateVisuals();

        if (notify)
            ValueChanged?.Invoke(Value);
    }

    /// <summary>
    /// Starts dragging from the track and snaps value to cursor position.
    /// </summary>
    private void OnTrackMouseDown(UIMouseEvent evt, UIElement listeningElement)
    {
        _dragging = true;
        UpdateValueFromMouse();
    }

    /// <summary>
    /// Ends active slider drag.
    /// </summary>
    private void OnTrackMouseUp(UIMouseEvent evt, UIElement listeningElement)
    {
        _dragging = false;
    }

    /// <summary>
    /// Converts cursor X position to a clamped 0-255 byte.
    /// </summary>
    private void UpdateValueFromMouse()
    {
        CalculatedStyle dimensions = _track.GetDimensions();
        if (dimensions.Width <= 0f)
            return;

        float knobWidth = Scale(KnobWidthBase);

        float sliderMinX = dimensions.X + 1f;
        float sliderWidth = Math.Max(1f, dimensions.Width - 2f);
        float travelWidth = Math.Max(1f, sliderWidth - knobWidth);

        float knobLeft = Main.MouseScreen.X - (knobWidth * 0.5f);
        float t = (knobLeft - sliderMinX) / travelWidth;
        t = MathHelper.Clamp(t, 0f, 1f);

        int asInt = (int)MathF.Round(t * 255f);
        SetValue((byte)Math.Clamp(asInt, 0, 255), notify: true);
    }

    /// <summary>
    /// Updates track fill, knob position, and displayed numeric text.
    /// </summary>
    private void UpdateVisuals()
    {
        _valueText.SetText(Value.ToString());
    }

    /// <summary>
    /// Draws the byte slider track, fill, and knob with exact edge clamping.
    /// </summary>
    /// <param name="spriteBatch">Current sprite batch.</param>
    private void DrawSlider(SpriteBatch spriteBatch)
    {
        CalculatedStyle dimensions = _track.GetDimensions();
        if (dimensions.Width <= 2f || dimensions.Height <= 2f)
            return;

        Rectangle outer = new((int)dimensions.X, (int)dimensions.Y, (int)dimensions.Width, (int)dimensions.Height);
        Rectangle inner = new(outer.X + 1, outer.Y + 1, Math.Max(1, outer.Width - 2), Math.Max(1, outer.Height - 2));

        float t = Value / 255f;
        int knobPixelWidth = Math.Max(1, (int)MathF.Round(Scale(KnobWidthBase)));
        int knobPixelHeight = Math.Max(1, (int)MathF.Round(Scale(KnobHeightBase)));
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
        spriteBatch.Draw(pixel, fillRect, _fillColor);

        spriteBatch.Draw(pixel, knobRect, KnobBorderColor);
        spriteBatch.Draw(pixel, knobInner, KnobFaceColor);

        int gripHeight = Math.Max(Math.Max(2, (int)MathF.Round(Scale(4f))), knobInner.Height - Math.Max(2, (int)MathF.Round(Scale(6f))));
        int gripTop = knobInner.Y + ((knobInner.Height - gripHeight) / 2);
        int gripInset = Math.Max(1, (int)MathF.Round(Scale(3f)));
        int gripSpacing = Math.Max(1, (int)MathF.Round(Scale(2f)));
        for (int i = 0; i < 3; i++)
        {
            int gripX = knobInner.X + gripInset + (i * gripSpacing);
            spriteBatch.Draw(pixel, new Rectangle(gripX, gripTop, 1, gripHeight), KnobGripColor);
        }
    }

    /// <summary>
    /// Converts baseline pixel constants to the slider's active UI scale.
    /// </summary>
    private float Scale(float baselinePixels)
    {
        return SettingsPanelScale.Pixels(baselinePixels, _uiScale);
    }

    /// <summary>
    /// Converts baseline text scales to the slider's active UI scale.
    /// </summary>
    private float ScaleText(float baselineTextScale)
    {
        return SettingsPanelScale.Text(baselineTextScale, _uiScale);
    }

    /// <summary>
    /// Chooses a channel-specific fill color for better visual readability.
    /// </summary>
    /// <param name="labelText">Channel label text.</param>
    /// <returns>Fill color used by the slider.</returns>
    private static Color ResolveFillColor(string labelText)
    {
        return labelText switch
        {
            "R" => new Color(220, 84, 90, 255),
            "G" => new Color(88, 208, 118, 255),
            "B" => new Color(98, 148, 255, 255),
            "A" => new Color(186, 204, 236, 255),
            _ => new Color(120, 176, 255, 255),
        };
    }
}
