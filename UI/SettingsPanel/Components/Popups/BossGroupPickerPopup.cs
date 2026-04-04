using System;
using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.Components.Common;
using LightingEssentials.UI.SettingsPanel.Styling;
using Terraria.GameContent.UI.Elements;
using Terraria.Localization;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Components.Popups;

internal sealed class BossGroupPickerPopup : UIPanel
{
    private readonly IReadOnlyList<CatalogPickerOption> _bossOptions;
    private readonly IReadOnlyList<CatalogPickerOption> _targetTileGroupOptions;
    private readonly Action<IReadOnlyList<CatalogPickerOption>, IReadOnlyList<CatalogPickerOption>, string> _onItemsSelected;
    private readonly Action _onClose;
    private readonly float _uiScale;
    private readonly string _confirmButtonText;

    private readonly UISearchBar _groupNameBar;
    private UISearchBar _bossSearchBar;
    private UISearchBar _targetSearchBar;
    private UIList _bossList;
    private UIList _targetList;
    private UIList _selectedBossList;
    private UIList _selectedTargetList;
    private readonly FlatTextButton _confirmButton;

    private readonly HashSet<string> _selectedBossKeys = [];
    private readonly HashSet<string> _selectedTargetGroupKeys = [];
    private readonly Dictionary<string, FlatTextButton> _bossButtonsByKey = new(StringComparer.Ordinal);
    private readonly Dictionary<string, FlatTextButton> _targetButtonsByKey = new(StringComparer.Ordinal);

    private string _groupNameText = string.Empty;
    private string _bossSearchTextLower = string.Empty;
    private string _targetSearchTextLower = string.Empty;

    public BossGroupPickerPopup(
        string title,
        IReadOnlyList<CatalogPickerOption> bossOptions,
        IReadOnlyList<CatalogPickerOption> targetTileGroupOptions,
        Action<IReadOnlyList<CatalogPickerOption>, IReadOnlyList<CatalogPickerOption>, string> onItemsSelected,
        Action onClose,
        float uiScale,
        IReadOnlyCollection<string> initiallySelectedBossKeys = null,
        IReadOnlyCollection<string> initiallySelectedTargetGroupKeys = null,
        string initialGroupName = "",
        string confirmButtonText = "Save Group")
    {
        _bossOptions = bossOptions;
        _targetTileGroupOptions = targetTileGroupOptions;
        _onItemsSelected = onItemsSelected;
        _onClose = onClose;
        _uiScale = uiScale;
        _confirmButtonText = confirmButtonText;

        if (initiallySelectedBossKeys is not null)
            foreach (string selectedKey in initiallySelectedBossKeys)
                _selectedBossKeys.Add(selectedKey);

        if (initiallySelectedTargetGroupKeys is not null)
            foreach (string selectedKey in initiallySelectedTargetGroupKeys)
                _selectedTargetGroupKeys.Add(selectedKey);

        Width.Set(Scale(740f), 0f);
        Height.Set(Scale(560f), 0f);
        SetPadding(Scale(12f));

        BackgroundColor = SettingsPanelTheme.PanelBackground;
        BorderColor = Color.Transparent;

        UIText header = new(title, ScaleText(0.86f));
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

        LocalizedText groupNamePlaceholder = Language.GetOrRegister("Mods.LightingEssentials.UI.GroupNamePlaceholder", static () => "Group name...");
        _groupNameBar = new UISearchBar(groupNamePlaceholder, ScaleText(0.72f));
        _groupNameBar.Top.Set(Scale(34f), 0f);
        _groupNameBar.Width.Set(0f, 1f);
        _groupNameBar.Height.Set(Scale(28f), 0f);
        _groupNameBar.OnContentsChanged += OnGroupNameChanged;
        _groupNameBar.OnLeftClick += (_, _) => FocusInput(_groupNameBar, _bossSearchBar, _targetSearchBar);
        Append(_groupNameBar);

        float columnGap = Scale(10f);
        float columnWidth = (GetInnerWidth() - columnGap) / 2f;
        float contentTop = Scale(70f);

        BuildBossColumn(columnWidth, contentTop);
        BuildTargetColumn(columnWidth, contentTop, columnGap);

        _confirmButton = new FlatTextButton(confirmButtonText, ScaleText(0.72f))
        {
            HAlign = 1f,
            HoverStyleEnabled = false,
        };
        _confirmButton.Width.Set(Scale(124f), 0f);
        _confirmButton.Height.Set(Scale(24f), 0f);
        _confirmButton.Top.Set(-Scale(30f), 1f);
        _confirmButton.OnLeftClick += OnConfirmPressed;
        Append(_confirmButton);

        _groupNameBar.SetContents(initialGroupName ?? string.Empty, forced: true);
        _bossSearchBar.SetContents(string.Empty, forced: true);
        _targetSearchBar.SetContents(string.Empty, forced: true);

        RebuildBossList();
        RebuildTargetList();
        UpdateSelectionFeedback();
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

        Vector2 mousePosition = Main.MouseScreen;

        if (_groupNameBar.IsWritingText && !_groupNameBar.ContainsPoint(mousePosition))
            _groupNameBar.ToggleTakingText();

        if (_bossSearchBar.IsWritingText && !_bossSearchBar.ContainsPoint(mousePosition))
            _bossSearchBar.ToggleTakingText();

        if (_targetSearchBar.IsWritingText && !_targetSearchBar.ContainsPoint(mousePosition))
            _targetSearchBar.ToggleTakingText();
    }

    public void EndInput()
    {
        if (_groupNameBar.IsWritingText)
            _groupNameBar.ToggleTakingText();

        if (_bossSearchBar.IsWritingText)
            _bossSearchBar.ToggleTakingText();

        if (_targetSearchBar.IsWritingText)
            _targetSearchBar.ToggleTakingText();
    }

    private void BuildBossColumn(float columnWidth, float top)
    {
        UIElement column = new();
        column.Top.Set(top, 0f);
        column.Width.Set(columnWidth, 0f);
        column.Height.Set(Scale(426f), 0f);
        Append(column);

        UIText title = new("Boss Members", ScaleText(0.72f));
        column.Append(title);

        LocalizedText searchPlaceholder = Language.GetOrRegister("Mods.LightingEssentials.UI.SearchPlaceholder", static () => "Search...");
        _bossSearchBar = new UISearchBar(searchPlaceholder, ScaleText(0.72f));
        _bossSearchBar.Top.Set(Scale(24f), 0f);
        _bossSearchBar.Width.Set(0f, 1f);
        _bossSearchBar.Height.Set(Scale(28f), 0f);
        _bossSearchBar.OnContentsChanged += OnBossSearchChanged;
        _bossSearchBar.OnLeftClick += (_, _) => FocusInput(_bossSearchBar, _groupNameBar, _targetSearchBar);
        column.Append(_bossSearchBar);

        UIText availableHeader = new("Available", ScaleText(0.64f));
        availableHeader.Top.Set(Scale(56f), 0f);
        column.Append(availableHeader);

        UIPanel listPanel = new();
        listPanel.Top.Set(Scale(74f), 0f);
        listPanel.Width.Set(0f, 1f);
        listPanel.Height.Set(Scale(236f), 0f);
        listPanel.SetPadding(Scale(8f));
        listPanel.BackgroundColor = new Color(14, 14, 14, 150);
        listPanel.BorderColor = SettingsPanelTheme.RowBorder;
        column.Append(listPanel);

        float scaledScrollbarWidth = Scale(20f);
        float scaledScrollbarGap = Scale(6f);

        _bossList = new UIList
        {
            ListPadding = Scale(4f),
            ManualSortMethod = static _ => { }
        };
        _bossList.Width.Set(-(scaledScrollbarWidth + scaledScrollbarGap), 1f);
        _bossList.Height.Set(0f, 1f);
        listPanel.Append(_bossList);

        UIScrollbar scrollbar = new DarkScrollbar
        {
            HAlign = 1f,
        };
        scrollbar.Width.Set(scaledScrollbarWidth, 0f);
        scrollbar.Height.Set(0f, 1f);
        listPanel.Append(scrollbar);
        _bossList.SetScrollbar(scrollbar);

        UIText selectedHeader = new("Selected Bosses", ScaleText(0.64f));
        selectedHeader.Top.Set(Scale(318f), 0f);
        column.Append(selectedHeader);

        UIPanel selectedPanel = new();
        selectedPanel.Top.Set(Scale(336f), 0f);
        selectedPanel.Width.Set(0f, 1f);
        selectedPanel.Height.Set(Scale(84f), 0f);
        selectedPanel.SetPadding(Scale(8f));
        selectedPanel.BackgroundColor = new Color(14, 14, 14, 150);
        selectedPanel.BorderColor = SettingsPanelTheme.RowBorder;
        column.Append(selectedPanel);

        _selectedBossList = new UIList
        {
            ListPadding = Scale(3f),
            ManualSortMethod = static _ => { }
        };
        _selectedBossList.Width.Set(-(scaledScrollbarWidth + scaledScrollbarGap), 1f);
        _selectedBossList.Height.Set(0f, 1f);
        selectedPanel.Append(_selectedBossList);

        UIScrollbar selectedScrollbar = new DarkScrollbar
        {
            HAlign = 1f,
        };
        selectedScrollbar.Width.Set(scaledScrollbarWidth, 0f);
        selectedScrollbar.Height.Set(0f, 1f);
        selectedPanel.Append(selectedScrollbar);
        _selectedBossList.SetScrollbar(selectedScrollbar);
    }

    private void BuildTargetColumn(float columnWidth, float top, float columnGap)
    {
        UIElement column = new();
        column.Left.Set(columnWidth + columnGap, 0f);
        column.Top.Set(top, 0f);
        column.Width.Set(columnWidth, 0f);
        column.Height.Set(Scale(426f), 0f);
        Append(column);

        UIText title = new("Target Tile Groups", ScaleText(0.72f));
        column.Append(title);

        LocalizedText searchPlaceholder = Language.GetOrRegister("Mods.LightingEssentials.UI.SearchPlaceholder", static () => "Search...");
        _targetSearchBar = new UISearchBar(searchPlaceholder, ScaleText(0.72f));
        _targetSearchBar.Top.Set(Scale(24f), 0f);
        _targetSearchBar.Width.Set(0f, 1f);
        _targetSearchBar.Height.Set(Scale(28f), 0f);
        _targetSearchBar.OnContentsChanged += OnTargetSearchChanged;
        _targetSearchBar.OnLeftClick += (_, _) => FocusInput(_targetSearchBar, _groupNameBar, _bossSearchBar);
        column.Append(_targetSearchBar);

        UIText availableHeader = new("Available", ScaleText(0.64f));
        availableHeader.Top.Set(Scale(56f), 0f);
        column.Append(availableHeader);

        UIPanel listPanel = new();
        listPanel.Top.Set(Scale(74f), 0f);
        listPanel.Width.Set(0f, 1f);
        listPanel.Height.Set(Scale(236f), 0f);
        listPanel.SetPadding(Scale(8f));
        listPanel.BackgroundColor = new Color(14, 14, 14, 150);
        listPanel.BorderColor = SettingsPanelTheme.RowBorder;
        column.Append(listPanel);

        float scaledScrollbarWidth = Scale(20f);
        float scaledScrollbarGap = Scale(6f);

        _targetList = new UIList
        {
            ListPadding = Scale(4f),
            ManualSortMethod = static _ => { }
        };
        _targetList.Width.Set(-(scaledScrollbarWidth + scaledScrollbarGap), 1f);
        _targetList.Height.Set(0f, 1f);
        listPanel.Append(_targetList);

        UIScrollbar scrollbar = new DarkScrollbar
        {
            HAlign = 1f,
        };
        scrollbar.Width.Set(scaledScrollbarWidth, 0f);
        scrollbar.Height.Set(0f, 1f);
        listPanel.Append(scrollbar);
        _targetList.SetScrollbar(scrollbar);

        UIText selectedHeader = new("Selected Tile Groups", ScaleText(0.64f));
        selectedHeader.Top.Set(Scale(318f), 0f);
        column.Append(selectedHeader);

        UIPanel selectedPanel = new();
        selectedPanel.Top.Set(Scale(336f), 0f);
        selectedPanel.Width.Set(0f, 1f);
        selectedPanel.Height.Set(Scale(84f), 0f);
        selectedPanel.SetPadding(Scale(8f));
        selectedPanel.BackgroundColor = new Color(14, 14, 14, 150);
        selectedPanel.BorderColor = SettingsPanelTheme.RowBorder;
        column.Append(selectedPanel);

        _selectedTargetList = new UIList
        {
            ListPadding = Scale(3f),
            ManualSortMethod = static _ => { }
        };
        _selectedTargetList.Width.Set(-(scaledScrollbarWidth + scaledScrollbarGap), 1f);
        _selectedTargetList.Height.Set(0f, 1f);
        selectedPanel.Append(_selectedTargetList);

        UIScrollbar selectedScrollbar = new DarkScrollbar
        {
            HAlign = 1f,
        };
        selectedScrollbar.Width.Set(scaledScrollbarWidth, 0f);
        selectedScrollbar.Height.Set(0f, 1f);
        selectedPanel.Append(selectedScrollbar);
        _selectedTargetList.SetScrollbar(selectedScrollbar);
    }

    private float GetInnerWidth()
    {
        float contentWidth = Scale(740f) - (Scale(12f) * 2f);
        return contentWidth;
    }

    private void OnGroupNameChanged(string value)
    {
        _groupNameText = value?.Trim() ?? string.Empty;
    }

    private void OnBossSearchChanged(string value)
    {
        _bossSearchTextLower = (value ?? string.Empty).Trim().ToLowerInvariant();
        RebuildBossList();
    }

    private void OnTargetSearchChanged(string value)
    {
        _targetSearchTextLower = (value ?? string.Empty).Trim().ToLowerInvariant();
        RebuildTargetList();
    }

    private void RebuildBossList()
    {
        _bossList.Clear();

        int renderedCount = 0;
        for (int i = 0; i < _bossOptions.Count; i++)
        {
            CatalogPickerOption option = _bossOptions[i];
            if (!MatchesSearch(option.Label, _bossSearchTextLower))
                continue;

            _bossList.Add(CreateBossOptionButton(option));
            renderedCount++;
        }

        if (renderedCount == 0)
            _bossList.Add(new UIText("No matching bosses.", ScaleText(0.64f)));

        _bossList.Recalculate();
    }

    private void RebuildTargetList()
    {
        _targetList.Clear();

        int renderedCount = 0;
        for (int i = 0; i < _targetTileGroupOptions.Count; i++)
        {
            CatalogPickerOption option = _targetTileGroupOptions[i];
            if (!MatchesSearch(option.Label, _targetSearchTextLower))
                continue;

            _targetList.Add(CreateTargetOptionButton(option));
            renderedCount++;
        }

        if (renderedCount == 0)
            _targetList.Add(new UIText("No matching tile groups.", ScaleText(0.64f)));

        _targetList.Recalculate();
    }

    private FlatTextButton CreateBossOptionButton(CatalogPickerOption option)
    {
        if (_bossButtonsByKey.TryGetValue(option.Key, out FlatTextButton existingButton))
        {
            existingButton.SetText(FormatOptionLabel(option.Label, _selectedBossKeys.Contains(option.Key)));
            return existingButton;
        }

        FlatTextButton button = new(FormatOptionLabel(option.Label, _selectedBossKeys.Contains(option.Key)), ScaleText(0.70f));
        button.Width.Set(0f, 1f);
        button.Height.Set(Scale(26f), 0f);
        button.OnLeftClick += (_, _) =>
        {
            if (!_selectedBossKeys.Add(option.Key))
                _selectedBossKeys.Remove(option.Key);

            button.SetText(FormatOptionLabel(option.Label, _selectedBossKeys.Contains(option.Key)));
            UpdateSelectionFeedback();
        };

        _bossButtonsByKey[option.Key] = button;
        return button;
    }

    private FlatTextButton CreateTargetOptionButton(CatalogPickerOption option)
    {
        if (_targetButtonsByKey.TryGetValue(option.Key, out FlatTextButton existingButton))
        {
            existingButton.SetText(FormatOptionLabel(option.Label, _selectedTargetGroupKeys.Contains(option.Key)));
            return existingButton;
        }

        FlatTextButton button = new(FormatOptionLabel(option.Label, _selectedTargetGroupKeys.Contains(option.Key)), ScaleText(0.70f));
        button.Width.Set(0f, 1f);
        button.Height.Set(Scale(26f), 0f);
        button.OnLeftClick += (_, _) =>
        {
            if (!_selectedTargetGroupKeys.Add(option.Key))
                _selectedTargetGroupKeys.Remove(option.Key);

            button.SetText(FormatOptionLabel(option.Label, _selectedTargetGroupKeys.Contains(option.Key)));
            UpdateSelectionFeedback();
        };

        _targetButtonsByKey[option.Key] = button;
        return button;
    }

    private void UpdateSelectionFeedback()
    {
        List<CatalogPickerOption> selectedBossOptions = GetSelectedOptionsInDisplayOrder(_bossOptions, _selectedBossKeys);
        List<CatalogPickerOption> selectedTargetOptions = GetSelectedOptionsInDisplayOrder(_targetTileGroupOptions, _selectedTargetGroupKeys);

        RebuildSelectedList(_selectedBossList, selectedBossOptions, "No bosses selected.");
        RebuildSelectedList(_selectedTargetList, selectedTargetOptions, "No tile groups selected.");
        _confirmButton.SetText(_confirmButtonText);

        bool canConfirm = selectedBossOptions.Count > 0 && selectedTargetOptions.Count > 0;
        _confirmButton.BackgroundColor = canConfirm
            ? SettingsPanelTheme.Positive
            : SettingsPanelTheme.ButtonBackground;
    }

    private void RebuildSelectedList(UIList list, IReadOnlyList<CatalogPickerOption> selectedOptions, string emptyText)
    {
        list.Clear();

        if (selectedOptions.Count == 0)
        {
            list.Add(new UIText(emptyText, ScaleText(0.64f)));
            list.Recalculate();
            return;
        }

        for (int i = 0; i < selectedOptions.Count; i++)
        {
            CatalogPickerOption option = selectedOptions[i];

            UIElement row = new();
            row.Width.Set(0f, 1f);
            row.Height.Set(Scale(22f), 0f);

            UIText label = new(option.Label, ScaleText(0.66f))
            {
                VAlign = 0.5f,
            };

            row.Append(label);
            list.Add(row);
        }

        list.Recalculate();
    }

    private static string FormatOptionLabel(string label, bool isSelected)
    {
        return isSelected
            ? $"[x] {label}"
            : $"[ ] {label}";
    }

    private static bool MatchesSearch(string label, string searchTextLower)
    {
        if (string.IsNullOrWhiteSpace(searchTextLower))
            return true;

        return label?.Contains(searchTextLower, StringComparison.OrdinalIgnoreCase) == true;
    }

    private List<CatalogPickerOption> GetSelectedOptionsInDisplayOrder(IReadOnlyList<CatalogPickerOption> options, HashSet<string> selectedKeys)
    {
        List<CatalogPickerOption> selected = [];

        for (int i = 0; i < options.Count; i++)
        {
            CatalogPickerOption option = options[i];
            if (selectedKeys.Contains(option.Key))
                selected.Add(option);
        }

        return selected;
    }

    private void OnConfirmPressed(UIMouseEvent evt, UIElement listeningElement)
    {
        List<CatalogPickerOption> selectedBossOptions = GetSelectedOptionsInDisplayOrder(_bossOptions, _selectedBossKeys);
        List<CatalogPickerOption> selectedTargetOptions = GetSelectedOptionsInDisplayOrder(_targetTileGroupOptions, _selectedTargetGroupKeys);

        if (selectedBossOptions.Count == 0 || selectedTargetOptions.Count == 0)
            return;

        string groupName = _groupNameText;
        if (string.IsNullOrWhiteSpace(groupName))
            groupName = selectedBossOptions.Count == 1 ? selectedBossOptions[0].Label : "Boss Group";

        _onItemsSelected(selectedBossOptions, selectedTargetOptions, groupName);
        RequestClose();
    }

    private void FocusInput(UISearchBar focusedInput, UISearchBar firstOther, UISearchBar secondOther)
    {
        if (firstOther.IsWritingText)
            firstOther.ToggleTakingText();

        if (secondOther.IsWritingText)
            secondOther.ToggleTakingText();

        if (!focusedInput.IsWritingText)
            focusedInput.ToggleTakingText();
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
