using System.Linq;
using Microsoft.Graphics.Canvas;
using Walterlv.GravityMaze.Game.Actions;
using Walterlv.GravityMaze.Game.Framework;
using Walterlv.GravityMaze.Game.Models;

namespace Walterlv.GravityMaze.Game
{
    public class MazeGame : GameObject
    {
        public MazeBoard Board { get; }

        public MazeGame()
        {
            Board = PredefinedOptions.Boards.First().Value;
            Player player = new Player(Board);
            AddChildren(Board, player);
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
        }
    }
}
