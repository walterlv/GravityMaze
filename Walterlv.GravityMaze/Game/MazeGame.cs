using Microsoft.Graphics.Canvas;
using Walterlv.GravityMaze.Game.Actions;
using Walterlv.GravityMaze.Game.Framework;
using Walterlv.GravityMaze.Game.Models;
using Windows.UI;
using Microsoft.Graphics.Canvas.UI;

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

        private MazeBoard Board { get; set; }
        private Player Player { get; set; }
        private int Index { get; set; }

        public MazeGame()
        {
            Index = 1;
            Reset(Index);
        }

        private void Reset(int index)
        {
            RemoveChildren(Board, Player);
            Board = PredefinedOptions.Boards[index.ToString("D")];
            Player = new Player(this, Board);
            AddChildren(Board, Player);
        }

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
            if (Player.EnteredAHole)
            {
                // 失败。
                Reset(Index);
            }
            else if (Player.EnteredDestination)
            {
                // 胜利。
                Index++;
                Index = Index % 6;
                Reset(Index);
            }
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