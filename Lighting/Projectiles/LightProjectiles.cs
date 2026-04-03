namespace LightingEssentials;

public class LightProjectiles : GlobalProjectile
{
    public override void PostAI(Projectile projectile)
    {
        if (!projectile.active)
            return;

        if (!LightRuntime.TryGetProjectileLightColor(projectile.type, out Vector3 lightColor))
            return;

        Lighting.AddLight(projectile.Center, lightColor);
    }
}
