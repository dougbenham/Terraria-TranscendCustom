using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DougCustom;

public class Projectile : GlobalProjectile
{
    private int _type = -1;
    
    public override bool InstancePerEntity => true;

    public override bool PreAI(Terraria.Projectile projectile)
    {
        if (ShouldModifyProjectile(projectile))
        {
            _type = projectile.type;
            projectile.type = ProjectileID.ChlorophyteBullet;
        }

        return base.PreAI(projectile);
    }

    public override void PostAI(Terraria.Projectile projectile)
    {
        if (_type >= 0)
        {
            projectile.type = _type;
        }

        base.PostAI(projectile);
    }

    private static bool ShouldModifyProjectile(Terraria.Projectile projectile)
    {
        return Config.Instance.HomingProjectiles
               && projectile.owner == Main.myPlayer
               && projectile.friendly
               && projectile.damage > 0
               && projectile.aiStyle == 1;
    }
}