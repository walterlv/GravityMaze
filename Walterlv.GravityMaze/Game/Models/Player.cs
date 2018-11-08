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
            var seconds = timing.ElapsedTime.TotalSeconds;

            var baseAcceleration = 8000;
            var resistanceAcceleration = 0;

            // 计算这一帧的倾斜角度。
            (_xAngle, _yAngle) = GetTiltAngles();

            // 倾斜角度带来的加速度。
            var xAcceleration = Math.Sin(_xAngle) * baseAcceleration;
            var yAcceleration = Math.Sin(_yAngle) * baseAcceleration;

            // 叠加阻力带来的与速度反向的加速度。
            if (_xSpeed > 0) xAcceleration -= resistanceAcceleration;
            else if (_xSpeed < 0) xAcceleration += resistanceAcceleration;
            if (_ySpeed > 0) yAcceleration -= resistanceAcceleration;
            else if (_ySpeed < 0) yAcceleration += resistanceAcceleration;

            // 计算此加速度和此初速度下的位置偏移量。
            var xOffset = (float) (_xSpeed * seconds + xAcceleration * seconds * seconds / 2);
            var yOffset = (float) (_ySpeed * seconds + yAcceleration * seconds * seconds / 2);

            // 叠加上一帧的位置以获得这一帧的位置。
            if (_xPosition == 0 && _yPosition == 0)
            {
                _xPosition = (float)
                    (_board.Area.Left + _board.CellWidth * _board.StartColumnIndex + _board.CellWidth / 2);
                _yPosition = (float)
                    (_board.Area.Top + _board.CellHeight * _board.StartRowIndex + _board.CellHeight / 2);
            }

            _xPosition += xOffset;
            _yPosition += yOffset;

            // 计算下一帧的速度。
            _xSpeed += (float) (xAcceleration * seconds);
            _ySpeed += (float) (yAcceleration * seconds);
            if (_xSpeed < 100 && _xSpeed > -100) _xSpeed = 0;
            if (_ySpeed < 100 && _ySpeed > -100) _ySpeed = 0;
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            var radius = (float) (Math.Min(_board.CellWidth, _board.CellHeight) * 0.4);
            ds.FillEllipse(_xPosition, _yPosition, radius, radius, Colors.Gray);
        }

        private (float xAngle, float yAngle) GetTiltAngles()
        {
            return GetTiltAnglesByKeyboard();
        }

        private (float xAngle, float yAngle) GetTiltAnglesByKeyboard()
        {
            var input = Context.Input;
            var radianUnit = (float) Math.PI / 180;
            float xAngle = _xAngle;
            var yAngle = _yAngle;

            if (input.Left && !input.Right)
            {
                xAngle = xAngle - radianUnit;
                if (xAngle < -Math.PI / 2) xAngle = (float) -Math.PI / 2;
            }
            else if (!input.Left && input.Right)
            {
                xAngle = xAngle + radianUnit;
                if (xAngle > Math.PI / 2) xAngle = (float) Math.PI / 2;
            }
            else
            {
                if (xAngle > Math.PI / 180) xAngle -= radianUnit;
                else if (xAngle < -Math.PI / 180) xAngle += radianUnit;
                else xAngle = 0;
            }

            if (input.Up && !input.Down)
            {
                yAngle = yAngle - radianUnit;
                if (yAngle < -Math.PI / 2) yAngle = (float) -Math.PI / 2;
            }
            else if (!input.Up && input.Down)
            {
                yAngle = yAngle + radianUnit;
                if (yAngle > Math.PI / 2) yAngle = (float) Math.PI / 2;
            }
            else
            {
                if (yAngle > Math.PI / 180) yAngle -= radianUnit;
                else if (yAngle < -Math.PI / 180) yAngle += radianUnit;
                else yAngle = 0;
            }

            return (xAngle, yAngle);
        }
    }
}