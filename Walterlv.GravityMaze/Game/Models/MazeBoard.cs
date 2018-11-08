using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Walterlv.GravityMaze.Game.Framework;

namespace Walterlv.GravityMaze.Game
{
    public class MazeBoard : GameObject
    {
        private readonly int _columnCount;
        private readonly int _rowCount;
        private readonly int[] _mazeData;

        private Rect _area;
        private float _cellWidth;
        private float _cellHeight;

        public MazeBoard(
            int startRowIndex, int startColumnIndex,
            int endRowIndex, int endColumnIndex,
            params int[] mazeData)
        {
            var columns = (mazeData[0].ToString("X").Length - 1) * 2;
            var rows = mazeData.Length;



            _mazeData = mazeData;
            _columnCount = columns;
            _rowCount = rows;
        }

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            _area = Context.SurfaceBounds;
            _cellWidth = (float) _area.Width / _columnCount;
            _cellHeight = (float) _area.Height / _rowCount;

            ds.FillRectangle(_area, Colors.White);

            for (var i = 0; i < _rowCount; i++)
            {
                for (var j = 0; j < _columnCount; j++)
                {
                    var (left, top) = GetWallInfo(j, i);

                    var x = (float) (_area.Left + _cellWidth * j);
                    var y = (float) (_area.Top + _cellHeight * i);

                    if (left)
                    {
                        ds.FillRectangle(x - 1, y - 1, 2, _cellHeight + 2, Colors.Black);
                    }

                    if (top)
                    {
                        ds.FillRectangle(x - 1, y - 1, _cellWidth + 2, 2, Colors.Black);
                    }
                }
            }
        }

        private (bool left, bool top) GetWallInfo(int column, int row)
        {
            var data = _mazeData[row];
            var leftFlag = 1 << (_columnCount - column);
            var topFlag = 1 << (_columnCount - column);
            var left = data & leftFlag;
            var top = data & topFlag;
            return (left != 0, top != 0);
        }
    }
}
