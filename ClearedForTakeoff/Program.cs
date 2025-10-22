using Microsoft.Xna.Framework;
using System;

namespace ClearedForTakeoff
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            var game = new Game1();
            game.Run();
        }
    }
}