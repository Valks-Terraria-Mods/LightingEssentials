using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Catalog;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.State.Runtime;
using LightingEssentials.UI.SettingsPanel.Styling;
using LightingEssentials.UI.SettingsPanel.Systems;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameInput;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.State;

internal sealed class LightingSettingsPanelState : UIState
{
    private const int PersistDebounceFrames = 20;

    private readonly LightingSettingsPanelRuntimeState _runtime = new();
    private readonly LightingSettingsPanelLayoutBuilder _layoutBuilder = new();
    private readonly LightingSettingsPanelRowBuilder _rowBuilder = new();
    private readonly LightingSettingsPanelPopupManager _popupManager = new();
    private readonly LightingSettingsPanelEntryCatalogService _entryCatalogService = new();

    private LightingSettingsPanelEntryEditService _entryEditService;

    public override void OnInitialize()
    {
        _runtime.DefaultSettings = LightingSettingsDefaults.CreateDefaults();
        EnsureEntryEditService();
        BuildScaledLayout();
    }

    public override void OnActivate()
    {
        EnsureEntryEditService();
        BuildScaledLayout();
        base.OnActivate();
    }

    public override void OnDeactivate()
    {
        PersistPendingSettingsChanges();
        _popupManager.CloseAll();
        base.OnDeactivate();
    }

    public override void Update(GameTime gameTime)
    {
        float currentScale = SettingsPanelScale.Current;
        if (MathF.Abs(currentScale - _runtime.UiScale) > SettingsPanelScale.ScaleEpsilon)
            BuildScaledLayout();

        if (_runtime.PendingConfigPersist)
        {
            _runtime.PersistCountdownFrames--;
            if (_runtime.PersistCountdownFrames <= 0)
                PersistPendingSettingsChanges();
        }

        base.Update(gameTime);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (_runtime.RootPanel?.ContainsPoint(Main.MouseScreen) == true)
            Main.LocalPlayer.mouseInterface = true;

        if (_runtime.SettingsList is not null && _runtime.SettingsList.IsMouseHovering)
            PlayerInput.LockVanillaMouseScroll("LightingEssentials/SettingsPanel");
    }

    private void EnsureEntryEditService()
    {
        _entryEditService ??= new LightingSettingsPanelEntryEditService(_entryCatalogService, OpenCatalogPicker, ApplySettingsChange, () => BuildRowsForTab(_runtime.ActiveTab));
    }

    private void BuildScaledLayout()
    {
        _runtime.RefreshScaleMetrics();

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        _popupManager.CloseAll();
        _runtime.ClearTabButtons();
        RemoveAllChildren();

        LightingSettingsPanelLayoutCallbacks callbacks = new(
            ToggleMinimize,
            ClosePanelFromUi,
            SelectTab,
            OpenCatalogPickerForActiveTab);
        _layoutBuilder.Build(this, _runtime, callbacks);

        BuildRowsForTab(_runtime.ActiveTab);
        LightingSettingsPanelVisualState.RefreshTabButtonStyles(_runtime);
        LightingSettingsPanelVisualState.ApplyMinimizeState(_runtime, _popupManager);
    }

    private void BuildRowsForTab(LightingSettingsTab tab)
    {
        if (_runtime.SettingsList is null)
            return;

        _runtime.ActiveTab = tab;

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        _popupManager.CloseAll();
        EnsureEntryEditService();

        LightingSettingsPanelRowCallbacks callbacks = new(
            ApplySettingsChange,
            OpenColorPicker,
            _entryEditService.EditTileEntry,
            _entryEditService.RemoveTileEntry,
            _entryEditService.EditEventEntry,
            _entryEditService.RemoveEventEntry,
            _entryEditService.EditEntityEntry,
            _entryEditService.RemoveEntityEntry,
            _entryEditService.EditBossEntry,
            _entryEditService.RemoveBossEntry);
        LightingSettingsPanelConfigActionCallbacks configActionCallbacks = new(
            () => LightingSettingsPanelSettingsActions.ToggleModEnabled(ApplySettingsChange),
            () => LightingSettingsPanelSettingsActions.ResetAllSettings(ApplySettingsChange, () => BuildRowsForTab(_runtime.ActiveTab)),
            LightingSettingsPanelSettingsActions.CopyModifiedSettings,
            OpenImportSettingsPopup,
            LightingSettingsPanelSettingsActions.ExportModifiedSettings);
        _rowBuilder.BuildRowsForTab(_runtime, settings, callbacks, configActionCallbacks);
    }

    private void ApplySettingsChange(LightingSettings settings)
    {
        LightingSettingsPanelSettingsActions.ApplySettingsChange(settings, _runtime, PersistDebounceFrames);
    }

    private void PersistPendingSettingsChanges()
    {
        LightingSettingsPanelSettingsActions.PersistPendingSettingsChanges(_runtime);
    }

    private void SelectTab(LightingSettingsTab tab)
    {
        if (_runtime.ActiveTab == tab)
            return;

        BuildRowsForTab(tab);
        LightingSettingsPanelVisualState.RefreshTabButtonStyles(_runtime);
    }

    private void ToggleMinimize()
    {
        _runtime.IsMinimized = !_runtime.IsMinimized;
        LightingSettingsPanelVisualState.ApplyMinimizeState(_runtime, _popupManager);
    }

    private void OpenColorPicker(ColorSettingDescriptor descriptor, LightingSettings settings)
    {
        _popupManager.OpenColorPicker(this, _runtime, descriptor, settings, _runtime.DefaultSettings, ApplySettingsChange);
    }

    private void OpenImportSettingsPopup()
    {
        _popupManager.OpenImportSettings(this, _runtime, ImportSettingsFromPopupText);
    }

    private void OpenCatalogPickerForActiveTab()
    {
        if (!LightingSettingsCatalog.UsesDynamicEntries(_runtime.ActiveTab))
            return;

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        List<CatalogPickerOption> options = _entryCatalogService.BuildCatalogOptionsForTab(_runtime.ActiveTab, settings);
        OpenCatalogPicker($"Select {LightingSettingsCatalog.GetTabTitle(_runtime.ActiveTab)} Entry", options, OnCatalogSelectionConfirmed, null, string.Empty, "Add Selected");
    }

    private void OpenCatalogPicker(
        string title,
        IReadOnlyList<CatalogPickerOption> options,
        Action<IReadOnlyList<CatalogPickerOption>, string> onConfirm,
        IReadOnlyCollection<string> initiallySelectedKeys,
        string initialGroupName,
        string confirmButtonText)
    {
        _popupManager.OpenCatalogPicker(this, _runtime, title, options, onConfirm, initiallySelectedKeys, initialGroupName, confirmButtonText);
    }

    private void OnCatalogSelectionConfirmed(IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        LightingSettingsPanelSettingsActions.TryApplyCatalogSelection(_entryCatalogService, _runtime.ActiveTab, selectedOptions, groupName, ApplySettingsChange, () => BuildRowsForTab(_runtime.ActiveTab));
    }

    private bool ImportSettingsFromPopupText(string importText)
    {
        return LightingSettingsPanelSettingsActions.ImportModifiedSettings(importText, ApplySettingsChange, () => BuildRowsForTab(_runtime.ActiveTab));
    }

    private static void ClosePanelFromUi()
    {
        ModContent.GetInstance<LightingSettingsUISystem>().ClosePanelFromUi();
    }
}
