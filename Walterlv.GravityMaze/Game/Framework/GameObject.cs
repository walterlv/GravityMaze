using Microsoft.Graphics.Canvas;
using Walterlv.GravityMaze.Game.Framework;

namespace Walterlv.GravityMaze.Game.Framework
{
    public class GameObject : IGameObject
    {
        public void Draw(CanvasDrawingSession ds)
        {
            OnDraw(ds);
        }

        protected virtual void OnDraw(CanvasDrawingSession ds)
        {
        }
    }
}
