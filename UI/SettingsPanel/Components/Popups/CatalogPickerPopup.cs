using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Styling;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Popups;

internal readonly record struct CatalogPickerOption(string Key, string Label);
internal readonly record struct CatalogPickerResetState(IReadOnlyCollection<string> SelectedKeys, string GroupName);

internal sealed class CatalogPickerPopup : UIPanel
{
    private const int OptionRowBuildBatchSize = 48;
    private const int SearchDebounceMilliseconds = 90;
    private const int MaxRenderedOptionsPerFilter = 320;

    private readonly IReadOnlyList<CatalogPickerOption> _allOptions;
    private readonly Action<IReadOnlyList<CatalogPickerOption>, string> _onItemsSelected;
    private readonly Func<CatalogPickerResetState> _onResetToDefaults;
    private readonly Action _onClose;
    private readonly float _uiScale;
    private readonly string _confirmButtonText;

    private readonly UISearchBar _searchBar;
    private readonly UISearchBar _groupNameBar;
    private readonly UIList _optionsList;
    private readonly UIList _selectedList;
    private readonly FlatTextButton _addSelectedButton;
    private readonly HashSet<string> _selectedKeys = [];
    private readonly List<CatalogPickerOption> _filteredOptions = [];
    private readonly Dictionary<string, FlatTextButton> _optionButtonCache = new(StringComparer.Ordinal);
    private readonly Dictionary<string, string> _optionSearchTextByKey = new(StringComparer.Ordinal);
    private readonly Dictionary<string, List<CatalogPickerOption>> _filterCache = new(StringComparer.Ordinal);

    private UIText _optionStatusText;

    private int _nextOptionBuildIndex;

    private bool _isBuildingOptionRows;
    private bool _searchRefreshPending;
    private bool _hasBuiltOptionRows;

    private long _pendingSearchApplyTick;
    private int _totalFilteredOptionCount;

    private string _searchText = string.Empty;
    private string _searchTextLower = string.Empty;
    private string _pendingSearchText = string.Empty;
    private string _pendingSearchTextLower = string.Empty;
    private string _groupNameText = string.Empty;

    /// <summary>
    /// Creates a searchable option picker popup with a scrollable option list.
    /// </summary>
    public CatalogPickerPopup(
        string title,
        IReadOnlyList<CatalogPickerOption> options,
        Action<IReadOnlyList<CatalogPickerOption>, string> onItemsSelected,
        Func<CatalogPickerResetState> onResetToDefaults,
        Action onClose,
        float uiScale,
        IReadOnlyCollection<string> initiallySelectedKeys = null,
        string initialGroupName = "",
        string confirmButtonText = "Add Selected")
    {
        _allOptions = options;
        _onItemsSelected = onItemsSelected;
        _onResetToDefaults = onResetToDefaults;
        _onClose = onClose;
        _uiScale = uiScale;
        _confirmButtonText = confirmButtonText;

        for (int i = 0; i < _allOptions.Count; i++)
        {
            CatalogPickerOption option = _allOptions[i];
            if (!_optionSearchTextByKey.ContainsKey(option.Key))
                _optionSearchTextByKey[option.Key] = option.Label.ToLowerInvariant();
        }

        if (initiallySelectedKeys is not null)
        {
            foreach (string selectedKey in initiallySelectedKeys)
                _selectedKeys.Add(selectedKey);
        }

        Width.Set(Scale(460f), 0f);
        Height.Set(Scale(560f), 0f);
        SetPadding(Scale(12f));

        BackgroundColor = SettingsPanelTheme.PanelBackground;
        BorderColor = Color.Transparent;

        UIText header = new(title, ScaleText(0.86f));
        Append(header);

        FlatTextButton closeButton = new("Close", ScaleText(0.72f))
        {
            HAlign = 1f,
        };
        closeButton.Width.Set(Scale(54f), 0f);
        closeButton.Height.Set(Scale(22f), 0f);
        closeButton.HoverStyleEnabled = false;
        closeButton.OnLeftClick += (_, _) => RequestClose();
        Append(closeButton);

        LocalizedText searchPlaceholder = Language.GetOrRegister("Mods.LightingEssentials.UI.SearchPlaceholder", static () => "Search...");
        _searchBar = new UISearchBar(searchPlaceholder, ScaleText(0.72f));
        _searchBar.Top.Set(Scale(34f), 0f);
        _searchBar.Width.Set(0f, 1f);
        _searchBar.Height.Set(Scale(28f), 0f);
        _searchBar.OnContentsChanged += OnSearchContentsChanged;
        _searchBar.OnLeftClick += (_, _) =>
        {
            FocusInput(_searchBar, _groupNameBar);
        };
        Append(_searchBar);

        LocalizedText groupNamePlaceholder = Language.GetOrRegister("Mods.LightingEssentials.UI.GroupNamePlaceholder", static () => "Group name...");
        _groupNameBar = new UISearchBar(groupNamePlaceholder, ScaleText(0.72f));
        _groupNameBar.Top.Set(Scale(66f), 0f);
        _groupNameBar.Width.Set(0f, 1f);
        _groupNameBar.Height.Set(Scale(28f), 0f);
        _groupNameBar.OnContentsChanged += OnGroupNameChanged;
        _groupNameBar.OnLeftClick += (_, _) =>
        {
            FocusInput(_groupNameBar, _searchBar);
        };
        Append(_groupNameBar);

        UIPanel listPanel = new();
        listPanel.Top.Set(Scale(96f), 0f);
        listPanel.Width.Set(0f, 1f);
        listPanel.Height.Set(Scale(240f), 0f);
        listPanel.SetPadding(Scale(8f));
        listPanel.BackgroundColor = new Color(14, 14, 14, 150);
        listPanel.BorderColor = SettingsPanelTheme.RowBorder;
        Append(listPanel);

        float scaledScrollbarWidth = Scale(20f);
        float scaledScrollbarGap = Scale(6f);

        _optionsList = new UIList
        {
            ListPadding = Scale(4f),
            ManualSortMethod = static _ => { }
        };
        _optionsList.Width.Set(-(scaledScrollbarWidth + scaledScrollbarGap), 1f);
        _optionsList.Height.Set(0f, 1f);
        listPanel.Append(_optionsList);

        UIScrollbar scrollbar = new DarkScrollbar
        {
            HAlign = 1f,
        };
        scrollbar.Width.Set(scaledScrollbarWidth, 0f);
        scrollbar.Height.Set(0f, 1f);
        listPanel.Append(scrollbar);
        _optionsList.SetScrollbar(scrollbar);

        UIText selectedHeader = new("Selected Entries", ScaleText(0.72f));
        selectedHeader.Top.Set(-Scale(192f), 1f);
        Append(selectedHeader);

        UIPanel selectedPanel = new();
        selectedPanel.Top.Set(-Scale(178f), 1f);
        selectedPanel.Width.Set(0f, 1f);
        selectedPanel.Height.Set(Scale(136f), 0f);
        selectedPanel.SetPadding(Scale(8f));
        selectedPanel.BackgroundColor = new Color(14, 14, 14, 150);
        selectedPanel.BorderColor = SettingsPanelTheme.RowBorder;
        Append(selectedPanel);

        _selectedList = new UIList
        {
            ListPadding = Scale(3f),
            ManualSortMethod = static _ => { }
        };
        _selectedList.Width.Set(-(scaledScrollbarWidth + scaledScrollbarGap), 1f);
        _selectedList.Height.Set(0f, 1f);
        selectedPanel.Append(_selectedList);

        UIScrollbar selectedScrollbar = new DarkScrollbar
        {
            HAlign = 1f,
        };
        selectedScrollbar.Width.Set(scaledScrollbarWidth, 0f);
        selectedScrollbar.Height.Set(0f, 1f);
        selectedPanel.Append(selectedScrollbar);
        _selectedList.SetScrollbar(selectedScrollbar);

        _addSelectedButton = new FlatTextButton(confirmButtonText, ScaleText(0.72f))
        {
            HAlign = 1f,
            HoverStyleEnabled = false,
        };
        _addSelectedButton.Width.Set(Scale(126f), 0f);
        _addSelectedButton.Height.Set(Scale(24f), 0f);
        _addSelectedButton.Top.Set(-Scale(36f), 1f);
        _addSelectedButton.OnLeftClick += OnAddSelectedPressed;
        Append(_addSelectedButton);

        if (_onResetToDefaults is not null)
        {
            FlatTextButton resetButton = new("Reset to Defaults", ScaleText(0.70f))
            {
                HoverStyleEnabled = false,
            };
            resetButton.Width.Set(Scale(140f), 0f);
            resetButton.Height.Set(Scale(24f), 0f);
            resetButton.Top.Set(-Scale(36f), 1f);
            resetButton.OnLeftClick += (_, _) =>
            {
                CatalogPickerResetState resetState = _onResetToDefaults();
                ApplyResetState(resetState);
            };
            Append(resetButton);
        }

        // Initialize contents after list construction so callback-driven rebuilds are safe.
        _searchBar.SetContents(string.Empty, forced: true);
        _groupNameBar.SetContents(initialGroupName ?? string.Empty, forced: true);
        ApplyScheduledSearchRefresh(force: true);
        UpdateSelectionFeedback();
    }

    /// <summary>
    /// Locks in-game mouse actions while interacting with this popup.
    /// </summary>
    protected override void DrawSelf(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);

        if (ContainsPoint(Main.MouseScreen))
            Main.LocalPlayer.mouseInterface = true;
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        ApplyScheduledSearchRefresh(force: false);
        BuildOptionRowsIncrementally();

        if (!Main.mouseLeft || !Main.mouseLeftRelease)
            return;

        Vector2 mousePosition = Main.MouseScreen;

        if (_searchBar.IsWritingText && !_searchBar.ContainsPoint(mousePosition))
            _searchBar.ToggleTakingText();

        if (_groupNameBar.IsWritingText && !_groupNameBar.ContainsPoint(mousePosition))
            _groupNameBar.ToggleTakingText();
    }

    private void OnSearchContentsChanged(string newSearchText)
    {
        _pendingSearchText = newSearchText?.Trim() ?? string.Empty;
        _pendingSearchTextLower = _pendingSearchText.ToLowerInvariant();
        _pendingSearchApplyTick = Environment.TickCount64 + SearchDebounceMilliseconds;
        _searchRefreshPending = true;

        if (_allOptions.Count <= 300)
            ApplyScheduledSearchRefresh(force: true);
    }

    private void OnGroupNameChanged(string groupNameText)
    {
        _groupNameText = groupNameText?.Trim() ?? string.Empty;
    }

    private void FocusInput(UISearchBar focusedInput, UISearchBar otherInput)
    {
        if (otherInput.IsWritingText)
            otherInput.ToggleTakingText();

        if (!focusedInput.IsWritingText)
            focusedInput.ToggleTakingText();
    }

    private void QueueOptionRowRebuild()
    {
        BuildFilteredOptions();
        _hasBuiltOptionRows = true;

        _optionsList.Clear();
        _optionStatusText = null;
        _nextOptionBuildIndex = 0;
        _isBuildingOptionRows = false;

        if (_filteredOptions.Count == 0)
        {
            _optionsList.Add(new UIText("No matching entries.", ScaleText(0.64f)));
            _optionsList.Recalculate();
            return;
        }

        _isBuildingOptionRows = true;
        _optionStatusText = new UIText(string.Empty, ScaleText(0.62f));
        _optionsList.Add(_optionStatusText);
        UpdateOptionStatusLabel();

        BuildOptionRowsIncrementally();
        _optionsList.Recalculate();
    }

    private void BuildFilteredOptions()
    {
        if (_filterCache.TryGetValue(_searchTextLower, out List<CatalogPickerOption> cachedOptions))
        {
            SetFilteredOptionsForRendering(cachedOptions);
            return;
        }

        bool hasFilter = !string.IsNullOrWhiteSpace(_searchTextLower);
        HashSet<string> addedKeys = [];
        List<CatalogPickerOption> allMatchingOptions = [];

        for (int i = 0; i < _allOptions.Count; i++)
        {
            CatalogPickerOption option = _allOptions[i];
            if (IsPinnedEntityOption(option.Key))
                continue;

            if (hasFilter && !OptionMatchesSearch(option.Key, _searchTextLower))
                continue;

            if (!addedKeys.Add(option.Key))
                continue;

            allMatchingOptions.Add(option);
        }

        for (int i = 0; i < _allOptions.Count; i++)
        {
            CatalogPickerOption option = _allOptions[i];
            if (!IsPinnedEntityOption(option.Key) || !addedKeys.Add(option.Key))
                continue;

            allMatchingOptions.Add(option);
        }

        _filterCache[_searchTextLower] = allMatchingOptions;
        SetFilteredOptionsForRendering(allMatchingOptions);
    }

    private void BuildOptionRowsIncrementally()
    {
        if (!_isBuildingOptionRows)
            return;

        int endIndex = Math.Min(_nextOptionBuildIndex + OptionRowBuildBatchSize, _filteredOptions.Count);
        for (int i = _nextOptionBuildIndex; i < endIndex; i++)
            _optionsList.Add(CreateOptionButton(_filteredOptions[i]));

        _nextOptionBuildIndex = endIndex;
        _isBuildingOptionRows = _nextOptionBuildIndex < _filteredOptions.Count;

        UpdateOptionStatusLabel();
        _optionsList.Recalculate();
    }

    private FlatTextButton CreateOptionButton(CatalogPickerOption option)
    {
        if (_optionButtonCache.TryGetValue(option.Key, out FlatTextButton cachedButton))
        {
            cachedButton.SetText(FormatOptionLabel(option));
            return cachedButton;
        }

        FlatTextButton button = new(FormatOptionLabel(option), ScaleText(0.72f));
        button.Width.Set(0f, 1f);
        button.Height.Set(Scale(26f), 0f);
        button.OnLeftClick += (_, _) =>
        {
            if (!_selectedKeys.Add(option.Key))
                _selectedKeys.Remove(option.Key);

            button.SetText(FormatOptionLabel(option));
            UpdateSelectionFeedback();
        };

        _optionButtonCache[option.Key] = button;
        return button;
    }

    private bool OptionMatchesSearch(string optionKey, string searchTextLower)
    {
        if (string.IsNullOrEmpty(searchTextLower))
            return true;

        return _optionSearchTextByKey.TryGetValue(optionKey, out string normalizedOptionLabel)
            && normalizedOptionLabel.Contains(searchTextLower, StringComparison.Ordinal);
    }

    private void SetFilteredOptionsForRendering(IReadOnlyList<CatalogPickerOption> allMatchingOptions)
    {
        _filteredOptions.Clear();
        _totalFilteredOptionCount = allMatchingOptions.Count;

        int renderCount = Math.Min(allMatchingOptions.Count, MaxRenderedOptionsPerFilter);
        for (int i = 0; i < renderCount; i++)
            _filteredOptions.Add(allMatchingOptions[i]);
    }

    private void UpdateOptionStatusLabel()
    {
        if (_optionStatusText is null)
            return;

        if (_isBuildingOptionRows)
        {
            _optionStatusText.SetText($"Loading {_nextOptionBuildIndex}/{_filteredOptions.Count} entries...");
            return;
        }

        if (_totalFilteredOptionCount > _filteredOptions.Count)
        {
            _optionStatusText.SetText($"Showing first {_filteredOptions.Count}/{_totalFilteredOptionCount} entries (refine search for more)");
            return;
        }

        _optionStatusText.SetText($"Showing {_filteredOptions.Count} entries");
    }

    private static bool IsPinnedEntityOption(string key)
    {
        return string.Equals(key, "entity:player", StringComparison.Ordinal)
            || string.Equals(key, "entity:npc-all", StringComparison.Ordinal)
            || string.Equals(key, "entity:projectile-all", StringComparison.Ordinal);
    }

    private void ApplyScheduledSearchRefresh(bool force)
    {
        if (!_searchRefreshPending)
            return;

        if (!force && Environment.TickCount64 < _pendingSearchApplyTick)
            return;

        _searchRefreshPending = false;

        bool searchUnchanged = string.Equals(_searchText, _pendingSearchText, StringComparison.Ordinal);
        _searchText = _pendingSearchText;
        _searchTextLower = _pendingSearchTextLower;

        if (searchUnchanged && _hasBuiltOptionRows)
            return;

        QueueOptionRowRebuild();
    }

    private string FormatOptionLabel(in CatalogPickerOption option)
    {
        return _selectedKeys.Contains(option.Key)
            ? $"[x] {option.Label}"
            : $"[ ] {option.Label}";
    }

    private void UpdateSelectionFeedback()
    {
        List<CatalogPickerOption> selectedOptions = GetSelectedOptionsInDisplayOrder();
        RebuildSelectedList(selectedOptions);

        _addSelectedButton.SetText(_confirmButtonText);
        _addSelectedButton.BackgroundColor = selectedOptions.Count > 0
            ? SettingsPanelTheme.Positive
            : SettingsPanelTheme.ButtonBackground;
    }

    private void RebuildSelectedList(IReadOnlyList<CatalogPickerOption> selectedOptions)
    {
        _selectedList.Clear();

        if (selectedOptions.Count == 0)
        {
            UIText emptyText = new("No entries selected.", ScaleText(0.64f));
            _selectedList.Add(emptyText);
            _selectedList.Recalculate();
            return;
        }

        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];

            UIElement row = new();
            row.Width.Set(0f, 1f);
            row.Height.Set(Scale(22f), 0f);

            UIText text = new(option.Label, ScaleText(0.66f))
            {
                VAlign = 0.5f,
            };
            row.Append(text);

            _selectedList.Add(row);
        }

        _selectedList.Recalculate();
    }

    private List<CatalogPickerOption> GetSelectedOptionsInDisplayOrder()
    {
        List<CatalogPickerOption> selectedOptions = [];
        for (int i = 0; i < _allOptions.Count; i++)
        {
            CatalogPickerOption option = _allOptions[i];
            if (_selectedKeys.Contains(option.Key))
                selectedOptions.Add(option);
        }

        return selectedOptions;
    }

    private void OnAddSelectedPressed(UIMouseEvent evt, UIElement listeningElement)
    {
        List<CatalogPickerOption> selectedOptions = GetSelectedOptionsInDisplayOrder();
        if (selectedOptions.Count == 0)
            return;

        string groupName = _groupNameText;
        if (string.IsNullOrWhiteSpace(groupName))
            groupName = selectedOptions.Count == 1 ? selectedOptions[0].Label : "Custom Group";

        _onItemsSelected(selectedOptions, groupName);
        RequestClose();
    }

    private void ApplyResetState(CatalogPickerResetState resetState)
    {
        _selectedKeys.Clear();
        if (resetState.SelectedKeys is not null)
        {
            foreach (string selectedKey in resetState.SelectedKeys)
            {
                if (!string.IsNullOrWhiteSpace(selectedKey))
                    _selectedKeys.Add(selectedKey);
            }
        }

        _groupNameBar.SetContents(resetState.GroupName ?? string.Empty, forced: true);
        QueueOptionRowRebuild();
        UpdateSelectionFeedback();
    }

    private float Scale(float baselinePixels)
    {
        return SettingsPanelScale.Pixels(baselinePixels, _uiScale);
    }

    private float ScaleText(float baselineTextScale)
    {
        return SettingsPanelScale.Text(baselineTextScale, _uiScale);
    }

    public void EndSearchInput()
    {
        if (_searchBar.IsWritingText)
            _searchBar.ToggleTakingText();

        if (_groupNameBar.IsWritingText)
            _groupNameBar.ToggleTakingText();
    }

    private void RequestClose()
    {
        EndSearchInput();
        _onClose();
    }
}
