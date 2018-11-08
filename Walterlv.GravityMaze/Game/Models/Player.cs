﻿using Microsoft.Graphics.Canvas;
using System;
using Windows.UI;
using Microsoft.Graphics.Canvas.UI;
using Walterlv.GravityMaze.Game.Framework;
using Windows.Devices.Sensors;

namespace Walterlv.GravityMaze.Game.Models
{
    public class Player : GameObject
    {
        private readonly MazeBoard _board;
        private readonly Accelerometer _accelerometer;
        private float _xAngle;
        private float _yAngle;
        private float _xSpeed;
        private float _ySpeed;
        private float _xPosition;
        private float _yPosition;
        private float _radius;
        private float _xAxis;
        private float _yAxis;
        private float _zAxis;

        public Player(MazeBoard board)
        {
            _board = board;
            _accelerometer = Accelerometer.GetDefault();
            if (_accelerometer != null)
            {
                uint minReportInterval = _accelerometer.MinimumReportInterval;
                uint reportInterval = minReportInterval > 16 ? minReportInterval : 16;
                _accelerometer.ReportInterval = reportInterval;
                _accelerometer.ReadingChanged += Accelerometer_ReadingChanged;
            }
        }

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
            var seconds = timing.ElapsedTime.TotalSeconds;

            var baseAcceleration = 10f;
            baseAcceleration = baseAcceleration * Math.Min(_board.CellWidth, _board.CellHeight) * 20;
            var resistanceAcceleration = 0.1f;
            resistanceAcceleration = resistanceAcceleration * Math.Min(_board.CellWidth, _board.CellHeight) * 20;
            _radius = (float) (Math.Min(_board.CellWidth, _board.CellHeight) * 0.4);
            var tooSlowSpeed = Math.Min(_board.CellWidth, _board.CellHeight) / 2;

            // 计算这一帧的倾斜角度。
            (_xAngle, _yAngle) = GetTiltAngles();

            // 倾斜角度带来的加速度。
            var xAcceleration = (float) Math.Sin(_xAngle) * baseAcceleration;
            var yAcceleration = (float) Math.Sin(_yAngle) * baseAcceleration;

            // 叠加阻力带来的与速度反向的加速度。
            (_xSpeed, xAcceleration) =
                SuperpositionAcceleration(_xSpeed, xAcceleration, resistanceAcceleration, seconds);
            (_ySpeed, yAcceleration) =
                SuperpositionAcceleration(_ySpeed, yAcceleration, resistanceAcceleration, seconds);

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

            // 进行边缘碰撞检测。
            // +--------+
            // |   x    |
            // |        |
            // +--------+
            var (left, leftPosition, up, upPosition, right, rightPosition, down, downPosition) =
                GetCellWallInfoByPosition(_xPosition, _yPosition);

            if (left && _xSpeed < 0 && _xPosition - _radius < leftPosition)
            {
                _xSpeed = -_xSpeed * 0.8f;
                _xPosition = leftPosition + _radius;
            }
            else if (right && _xSpeed > 0 && _xPosition + _radius > rightPosition)
            {
                _xSpeed = -_xSpeed * 0.8f;
                _xPosition = rightPosition - _radius;
            }

            if (up && _ySpeed < 0 && _yPosition - _radius < upPosition)
            {
                _ySpeed = -_ySpeed * 0.8f;
                _yPosition = upPosition + _radius;
            }
            else if (down && _ySpeed > 0 && _yPosition + _radius > downPosition)
            {
                _ySpeed = -_ySpeed * 0.8f;
                _yPosition = downPosition - _radius;
            }

            // 进行拐角碰撞检测。
            //     |
            // --- x ---
            //     |
            // 未完待续。

            // 计算下一帧的速度。
            _xSpeed += (float) (xAcceleration * seconds);
            _ySpeed += (float) (yAcceleration * seconds);
        }

        /// <summary>
        /// 根据当前速度、加速度和经过的时间，计算叠加了阻力后的加速度和速度。
        /// </summary>
        private static (float speed, float acceleration) SuperpositionAcceleration(
            float speed, float acceleration, float resistanceAcceleration, double seconds)
        {
            if (speed > 0)
            {
                var newAcceleration = acceleration - resistanceAcceleration;
                var newSpeed = speed + newAcceleration * seconds;

                if (newSpeed < 0 && acceleration == 0)
                {
                    speed = 0;
                }

                if (newSpeed > 0)
                {
                    acceleration = newAcceleration;
                }
            }
            else if (speed < 0)
            {
                var xa = acceleration + resistanceAcceleration;
                var newSpeed = speed + xa * seconds;

                if (newSpeed > 0 && acceleration == 0)
                {
                    speed = 0;
                }

                if (newSpeed < 0)
                {
                    acceleration = xa;
                }
            }

            return (speed, acceleration);
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            ds.FillEllipse(_xPosition, _yPosition, _radius, _radius, Colors.Gray);
        }

        private (bool left, float leftPosition,
            bool up, float upPosition,
            bool right, float rightPosition,
            bool down, float downPosition) GetCellWallInfoByPosition(float xPosition, float yPosition)
        {
            var column = (int) ((xPosition - _board.Area.Left) / _board.CellWidth);
            var row = (int) ((yPosition - _board.Area.Top) / _board.CellHeight);
            var (left, up) = _board.GetWallInfo(column, row);
            var right = _board.GetWallInfo(column + 1, row).left;
            var down = _board.GetWallInfo(column, row + 1).up;

            return (left, (float) (_board.Area.Left + _board.CellWidth * column),
                up, (float) (_board.Area.Top + _board.CellHeight * row),
                right, (float) (_board.Area.Left + _board.CellWidth * (column + 1)),
                down, (float) (_board.Area.Top + _board.CellHeight * (row + 1)));
        }

        private void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs e)
        {
            AccelerometerReading reading = e.Reading;
            _xAxis = (float) reading.AccelerationX;
            _yAxis = (float) reading.AccelerationY;
            _zAxis = (float) reading.AccelerationZ;
        }

        private (float xAngle, float yAngle) GetTiltAngles()
        {
            if (_accelerometer != null)
            {
                return GetTiltAnglesByKeyboard();
            }
            else
            {
                return GetTiltAnglesByKeyboard();
            }
        }

        private (float xAngle, float yAngle) GetTiltAnglesByKeyboard()
        {
            var input = Context.Input;
            var radianUnit = (float) Math.PI / 720;
            var maxAngle = (float) Math.PI / 36;
            float xAngle = _xAngle;
            var yAngle = _yAngle;

            if (input.Left && !input.Right)
            {
                xAngle = xAngle - radianUnit;
                if (xAngle < -maxAngle) xAngle = -maxAngle;
            }
            else if (!input.Left && input.Right)
            {
                xAngle = xAngle + radianUnit;
                if (xAngle > maxAngle) xAngle = maxAngle;
            }
            else
            {
                if (xAngle > radianUnit) xAngle -= radianUnit;
                else if (xAngle < -radianUnit) xAngle += radianUnit;
                else xAngle = 0;
            }

            if (input.Up && !input.Down)
            {
                yAngle = yAngle - radianUnit;
                if (yAngle < -maxAngle) yAngle = -maxAngle;
            }
            else if (!input.Up && input.Down)
            {
                yAngle = yAngle + radianUnit;
                if (yAngle > maxAngle) yAngle = maxAngle;
            }
            else
            {
                if (yAngle > radianUnit) yAngle -= radianUnit;
                else if (yAngle < -radianUnit) yAngle += radianUnit;
                else yAngle = 0;
            }

            return (xAngle, yAngle);
        }
    }
}