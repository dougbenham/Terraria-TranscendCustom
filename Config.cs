using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace TranscendsCustomizations
{
    public class Config : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static Config Instance;

        [Label("Map Teleport")]
        [DefaultValue(true)]
        public bool MapTeleport;

        [Label("Perma Buffs")]
        [DefaultValue(false)]
        public bool PermaBuffs;

        [Label("No Potion Sickness")]
        [DefaultValue(false)]
        public bool NoPotionSickness;

        [Label("Demi-God Mode")]
        [DefaultValue(false)]
        public bool DemigodMode;

        [Label("Scaling Potions in Shop")]
        [DefaultValue(true)]
        public bool ScalingPotions;

        [Label("Infinite Ammo")]
        [DefaultValue(false)]
        public bool InfiniteAmmo;

        [Label("Infinite Mana")]
        [DefaultValue(false)]
        public bool InfiniteMana;

        [Label("Instant Respawn with Full Health")]
        [DefaultValue(true)]
        public bool InstantRespawn;

        [Label("Max Pick Speed")]
        [DefaultValue(true)]
        public bool MaxPickSpeed;

        [Label("Max Tile Speed")]
        [DefaultValue(true)]
        public bool MaxTileSpeed;

        [Label("Max Wall Speed")]
        [DefaultValue(true)]
        public bool MaxWallSpeed;

        [Label("Max Reach Range")]
        [DefaultValue(true)]
        public bool MaxReachRange;

        [Label("Max Crafting Range")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool MaxCraftingRange;

        [Label("Max Item Pickup Speed")]
        [DefaultValue(true)]
        [ReloadRequired]
        public bool MaxItemPickupSpeed;

        [Label("Max Item Pickup Range")]
        [DefaultValue(true)]
        public bool MaxItemPickupRange;

        [Label("Max Minions = 100")]
        [DefaultValue(true)]
        public bool MaxMinions;

        [Label("Max Turrets = 100")]
        [DefaultValue(true)]
        public bool MaxTurrets;

        [Label("Homing Projectiles")]
        [DefaultValue(true)]
        public bool HomingProjectiles;

        [Label("Max Life Allowed")]
        [DefaultValue(500)]
        [Range(500, 5000)]
        [ReloadRequired]
        public int MaxLifeAllowed;

        [Label("Max Mana Allowed")]
        [DefaultValue(200)]
        [Range(200, 5000)]
        [ReloadRequired]
        public int MaxManaAllowed;
    }
}