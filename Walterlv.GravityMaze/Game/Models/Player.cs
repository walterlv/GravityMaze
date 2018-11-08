using Microsoft.Graphics.Canvas;
using System;
using Windows.UI;
using Microsoft.Graphics.Canvas.UI;
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

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
            
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            var x = (float) (_board.Area.Left + _board.CellWidth * _board.StartColumnIndex + _board.CellWidth / 2);
            var y = (float) (_board.Area.Top + _board.CellHeight * _board.StartRowIndex + _board.CellHeight / 2);
            var radius = (float) (Math.Min(_board.CellWidth, _board.CellHeight) * 0.4);
            ds.FillEllipse(x, y, radius, radius, Colors.Gray);
        }
    }
}
