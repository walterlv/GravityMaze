using Windows.Foundation;

namespace Walterlv.GravityMaze.Game.Framework
{
    public class GameContext : IGameContext
    {
        public Rect SurfaceBounds { get; set; }
        public IGameInput Input { get; set; }
        public float PixelsPerMetre { get; } = 400f;
    }
}
