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
                    0b_1100_0111_0110_0110_10
                )
            },
        };
    }
}
