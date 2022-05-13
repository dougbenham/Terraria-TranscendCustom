using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace DougCustom;

public class Player : ModPlayer
{
    private static readonly int _defaultTileRangeX = Terraria.Player.tileRangeX;
    private static readonly int _defaultTileRangeY = Terraria.Player.tileRangeY;
    private static readonly int _defaultItemGrabRange = Terraria.Player.defaultItemGrabRange;

    public Player()
    {
        var itemGrabSpeedField = typeof(Terraria.Player).GetField("itemGrabSpeed", BindingFlags.NonPublic | BindingFlags.Static);
        if (itemGrabSpeedField != null)
            itemGrabSpeedField.SetValue(null, 10f);
    }

    public override void PreUpdate()
    {
        if (Config.Instance.PermaBuffs)
        {
            Player.AddBuff(BuffID.Dangersense, 2);
            Player.AddBuff(BuffID.Hunter, 2);
            Player.AddBuff(BuffID.NightOwl, 2);
            Player.AddBuff(BuffID.Spelunker, 2);
        }

        if (Config.Instance.NoPotionSickness)
        {
            Player.potionDelay = 0;

            var i = Player.FindBuffIndex(BuffID.PotionSickness);
            if (i >= 0)
                Player.DelBuff(i);
        }
    }
    
    public override void ProcessTriggers(TriggersSet triggersSet)
    {
        if (DougCustom.CrazySpawnsKeybind.JustPressed)
        {
            NPC.FarmingMode = !NPC.FarmingMode;
            Main.NewText("Farming mode is " + (NPC.FarmingMode ? "on" : "off"));
        }

        if (DougCustom.TeleportKeybind.JustPressed)
        {
            var vector = new Vector2(Main.mouseX + Main.screenPosition.X, Main.mouseY + Main.screenPosition.Y);
            Player.Teleport(vector, 1, 0);
            Player.velocity = Vector2.Zero;
            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, Player.whoAmI, vector.X, vector.Y, 1, 0, 0);
        }
    }
    
    public override void ResetEffects()
    {
        SetBlockRange();
    }

    public override void PostUpdateBuffs()
    {
        SetBlockRange();
    }

    private void SetBlockRange()
    {
        Terraria.Player.tileRangeX = Config.Instance.MaxReachRange ? 100 : _defaultTileRangeX;
        Terraria.Player.tileRangeY = Config.Instance.MaxReachRange ? 100 : _defaultTileRangeY;
        Terraria.Player.defaultItemGrabRange = Config.Instance.MaxItemPickupRange ? 1000 : _defaultItemGrabRange;
    }

    public override void UpdateEquips()
    {
        if (Config.Instance.MaxMinions)
            Player.maxMinions = 100;
        if (Config.Instance.MaxTurrets)
            Player.maxTurrets = 100;
    }
    
    public override void ModifyManaCost(Terraria.Item item, ref float reduce, ref float mult)
    {
        if (Config.Instance.InfiniteMana)
            mult = 0;
    }

    public override bool CanConsumeAmmo(Terraria.Item weapon, Terraria.Item ammo)
    {
        return !Config.Instance.InfiniteAmmo;
    }

    public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
    {
        return !Config.Instance.DemigodMode;
    }

    public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
    {
        if (Config.Instance.InstantRespawn)
            Player.respawnTimer = 0;
    }

    public override void OnRespawn(Terraria.Player player)
    {
        if (Config.Instance.InstantRespawn)
            player.statLife = player.statLifeMax;
    }

    public override void SaveData(TagCompound tag)
    {
        tag.Add(nameof(Item.CustomUseTimes) + "_Keys", Item.CustomUseTimes.Keys.ToList());
        tag.Add(nameof(Item.CustomUseTimes) + "_Values", Item.CustomUseTimes.Values.ToList());
    }

    public override void LoadData(TagCompound tag)
    {
        try
        {
            var keys = tag.Get<List<int>>(nameof(Item.CustomUseTimes) + "_Keys");
            var values = tag.Get<List<int>>(nameof(Item.CustomUseTimes) + "_Values");
            Item.CustomUseTimes = new(Enumerable.Zip(keys, values, KeyValuePair.Create));
        }
        catch (Exception e)
        {
            base.Mod.Logger.WarnFormat("DougCustom.Player.LoadData() - WARNING! Could not load CustomUseTimes. Exception: {0}", e.Message);
        }
    }
}