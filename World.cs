using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace DougCustom;

public class World : ModSystem
{
    public override void PostDrawFullscreenMap(ref string mouseText)
    {
        if (Config.Instance.MapTeleport && Main.mouseRight && Main.keyState.IsKeyUp(Keys.LeftControl))
        {
            var player = Main.player[Main.myPlayer];
            
            var target = Main.MouseScreen;
            target.X -= Main.screenWidth / 2;
            target.Y -= Main.screenHeight / 2;
            target /= Main.mapFullscreenScale - 0.22f; // weird extra scaling factor..
            target += Main.mapFullscreenPos;
            target *= 16f;
            target.X -= player.width / 2;
            target.Y -= player.height / 2;
            
            var maxX = Main.maxTilesX * 16;
            if (target.X < 0f)
            {
                target.X = 0f;
            }
            else if (target.X + player.width > maxX)
            {
                target.X = maxX - player.width;
            }
            
            var maxY = Main.maxTilesY * 16;
            if (target.Y < 0f)
            {
                target.Y = 0f;
            }
            else if (target.Y + player.height > maxY)
            {
                target.Y = maxY - player.height;
            }
            
            player.Teleport(target, 1, 0);
            player.velocity = Vector2.Zero;
            NetMessage.SendData(MessageID.Teleport, -1, -1, null, 0, player.whoAmI, target.X, target.Y, 1, 0, 0);
        }
    }
}