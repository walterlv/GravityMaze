using System;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Walterlv.GravityMaze.Game.Framework;
using Walterlv.GravityMaze.Game.Models;

namespace Walterlv.GravityMaze.Game
{
    public class MazeBoard : GameObject
    {
        private readonly int[] _mazeData;

        public int ColumnCount { get; }
        public int RowCount { get; }
        public int StartColumnIndex { get; }
        public int StartRowIndex { get; }
        public int EndColumnIndex { get; }
        public int EndRowIndex { get; }
        public Rect Area { get; private set; }
        public float CellWidth { get; private set; }
        public float CellHeight { get; private set; }

        public Material Material { get; set; } = Material.Default;

        public MazeBoard(
            int startColumnIndex, int startRowIndex,
            int endColumnIndex, int endRowIndex,
            params int[] mazeData)
        {
            _mazeData = mazeData;

            var columns = (mazeData[0].ToString("X").Length - 1) * 2;
            var rows = mazeData.Length - 1;
            ColumnCount = columns;
            RowCount = rows;

            StartColumnIndex = startColumnIndex;
            StartRowIndex = startRowIndex;
            EndColumnIndex = endColumnIndex;
            EndRowIndex = endRowIndex;
        }

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
            var full = Context.SurfaceBounds;
            var min = Math.Min(full.Width, full.Height);
            Area = new Rect((full.Width - min) / 2 + 1, (full.Height - min) / 2 + 1, min - 2, min - 2);
            CellWidth = (float) Area.Width / ColumnCount;
            CellHeight = (float) Area.Height / RowCount;
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            for (var i = 0; i < RowCount; i++)
            {
                for (var j = 0; j < ColumnCount; j++)
                {
                    var (left, up) = GetWallInfo(j, i);

                    var x = (float) (Area.Left + CellWidth * j);
                    var y = (float) (Area.Top + CellHeight * i);

                    if (left)
                    {
                        ds.FillRectangle(x - 1, y - 1, 2, CellHeight + 2, Colors.Black);
                    }

                    if (up)
                    {
                        ds.FillRectangle(x - 1, y - 1, CellWidth + 2, 2, Colors.Black);
                    }
                }
            }

            ds.FillRectangle((float) Area.Right - 1, (float) Area.Top - 1,
                2, (float) Area.Height + 2, Colors.Black);
            ds.FillRectangle((float) Area.Left - 1, (float) Area.Bottom - 1,
                (float) Area.Width + 2, 2, Colors.Black);
        }

        public (bool left, bool up) GetWallInfo(int column, int row)
        {
            var data = _mazeData[row];
            var leftFlag = 1 << (ColumnCount - column) * 2 + 1;
            var upFlag = 1 << (ColumnCount - column) * 2;
            var left = data & leftFlag;
            var up = data & upFlag;
            return (left != 0, up != 0);
        }
    }
}