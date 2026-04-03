using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using LightingEssentials.UI.SettingsPanel.Catalog;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Components.Popups;
using LightingEssentials.UI.SettingsPanel.Components.Rows;
using LightingEssentials.UI.SettingsPanel.Components.Tabs;
using LightingEssentials.UI.SettingsPanel.Models;
using LightingEssentials.UI.SettingsPanel.Styling;
using LightingEssentials.UI.SettingsPanel.Systems;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.OS;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.State;

internal sealed class LightingSettingsPanelState : UIState
{
    private static readonly FieldInfo[] CopyableSettingsFields = typeof(LightingSettings).GetFields(BindingFlags.Instance | BindingFlags.Public);
    private static readonly HashSet<string> NonScalarCopiedFields = ["TileEffectEntries", "EventEffectEntries", "BossEffectEntries"];
    private const int PersistDebounceFrames = 20;

    private readonly Dictionary<LightingSettingsTab, SettingsTabButton> _tabButtons = [];

    private LightingSettingsTab _activeTab = LightingSettingsTab.TileEffects;

    private UIPanel _rootPanel;
    private UIElement _contentContainer;
    private UIText _headerText;
    private FlatTextButton _minimizeButton;
    private FlatTextButton _closeButton;

    private UIList _settingsList;
    private UIPanel _settingsScrollPanel;
    private ColorPickerPopup _colorPickerPopup;
    private CatalogPickerPopup _catalogPickerPopup;
    private FlatTextButton _addEntryButton;
    private LightingSettings _defaultSettings;

    private bool _isMinimized;
    private bool _pendingConfigPersist;
    private float _uiScale;
    private float _panelWidth;
    private float _expandedPanelHeight;
    private float _edgeMargin;
    private int _persistCountdownFrames;

    /// <summary>
    /// Builds the full settings panel UI tree and wires interaction callbacks.
    /// </summary>
    public override void OnInitialize()
    {
        _defaultSettings = LightingSettingsDefaults.CreateDefaults();
        BuildScaledLayout();
    }

    /// <summary>
    /// Re-syncs UI content whenever this state is shown to reflect persisted config changes.
    /// </summary>
    public override void OnActivate()
    {
        BuildScaledLayout();
        base.OnActivate();
    }

    /// <summary>
    /// Flushes pending config writes whenever this panel is hidden or swapped out.
    /// </summary>
    public override void OnDeactivate()
    {
        PersistPendingSettingsChanges();
        base.OnDeactivate();
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

        if (_pendingConfigPersist)
        {
            _persistCountdownFrames--;
            if (_persistCountdownFrames <= 0)
                PersistPendingSettingsChanges();
        }

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
        settings.EnsureDynamicEntries();

        CloseColorPicker();
        CloseCatalogPicker();
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
            LightingSettings currentSettings = ModContent.GetInstance<LightingSettings>();
            currentSettings.ModEnabled = !currentSettings.ModEnabled;
            ApplySettingsChange(currentSettings);
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
            LightingSettings currentSettings = ModContent.GetInstance<LightingSettings>();
            LightingSettingsDefaults.ApplyDefaults(currentSettings);
            ApplySettingsChange(currentSettings);
            BuildRowsForTab(_activeTab);
            RefreshModEnabledButton();
        };
        topControls.Append(resetAllButton);

        FlatTextButton copyModifiedButton = new("Copy Modified", ScaleText(0.72f))
        {
            VAlign = 0.5f,
        };
        copyModifiedButton.Left.Set(Scale(284f), 0f);
        copyModifiedButton.Width.Set(Scale(98f), 0f);
        copyModifiedButton.Height.Set(Scale(22f), 0f);
        copyModifiedButton.OnLeftClick += (_, _) =>
        {
            LightingSettings currentSettings = ModContent.GetInstance<LightingSettings>();
            string clipboardText = BuildModifiedSettingsClipboardText(currentSettings);
            if (string.IsNullOrWhiteSpace(clipboardText))
            {
                Main.NewText("No settings were modified, nothing was copied to clipboard.");
                return;
            }

            Platform.Get<IClipboard>().Value = clipboardText;
            Main.NewText("Copied modified settings to clipboard");
        };
        topControls.Append(copyModifiedButton);

        RefreshModEnabledButton();

        void RefreshModEnabledButton()
        {
            LightingSettings currentSettings = ModContent.GetInstance<LightingSettings>();
            bool isEnabled = currentSettings.ModEnabled;
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

        _settingsScrollPanel = new UIPanel();
        _settingsScrollPanel.Width.Set(0f, 1f);
        _settingsScrollPanel.Height.Set(-Scale(76f), 1f);
        _settingsScrollPanel.Top.Set(Scale(80f), 0f);
        _settingsScrollPanel.SetPadding(Scale(8f));
        _settingsScrollPanel.BackgroundColor = new Color(14, 14, 14, 150);
        _settingsScrollPanel.BorderColor = SettingsPanelTheme.RowBorder;
        _contentContainer.Append(_settingsScrollPanel);

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
        _settingsScrollPanel.Append(_settingsList);

        UIScrollbar scrollbar = new DarkScrollbar
        {
            HAlign = 1f
        };
        scrollbar.Width.Set(scaledScrollbarWidth, 0f);
        scrollbar.Height.Set(0f, 1f);
        _settingsScrollPanel.Append(scrollbar);
        _settingsList.SetScrollbar(scrollbar);

        _addEntryButton = new FlatTextButton(string.Empty, ScaleText(0.74f));
        _addEntryButton.Width.Set(0f, 1f);
        _addEntryButton.Height.Set(Scale(24f), 0f);
        _addEntryButton.Top.Set(-Scale(30f), 1f);
        _addEntryButton.OnLeftClick += (_, _) => OpenCatalogPickerForActiveTab();
        _contentContainer.Append(_addEntryButton);

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

    private void ApplySettingsChange(LightingSettings settings)
    {
        settings.EnsureDynamicEntries();
        settings.ApplyRuntimeChanges();
        QueueSettingsPersist();
    }

    private void QueueSettingsPersist()
    {
        _pendingConfigPersist = true;
        _persistCountdownFrames = PersistDebounceFrames;
    }

    private void PersistPendingSettingsChanges()
    {
        if (!_pendingConfigPersist)
            return;

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();
        settings.SaveChanges();

        _pendingConfigPersist = false;
        _persistCountdownFrames = 0;
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
        {
            CloseColorPicker();
            CloseCatalogPicker();
        }
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
        settings.EnsureDynamicEntries();

        CloseColorPicker();
        CloseCatalogPicker();

        _settingsList.Clear();

        bool usesDynamicEntries = LightingSettingsCatalog.UsesDynamicEntries(tab);
        _settingsList.ListPadding = tab == LightingSettingsTab.BossEffects ? Scale(2f) : Scale(8f);
        ApplyTabLayoutMode(tab, usesDynamicEntries);

        if (usesDynamicEntries)
        {
            if (tab == LightingSettingsTab.TileEffects)
            {
                BuildTileRows(settings);
            }
            else if (tab == LightingSettingsTab.Events)
            {
                BuildEventRows(settings);
            }
            else if (tab == LightingSettingsTab.BossEffects)
            {
                BuildBossRows(settings);
            }

            _settingsList.Recalculate();
            return;
        }

        IReadOnlyList<LightingSettingDescriptor> descriptors = LightingSettingsCatalog.GetTabDescriptors(tab);
        for (int i = 0; i < descriptors.Count; i++)
        {
            LightingSettingDescriptor descriptor = descriptors[i];
            UIElement row = CreateRow(descriptor, settings);
            _settingsList.Add(row);
        }

        _settingsList.Recalculate();
    }

    private void ApplyTabLayoutMode(LightingSettingsTab tab, bool usesDynamicEntries)
    {
        if (_settingsScrollPanel is not null)
        {
            _settingsScrollPanel.Height.Set(usesDynamicEntries ? -Scale(112f) : -Scale(76f), 1f);
        }

        if (_addEntryButton is null)
            return;

        _addEntryButton.SetText(LightingSettingsCatalog.GetAddButtonLabel(tab));

        if (usesDynamicEntries)
        {
            if (_addEntryButton.Parent is null)
                _contentContainer?.Append(_addEntryButton);
        }
        else
        {
            _addEntryButton.Remove();
        }
    }

    private void BuildTileRows(LightingSettings settings)
    {
        bool hasRows = false;

        for (int i = 0; i < settings.TileEffectEntries.Count; i++)
        {
            int entryIndex = i;
            LightingTileEffectEntry entry = settings.TileEffectEntries[i];
            if (entry is null || entry.TileIds is null || entry.TileIds.Count == 0)
                continue;

            int firstTileId = entry.TileIds[0];
            Color defaultColor = LightingDynamicCatalogs.GetSuggestedTileColor(firstTileId);

            string label = string.IsNullOrWhiteSpace(entry.Name)
                ? "Tile Group"
                : entry.Name;

            string displayLabel = FormatGroupLabel(label, entry.TileIds.Count);

            ColorSettingDescriptor descriptor = new(
                displayLabel,
                _ => entry.Color,
                (_, value) =>
                {
                    entry.Color = value;
                },
                _ => defaultColor,
                _ => entry.Enabled,
                (_, value) =>
                {
                    entry.Enabled = value;
                });

            ColorSettingRow row = new(
                descriptor,
                settings,
                d => OpenColorPicker(d, settings),
                _uiScale,
                () => EditTileEntry(entryIndex),
                () => RemoveTileEntry(entryIndex),
                () => ApplySettingsChange(settings));

            _settingsList.Add(row);
            hasRows = true;
        }

        if (!hasRows)
            _settingsList.Add(new UIText("No tile entries. Add one below.", ScaleText(0.74f)));
    }

    private void BuildEventRows(LightingSettings settings)
    {
        bool hasRows = false;

        for (int i = 0; i < settings.EventEffectEntries.Count; i++)
        {
            int entryIndex = i;
            LightingEventEffectEntry entry = settings.EventEffectEntries[i];
            if (entry is null)
                continue;

            LightingEventId displayEventId = entry.EventId;
            if (entry.EventIds is not null && entry.EventIds.Count > 0)
                displayEventId = entry.EventIds[0];

            if (!LightingDynamicCatalogs.TryGetEventCatalogItem(displayEventId, out LightingEventCatalogItem eventCatalogItem))
                continue;

            string label = string.IsNullOrWhiteSpace(entry.Name)
                ? $"{eventCatalogItem.DisplayName} Group"
                : entry.Name;

            int eventCount = entry.EventIds is null || entry.EventIds.Count == 0 ? 1 : entry.EventIds.Count;
            string displayLabel = FormatGroupLabel(label, eventCount);

            ColorSettingDescriptor descriptor = new(
                displayLabel,
                _ => entry.Color,
                (_, value) =>
                {
                    entry.Color = value;
                },
                _ => eventCatalogItem.DefaultColor,
                _ => entry.Enabled,
                (_, value) =>
                {
                    entry.Enabled = value;
                });

            _settingsList.Add(new ColorSettingRow(
                descriptor,
                settings,
                d => OpenColorPicker(d, settings),
                _uiScale,
                () => EditEventEntry(entryIndex),
                () => RemoveEventEntry(entryIndex),
                () => ApplySettingsChange(settings)));

            hasRows = true;
        }

        if (!hasRows)
            _settingsList.Add(new UIText("No event entries. Add one below.", ScaleText(0.74f)));
    }

    private void BuildBossRows(LightingSettings settings)
    {
        void OnChanged()
        {
            ApplySettingsChange(settings);
        }

        bool hasRows = false;

        for (int i = 0; i < settings.BossEffectEntries.Count; i++)
        {
            int entryIndex = i;
            LightingBossEffectEntry entry = settings.BossEffectEntries[i];
            if (entry is null)
                continue;

            LightingBossId displayBossId = entry.BossId;
            if (entry.BossIds is not null && entry.BossIds.Count > 0)
                displayBossId = entry.BossIds[0];

            if (!LightingDynamicCatalogs.TryGetBossCatalogItem(displayBossId, out LightingBossCatalogItem bossCatalogItem))
                continue;

            string label = string.IsNullOrWhiteSpace(entry.Name)
                ? $"{bossCatalogItem.DisplayName} Group"
                : entry.Name;

            int bossCount = entry.BossIds is null || entry.BossIds.Count == 0 ? 1 : entry.BossIds.Count;
            string displayLabel = FormatGroupLabel(label, bossCount);

            FloatSettingDescriptor floatDescriptor = new(
                displayLabel,
                1f,
                2f,
                0.05f,
                _ => entry.Multiplier,
                (_, value) =>
                {
                    entry.Multiplier = Math.Clamp(value, 1f, 2f);
                },
                _ => entry.Enabled,
                (_, value) =>
                {
                    entry.Enabled = value;
                });

            _settingsList.Add(new FloatSettingRow(
                floatDescriptor,
                settings,
                OnChanged,
                _uiScale,
                () => EditBossEntry(entryIndex),
                () => RemoveBossEntry(entryIndex)));

            hasRows = true;
        }

        if (!hasRows)
            _settingsList.Add(new UIText("No boss entries. Add one below.", ScaleText(0.74f)));
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
            ApplySettingsChange(settings);
        }

        return descriptor switch
        {
            BoolSettingDescriptor boolDescriptor => new BoolSettingRow(boolDescriptor, settings, OnChanged, _uiScale),
            FloatSettingDescriptor floatDescriptor => new FloatSettingRow(floatDescriptor, settings, OnChanged, _uiScale),
            ColorSettingDescriptor colorDescriptor => new ColorSettingRow(colorDescriptor, settings, d => OpenColorPicker(d, settings), _uiScale, onSettingChanged: OnChanged),
            _ => new UIText("Unsupported setting"),
        };
    }

    private void OpenColorPicker(ColorSettingDescriptor colorDescriptor, LightingSettings settings)
    {
        CloseColorPicker();
        CloseCatalogPicker();

        Color defaultColor = colorDescriptor.DefaultGetter is not null
            ? colorDescriptor.DefaultGetter(_defaultSettings ?? settings)
            : (_defaultSettings is null ? colorDescriptor.Getter(settings) : colorDescriptor.Getter(_defaultSettings));

        _colorPickerPopup = new ColorPickerPopup(
            colorDescriptor.Label,
            colorDescriptor.Getter(settings),
            defaultColor,
            colorValue =>
            {
                colorDescriptor.Setter(settings, colorValue);
                ApplySettingsChange(settings);
            },
            CloseColorPicker,
            _uiScale);

        PositionPopupToPanelLeft(_colorPickerPopup, Scale(352f), Scale(230f));
        Append(_colorPickerPopup);
    }

    private void OpenCatalogPickerForActiveTab()
    {
        if (!LightingSettingsCatalog.UsesDynamicEntries(_activeTab))
            return;

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        List<CatalogPickerOption> options = BuildCatalogOptionsForTab(_activeTab, settings);
        OpenCatalogPicker($"Select {LightingSettingsCatalog.GetTabTitle(_activeTab)} Entry", options, OnCatalogSelectionConfirmed);
    }

    private void OpenCatalogPicker(
        string title,
        IReadOnlyList<CatalogPickerOption> options,
        Action<IReadOnlyList<CatalogPickerOption>, string> onConfirm,
        IReadOnlyCollection<string> initiallySelectedKeys = null,
        string initialGroupName = "",
        string confirmButtonText = "Add Selected")
    {
        CloseColorPicker();
        CloseCatalogPicker();

        _catalogPickerPopup = new CatalogPickerPopup(
            title,
            options,
            onConfirm,
            CloseCatalogPicker,
            _uiScale,
            initiallySelectedKeys,
            initialGroupName,
            confirmButtonText);

        PositionPopupToPanelLeft(_catalogPickerPopup, Scale(460f), Scale(560f));
        Append(_catalogPickerPopup);
    }

    private List<CatalogPickerOption> BuildCatalogOptionsForTab(LightingSettingsTab tab, LightingSettings settings, int ignoreEntryIndex = -1)
    {
        List<CatalogPickerOption> options = [];

        if (tab == LightingSettingsTab.TileEffects)
        {
            HashSet<int> usedTileIds = [];
            for (int i = 0; i < settings.TileEffectEntries.Count; i++)
            {
                if (i == ignoreEntryIndex)
                    continue;

                LightingTileEffectEntry entry = settings.TileEffectEntries[i];
                if (entry?.TileIds is null)
                    continue;

                for (int j = 0; j < entry.TileIds.Count; j++)
                    usedTileIds.Add(entry.TileIds[j]);
            }

            IReadOnlyList<LightingTileCatalogItem> tiles = LightingDynamicCatalogs.GetTileCatalogItems();
            for (int i = 0; i < tiles.Count; i++)
            {
                LightingTileCatalogItem tile = tiles[i];
                if (usedTileIds.Contains(tile.TileId))
                    continue;

                options.Add(new CatalogPickerOption($"tile:{tile.TileId}", tile.DisplayName));
            }
        }
        else if (tab == LightingSettingsTab.Events)
        {
            HashSet<LightingEventId> usedEvents = [];
            for (int i = 0; i < settings.EventEffectEntries.Count; i++)
            {
                if (i == ignoreEntryIndex)
                    continue;

                LightingEventEffectEntry entry = settings.EventEffectEntries[i];
                if (entry is null)
                    continue;

                if (entry.EventIds is null || entry.EventIds.Count == 0)
                {
                    usedEvents.Add(entry.EventId);
                    continue;
                }

                for (int j = 0; j < entry.EventIds.Count; j++)
                    usedEvents.Add(entry.EventIds[j]);
            }

            IReadOnlyList<LightingEventCatalogItem> events = LightingDynamicCatalogs.GetEventCatalogItems();
            for (int i = 0; i < events.Count; i++)
            {
                LightingEventCatalogItem eventItem = events[i];
                if (usedEvents.Contains(eventItem.EventId))
                    continue;

                options.Add(new CatalogPickerOption($"event:{(int)eventItem.EventId}", eventItem.DisplayName));
            }
        }
        else if (tab == LightingSettingsTab.BossEffects)
        {
            HashSet<LightingBossId> usedBosses = [];
            for (int i = 0; i < settings.BossEffectEntries.Count; i++)
            {
                if (i == ignoreEntryIndex)
                    continue;

                LightingBossEffectEntry entry = settings.BossEffectEntries[i];
                if (entry is null)
                    continue;

                if (entry.BossIds is null || entry.BossIds.Count == 0)
                {
                    usedBosses.Add(entry.BossId);
                    continue;
                }

                for (int j = 0; j < entry.BossIds.Count; j++)
                    usedBosses.Add(entry.BossIds[j]);
            }

            IReadOnlyList<LightingBossCatalogItem> bosses = LightingDynamicCatalogs.GetBossCatalogItems();
            for (int i = 0; i < bosses.Count; i++)
            {
                LightingBossCatalogItem bossItem = bosses[i];
                if (usedBosses.Contains(bossItem.BossId))
                    continue;

                options.Add(new CatalogPickerOption($"boss:{(int)bossItem.BossId}", bossItem.DisplayName));
            }
        }

        options.Sort(static (a, b) => string.Compare(a.Label, b.Label, StringComparison.OrdinalIgnoreCase));
        return options;
    }

    private void OnCatalogSelectionConfirmed(IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        if (selectedOptions is null || selectedOptions.Count == 0)
            return;

        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        bool added = _activeTab switch
        {
            LightingSettingsTab.TileEffects => TryAddTileGroup(settings, selectedOptions, groupName),
            LightingSettingsTab.Events => TryAddEventGroup(settings, selectedOptions, groupName),
            LightingSettingsTab.BossEffects => TryAddBossGroup(settings, selectedOptions, groupName),
            _ => false,
        };

        if (!added)
            return;

        ApplySettingsChange(settings);
        BuildRowsForTab(_activeTab);
    }

    private bool TryAddTileGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        HashSet<int> usedTileIds = [];
        for (int i = 0; i < settings.TileEffectEntries.Count; i++)
        {
            LightingTileEffectEntry entry = settings.TileEffectEntries[i];
            if (entry?.TileIds is null)
                continue;

            for (int j = 0; j < entry.TileIds.Count; j++)
                usedTileIds.Add(entry.TileIds[j]);
        }

        List<int> tileIds = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!TryParseOptionKey(option.Key, "tile", out int tileId))
                continue;

            if (!usedTileIds.Add(tileId))
                continue;

            tileIds.Add(tileId);
        }

        if (tileIds.Count == 0)
            return false;

        string resolvedName = ResolveGroupName(groupName, selectedOptions, "Tile Group");
        Color suggestedColor = LightingDynamicCatalogs.GetSuggestedTileColor(tileIds[0]);
        settings.TileEffectEntries.Add(new LightingTileEffectEntry(resolvedName, tileIds, suggestedColor, enabled: true));
        return true;
    }

    private bool TryAddEventGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        HashSet<LightingEventId> usedEventIds = [];
        for (int i = 0; i < settings.EventEffectEntries.Count; i++)
        {
            LightingEventEffectEntry entry = settings.EventEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.EventIds is null || entry.EventIds.Count == 0)
            {
                usedEventIds.Add(entry.EventId);
                continue;
            }

            for (int j = 0; j < entry.EventIds.Count; j++)
                usedEventIds.Add(entry.EventIds[j]);
        }

        List<LightingEventId> eventIds = [];
        Color defaultColor = Color.White;

        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!TryParseOptionKey(option.Key, "event", out int eventIdRaw))
                continue;

            LightingEventId eventId = (LightingEventId)eventIdRaw;
            if (!LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out LightingEventCatalogItem eventItem))
                continue;

            if (!usedEventIds.Add(eventId))
                continue;

            if (eventIds.Count == 0)
                defaultColor = eventItem.DefaultColor;

            eventIds.Add(eventId);
        }

        if (eventIds.Count == 0)
            return false;

        string resolvedName = ResolveGroupName(groupName, selectedOptions, "Event Group");
        settings.EventEffectEntries.Add(new LightingEventEffectEntry(resolvedName, eventIds, true, defaultColor));
        return true;
    }

    private bool TryAddBossGroup(LightingSettings settings, IReadOnlyList<CatalogPickerOption> selectedOptions, string groupName)
    {
        HashSet<LightingBossId> usedBossIds = [];
        for (int i = 0; i < settings.BossEffectEntries.Count; i++)
        {
            LightingBossEffectEntry entry = settings.BossEffectEntries[i];
            if (entry is null)
                continue;

            if (entry.BossIds is null || entry.BossIds.Count == 0)
            {
                usedBossIds.Add(entry.BossId);
                continue;
            }

            for (int j = 0; j < entry.BossIds.Count; j++)
                usedBossIds.Add(entry.BossIds[j]);
        }

        List<LightingBossId> bossIds = [];
        float defaultMultiplier = 1.4f;

        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!TryParseOptionKey(option.Key, "boss", out int bossIdRaw))
                continue;

            LightingBossId bossId = (LightingBossId)bossIdRaw;
            if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out LightingBossCatalogItem bossItem))
                continue;

            if (!usedBossIds.Add(bossId))
                continue;

            if (bossIds.Count == 0)
                defaultMultiplier = bossItem.DefaultMultiplier;

            bossIds.Add(bossId);
        }

        if (bossIds.Count == 0)
            return false;

        string resolvedName = ResolveGroupName(groupName, selectedOptions, "Boss Group");
        settings.BossEffectEntries.Add(new LightingBossEffectEntry(resolvedName, bossIds, true, Math.Clamp(defaultMultiplier, 1f, 2f)));
        return true;
    }

    private static bool TryParseOptionKey(string key, string expectedPrefix, out int value)
    {
        value = 0;
        if (string.IsNullOrWhiteSpace(key))
            return false;

        string[] parts = key.Split(':', 2, StringSplitOptions.RemoveEmptyEntries);
        return parts.Length == 2
            && parts[0] == expectedPrefix
            && int.TryParse(parts[1], out value);
    }

    private static string ResolveGroupName(string groupName, IReadOnlyList<CatalogPickerOption> selectedOptions, string multiSelectionFallback)
    {
        if (!string.IsNullOrWhiteSpace(groupName))
            return groupName.Trim();

        return selectedOptions.Count == 1
            ? selectedOptions[0].Label
            : multiSelectionFallback;
    }

    private static string FormatGroupLabel(string baseLabel, int groupCount)
    {
        return groupCount > 1
            ? $"{baseLabel} ({groupCount})"
            : baseLabel;
    }

    private string BuildModifiedSettingsClipboardText(LightingSettings settings)
    {
        settings.EnsureDynamicEntries();
        LightingSettings defaults = LightingSettingsDefaults.CreateDefaults();

        List<string> sections = [];

        string scalarSection = BuildModifiedScalarSection(settings, defaults);
        if (!string.IsNullOrWhiteSpace(scalarSection))
            sections.Add(scalarSection);

        string tileSection = BuildTileEntriesSection(settings.TileEffectEntries, defaults.TileEffectEntries);
        if (!string.IsNullOrWhiteSpace(tileSection))
            sections.Add(tileSection);

        string eventSection = BuildEventEntriesSection(settings.EventEffectEntries, defaults.EventEffectEntries);
        if (!string.IsNullOrWhiteSpace(eventSection))
            sections.Add(eventSection);

        string bossSection = BuildBossEntriesSection(settings.BossEffectEntries, defaults.BossEffectEntries);
        if (!string.IsNullOrWhiteSpace(bossSection))
            sections.Add(bossSection);

        if (sections.Count == 0)
            return string.Empty;

        return "LightingEssentials Modified Settings\n\n" + string.Join("\n\n", sections);
    }

    private static string BuildModifiedScalarSection(LightingSettings settings, LightingSettings defaults)
    {
        List<string> lines = [];

        for (int i = 0; i < CopyableSettingsFields.Length; i++)
        {
            FieldInfo field = CopyableSettingsFields[i];
            if (NonScalarCopiedFields.Contains(field.Name))
                continue;

            object currentValue = field.GetValue(settings);
            object defaultValue = field.GetValue(defaults);
            if (ValuesEqual(currentValue, defaultValue))
                continue;

            lines.Add($"- {field.Name}: {FormatValue(currentValue)} (default: {FormatValue(defaultValue)})");
        }

        if (lines.Count == 0)
            return string.Empty;

        return "Scalar fields:\n" + string.Join("\n", lines);
    }

    private static string BuildTileEntriesSection(IReadOnlyList<LightingTileEffectEntry> currentEntries, IReadOnlyList<LightingTileEffectEntry> defaultEntries)
    {
        StringBuilder builder = new();
        builder.AppendLine("Tile groups:");

        bool hasModifiedEntries = false;
        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {

            LightingTileEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingTileEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;

            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                AppendTileDiffLine(builder, "Removed", defaultEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (defaultEntry is null)
            {
                AppendTileDiffLine(builder, "Added", currentEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (AreTileEntryEquivalent(currentEntry, defaultEntry))
                continue;

            AppendTileDiffLine(builder, "Modified", currentEntry, defaultEntry);
            hasModifiedEntries = true;
        }

        if (!hasModifiedEntries)
            return string.Empty;

        return builder.ToString().TrimEnd();
    }

    private static string BuildEventEntriesSection(IReadOnlyList<LightingEventEffectEntry> currentEntries, IReadOnlyList<LightingEventEffectEntry> defaultEntries)
    {
        StringBuilder builder = new();
        builder.AppendLine("Event groups:");

        bool hasModifiedEntries = false;
        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {

            LightingEventEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingEventEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;

            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                AppendEventDiffLine(builder, "Removed", defaultEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (defaultEntry is null)
            {
                AppendEventDiffLine(builder, "Added", currentEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (AreEventEntryEquivalent(currentEntry, defaultEntry))
                continue;

            AppendEventDiffLine(builder, "Modified", currentEntry, defaultEntry);
            hasModifiedEntries = true;
        }

        if (!hasModifiedEntries)
            return string.Empty;

        return builder.ToString().TrimEnd();
    }

    private static string BuildBossEntriesSection(IReadOnlyList<LightingBossEffectEntry> currentEntries, IReadOnlyList<LightingBossEffectEntry> defaultEntries)
    {
        StringBuilder builder = new();
        builder.AppendLine("Boss groups:");

        bool hasModifiedEntries = false;
        int maxCount = Math.Max(currentEntries.Count, defaultEntries.Count);
        for (int i = 0; i < maxCount; i++)
        {

            LightingBossEffectEntry currentEntry = i < currentEntries.Count ? currentEntries[i] : null;
            LightingBossEffectEntry defaultEntry = i < defaultEntries.Count ? defaultEntries[i] : null;

            if (currentEntry is null && defaultEntry is null)
                continue;

            if (currentEntry is null)
            {
                AppendBossDiffLine(builder, "Removed", defaultEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (defaultEntry is null)
            {
                AppendBossDiffLine(builder, "Added", currentEntry, null);
                hasModifiedEntries = true;
                continue;
            }

            if (AreBossEntryEquivalent(currentEntry, defaultEntry))
                continue;

            AppendBossDiffLine(builder, "Modified", currentEntry, defaultEntry);
            hasModifiedEntries = true;
        }

        if (!hasModifiedEntries)
            return string.Empty;

        return builder.ToString().TrimEnd();
    }

    private static void AppendTileDiffLine(StringBuilder builder, string tag, LightingTileEffectEntry entry, LightingTileEffectEntry defaultEntry)
    {
        int memberCount = entry.TileIds?.Count ?? 0;
        string name = string.IsNullOrWhiteSpace(entry.Name) ? "Tile Group" : entry.Name.Trim();
        builder.AppendLine($"- [{tag}] {name} | Enabled={entry.Enabled} | Color={FormatColor(entry.Color)} | Count={memberCount}");

        if (memberCount > 0)
            builder.AppendLine($"  Members: {JoinFormattedMembers(entry.TileIds, static tileId => FormatTileMember(tileId))}");

        if (defaultEntry is null || tag != "Modified")
            return;

        int defaultCount = defaultEntry.TileIds?.Count ?? 0;
        builder.AppendLine($"  Default -> Enabled={defaultEntry.Enabled} | Color={FormatColor(defaultEntry.Color)} | Count={defaultCount}");
        if (defaultCount > 0)
            builder.AppendLine($"  Default Members: {JoinFormattedMembers(defaultEntry.TileIds, static tileId => FormatTileMember(tileId))}");
    }

    private static void AppendEventDiffLine(StringBuilder builder, string tag, LightingEventEffectEntry entry, LightingEventEffectEntry defaultEntry)
    {
        List<LightingEventId> eventIds = GetEventIds(entry);
        string name = string.IsNullOrWhiteSpace(entry.Name) ? "Event Group" : entry.Name.Trim();
        builder.AppendLine($"- [{tag}] {name} | Enabled={entry.Enabled} | Color={FormatColor(entry.Color)} | Count={eventIds.Count}");
        if (eventIds.Count > 0)
            builder.AppendLine($"  Members: {JoinFormattedMembers(eventIds, static eventId => FormatEventMember(eventId))}");

        if (defaultEntry is null || tag != "Modified")
            return;

        List<LightingEventId> defaultEventIds = GetEventIds(defaultEntry);
        builder.AppendLine($"  Default -> Enabled={defaultEntry.Enabled} | Color={FormatColor(defaultEntry.Color)} | Count={defaultEventIds.Count}");
        if (defaultEventIds.Count > 0)
            builder.AppendLine($"  Default Members: {JoinFormattedMembers(defaultEventIds, static eventId => FormatEventMember(eventId))}");
    }

    private static void AppendBossDiffLine(StringBuilder builder, string tag, LightingBossEffectEntry entry, LightingBossEffectEntry defaultEntry)
    {
        List<LightingBossId> bossIds = GetBossIds(entry);
        string name = string.IsNullOrWhiteSpace(entry.Name) ? "Boss Group" : entry.Name.Trim();
        builder.AppendLine($"- [{tag}] {name} | Enabled={entry.Enabled} | Multiplier={entry.Multiplier.ToString("0.00", CultureInfo.InvariantCulture)} | Count={bossIds.Count}");
        if (bossIds.Count > 0)
            builder.AppendLine($"  Members: {JoinFormattedMembers(bossIds, static bossId => FormatBossMember(bossId))}");

        if (defaultEntry is null || tag != "Modified")
            return;

        List<LightingBossId> defaultBossIds = GetBossIds(defaultEntry);
        builder.AppendLine($"  Default -> Enabled={defaultEntry.Enabled} | Multiplier={defaultEntry.Multiplier.ToString("0.00", CultureInfo.InvariantCulture)} | Count={defaultBossIds.Count}");
        if (defaultBossIds.Count > 0)
            builder.AppendLine($"  Default Members: {JoinFormattedMembers(defaultBossIds, static bossId => FormatBossMember(bossId))}");
    }

    private static string JoinFormattedMembers<T>(IReadOnlyList<T> values, Func<T, string> formatter)
    {
        StringBuilder builder = new();
        for (int i = 0; i < values.Count; i++)
        {
            if (i > 0)
                builder.Append(", ");

            builder.Append(formatter(values[i]));
        }

        return builder.ToString();
    }

    private static bool AreTileEntryEquivalent(LightingTileEffectEntry left, LightingTileEffectEntry right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (!string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            return false;

        if (left.Enabled != right.Enabled || left.Color != right.Color)
            return false;

        return SequenceEquals(left.TileIds, right.TileIds);
    }

    private static bool AreEventEntryEquivalent(LightingEventEffectEntry left, LightingEventEffectEntry right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (!string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            return false;

        if (left.Enabled != right.Enabled || left.Color != right.Color)
            return false;

        return SequenceEquals(GetEventIds(left), GetEventIds(right));
    }

    private static bool AreBossEntryEquivalent(LightingBossEffectEntry left, LightingBossEffectEntry right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (!string.Equals(left.Name, right.Name, StringComparison.Ordinal))
            return false;

        if (left.Enabled != right.Enabled || MathF.Abs(left.Multiplier - right.Multiplier) > 0.0001f)
            return false;

        return SequenceEquals(GetBossIds(left), GetBossIds(right));
    }

    private static List<LightingEventId> GetEventIds(LightingEventEffectEntry entry)
    {
        if (entry.EventIds is not null && entry.EventIds.Count > 0)
            return entry.EventIds;

        return [entry.EventId];
    }

    private static List<LightingBossId> GetBossIds(LightingBossEffectEntry entry)
    {
        if (entry.BossIds is not null && entry.BossIds.Count > 0)
            return entry.BossIds;

        return [entry.BossId];
    }

    private static bool SequenceEquals<T>(IReadOnlyList<T> left, IReadOnlyList<T> right)
    {
        if (left is null || right is null)
            return ReferenceEquals(left, right);

        if (left.Count != right.Count)
            return false;

        for (int i = 0; i < left.Count; i++)
        {
            if (!EqualityComparer<T>.Default.Equals(left[i], right[i]))
                return false;
        }

        return true;
    }

    private static bool ValuesEqual(object left, object right)
    {
        if (left is null || right is null)
            return left is null && right is null;

        if (left is float leftFloat && right is float rightFloat)
            return MathF.Abs(leftFloat - rightFloat) <= 0.0001f;

        return left.Equals(right);
    }

    private static string FormatValue(object value)
    {
        return value switch
        {
            null => "<null>",
            bool boolValue => boolValue ? "true" : "false",
            float floatValue => floatValue.ToString("0.###", CultureInfo.InvariantCulture),
            Color color => FormatColor(color),
            _ => value.ToString(),
        };
    }

    private static string FormatColor(Color color)
    {
        return $"{color.R},{color.G},{color.B},{color.A}";
    }

    private static string FormatTileMember(int tileId)
    {
        if (TileID.Search.TryGetName(tileId, out string tileName) && !string.IsNullOrWhiteSpace(tileName))
            return $"{tileName}({tileId})";

        return $"Tile{tileId}";
    }

    private static string FormatEventMember(LightingEventId eventId)
    {
        return LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out LightingEventCatalogItem item)
            ? item.DisplayName
            : eventId.ToString();
    }

    private static string FormatBossMember(LightingBossId bossId)
    {
        return LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out LightingBossCatalogItem item)
            ? item.DisplayName
            : bossId.ToString();
    }

    private void EditTileEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.TileEffectEntries.Count)
            return;

        LightingTileEffectEntry entry = settings.TileEffectEntries[index];
        if (entry is null || entry.TileIds is null || entry.TileIds.Count == 0)
            return;

        List<CatalogPickerOption> options = BuildCatalogOptionsForTab(LightingSettingsTab.TileEffects, settings, index);
        List<string> initialSelectionKeys = [];
        for (int i = 0; i < entry.TileIds.Count; i++)
            initialSelectionKeys.Add($"tile:{entry.TileIds[i]}");

        OpenCatalogPicker(
            "Edit Tile Group",
            options,
            (selectedOptions, groupName) =>
            {
                List<int> tileIds = ParseSelectedTileIds(selectedOptions);
                if (tileIds.Count == 0 || index < 0 || index >= settings.TileEffectEntries.Count)
                    return;

                string fallbackName = string.IsNullOrWhiteSpace(entry.Name) ? "Tile Group" : entry.Name;
                string resolvedName = ResolveGroupName(groupName, selectedOptions, fallbackName);
                settings.TileEffectEntries[index] = new LightingTileEffectEntry(resolvedName, tileIds, entry.Color, entry.Enabled);
                ApplySettingsChange(settings);
                BuildRowsForTab(_activeTab);
            },
            initialSelectionKeys,
            entry.Name,
            "Save Group");
    }

    private void EditEventEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.EventEffectEntries.Count)
            return;

        LightingEventEffectEntry entry = settings.EventEffectEntries[index];
        if (entry is null)
            return;

        List<CatalogPickerOption> options = BuildCatalogOptionsForTab(LightingSettingsTab.Events, settings, index);
        List<string> initialSelectionKeys = [];

        if (entry.EventIds is null || entry.EventIds.Count == 0)
        {
            initialSelectionKeys.Add($"event:{(int)entry.EventId}");
        }
        else
        {
            for (int i = 0; i < entry.EventIds.Count; i++)
                initialSelectionKeys.Add($"event:{(int)entry.EventIds[i]}");
        }

        OpenCatalogPicker(
            "Edit Event Group",
            options,
            (selectedOptions, groupName) =>
            {
                List<LightingEventId> eventIds = ParseSelectedEventIds(selectedOptions);
                if (eventIds.Count == 0 || index < 0 || index >= settings.EventEffectEntries.Count)
                    return;

                string fallbackName = string.IsNullOrWhiteSpace(entry.Name) ? "Event Group" : entry.Name;
                string resolvedName = ResolveGroupName(groupName, selectedOptions, fallbackName);
                settings.EventEffectEntries[index] = new LightingEventEffectEntry(resolvedName, eventIds, entry.Enabled, entry.Color);
                ApplySettingsChange(settings);
                BuildRowsForTab(_activeTab);
            },
            initialSelectionKeys,
            entry.Name,
            "Save Group");
    }

    private void EditBossEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.BossEffectEntries.Count)
            return;

        LightingBossEffectEntry entry = settings.BossEffectEntries[index];
        if (entry is null)
            return;

        List<CatalogPickerOption> options = BuildCatalogOptionsForTab(LightingSettingsTab.BossEffects, settings, index);
        List<string> initialSelectionKeys = [];

        if (entry.BossIds is null || entry.BossIds.Count == 0)
        {
            initialSelectionKeys.Add($"boss:{(int)entry.BossId}");
        }
        else
        {
            for (int i = 0; i < entry.BossIds.Count; i++)
                initialSelectionKeys.Add($"boss:{(int)entry.BossIds[i]}");
        }

        OpenCatalogPicker(
            "Edit Boss Group",
            options,
            (selectedOptions, groupName) =>
            {
                List<LightingBossId> bossIds = ParseSelectedBossIds(selectedOptions);
                if (bossIds.Count == 0 || index < 0 || index >= settings.BossEffectEntries.Count)
                    return;

                string fallbackName = string.IsNullOrWhiteSpace(entry.Name) ? "Boss Group" : entry.Name;
                string resolvedName = ResolveGroupName(groupName, selectedOptions, fallbackName);
                settings.BossEffectEntries[index] = new LightingBossEffectEntry(resolvedName, bossIds, entry.Enabled, entry.Multiplier);
                ApplySettingsChange(settings);
                BuildRowsForTab(_activeTab);
            },
            initialSelectionKeys,
            entry.Name,
            "Save Group");
    }

    private List<int> ParseSelectedTileIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<int> tileIds = [];
        HashSet<int> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!TryParseOptionKey(option.Key, "tile", out int tileId) || !seen.Add(tileId))
                continue;

            tileIds.Add(tileId);
        }

        return tileIds;
    }

    private List<LightingEventId> ParseSelectedEventIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<LightingEventId> eventIds = [];
        HashSet<LightingEventId> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!TryParseOptionKey(option.Key, "event", out int eventIdRaw))
                continue;

            LightingEventId eventId = (LightingEventId)eventIdRaw;
            if (!LightingDynamicCatalogs.TryGetEventCatalogItem(eventId, out _) || !seen.Add(eventId))
                continue;

            eventIds.Add(eventId);
        }

        return eventIds;
    }

    private List<LightingBossId> ParseSelectedBossIds(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        List<LightingBossId> bossIds = [];
        HashSet<LightingBossId> seen = [];
        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];
            if (!TryParseOptionKey(option.Key, "boss", out int bossIdRaw))
                continue;

            LightingBossId bossId = (LightingBossId)bossIdRaw;
            if (!LightingDynamicCatalogs.TryGetBossCatalogItem(bossId, out _) || !seen.Add(bossId))
                continue;

            bossIds.Add(bossId);
        }

        return bossIds;
    }

    private void RemoveTileEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.TileEffectEntries.Count)
            return;

        settings.TileEffectEntries.RemoveAt(index);
        ApplySettingsChange(settings);
        BuildRowsForTab(_activeTab);
    }

    private void RemoveEventEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.EventEffectEntries.Count)
            return;

        settings.EventEffectEntries.RemoveAt(index);
        ApplySettingsChange(settings);
        BuildRowsForTab(_activeTab);
    }

    private void RemoveBossEntry(int index)
    {
        LightingSettings settings = ModContent.GetInstance<LightingSettings>();
        settings.EnsureDynamicEntries();

        if (index < 0 || index >= settings.BossEffectEntries.Count)
            return;

        settings.BossEffectEntries.RemoveAt(index);
        ApplySettingsChange(settings);
        BuildRowsForTab(_activeTab);
    }

    private void PositionPopupToPanelLeft(UIElement popup, float popupWidth, float popupHeight)
    {
        CalculatedStyle rootBounds = _rootPanel.GetDimensions();
        float popupGap = Scale(6f);
        float viewportInset = Scale(8f);

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

    private void CloseCatalogPicker()
    {
        if (_catalogPickerPopup is null)
            return;

        _catalogPickerPopup.EndSearchInput();
        _catalogPickerPopup.Remove();
        _catalogPickerPopup = null;
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
