using Microsoft.Xna.Framework.Input;
using Terraria.ModLoader;

namespace DougCustom;

public class DougCustom : Mod
{
    public override uint ExtraPlayerBuffSlots => 200;

    public static ModKeybind TeleportKeybind { get; set; }

    public static ModKeybind CrazySpawnsKeybind { get; set; }

    public override void Load()
    {
        TeleportKeybind = KeybindLoader.RegisterKeybind(this, "Teleport to cursor", Keys.F);
        CrazySpawnsKeybind = KeybindLoader.RegisterKeybind(this, "Farming mode", Keys.OemOpenBrackets);
    }
}