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
        private (string name, CanvasBitmap bitmap) _material;

        public (string name, CanvasBitmap bitmap) Material
        {
            get => _material;
            set
            {
                _material = value;
                var material = PredefinedOptions.Materials[value.name];
                Board.Material = material;
            }
        }

        private MazeBoard Board { get; }

        public MazeGame()
        {
            Board = PredefinedOptions.Boards["3"];
            Player player = new Player(this, Board);
            AddChildren(Board, player);
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            if (Material.bitmap != null)
            {
                ds.DrawImage(Material.bitmap, Context.SurfaceBounds);
            }
            else
            {
                ds.FillRectangle(Context.SurfaceBounds, Colors.White);
            }
        }
    }
}