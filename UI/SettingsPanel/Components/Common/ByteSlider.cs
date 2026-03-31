using System;
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

    private bool _dragging;

    private const float KnobWidth = 12f;
    private const float KnobHeight = 14f;

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
    public ByteSlider(string labelText, byte initialValue)
    {
        Width.Set(0f, 1f);
        Height.Set(24f, 0f);

        _fillColor = ResolveFillColor(labelText);

        UIText label = new(labelText, 0.78f)
        {
            Top = StyleDimension.FromPixels(4f),
        };
        Append(label);

        _valueText = new UIText(string.Empty, 0.74f)
        {
            HAlign = 1f,
            Top = StyleDimension.FromPixels(4f),
        };
        Append(_valueText);

        _track = new UIElement();
        _track.Left.Set(24f, 0f);
        _track.Width.Set(-60f, 1f);
        _track.Top.Set(14f, 0f);
        _track.Height.Set(8f, 0f);
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

        float sliderMinX = dimensions.X + 1f;
        float sliderWidth = Math.Max(1f, dimensions.Width - 2f);
        float travelWidth = Math.Max(1f, sliderWidth - KnobWidth);

        float knobLeft = Main.MouseScreen.X - (KnobWidth * 0.5f);
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
        spriteBatch.Draw(pixel, fillRect, _fillColor);

        spriteBatch.Draw(pixel, knobRect, KnobBorderColor);
        spriteBatch.Draw(pixel, knobInner, KnobFaceColor);

        int gripHeight = Math.Max(4, knobInner.Height - 6);
        int gripTop = knobInner.Y + ((knobInner.Height - gripHeight) / 2);
        for (int i = 0; i < 3; i++)
        {
            int gripX = knobInner.X + 3 + (i * 2);
            spriteBatch.Draw(pixel, new Rectangle(gripX, gripTop, 1, gripHeight), KnobGripColor);
        }
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
