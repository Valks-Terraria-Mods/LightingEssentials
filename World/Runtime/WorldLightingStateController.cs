namespace LightingEssentials;

internal static class WorldLightingStateController
{
    private static WorldLightingState _currentState;
    private static bool _hasState;
    private static int _pendingBossRefreshFrames;

    public static WorldLightingState CurrentState => _hasState ? _currentState : WorldLightingStateCapture.Capture();

    public static void OnWorldLoad()
    {
        SetState(WorldLightingStateCapture.Capture(), forceRefresh: true);
    }

    public static void ClearWorld()
    {
        _hasState = false;
        _currentState = default;
        _pendingBossRefreshFrames = 0;
    }

    public static void NetReceive()
    {
        RefreshFromWorld();
    }

    public static void PreUpdateTime()
    {
        // Event start/end (blood moon, eclipse, invasions) can flip without kill hooks,
        // so we check lightweight event flags once per tick only when event effects are enabled.
        if (!_hasState)
        {
            SetState(WorldLightingStateCapture.Capture(), forceRefresh: true);
            return;
        }

        Config config = LightingEssentials.Config;
        bool trackEventFlags = LightRuntime.ModEnabled
            && config is not null
            && (config.BloodMoonEventEffects || config.SolarEclipseEventEffects || config.FrostLegionEventEffects);

        if (!trackEventFlags)
        {
            ProcessPendingBossRefresh();
            return;
        }

        WorldLightingFlags eventFlags = WorldLightingStateCapture.CaptureEventFlags();
        WorldLightingFlags currentEventFlags = _currentState.Flags & WorldLightingState.EventMask;
        if (eventFlags == currentEventFlags)
        {
            ProcessPendingBossRefresh();
            return;
        }

        SetState(_currentState.WithEventFlags(eventFlags));
        ProcessPendingBossRefresh();
    }

    public static void NotifyTrackedBossKill(NPC npc)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        if (!TrackedBossNpcTypes.Contains(npc.type))
            return;

        // Downed flags are not guaranteed to be updated the same tick OnKill runs.
        _pendingBossRefreshFrames = System.Math.Max(_pendingBossRefreshFrames, 2);
    }

    private static void ProcessPendingBossRefresh()
    {
        if (_pendingBossRefreshFrames <= 0)
            return;

        _pendingBossRefreshFrames--;
        if (_pendingBossRefreshFrames == 0)
        {
            RefreshFromWorld();
        }
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