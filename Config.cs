using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace DougCustom;

public class Config : ModConfig
{
    public override ConfigScope Mode => ConfigScope.ServerSide;

    public static Config Instance;
    
    [Label("Map Teleport")] [DefaultValue(true)]
    public bool MapTeleport;
    
    [Label("Perma Buffs")] [DefaultValue(false)]
    public bool PermaBuffs;
    
    [Label("No Potion Sickness")] [DefaultValue(false)]
    public bool NoPotionSickness;
    
    [Label("Demi-God Mode")] [DefaultValue(false)]
    public bool DemigodMode;
    
    [Label("Scaling Potions in Shop")] [DefaultValue(true)]
    public bool ScalingPotions;

    [Label("Infinite Ammo")] [DefaultValue(false)]
    public bool InfiniteAmmo;

    [Label("Infinite Mana")] [DefaultValue(false)]
    public bool InfiniteMana;
    
    [Label("Instant Respawn")] [DefaultValue(true)]
    public bool InstantRespawn;

    [Label("Max Pick Speed")] [DefaultValue(true)]
    public bool MaxPickSpeed;

    [Label("Max Tile Speed")] [DefaultValue(true)]
    public bool MaxTileSpeed;

    [Label("Max Wall Speed")] [DefaultValue(true)]
    public bool MaxWallSpeed;

    [Label("Max Reach Range")] [DefaultValue(true)]
    public bool MaxReachRange;

    [Label("Max Item Pickup Range")] [DefaultValue(true)]
    public bool MaxItemPickupRange;

    [Label("Max Minions = 100")] [DefaultValue(true)]
    public bool MaxMinions;

    [Label("Max Turrets = 100")] [DefaultValue(true)]
    public bool MaxTurrets;

    [Label("Homing Projectiles")] [DefaultValue(true)]
    public bool HomingProjectiles;
}