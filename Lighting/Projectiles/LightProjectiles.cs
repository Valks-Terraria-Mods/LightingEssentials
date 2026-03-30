namespace LightingEssentials;

public class LightProjectiles : GlobalProjectile
{
    public override void PostAI(Projectile projectile)
    {
        if (!LightRuntime.ProjectileLightEnabled)
            return;

        if (!projectile.active)
            return;

        Lighting.AddLight(projectile.Center, LightRuntime.ProjectileLightColor);
    }
}
