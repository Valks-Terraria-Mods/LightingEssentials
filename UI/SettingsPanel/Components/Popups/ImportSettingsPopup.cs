using System;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Styling;
using ReLogic.OS;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Popups;

internal sealed class ImportSettingsPopup : UIPanel
{
    private readonly Func<string, bool> _onImportRequested;
    private readonly Action _onClose;
    private readonly float _uiScale;

    private readonly UISearchBar _importTextBar;
    private readonly UIText _statusText;
    private readonly FlatTextButton _importButton;

    private string _importText = string.Empty;

    public ImportSettingsPopup(Func<string, bool> onImportRequested, Action onClose, float uiScale)
    {
        _onImportRequested = onImportRequested;
        _onClose = onClose;
        _uiScale = uiScale;

        Width.Set(Scale(460f), 0f);
        Height.Set(Scale(224f), 0f);
        SetPadding(Scale(12f));

        BackgroundColor = SettingsPanelTheme.PanelBackground;
        BorderColor = Color.Transparent;

        UIText header = new("Import Settings", ScaleText(0.86f));
        Append(header);

        FlatTextButton closeButton = new("Close", ScaleText(0.72f))
        {
            HAlign = 1f,
            HoverStyleEnabled = false,
        };
        closeButton.Width.Set(Scale(54f), 0f);
        closeButton.Height.Set(Scale(22f), 0f);
        closeButton.OnLeftClick += (_, _) => RequestClose();
        Append(closeButton);

        UIText descriptionLineOne = new("Paste settings below.", ScaleText(0.62f));
        descriptionLineOne.Top.Set(Scale(30f), 0f);
        Append(descriptionLineOne);

        UIPanel textAreaPanel = new();
        textAreaPanel.Top.Set(Scale(50f), 0f);
        textAreaPanel.Width.Set(0f, 1f);
        textAreaPanel.Height.Set(Scale(112f), 0f);
        textAreaPanel.SetPadding(Scale(2f));
        textAreaPanel.BackgroundColor = new Color(14, 14, 14, 150);
        textAreaPanel.BorderColor = SettingsPanelTheme.RowBorder;
        Append(textAreaPanel);

        LocalizedText importPlaceholder = Language.GetOrRegister("Mods.LightingEssentials.UI.ImportSettingsPlaceholder", static () => "Paste encoded settings...");
        _importTextBar = new UISearchBar(importPlaceholder, ScaleText(0.72f));
        _importTextBar.Width.Set(0f, 1f);
        _importTextBar.Height.Set(Scale(24f), 0f);
        _importTextBar.OnContentsChanged += OnImportTextChanged;
        _importTextBar.OnLeftClick += (_, _) => FocusInput();
        textAreaPanel.Append(_importTextBar);

        FlatTextButton pasteClipboardButton = new("Paste Clipboard", ScaleText(0.70f))
        {
            HoverStyleEnabled = false,
        };
        pasteClipboardButton.Width.Set(Scale(112f), 0f);
        pasteClipboardButton.Height.Set(Scale(22f), 0f);
        pasteClipboardButton.Top.Set(-Scale(28f), 1f);
        pasteClipboardButton.OnLeftClick += (_, _) => PasteClipboardText();
        Append(pasteClipboardButton);

        _statusText = new UIText(string.Empty, ScaleText(0.64f));
        _statusText.Top.Set(-Scale(58f), 1f);
        _statusText.Left.Set(Scale(10f), 0f);
        _statusText.TextColor = new Color(190, 190, 190);
        Append(_statusText);

        _importButton = new FlatTextButton("Import", ScaleText(0.72f))
        {
            HAlign = 1f,
            HoverStyleEnabled = false,
        };
        _importButton.Width.Set(Scale(102f), 0f);
        _importButton.Height.Set(Scale(22f), 0f);
        _importButton.Top.Set(-Scale(28f), 1f);
        _importButton.OnLeftClick += (_, _) => RequestImport();
        Append(_importButton);

        _importTextBar.SetContents(string.Empty, forced: true);
        UpdateImportButtonState();
    }

    protected override void DrawSelf(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (ContainsPoint(Main.MouseScreen))
            Main.LocalPlayer.mouseInterface = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (!Main.mouseLeft || !Main.mouseLeftRelease)
            return;

        if (_importTextBar.IsWritingText && !_importTextBar.ContainsPoint(Main.MouseScreen))
            _importTextBar.ToggleTakingText();
    }

    public void EndInput()
    {
        if (_importTextBar.IsWritingText)
            _importTextBar.ToggleTakingText();
    }

    private void FocusInput()
    {
        if (!_importTextBar.IsWritingText)
            _importTextBar.ToggleTakingText();
    }

    private void PasteClipboardText()
    {
        string clipboardValue = Platform.Get<IClipboard>().Value ?? string.Empty;
        _importTextBar.SetContents(clipboardValue, forced: true);
        FocusInput();
        SetStatus("Clipboard text loaded.", SettingsPanelTheme.Positive);
    }

    private void OnImportTextChanged(string value)
    {
        _importText = value ?? string.Empty;
        SetStatus(string.Empty, new Color(190, 190, 190));
        UpdateImportButtonState();
    }

    private void UpdateImportButtonState()
    {
        bool hasText = !string.IsNullOrWhiteSpace(_importText);
        _importButton.BackgroundColor = hasText ? SettingsPanelTheme.Positive : SettingsPanelTheme.ButtonBackground;
    }

    private void RequestImport()
    {
        if (string.IsNullOrWhiteSpace(_importText))
        {
            SetStatus("Paste an encoded settings string first.", SettingsPanelTheme.Negative);
            return;
        }

        bool importSucceeded = _onImportRequested(_importText);
        if (!importSucceeded)
        {
            SetStatus("Import failed. Verify the encoded text and try again.", SettingsPanelTheme.Negative);
            return;
        }

        SetStatus("Import succeeded.", SettingsPanelTheme.Positive);
        RequestClose();
    }

    private void SetStatus(string text, Color color)
    {
        _statusText.SetText(text);
        _statusText.TextColor = color;
    }

    private void RequestClose()
    {
        EndInput();
        _onClose();
    }

    private float Scale(float baselinePixels)
    {
        return SettingsPanelScale.Pixels(baselinePixels, _uiScale);
    }

    private float ScaleText(float baselineTextScale)
    {
        return SettingsPanelScale.Text(baselineTextScale, _uiScale);
    }
}
