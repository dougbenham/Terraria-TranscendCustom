using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace TranscendsCustomizations
{
    public class CustomItem : GlobalItem
    {
        public static readonly ConcurrentDictionary<int, int> DefaultUseTimes = new();
        public static ConcurrentDictionary<int, int> CustomUseTimes = new();

        /// <inheritdoc />
        public override void GrabRange(Item item, Player player, ref int itemGrabRange)
        {
	        if (Config.Instance.MaxItemPickupSpeed)
	        {
                if (Config.Instance.MaxItemPickupRange || 
                    (new Rectangle((int)player.position.X - itemGrabRange, (int)player.position.Y - itemGrabRange, player.width + itemGrabRange * 2, player.height + itemGrabRange * 2).Intersects(item.Hitbox)))
		            item.position = player.position;
	        }
        }

        public override void SetDefaults(Terraria.Item item)
        {
            if (!DefaultUseTimes.ContainsKey(item.type))
				DefaultUseTimes[item.type] = item.useTime;

            SetUseTime(item);
        }
        
        internal static void SetUseTime(Terraria.Item item)
        {
            if (CustomUseTimes.TryGetValue(item.type, out var v))
                item.useTime = v;

            else if (Config.Instance.MaxPickSpeed && (item.axe > 0 ||
                                                      item.pick > 0 ||
                                                      item.hammer > 0))
                item.useTime = 1;

            else if (Config.Instance.MaxTileSpeed && item.createTile >= 0)
                item.useTime = 1;

            else if (Config.Instance.MaxWallSpeed && item.createWall >= 0)
                item.useTime = 1;

            else if (DefaultUseTimes.TryGetValue(item.type, out var d))
                item.useTime = d;
        }
    }
}