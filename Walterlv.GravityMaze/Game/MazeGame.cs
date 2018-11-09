using System.Linq;
using Microsoft.Graphics.Canvas;
using Walterlv.GravityMaze.Game.Actions;
using Walterlv.GravityMaze.Game.Framework;
using Walterlv.GravityMaze.Game.Models;
using Windows.UI;

namespace Walterlv.GravityMaze.Game
{
    public class MazeGame : GameObject
    {
        public CanvasBitmap Material { get; set; }

        public MazeGame()
        {
            var Board = PredefinedOptions.Boards.First().Value;
            Player player = new Player(Board);
            AddChildren(Board, player);
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            if (Material != null)
            {
                ds.DrawImage(Material, Context.SurfaceBounds);
            }
            else
            {
                ds.FillRectangle(Context.SurfaceBounds, Colors.White);
            }
        }
    }
}