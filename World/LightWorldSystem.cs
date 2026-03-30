namespace LightingEssentials;

public class LightWorldSystem : ModSystem
{
    private BossProgressionState _lastProgression;

    public override void OnWorldLoad()
    {
        _lastProgression = BossProgressionState.Capture();
        LightTiles.InitLight();
    }

    public override void OnWorldUnload()
    {
        _lastProgression = default;
    }

    public override void PostUpdateWorld()
    {
        if (!LightRuntime.ModEnabled)
            return;

        BossProgressionState currentProgression = BossProgressionState.Capture();
        if (currentProgression == _lastProgression)
            return;

        _lastProgression = currentProgression;
        LightTiles.InitLight();
    }
}
