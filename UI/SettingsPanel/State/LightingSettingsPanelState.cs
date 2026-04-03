using System;
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
    private float _uiScale;
    private float _panelWidth;
    private float _expandedPanelHeight;
    private float _edgeMargin;

    /// <summary>
    /// Builds the full settings panel UI tree and wires interaction callbacks.
    /// </summary>
    public override void OnInitialize()
    {
        _defaultSettings = LightingSettingsDefaults.CreateDefaults();
        BuildScaledLayout();
    }

    /// <summary>
    /// Rebuilds panel layout when the resolution scale changes while the panel is open.
    /// </summary>
    /// <param name="gameTime">Current frame timing data.</param>
    public override void Update(GameTime gameTime)
    {
        float currentScale = SettingsPanelScale.Current;
        if (MathF.Abs(currentScale - _uiScale) > SettingsPanelScale.ScaleEpsilon)
            BuildScaledLayout();

        base.Update(gameTime);
    }

    /// <summary>
    /// Builds the panel with dimensions derived from current screen size.
    /// </summary>
    private void BuildScaledLayout()
    {
        _uiScale = SettingsPanelScale.Current;
        _panelWidth = Scale(508f);
        _expandedPanelHeight = Scale(504f);
        _edgeMargin = Scale(24f);

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();

        CloseColorPicker();
        _tabButtons.Clear();
        RemoveAllChildren();

        // Main docking container anchored to bottom-right.
        _rootPanel = new UIPanel();
        _rootPanel.Width.Set(_panelWidth, 0f);
        _rootPanel.Height.Set(_expandedPanelHeight, 0f);
        _rootPanel.Left.Set(-(_panelWidth + _edgeMargin), 1f);
        _rootPanel.Top.Set(-(_expandedPanelHeight + _edgeMargin), 1f);
        _rootPanel.SetPadding(Scale(12f));
        _rootPanel.BackgroundColor = SettingsPanelTheme.PanelBackground;
        _rootPanel.BorderColor = SettingsPanelTheme.PanelBorder;
        Append(_rootPanel);

        _headerText = new UIText("Lighting Essentials", ScaleText(0.92f))
        {
            HAlign = 0f,
        };
        _headerText.Top.Set(Scale(2f), 0f);
        _rootPanel.Append(_headerText);

        _minimizeButton = new FlatTextButton("-", ScaleText(0.74f))
        {
            HAlign = 1f,
        };
        _minimizeButton.Width.Set(Scale(22f), 0f);
        _minimizeButton.Height.Set(Scale(22f), 0f);
        _minimizeButton.Left.Set(-Scale(35f), 0f);
        _minimizeButton.OnLeftClick += (_, _) => ToggleMinimize();
        _rootPanel.Append(_minimizeButton);

        _closeButton = new FlatTextButton("X", ScaleText(0.74f))
        {
            HAlign = 1f,
        };
        _closeButton.Width.Set(Scale(22f), 0f);
        _closeButton.Height.Set(Scale(22f), 0f);
        _closeButton.OnLeftClick += (_, _) => ModContent.GetInstance<LightingSettingsUISystem>().ClosePanelFromUi();
        _rootPanel.Append(_closeButton);

        // Content container is detachable so minimize mode can collapse to titlebar only.
        _contentContainer = new UIElement();
        _contentContainer.Width.Set(0f, 1f);
        _contentContainer.Height.Set(-Scale(30f), 1f);
        _contentContainer.Top.Set(Scale(30f), 0f);
        _rootPanel.Append(_contentContainer);

        UIElement topControls = new();
        topControls.Width.Set(0f, 1f);
        topControls.Height.Set(Scale(26f), 0f);
        topControls.Top.Set(0f, 0f);
        _contentContainer.Append(topControls);

        UIText modEnabledText = new("Mod Enabled", ScaleText(0.78f))
        {
            VAlign = 0.5f,
        };
        topControls.Append(modEnabledText);

        FlatTextButton modEnabledButton = new(string.Empty, ScaleText(0.72f))
        {
            VAlign = 0.5f,
        };
        modEnabledButton.Left.Set(Scale(92f), 0f);
        modEnabledButton.Width.Set(Scale(58f), 0f);
        modEnabledButton.Height.Set(Scale(22f), 0f);
        modEnabledButton.OnLeftClick += (_, _) =>
        {
            settings.ModEnabled = !settings.ModEnabled;
            settings.ApplyRuntimeChanges();
            RefreshModEnabledButton();
        };
        topControls.Append(modEnabledButton);

        FlatTextButton resetAllButton = new("Reset to Defaults", ScaleText(0.72f))
        {
            VAlign = 0.5f,
        };
        resetAllButton.Left.Set(Scale(156f), 0f);
        resetAllButton.Width.Set(Scale(122f), 0f);
        resetAllButton.Height.Set(Scale(22f), 0f);
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
        tabBar.Height.Set(Scale(32f), 0f);
        tabBar.Top.Set(Scale(34f), 0f);
        _contentContainer.Append(tabBar);

        LightingSettingsTab[] tabs =
        [
            LightingSettingsTab.TileEffects,
            LightingSettingsTab.Events,
            LightingSettingsTab.EntityLights,
            LightingSettingsTab.BossEffects,
            LightingSettingsTab.Config
        ];
        
        int numTabs = tabs.Length;
        float tabWidth = 1f / numTabs;

        for (int i = 0; i < numTabs; i++)
        {
            AddTabButton(tabBar, tabs[i], i, tabWidth);
        }

        UIPanel scrollPanel = new();
        scrollPanel.Width.Set(0f, 1f);
        scrollPanel.Height.Set(-Scale(76f), 1f);
        scrollPanel.Top.Set(Scale(80f), 0f);
        scrollPanel.SetPadding(Scale(8f));
        scrollPanel.BackgroundColor = new Color(14, 14, 14, 150);
        scrollPanel.BorderColor = SettingsPanelTheme.RowBorder;
        _contentContainer.Append(scrollPanel);

        float scaledScrollbarWidth = Scale(20f);
        float scaledScrollbarGap = Scale(6f);

        _settingsList = new UIList
        {
            ListPadding = Scale(8f),
            // UIList can reorder elements unless a manual sort delegate is supplied.
            ManualSortMethod = static _ => { }
        };
        _settingsList.Width.Set(-(scaledScrollbarWidth + scaledScrollbarGap), 1f);
        _settingsList.Height.Set(0f, 1f);
        scrollPanel.Append(_settingsList);

        UIScrollbar scrollbar = new DarkScrollbar
        {
            HAlign = 1f
        };
        scrollbar.Width.Set(scaledScrollbarWidth, 0f);
        scrollbar.Height.Set(0f, 1f);
        scrollPanel.Append(scrollbar);
        _settingsList.SetScrollbar(scrollbar);

        BuildRowsForTab(_activeTab);
        RefreshTabButtonStyles();
        ApplyMinimizeState();
    }

    /// <summary>
    /// Converts baseline pixel constants to the active UI scale.
    /// </summary>
    private float Scale(float baselinePixels)
    {
        return SettingsPanelScale.Pixels(baselinePixels, _uiScale);
    }

    /// <summary>
    /// Converts baseline text scales to the active UI scale.
    /// </summary>
    private float ScaleText(float baselineTextScale)
    {
        return SettingsPanelScale.Text(baselineTextScale, _uiScale);
    }

    /// <summary>
    /// Adds a tab button to the strip and wires tab-switch behavior.
    /// </summary>
    /// <param name="tabBar">Parent tab strip container.</param>
    /// <param name="tab">Associated logical tab.</param>
    /// <param name="index">Zero-based tab index in display order.</param>
    private void AddTabButton(UIElement tabBar, LightingSettingsTab tab, int index, float tabWidth)
    {
        SettingsTabButton button = new(LightingSettingsCatalog.GetTabTitle(tab), _uiScale);
        button.Width.Set(0f, tabWidth);
        button.Left.Set(0f, index * tabWidth);
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
        ApplyMinimizeState();
    }

    /// <summary>
    /// Applies minimized or expanded visual state using scaled dimensions.
    /// </summary>
    private void ApplyMinimizeState()
    {
        _minimizeButton?.SetText(_isMinimized ? "+" : "-");

        if (_headerText is not null)
        {
            _headerText.VAlign = _isMinimized ? 0.5f : 0f;
            _headerText.Top.Set(_isMinimized ? 0f : Scale(2f), 0f);
        }

        if (_minimizeButton is not null)
        {
            _minimizeButton.VAlign = _isMinimized ? 0.5f : 0f;
            _minimizeButton.Top.Set(0f, 0f);
        }

        if (_closeButton is not null)
        {
            _closeButton.VAlign = _isMinimized ? 0.5f : 0f;
            _closeButton.Top.Set(0f, 0f);
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

        float targetHeight = _isMinimized ? Scale(36f) : _expandedPanelHeight;
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
        if (_settingsList is null)
            return;

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        CloseColorPicker();

        _settingsList.Clear();
        _settingsList.ListPadding = tab == LightingSettingsTab.BossEffects ? Scale(2f) : Scale(8f);

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
                CloseColorPicker,
                _uiScale);

            CalculatedStyle rootBounds = _rootPanel.GetDimensions();
            float popupWidth = Scale(352f);
            float popupHeight = Scale(220f);
            float popupGap = Scale(6f);
            float viewportInset = Scale(8f);

            float preferredLeft = rootBounds.X - popupWidth - popupGap;
            float maxLeft = Math.Max(viewportInset, Main.screenWidth - popupWidth - viewportInset);
            float popupLeft = MathHelper.Clamp(preferredLeft, viewportInset, maxLeft);

            float preferredTop = (rootBounds.Y + rootBounds.Height) - popupHeight - Scale(12f);
            float maxTop = Math.Max(viewportInset, Main.screenHeight - popupHeight - viewportInset);
            float popupTop = MathHelper.Clamp(preferredTop, viewportInset, maxTop);

            _colorPickerPopup.HAlign = 0f;
            _colorPickerPopup.VAlign = 0f;
            _colorPickerPopup.Left.Set(popupLeft, 0f);
            _colorPickerPopup.Top.Set(popupTop, 0f);

            Append(_colorPickerPopup);
        }

        return descriptor switch
        {
            BoolSettingDescriptor boolDescriptor => new BoolSettingRow(boolDescriptor, settings, OnChanged, _uiScale),
            FloatSettingDescriptor floatDescriptor => new FloatSettingRow(floatDescriptor, settings, OnChanged, _uiScale),
            ColorSettingDescriptor colorDescriptor => new ColorSettingRow(colorDescriptor, settings, OpenColorPicker, _uiScale),
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
