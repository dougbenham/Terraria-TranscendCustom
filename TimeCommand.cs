using Terraria;
using Terraria.ModLoader;

namespace DougCustom;

public class TimeCommand : ModCommand
{
    public override CommandType Type => CommandType.Chat;

    public override string Command => "Time";

    public override string Usage => "/time dawn\n/time noon\n/time midnight\n/time dusk";

    public override string Description => "Sets the time";

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        if (args.Length == 0)
            return;

        switch (args[0].ToLower())
        {
            case "dusk":
                Main.dayTime = true;
                Main.time = 54001.0; // 7:30 PM (dusk), triggers all night time events
                Main.NewText("Time changed to dusk.");
                break;
            case "midnight":
                Main.dayTime = false;
                Main.time = 16200.0; // 12:00 AM (midnight)
                Main.NewText("Time changed to midnight.");
                break;
            case "dawn":
                Main.dayTime = false;
                Main.time = 32401.0; // 4:30 AM (dawn), triggers all day time events
                Main.NewText("Time changed to dawn.");
                break;
            case "noon":
                Main.dayTime = true;
                Main.time = 27000.0; // 12:00 PM (noon)
                Main.NewText("Time changed to noon.");
                break;
        }
    }
}