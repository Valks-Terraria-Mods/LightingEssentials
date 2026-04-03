using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.State.Runtime;

internal sealed class LightingSettingsPanelPopupManager
{
    private ColorPickerPopup _colorPickerPopup;
    private CatalogPickerPopup _catalogPickerPopup;
    private ImportSettingsPopup _importSettingsPopup;

    public void CloseAll()
    {
        CloseColorPicker();
        CloseCatalogPicker();
        CloseImportSettings();
    }

    public void OpenColorPicker(UIState host, LightingSettingsPanelRuntimeState state, ColorSettingDescriptor descriptor, LightingSettings settings, LightingSettings defaults, Action<LightingSettings> applySettingsChange)
    {
        CloseAll();

        Color defaultColor = descriptor.DefaultGetter is not null
            ? descriptor.DefaultGetter(defaults ?? settings)
            : (defaults is null ? descriptor.Getter(settings) : descriptor.Getter(defaults));

        _colorPickerPopup = new ColorPickerPopup(
            descriptor.Label,
            descriptor.Getter(settings),
            defaultColor,
            colorValue =>
            {
                descriptor.Setter(settings, colorValue);
                applySettingsChange(settings);
            },
            CloseColorPicker,
            state.UiScale);

        PositionPopupToPanelLeft(state, _colorPickerPopup, state.Scale(352f), state.Scale(230f));
        host.Append(_colorPickerPopup);
    }

    public void OpenCatalogPicker(
        UIState host,
        LightingSettingsPanelRuntimeState state,
        string title,
        IReadOnlyList<CatalogPickerOption> options,
        Action<IReadOnlyList<CatalogPickerOption>, string> onConfirm,
        IReadOnlyCollection<string> initiallySelectedKeys = null,
        string initialGroupName = "",
        string confirmButtonText = "Add Selected")
    {
        CloseAll();

        _catalogPickerPopup = new CatalogPickerPopup(
            title,
            options,
            onConfirm,
            CloseCatalogPicker,
            state.UiScale,
            initiallySelectedKeys,
            initialGroupName,
            confirmButtonText);

        PositionPopupToPanelLeft(state, _catalogPickerPopup, state.Scale(460f), state.Scale(560f));
        host.Append(_catalogPickerPopup);
    }

    public void OpenImportSettings(UIState host, LightingSettingsPanelRuntimeState state, Func<string, bool> onImportRequested)
    {
        CloseAll();

        _importSettingsPopup = new ImportSettingsPopup(onImportRequested, CloseImportSettings, state.UiScale);

        PositionPopupToPanelLeft(state, _importSettingsPopup, state.Scale(460f), state.Scale(224f));
        host.Append(_importSettingsPopup);
    }

    public void CloseColorPicker()
    {
        if (_colorPickerPopup is null)
            return;

        _colorPickerPopup.Remove();
        _colorPickerPopup = null;
    }

    public void CloseCatalogPicker()
    {
        if (_catalogPickerPopup is null)
            return;

        _catalogPickerPopup.EndSearchInput();
        _catalogPickerPopup.Remove();
        _catalogPickerPopup = null;
    }

    public void CloseImportSettings()
    {
        if (_importSettingsPopup is null)
            return;

        _importSettingsPopup.EndInput();
        _importSettingsPopup.Remove();
        _importSettingsPopup = null;
    }

    private static void PositionPopupToPanelLeft(LightingSettingsPanelRuntimeState state, UIElement popup, float popupWidth, float popupHeight)
    {
        if (state.RootPanel is null)
            return;

        CalculatedStyle rootBounds = state.RootPanel.GetDimensions();
        float popupGap = state.Scale(6f);
        float viewportInset = state.Scale(8f);

        float preferredLeft = rootBounds.X - popupWidth - popupGap;
        float maxLeft = Math.Max(viewportInset, Main.screenWidth - popupWidth - viewportInset);
        float popupLeft = MathHelper.Clamp(preferredLeft, viewportInset, maxLeft);

        float preferredTop = (rootBounds.Y + rootBounds.Height) - popupHeight;
        float maxTop = Math.Max(viewportInset, Main.screenHeight - popupHeight - viewportInset);
        float popupTop = MathHelper.Clamp(preferredTop, viewportInset, maxTop);

        popup.HAlign = 0f;
        popup.VAlign = 0f;
        popup.Left.Set(popupLeft, 0f);
        popup.Top.Set(popupTop, 0f);
    }
}
