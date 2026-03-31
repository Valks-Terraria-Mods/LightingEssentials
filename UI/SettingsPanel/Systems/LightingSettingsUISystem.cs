using System.Collections.Generic;
using LightingEssentials.UI.SettingsPanel.State;
using Microsoft.Xna.Framework.Input;
using Terraria.UI;

namespace LightingEssentials.UI.SettingsPanel.Systems;

[Autoload(Side = ModSide.Client)]
internal sealed class LightingSettingsUISystem : ModSystem
{
    private UserInterface _settingsInterface;
    private LightingSettingsPanelState _settingsPanelState;
    private ModKeybind _togglePanelKeybind;
    private bool _visible;
    private GameTime _lastUpdateUiGameTime;

    /// <summary>
    /// Allocates client-side UI resources and registers the rebindable panel keybind.
    /// </summary>
    public override void Load()
    {
        if (Main.dedServ)
            return;

        // This registration is what exposes a true keybind entry in Terraria controls.
        _togglePanelKeybind = KeybindLoader.RegisterKeybind(Mod, "Lighting Settings Panel", "L");
        _settingsInterface = new UserInterface();

        _settingsPanelState = new LightingSettingsPanelState();
        _settingsPanelState.Activate();
    }

    /// <summary>
    /// Releases UI references when the mod unloads.
    /// </summary>
    public override void Unload()
    {
        _togglePanelKeybind = null;
        _settingsPanelState = null;
        _settingsInterface = null;
        _visible = false;
        _lastUpdateUiGameTime = null;
    }

    /// <summary>
    /// Handles keybind interactions and updates the panel when visible.
    /// </summary>
    /// <param name="gameTime">Current frame timing data.</param>
    public override void UpdateUI(GameTime gameTime)
    {
        _lastUpdateUiGameTime = gameTime;

        // Keep panel closed at menu time to avoid stale state and accidental input capture.
        if (Main.gameMenu)
        {
            HidePanel();
            return;
        }

        if (_togglePanelKeybind is not null && _togglePanelKeybind.JustPressed)
            TogglePanel();

        if (_visible && Main.keyState.IsKeyDown(Keys.Escape) && Main.oldKeyState.IsKeyUp(Keys.Escape))
            HidePanel();

        if (_visible && _settingsInterface is not null)
            _settingsInterface.Update(gameTime);
    }

    /// <summary>
    /// Draws the active panel by inserting a custom layer before mouse text.
    /// </summary>
    /// <param name="layers">Current mutable interface layer collection.</param>
    public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
    {
        if (!_visible || _settingsInterface is null || _lastUpdateUiGameTime is null)
            return;

        int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
        if (mouseTextIndex == -1)
            return;

        layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
            "LightingEssentials: Settings Panel",
            delegate
            {
                _settingsInterface.Draw(Main.spriteBatch, _lastUpdateUiGameTime);
                return true;
            },
            InterfaceScaleType.UI));
    }

    /// <summary>
    /// Toggles panel visibility based on current state.
    /// </summary>
    private void TogglePanel()
    {
        if (_visible)
            HidePanel();
        else
            ShowPanel();
    }

    /// <summary>
    /// Shows the settings panel.
    /// </summary>
    private void ShowPanel()
    {
        _visible = true;
        _settingsInterface?.SetState(_settingsPanelState);
    }

    /// <summary>
    /// Hides the settings panel.
    /// </summary>
    private void HidePanel()
    {
        _visible = false;
        _settingsInterface?.SetState(null);
    }

    /// <summary>
    /// Called by UI controls (titlebar close button) to close the panel through system state.
    /// </summary>
    internal void ClosePanelFromUi()
    {
        HidePanel();
    }
}
