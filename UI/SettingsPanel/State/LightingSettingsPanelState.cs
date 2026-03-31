using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Catalog;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Components.Rows;
using LightingEssentials.UI.SettingsPanel.Components.Tabs;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.Styling;
using LightingEssentials.UI.SettingsPanel.Systems;
using Microsoft.Xna.Framework.Graphics;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.State;

internal sealed class LightingSettingsPanelState : UIState
{
    private readonly Dictionary<LightingSettingsTab, SettingsTabButton> _tabButtons = [];

    private LightingSettingsTab _activeTab = LightingSettingsTab.TileEffects;

    private UIPanel _rootPanel;
    private UIElement _contentContainer;
    private UIText _headerText;
    private FlatTextButton _minimizeButton;
    private FlatTextButton _closeButton;

    private UIList _settingsList;
    private ColorPickerPopup _colorPickerPopup;
    private LightingSettings _defaultSettings;

    private bool _isMinimized;
    private float _panelWidth;
    private float _expandedPanelHeight;
    private float _edgeMargin;

    /// <summary>
    /// Builds the full settings panel UI tree and wires interaction callbacks.
    /// </summary>
    public override void OnInitialize()
    {
        _panelWidth = 508f;
        _expandedPanelHeight = 504f;
        _edgeMargin = 24f;

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        _defaultSettings = LightingSettingsDefaults.CreateDefaults();

        // Main docking container anchored to bottom-right.
        _rootPanel = new UIPanel();
        _rootPanel.Width.Set(_panelWidth, 0f);
        _rootPanel.Height.Set(_expandedPanelHeight, 0f);
        _rootPanel.Left.Set(-(_panelWidth + _edgeMargin), 1f);
        _rootPanel.Top.Set(-(_expandedPanelHeight + _edgeMargin), 1f);
        _rootPanel.SetPadding(12f);
        _rootPanel.BackgroundColor = SettingsPanelTheme.PanelBackground;
        _rootPanel.BorderColor = SettingsPanelTheme.PanelBorder;
        Append(_rootPanel);

        _headerText = new UIText("Lighting Essentials", 0.92f)
        {
            HAlign = 0f,
        };
        _headerText.Top.Set(2f, 0f);
        _rootPanel.Append(_headerText);

        _minimizeButton = new FlatTextButton("-", 0.74f)
        {
            HAlign = 1f,
        };
        _minimizeButton.Width.Set(22f, 0f);
        _minimizeButton.Height.Set(22f, 0f);
        _minimizeButton.Left.Set(-30f, 0f);
        _minimizeButton.OnLeftClick += (_, _) => ToggleMinimize();
        _rootPanel.Append(_minimizeButton);

        _closeButton = new FlatTextButton("X", 0.74f)
        {
            HAlign = 1f,
        };
        _closeButton.Width.Set(22f, 0f);
        _closeButton.Height.Set(22f, 0f);
        _closeButton.OnLeftClick += (_, _) => ModContent.GetInstance<LightingSettingsUISystem>().ClosePanelFromUi();
        _rootPanel.Append(_closeButton);

        // Content container is detachable so minimize mode can collapse to titlebar only.
        _contentContainer = new UIElement();
        _contentContainer.Width.Set(0f, 1f);
        _contentContainer.Height.Set(-30f, 1f);
        _contentContainer.Top.Set(30f, 0f);
        _rootPanel.Append(_contentContainer);

        UIElement topControls = new();
        topControls.Width.Set(0f, 1f);
        topControls.Height.Set(26f, 0f);
        topControls.Top.Set(0f, 0f);
        _contentContainer.Append(topControls);

        UIText modEnabledText = new("Mod Enabled", 0.78f)
        {
            VAlign = 0.5f,
        };
        topControls.Append(modEnabledText);

        FlatTextButton modEnabledButton = new(string.Empty, 0.72f)
        {
            VAlign = 0.5f,
        };
        modEnabledButton.Left.Set(92f, 0f);
        modEnabledButton.Width.Set(58f, 0f);
        modEnabledButton.Height.Set(22f, 0f);
        modEnabledButton.OnLeftClick += (_, _) =>
        {
            settings.ModEnabled = !settings.ModEnabled;
            settings.ApplyRuntimeChanges();
            RefreshModEnabledButton();
        };
        topControls.Append(modEnabledButton);

        FlatTextButton resetAllButton = new("Reset to Defaults", 0.72f)
        {
            VAlign = 0.5f,
        };
        resetAllButton.Left.Set(156f, 0f);
        resetAllButton.Width.Set(122f, 0f);
        resetAllButton.Height.Set(22f, 0f);
        resetAllButton.OnLeftClick += (_, _) =>
        {
            LightingSettingsDefaults.ApplyDefaults(settings);
            settings.ApplyRuntimeChanges();
            BuildRowsForTab(_activeTab);
            RefreshModEnabledButton();
        };
        topControls.Append(resetAllButton);

        RefreshModEnabledButton();

        void RefreshModEnabledButton()
        {
            bool isEnabled = settings.ModEnabled;
            modEnabledButton.SetText(isEnabled ? "ON" : "OFF");
            modEnabledButton.BackgroundColor = isEnabled ? SettingsPanelTheme.Positive : SettingsPanelTheme.Negative;
        }

        UIElement tabBar = new();
        tabBar.Width.Set(0f, 1f);
        tabBar.Height.Set(32f, 0f);
        tabBar.Top.Set(34f, 0f);
        _contentContainer.Append(tabBar);

        AddTabButton(tabBar, LightingSettingsTab.TileEffects, 0);
        AddTabButton(tabBar, LightingSettingsTab.Events, 1);
        AddTabButton(tabBar, LightingSettingsTab.EntityLights, 2);
        AddTabButton(tabBar, LightingSettingsTab.BossEffects, 3);

        UIPanel scrollPanel = new();
        scrollPanel.Width.Set(0f, 1f);
        scrollPanel.Height.Set(-76f, 1f);
        scrollPanel.Top.Set(72f, 0f);
        scrollPanel.SetPadding(8f);
        scrollPanel.BackgroundColor = new Color(14, 14, 14, 150);
        scrollPanel.BorderColor = SettingsPanelTheme.RowBorder;
        _contentContainer.Append(scrollPanel);

        _settingsList = new UIList
        {
            ListPadding = 8f,
        };
        // UIList can reorder elements unless a manual sort delegate is supplied.
        _settingsList.ManualSortMethod = static _ => { };
        _settingsList.Width.Set(-26f, 1f);
        _settingsList.Height.Set(0f, 1f);
        scrollPanel.Append(_settingsList);

        UIScrollbar scrollbar = new DarkScrollbar();
        scrollbar.HAlign = 1f;
        scrollbar.Height.Set(0f, 1f);
        scrollPanel.Append(scrollbar);
        _settingsList.SetScrollbar(scrollbar);

        BuildRowsForTab(_activeTab);
        RefreshTabButtonStyles();
    }

    /// <summary>
    /// Adds a tab button to the strip and wires tab-switch behavior.
    /// </summary>
    /// <param name="tabBar">Parent tab strip container.</param>
    /// <param name="tab">Associated logical tab.</param>
    /// <param name="index">Zero-based tab index in display order.</param>
    private void AddTabButton(UIElement tabBar, LightingSettingsTab tab, int index)
    {
        SettingsTabButton button = new(LightingSettingsCatalog.GetTabTitle(tab));
        button.Width.Set(0f, 0.25f);
        button.Left.Set(0f, index * 0.25f);
        button.OnLeftClick += (_, _) =>
        {
            if (_activeTab == tab)
                return;

            _activeTab = tab;
            BuildRowsForTab(tab);
            RefreshTabButtonStyles();
        };

        _tabButtons[tab] = button;
        tabBar.Append(button);
    }

    /// <summary>
    /// Collapses or expands the panel while preserving bottom-right docking.
    /// </summary>
    private void ToggleMinimize()
    {
        _isMinimized = !_isMinimized;
        _minimizeButton?.SetText(_isMinimized ? "+" : "-");

        if (_isMinimized)
        {
            if (_headerText is not null)
            {
                _headerText.VAlign = 0.5f;
                _headerText.Top.Set(0f, 0f);
            }

            if (_minimizeButton is not null)
            {
                _minimizeButton.VAlign = 0.5f;
                _minimizeButton.Top.Set(0f, 0f);
            }

            if (_closeButton is not null)
            {
                _closeButton.VAlign = 0.5f;
                _closeButton.Top.Set(0f, 0f);
            }
        }
        else
        {
            if (_headerText is not null)
            {
                _headerText.VAlign = 0f;
                _headerText.Top.Set(2f, 0f);
            }

            if (_minimizeButton is not null)
            {
                _minimizeButton.VAlign = 0f;
                _minimizeButton.Top.Set(0f, 0f);
            }

            if (_closeButton is not null)
            {
                _closeButton.VAlign = 0f;
                _closeButton.Top.Set(0f, 0f);
            }
        }

        if (_contentContainer is not null)
        {
            if (_isMinimized)
            {
                _contentContainer.Remove();
            }
            else if (_contentContainer.Parent is null)
            {
                _rootPanel?.Append(_contentContainer);
            }
        }

        float targetHeight = _isMinimized ? 36f : _expandedPanelHeight;
        if (_rootPanel is not null)
        {
            _rootPanel.Height.Set(targetHeight, 0f);
            _rootPanel.Left.Set(-(_panelWidth + _edgeMargin), 1f);
            _rootPanel.Top.Set(-(targetHeight + _edgeMargin), 1f);
        }

        if (_isMinimized)
            CloseColorPicker();
    }

    /// <summary>
    /// Rebuilds the scroll list for the selected tab.
    /// </summary>
    /// <param name="tab">Tab to render.</param>
    private void BuildRowsForTab(LightingSettingsTab tab)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        CloseColorPicker();

        _settingsList.Clear();
        _settingsList.ListPadding = tab == LightingSettingsTab.BossEffects ? 2f : 8f;

        IReadOnlyList<LightingSettingDescriptor> descriptors = LightingSettingsCatalog.GetTabDescriptors(tab);
        for (int i = 0; i < descriptors.Count; i++)
        {
            LightingSettingDescriptor descriptor = descriptors[i];
            UIElement row = CreateRow(descriptor, settings);
            _settingsList.Add(row);
        }

        _settingsList.Recalculate();
    }

    /// <summary>
    /// Creates a concrete row UI element from a descriptor definition.
    /// </summary>
    /// <param name="descriptor">Descriptor to materialize.</param>
    /// <param name="settings">Mutable settings source object.</param>
    /// <returns>Configured row UI element.</returns>
    private UIElement CreateRow(LightingSettingDescriptor descriptor, LightingSettings settings)
    {
        void OnChanged()
        {
            settings.ApplyRuntimeChanges();
        }

        // Color rows open a shared popup editor to keep rows compact.
        void OpenColorPicker(ColorSettingDescriptor colorDescriptor)
        {
            CloseColorPicker();

            _colorPickerPopup = new ColorPickerPopup(
                colorDescriptor.Label,
                colorDescriptor.Getter(settings),
                _defaultSettings is null ? colorDescriptor.Getter(settings) : colorDescriptor.Getter(_defaultSettings),
                colorValue =>
                {
                    colorDescriptor.Setter(settings, colorValue);
                    settings.ApplyRuntimeChanges();
                },
                CloseColorPicker);

            CalculatedStyle rootBounds = _rootPanel.GetDimensions();
            const float popupWidth = 352f;
            const float popupHeight = 220f;
            const float popupGap = 6f;

            _colorPickerPopup.HAlign = 0f;
            _colorPickerPopup.VAlign = 0f;
            _colorPickerPopup.Left.Set(rootBounds.X - popupWidth - popupGap, 0f);
            _colorPickerPopup.Top.Set((rootBounds.Y + rootBounds.Height) - popupHeight - 12f, 0f);

            Append(_colorPickerPopup);
        }

        return descriptor switch
        {
            BoolSettingDescriptor boolDescriptor => new BoolSettingRow(boolDescriptor, settings, OnChanged),
            FloatSettingDescriptor floatDescriptor => new FloatSettingRow(floatDescriptor, settings, OnChanged),
            ColorSettingDescriptor colorDescriptor => new ColorSettingRow(colorDescriptor, settings, OpenColorPicker),
            _ => new UIText("Unsupported setting"),
        };
    }

    /// <summary>
    /// Closes and disposes the currently-open color picker popup if one exists.
    /// </summary>
    private void CloseColorPicker()
    {
        if (_colorPickerPopup is null)
            return;

        _colorPickerPopup.Remove();
        _colorPickerPopup = null;
    }

    /// <summary>
    /// Updates tab button visual states to reflect the currently selected tab.
    /// </summary>
    private void RefreshTabButtonStyles()
    {
        foreach ((LightingSettingsTab tab, SettingsTabButton button) in _tabButtons)
        {
            button.SetActive(tab == _activeTab);
        }
    }

    /// <summary>
    /// Handles panel-level mouse-interaction flags during draw.
    /// </summary>
    /// <param name="spriteBatch">Current sprite batch.</param>
    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (_rootPanel?.ContainsPoint(Main.MouseScreen) == true)
            Main.LocalPlayer.mouseInterface = true;

        if (_settingsList is not null && _settingsList.IsMouseHovering)
            PlayerInput.LockVanillaMouseScroll("LightingEssentials/SettingsPanel");
    }
}
