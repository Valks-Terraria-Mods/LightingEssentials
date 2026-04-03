namespace LightingEssentials;

internal static class WorldLightingStateController
{
    private static WorldLightingState _currentState;
    private static bool _hasState;

    public static WorldLightingState CurrentState => _hasState ? _currentState : WorldLightingStateCapture.Capture();

    public static void OnWorldLoad()
    {
        SetState(WorldLightingStateCapture.Capture(), forceRefresh: true);
    }

    public static void ClearWorld()
    {
        _hasState = false;
        _currentState = default;
    }

    public static void NetReceive()
    {
        RefreshFromWorld();
    }

    public static void PreUpdateTime()
    {
        if (!_hasState)
        {
            SetState(WorldLightingStateCapture.Capture(), forceRefresh: true);
            return;
        }

        SetState(WorldLightingStateCapture.Capture());
    }

    public static void NotifyTrackedBossKill(NPC npc)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        SetState(WorldLightingStateCapture.Capture());
    }

    private static void RefreshFromWorld()
    {
        SetState(WorldLightingStateCapture.Capture());
    }

    private static void SetState(WorldLightingState state, bool forceRefresh = false)
    {
        if (!forceRefresh && _hasState && state == _currentState)
            return;

        _currentState = state;
        _hasState = true;
        LightTiles.InitLight();
    }
}