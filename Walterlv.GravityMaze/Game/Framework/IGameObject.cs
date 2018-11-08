using Microsoft.Graphics.Canvas;

namespace Walterlv.GravityMaze.Game.Framework
{
    public interface IGameObject
    {
        void Draw(CanvasDrawingSession ds);
    }
}
