using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Walterlv.GravityMaze.Game.Framework;

namespace Walterlv.GravityMaze.Game
{
    public class MazeBoard : GameObject
    {
        private Rect _area;

        public MazeBoard(
            int startRowIndex, int startColumnIndex,
            int endRowIndex, int endColumnIndex,
            params int[] mazeData)
        {

        }

        public void Update()
        {
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            ds.FillRectangle(new Rect(0, 0, 200, 200), Colors.RoyalBlue);
        }
    }
}
