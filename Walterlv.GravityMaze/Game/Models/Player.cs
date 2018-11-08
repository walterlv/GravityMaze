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
        private float _xAngle;
        private float _yAngle;
        private float _xSpeed;
        private float _ySpeed;
        private float _xPosition;
        private float _yPosition;

        public Player(MazeBoard board)
        {
            _board = board;
        }

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
            var input = Context.Input;
            var seconds = timing.ElapsedTime.TotalSeconds;

            var radianUnit = (float) Math.PI / 180;
            var baseAcceleration = 10000;

            var xAcceleration = Math.Sin(_xAngle) * baseAcceleration;
            var yAcceleration = Math.Sin(_yAngle) * baseAcceleration;
            var xOffset = (float) (_xSpeed * seconds + xAcceleration * seconds * seconds / 2);
            var yOffset = (float) (_ySpeed * seconds + yAcceleration * seconds * seconds / 2);
            _xSpeed += (float) (xAcceleration * seconds);
            _ySpeed += (float) (yAcceleration * seconds);

            if (_xPosition == 0 && _yPosition == 0)
            {
                _xPosition = (float)
                    (_board.Area.Left + _board.CellWidth * _board.StartColumnIndex + _board.CellWidth / 2);
                _yPosition = (float)
                    (_board.Area.Top + _board.CellHeight * _board.StartRowIndex + _board.CellHeight / 2);
            }

            _xPosition += xOffset;
            _yPosition += yOffset;

            if (input.Left && !input.Right)
            {
                _xAngle = _xAngle - radianUnit;
                if (_xAngle < -Math.PI / 2) _xAngle = (float) -Math.PI / 2;
            }
            else if (!input.Left && input.Right)
            {
                _xAngle = _xAngle + radianUnit;
                if (_xAngle > Math.PI / 2) _xAngle = (float) Math.PI / 2;
            }
            else
            {
                if (_xAngle > Math.PI / 180) _xAngle -= radianUnit;
                else if (_xAngle < -Math.PI / 180) _xAngle += radianUnit;
                else _xAngle = 0;
            }

            if (input.Up && !input.Down)
            {
                _yAngle = _yAngle - radianUnit;
                if (_yAngle < -Math.PI / 2) _yAngle = (float) -Math.PI / 2;
            }
            else if (!input.Up && input.Down)
            {
                _yAngle = _yAngle + radianUnit;
                if (_yAngle > Math.PI / 2) _yAngle = (float) Math.PI / 2;
            }
            else
            {
                if (_yAngle > Math.PI / 180) _yAngle -= radianUnit;
                else if (_yAngle < -Math.PI / 180) _yAngle += radianUnit;
                else _yAngle = 0;
            }
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            var radius = (float) (Math.Min(_board.CellWidth, _board.CellHeight) * 0.4);
            ds.FillEllipse(_xPosition, _yPosition, radius, radius, Colors.Gray);
        }
    }
}