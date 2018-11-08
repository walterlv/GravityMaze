using System;
using System.Collections.Generic;

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
    }
}
