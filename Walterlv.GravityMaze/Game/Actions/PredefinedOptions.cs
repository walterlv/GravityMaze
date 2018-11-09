using System.Collections.Generic;
using Walterlv.GravityMaze.Game.Models;

namespace Walterlv.GravityMaze.Game.Actions
{
    public static class PredefinedOptions
    {
        public static Dictionary<string, MazeBoard> Boards = new Dictionary<string, MazeBoard>
        {
            {
                "1", new MazeBoard(6, 1, 2, 7,
                    0b_1101_0111_0111_0111_10,
                    0b_1100_0111_0110_0110_10,
                    0b_1010_1010_1010_0000_10,
                    0b_1110_1010_1011_0010_10,
                    0b_1100_1000_1010_1011_10,
                    0b_1010_0100_1010_0110_10,
                    0b_1110_0101_1001_0001_10,
                    0b_1000_1100_1101_0100_10,
                    0b_0101_0101_0101_0101_00
                )
            },
        };

        public static Dictionary<string, Material> Materials = new Dictionary<string, Material>
        {
            {"towel", new Material("毛巾", 0.2f, 0.05f)},
            {"sofa-fabric", new Material("沙发织物", 0.08f, 0.1f)},
            {"cardboard", new Material("硬纸板", 0.01f, 0.4f)},
            {"soft-sofa-fabric", new Material("柔软的沙发织物", 0.1f, 0.1f)},
            {"stock", new Material("实木", 0.008f, 0.4f)},
            {"mac-touchpad-aluminum", new Material("MacBook 的铝合金触控板", 0.001f, 0.5f)},
            {"wood-table", new Material("木质桌子", 0.004f, 0.5f)},
            {"hard-fabric", new Material("织物板", 0.04f, 0.2f)},
        };
    }
}
