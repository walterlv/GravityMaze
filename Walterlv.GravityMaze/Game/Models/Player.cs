using Microsoft.Graphics.Canvas;
using Walterlv.GravityMaze.Game.Framework;

namespace Walterlv.GravityMaze.Game.Models
{
    public class Player : GameObject
    {
        private readonly MazeBoard _board;

        public Player(MazeBoard board)
        {
            _board = board;
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
        }
    }
}
