using Windows.Foundation;

namespace Walterlv.GravityMaze.Game.Framework
{
    public interface IGameContext
    {
        Rect SurfaceBounds { get; }
        IGameInput Input { get; }
    }
}
