using System.IO;

namespace LightingEssentials;

public class LightWorldSystem : ModSystem
{
    private static WorldLightingState _currentState;
    private static bool _hasState;
    private static int _pendingBossRefreshFrames;

    public static WorldLightingState CurrentState => _hasState ? _currentState : WorldLightingState.Capture();

    public override void OnWorldLoad()
    {
        SetState(WorldLightingState.Capture(), forceRefresh: true);
    }

    public override void ClearWorld()
    {
        _hasState = false;
        _currentState = default;
        _pendingBossRefreshFrames = 0;
    }

    public override void OnWorldUnload()
    {
        ClearWorld();
    }

    public override void NetReceive(BinaryReader reader)
    {
        RefreshFromWorld();
    }

    public override void NetSend(BinaryWriter writer)
    {
    }

    public override void PreUpdateTime()
    {
        // Event start/end (blood moon, eclipse, invasions) can flip without kill hooks,
        // so we check lightweight event flags once per tick only when event effects are enabled.
        if (!_hasState)
        {
            SetState(WorldLightingState.Capture(), forceRefresh: true);
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

        WorldLightingFlags eventFlags = WorldLightingState.CaptureEventFlags();
        WorldLightingFlags currentEventFlags = _currentState.Flags & WorldLightingState.EventMask;
        if (eventFlags == currentEventFlags)
        {
            ProcessPendingBossRefresh();
            return;
        }

        SetState(_currentState.WithEventFlags(eventFlags));
        ProcessPendingBossRefresh();
    }

    public static WorldLightingState GetCurrentState()
    {
        return CurrentState;
    }

    internal static void NotifyTrackedBossKill(NPC npc)
    {
        if (Main.netMode == NetmodeID.MultiplayerClient)
            return;

        if (!IsTrackedBossNpcType(npc.type))
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
        SetState(WorldLightingState.Capture());
    }

    private static void SetState(WorldLightingState state, bool forceRefresh = false)
    {
        if (!forceRefresh && _hasState && state == _currentState)
            return;

        _currentState = state;
        _hasState = true;
        LightTiles.InitLight();
    }

    private static bool IsTrackedBossNpcType(int npcType)
    {
        return npcType switch
        {
            NPCID.KingSlime => true,
            NPCID.EyeofCthulhu => true,
            NPCID.EaterofWorldsHead => true,
            NPCID.BrainofCthulhu => true,
            NPCID.QueenBee => true,
            NPCID.SkeletronHead => true,
            NPCID.Deerclops => true,
            NPCID.WallofFlesh => true,
            NPCID.QueenSlimeBoss => true,
            NPCID.TheDestroyer => true,
            NPCID.Retinazer => true,
            NPCID.Spazmatism => true,
            NPCID.SkeletronPrime => true,
            NPCID.Plantera => true,
            NPCID.Golem => true,
            NPCID.DukeFishron => true,
            NPCID.HallowBoss => true,
            NPCID.CultistBoss => true,
            NPCID.MoonLordCore => true,
            _ => false,
        };
    }
}
