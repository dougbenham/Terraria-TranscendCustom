using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TranscendCustom;

public class PlanteraCommand : ModCommand
{
    private int _planteraBulbTileLookup;

    public override CommandType Type => CommandType.Chat;

    public override string Command => "plantera";

    public override string Usage => "/plantera";

    public override string Description => "Teleport to plantera bulb";
    
    public override void SetStaticDefaults()
    {
        if (Terraria.Map.MapHelper.tileLookup != null)
            _planteraBulbTileLookup = Terraria.Map.MapHelper.TileToLookup(TileID.PlanteraBulb, 0);
    }

    public override void Action(CommandCaller caller, string input, string[] args)
    {
        for (int i = 0; i < Main.Map.MaxWidth; i++)
        {
            for (int j = 0; j < Main.Map.MaxHeight; j++)
            {
                if (Main.Map[i, j].Type == _planteraBulbTileLookup)
                {
                    var player = caller.Player;
                    var vector = new Vector2(i * 16, j * 16);
                    player.Teleport(vector, 1, 0);
                    player.velocity = Vector2.Zero;
                    NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, vector.X, vector.Y, 1, 0, 0);
                    return;
                }
            }
        }
    }
}