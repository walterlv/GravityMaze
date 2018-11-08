using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Walterlv.GravityMaze.Game.Framework;

namespace Walterlv.GravityMaze.Game
{
    public class MazeBoard : GameObject
    {
        private readonly int[] _mazeData;
        public int ColumnCount { get; }
        public int RowCount { get; }

        public Rect Area;
        public float CellWidth;
        public float CellHeight;

        public MazeBoard(
            int startRowIndex, int startColumnIndex,
            int endRowIndex, int endColumnIndex,
            params int[] mazeData)
        {
            var columns = (mazeData[0].ToString("X").Length - 1) * 2;
            var rows = mazeData.Length;



            _mazeData = mazeData;
            ColumnCount = columns;
            RowCount = rows;
        }

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            Area = Context.SurfaceBounds;
            CellWidth = (float) Area.Width / ColumnCount;
            CellHeight = (float) Area.Height / RowCount;

            ds.FillRectangle(Area, Colors.White);

            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    var (left, top) = GetWallInfo(j, i);

                    var x = (float) (Area.Left + CellWidth * j);
                    var y = (float) (Area.Top + CellHeight * i);

                    if (left)
                    {
                        ds.FillRectangle(x - 1, y - 1, 2, CellHeight + 2, Colors.Black);
                    }

                    if (top)
                    {
                        ds.FillRectangle(x - 1, y - 1, CellWidth + 2, 2, Colors.Black);
                    }
                }
            }
        }

        private (bool left, bool top) GetWallInfo(int column, int row)
        {
            var data = _mazeData[row];
            var leftFlag = 1 << (ColumnCount - column) * 2 + 1;
            var topFlag = 1 << (ColumnCount - column) * 2;
            var left = data & leftFlag;
            var top = data & topFlag;
            return (left != 0, top != 0);
        }
    }
}
