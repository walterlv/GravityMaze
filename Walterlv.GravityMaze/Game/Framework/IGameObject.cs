using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI;

namespace Walterlv.GravityMaze.Game.Framework
{
    public interface IGameObject
    {
        void Update(CanvasTimingInformation timing);
        void Draw(CanvasDrawingSession ds);
    }
}
