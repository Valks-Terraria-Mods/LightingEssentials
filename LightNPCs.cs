namespace LightingEssentials;

public class LightNPCs : GlobalNPC
{
    private float red = 0f;

    public override bool InstancePerEntity
    {
        get => true;
    }

    public override void DrawEffects(NPC npc, ref Color drawColor)
    {
        if (LightingEssentials.Config.Experimental)
        {
            if (npc.type == NPCID.EyeofCthulhu)
            {
                for (int x = 0; x < 10; x++)
                {
                    for (int y = 0; y < 10; y++)
                    {
                        Lighting.AddLight(npc.position + new Vector2(16 * x, 16 * y), new Vector3(1.0f, 0, 0));
                    }
                }
            }
        }

        if (LightingEssentials.Config.EntityRedHitLight)
        {
            if (red > 0.0f)
            {
                Lighting.AddLight(npc.position, new Vector3(red, 0, 0));
                red -= 0.01f;
            }
        }
    }

    public override void HitEffect(NPC npc, NPC.HitInfo hit)
    {
        if (!LightingEssentials.Config.EntityRedHitLight)
        {
            return;
        }

        float ratio = (float) npc.life / npc.lifeMax;
        red = (ratio - 1) * -1;
    }
}
