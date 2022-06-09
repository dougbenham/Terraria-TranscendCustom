using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TranscendsCustomizations
{
    public class UseTimeCommand : ModCommand
    {
        public override CommandType Type => CommandType.Chat;

        public override string Command => "usetime";

        public override string Usage
            => "/usetime [number]";

        public override string Description
            => "Change the use time of the current equipped item";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // get item on cursor, if nothing there, get hotbar item
            var item = Main.mouseItem;
            if (item == null || item.type == ItemID.None)
            {
                item = caller.Player.HeldItem;
                if (item == null || item.type == ItemID.None)
                {
                    throw new UsageException("No item selected");
                }
            }

            if (args.Length < 1 || !int.TryParse(args[0], out var value))
            {
                Item.CustomUseTimes.Remove(item.type);
                Main.NewText($"Reset UseTime on {item.Name} to {Item.DefaultUseTimes[item.type]}");
            }
            else
            {
                Item.CustomUseTimes[item.type] = value;
                Main.NewText($"Set UseTime of {item.Name} to {value} (default is {Item.DefaultUseTimes[item.type]})");
            }
        }
    }
}