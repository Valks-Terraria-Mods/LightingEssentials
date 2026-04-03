namespace LightingEssentials;

public class EnemyLightGlobalNPC : GlobalNPC
{
    public override void PostAI(NPC npc)
    {
        if (!npc.active || !npc.CanBeChasedBy())
            return;

        if (!LightRuntime.TryGetEnemyLightColor(npc.type, out Vector3 lightColor))
            return;

        Lighting.AddLight(npc.Center, lightColor);
    }
}