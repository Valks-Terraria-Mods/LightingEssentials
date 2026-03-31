namespace LightingEssentials;

public class EnemyLightGlobalNPC : GlobalNPC
{
    public override void PostAI(NPC npc)
    {
        if (!LightRuntime.EnemyLightEnabled)
            return;

        if (!npc.active || !npc.CanBeChasedBy())
            return;

        Lighting.AddLight(npc.Center, LightRuntime.EnemyLightColor);
    }
}