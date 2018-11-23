using Windows.Foundation;

namespace Walterlv.GravityMaze.Game.Framework
{
    public class GameContext : IGameContext
    {
        public Rect SurfaceBounds { get; set; }
        public IGameInput Input { get; set; }
        public float PixelsPerMetre { get; } = 400f;
        public float[] Orientation { get; } = {1f, 1f, 1f};
        public bool IsPortrait { get; set; } = false;
        public bool IsMobile { get; set; } = false;

        public (float x, float y, float z) TranslateForOrientation(float x, float y, float z)
        {
            if (IsMobile)
            {
                return (-y, x, z);
            }
            else
            {
                return (x * Orientation[0], y * Orientation[1], z * Orientation[2]);
            }
        }
    }
}
