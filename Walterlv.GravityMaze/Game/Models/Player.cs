using Microsoft.Graphics.Canvas;
using System;
using Windows.UI;
using Microsoft.Graphics.Canvas.UI;
using Walterlv.GravityMaze.Game.Framework;
using Windows.Devices.Sensors;
using Windows.Foundation;
using Windows.Graphics.Display;
using Microsoft.Graphics.Canvas.Effects;
using static System.Math;

namespace Walterlv.GravityMaze.Game.Models
{
    public class Player : GameObject
    {
        private readonly MazeGame _game;
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

        /// <summary>
        /// 重力加速度，单位 m/s²。
        /// </summary>
        public float GravityAcceleration { get; } = 10f;

        /// <summary>
        /// 玩家尺寸占一个格子中的尺寸百分比。
        /// </summary>
        public float SizeRatio { get; } = 0.8f;

        public bool EnteredAHole { get; private set; }

        public bool EnteredDestination { get; private set; }

        public Player(MazeGame game, MazeBoard board)
        {
            _game = game;
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

        private void Accelerometer_ReadingChanged(Accelerometer sender, AccelerometerReadingChangedEventArgs e)
        {
            AccelerometerReading reading = e.Reading;
            _xAxis = (float) reading.AccelerationX;
            _yAxis = (float) reading.AccelerationY;
            _zAxis = (float) reading.AccelerationZ;
        }

        protected override void OnUpdate(CanvasTimingInformation timing)
        {
            var seconds = timing.ElapsedTime.TotalSeconds;

            var pixelsPerMetre = Context.PixelsPerMetre;
            var cellSize = Min(_board.CellWidth, _board.CellHeight);
            var baseAcceleration = GravityAcceleration * pixelsPerMetre;
            var resistanceAcceleration = _board.Material.ResistanceAcceleration * pixelsPerMetre;
            _radius = cellSize * SizeRatio / 2;

            // 计算这一帧的倾斜角度。
            (_xAngle, _yAngle) = GetTiltAngles();

            // 倾斜角度带来的加速度。
            var xAcceleration = (float) Sin(_xAngle) * baseAcceleration;
            var yAcceleration = (float) Sin(_yAngle) * baseAcceleration;

            // 叠加洞带来的引力加速度。
            var (enteredAHole, enteredDestination, xHolePosition, yHolePosition) =
                GetAccelerationFromHoles(_xPosition, _yPosition);
            EnteredAHole = enteredAHole;
            EnteredDestination = enteredDestination;
            if (EnteredAHole || EnteredDestination)
            {
                _xPosition = xHolePosition;
                _yPosition = yHolePosition;
                return;
            }

            // 叠加阻力带来的与速度反向的加速度。
            var speedTheta = CalculateTheta(0f, 0f, _xSpeed, _ySpeed);
            (_xSpeed, xAcceleration) = SuperpositionAcceleration(
                _xSpeed, xAcceleration, (float) Abs(resistanceAcceleration * Sin(speedTheta)), seconds);
            (_ySpeed, yAcceleration) = SuperpositionAcceleration(
                _ySpeed, yAcceleration, (float) Abs(resistanceAcceleration * Cos(speedTheta)), seconds);

            // 洞带来了速度跌落。
            if (EnteredAHole)
            {
                _xPosition = xAcceleration;
                _yPosition = yAcceleration;
            }

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

            var collisionLoss = _board.Material.CollisionLoss;
            if (left && _xSpeed < 0 && _xPosition - _radius < leftPosition)
            {
                _xSpeed = -_xSpeed * collisionLoss;
                xOffset = 0;
                _xPosition = leftPosition + _radius;
            }
            else if (right && _xSpeed > 0 && _xPosition + _radius > rightPosition)
            {
                _xSpeed = -_xSpeed * collisionLoss;
                xOffset = 0;
                _xPosition = rightPosition - _radius;
            }

            if (up && _ySpeed < 0 && _yPosition - _radius < upPosition)
            {
                _ySpeed = -_ySpeed * collisionLoss;
                yOffset = 0;
                _yPosition = upPosition + _radius;
            }
            else if (down && _ySpeed > 0 && _yPosition + _radius > downPosition)
            {
                _ySpeed = -_ySpeed * collisionLoss;
                yOffset = 0;
                _yPosition = downPosition - _radius;
            }

            // 进行拐角碰撞检测。
            //     |
            // --- x ---
            //     |
            var (newXSpeedFromXPart, newYSpeedFromXPart, newXSpeedFromYPart, newYSpeedFromYPart) = (0f, 0f, 0f, 0f);
            var (hasCorner, xCornerPosition, yCornerPosition) = GetCellCornerInfoByPosition(_xPosition, _yPosition);
            var xTo = xCornerPosition - _xPosition > 0 && _xSpeed >= 0
                      || xCornerPosition - _xPosition < 0 && _xSpeed <= 0;
            var yTo = yCornerPosition - _yPosition > 0 && _ySpeed >= 0
                      || yCornerPosition - _yPosition < 0 && _ySpeed <= 0;
            if (hasCorner && (_xSpeed != 0 || _ySpeed != 0) && (xTo || yTo))
            {
                var distance = CalculateDistance(xCornerPosition, yCornerPosition, _xPosition, _yPosition);
                if (distance < _radius)
                {
                    var theta = CalculateTheta(xCornerPosition, yCornerPosition, _xPosition, _yPosition);
                    var angle = theta * 180 / PI;
                    newXSpeedFromXPart =
                        (float) (-_xSpeed * Sin(theta) * Sin(theta) + _xSpeed * Cos(theta) * Cos(theta));
                    newYSpeedFromXPart =
                        (float) (-_xSpeed * Sin(theta) * Cos(theta) - _xSpeed * Cos(theta) * Sin(theta));
                    newXSpeedFromYPart =
                        (float) (-_ySpeed * Cos(theta) * Sin(theta) - _ySpeed * Sin(theta) * Cos(theta));
                    newYSpeedFromYPart =
                        (float) (-_ySpeed * Cos(theta) * Cos(theta) + _ySpeed * Sin(theta) * Sin(theta));
                    // 如果已经插入到墙壁中，则调整位置。
                    xOffset = 0;
                    yOffset = 0;
                    _xSpeed = newXSpeedFromXPart + newXSpeedFromYPart;
                    _ySpeed = newYSpeedFromXPart + newYSpeedFromYPart;
                }
            }

            // 计算下一帧的速度。
            _xSpeed += (float) (xAcceleration * seconds);
            _ySpeed += (float) (yAcceleration * seconds);
        }

        /// <summary>
        /// 
        /// </summary>
        private (bool enteredAHole, bool enteredDestination, float xHolePosition, float yHolePosition) 
            GetAccelerationFromHoles(float xPosition, float yPosition)
        {
            var columnIndex = (float) ((xPosition - _board.Area.Left) / _board.CellWidth);
            var rowIndex = (float) ((yPosition - _board.Area.Top) / _board.CellHeight);

            var (x, y) = _board.GetHolePosition(columnIndex, rowIndex);
            var (dx, dy) = _board.GetDestinationPosition(columnIndex, rowIndex);
            return (x != 0 || y != 0, dx != 0 || dy != 0, (float) (_board.Area.Left + x * _board.CellWidth),
                (float) (_board.Area.Top + y * _board.CellHeight));
            // 万有引力定律：F引 = GMm/r²
            //var offsetSqure = x * x + y * y;
            //var force = 1000 / offsetSqure;
            //var radius = Sqrt(offsetSqure);
            //var theta = Sinh(column / radius);
            //return ((float)(-force * Sin(theta)), (float)(-force * Cos(theta)));
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

        private (bool hasCorner, float xCornerPosition, float yCornerPosition)
            GetCellCornerInfoByPosition(float xPosition, float yPosition)
        {
            var columnIndex = (xPosition - _board.Area.Left) / _board.CellWidth;
            var rowIndex = (yPosition - _board.Area.Top) / _board.CellHeight;

            var left = columnIndex - SizeRatio / 2;
            var up = rowIndex - SizeRatio / 2;
            var right = columnIndex + SizeRatio / 2;
            var down = rowIndex + SizeRatio / 2;

            var leftPosition = (float) (_board.Area.Left + _board.CellWidth * (int) left);
            var centerPosition = (float) (_board.Area.Left + _board.CellWidth * (int) right);
            var RightPosition = (float) (_board.Area.Left + _board.CellWidth * ((int) right + 1));
            var upPosition = (float) (_board.Area.Top + _board.CellHeight * (int) up);
            var middlePosition = (float) (_board.Area.Top + _board.CellHeight * (int) down);
            var downPosition = (float) (_board.Area.Top + _board.CellHeight * ((int) down + 1));

            var hasCorner = (int) left != (int) right || (int) up != (int) down;
            if (hasCorner)
            {
                var hasLeft = _board.GetWallInfo((int) left, (int) down).up;
                var hasUp = _board.GetWallInfo((int) right, (int) up).left;
                var hasRight = _board.GetWallInfo((int) right, (int) down).up;
                var hasDown = _board.GetWallInfo((int) right, (int) down).left;
                hasCorner = hasLeft || hasUp || hasRight || hasDown;
            }

            return (hasCorner, centerPosition, middlePosition);
        }

        private (float xAngle, float yAngle) GetTiltAngles()
        {
            if (_accelerometer != null)
            {
                return ((float) (-_yAxis * PI / 2), (float) (-_xAxis * PI / 2));
            }
            else
            {
                return GetTiltAnglesByKeyboard();
            }
        }

        private (float xAngle, float yAngle) GetTiltAnglesByKeyboard()
        {
            var input = Context.Input;
            var radianUnit = (float) PI / 720;
            var maxAngle = (float) PI / 3;
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

        private float CalculateDistance(float x1, float y1, float x2, float y2)
        {
            return (float) Sqrt((x1 - x2) * (x1 - x2) + (y1 - y2) * (y1 - y2));
        }

        private float CalculateTheta(float x1, float y1, float x2, float y2)
        {
            return (float) Tanh((x2 - x1) / (y2 - y1));
        }

        protected override void OnDraw(CanvasDrawingSession ds)
        {
            var creator = ResourceCreator;
            var bitmap = _game.Material.bitmap;
            var bounds = Context.SurfaceBounds;
            if (creator == null || bitmap == null)
            {
                ds.FillEllipse(_xPosition, _yPosition, _radius, _radius, Colors.Gray);
            }
            else
            {
                var morphology = new MorphologyEffect
                {
                    Source = bitmap,
                    Mode = MorphologyEffectMode.Dilate,
                    Width = 40,
                    Height = 40,
                };

                var crop = new CropEffect
                {
                    Source = morphology,
                    SourceRectangle = new Rect(
                        _xPosition - _radius, _yPosition - _radius,
                        _radius + _radius, _radius + _radius),
                };

                using (var list = new CanvasCommandList(creator))
                {
                    using (var s = list.CreateDrawingSession())
                    {
                        s.FillEllipse(_xPosition, _yPosition, _radius, _radius, Colors.Black);
                    }

                    var mask = new AlphaMaskEffect
                    {
                        Source = crop,
                        AlphaMask = list,
                    };

                    var shadow = new ShadowEffect
                    {
                        Source = mask,
                        BlurAmount = 4,
                        ShadowColor = Color.FromArgb(0x40, 0x00, 0x00, 0x00),
                    };

                    ds.DrawImage(shadow);
                    ds.DrawImage(mask);
                }
            }
        }
    }
}