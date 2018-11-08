using System.Linq;
using Microsoft.Graphics.Canvas;
using Walterlv.GravityMaze.Game.Actions;
using Walterlv.GravityMaze.Game.Framework;
using Walterlv.GravityMaze.Game.Models;

namespace Walterlv.GravityMaze.Game
{
    public class MazeGame : GameObject
    {
        private MazeBoard Board { get; } = PredefinedOptions.Boards.First().Value;
        private Player Player { get; } = new Player();

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            Board.Draw(ds);
            Player.Draw(ds);
        }
    }
}
