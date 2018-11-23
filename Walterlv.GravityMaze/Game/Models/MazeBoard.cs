using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;
using Walterlv.GravityMaze.Game.Framework;
using Walterlv.GravityMaze.Game.Models;
using static System.Math;

namespace Walterlv.GravityMaze.Game
{
    public class MazeBoard : GameObject, IEnumerable
    {
        private readonly int[] _mazeData;

        public int ColumnCount { get; }
        public int RowCount { get; }
        public float StartColumnIndex { get; }
        public float StartRowIndex { get; }
        public float EndColumnIndex { get; }
        public float EndRowIndex { get; }
        public Rect Area { get; private set; }
        public float CellWidth { get; private set; }
        public float CellHeight { get; private set; }

        public Material Material { get; set; } = Material.Default;
        private readonly List<Hole> Holes = new List<Hole>();

        public MazeBoard(
            float startColumnIndex, float startRowIndex,
            float endColumnIndex, float endRowIndex,
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

            foreach (var hole in Holes)
            {
                ds.FillEllipse(
                    (float) (Area.Left + CellWidth * (hole.Column + 0.5f)),
                    (float) (Area.Top + CellHeight * (hole.Row + 0.5f)),
                    (float) (CellWidth / 2), (float) (CellWidth / 2), Colors.Black);
            }

            ds.FillEllipse(
                (float) (Area.Left + CellWidth * (EndColumnIndex + 0.5f)),
                (float) (Area.Top + CellHeight * (EndRowIndex + 0.5f)),
                (float) (CellWidth / 2), (float) (CellWidth / 2), Colors.Orange);
        }

        public (bool left, bool up) GetWallInfo(int column, int row)
        {
            if (row >= _mazeData.Length || row < 0)
            {
                return (true, true);
            }

            var data = _mazeData[row];
            var leftFlag = 1 << (ColumnCount - column) * 2 + 1;
            var upFlag = 1 << (ColumnCount - column) * 2;
            var left = data & leftFlag;
            var up = data & upFlag;
            return (left != 0, up != 0);
        }

        public (float xPosition, float yPosition) GetHolePosition(float column, float row)
        {
            foreach (var hole in Holes)
            {
                var columnOffset = hole.Column + 0.5f - column;
                var rowOffset = hole.Row + 0.5f - row;
                var offsetSqure = columnOffset * columnOffset + rowOffset * rowOffset;
                if (offsetSqure <= 0.25)
                {
                    return (hole.Column + 0.5f, hole.Row + 0.5f);
                }
            }

            return (0f, 0f);
        }

        public (float xPosition, float yPosition) GetDestinationPosition(float column, float row)
        {
            var columnOffset = EndColumnIndex + 0.5f - column;
            var rowOffset = EndRowIndex + 0.5f - row;
            var offsetSqure = columnOffset * columnOffset + rowOffset * rowOffset;
            if (offsetSqure <= 0.25)
            {
                return (EndColumnIndex + 0.5f, EndRowIndex + 0.5f);
            }

            return (0f, 0f);
        }

        public void Add(float column, float row) => Holes.Add(new Hole(column, row));
        public IEnumerator GetEnumerator() => Holes.GetEnumerator();
    }
}