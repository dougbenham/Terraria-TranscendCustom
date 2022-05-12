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
               && projectile.type != ProjectileID.LunarFlare
               && projectile.type != ProjectileID.NebulaBlaze1
               && projectile.type != ProjectileID.NebulaBlaze2
               && projectile.type != ProjectileID.ChlorophyteBullet
               && projectile.type != ProjectileID.VortexBeaterRocket
               && projectile.type != ProjectileID.PygmySpear
               && projectile.type != ProjectileID.MiniRetinaLaser
               && projectile.type != ProjectileID.ElectrosphereMissile
               && projectile.type != ProjectileID.Meteor1
               && projectile.type != ProjectileID.Meteor2
               && projectile.type != ProjectileID.Meteor3
               && projectile.type != ProjectileID.MoonlordArrow
               && projectile.type != ProjectileID.MoonlordArrowTrail
               && projectile.type != ProjectileID.MiniSharkron;
    }
}