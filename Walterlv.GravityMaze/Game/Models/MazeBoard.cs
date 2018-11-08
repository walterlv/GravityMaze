using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
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

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
            _area = Context.SurfaceBounds;
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            ds.FillRectangle(_area, Colors.RoyalBlue);
        }
    }
}
