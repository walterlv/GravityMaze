using System.Linq;
using Microsoft.Graphics.Canvas;
using Walterlv.GravityMaze.Game.Actions;
using Walterlv.GravityMaze.Game.Framework;
using Walterlv.GravityMaze.Game.Models;

namespace Walterlv.GravityMaze.Game
{
    public class MazeGame : GameObject
    {
        public MazeGame()
        {
            MazeBoard board = PredefinedOptions.Boards.First().Value;
            Player player = new Player(board);
            AddChildren(board, player);
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
        }
    }
}
