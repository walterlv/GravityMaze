using Windows.Foundation;

namespace Walterlv.GravityMaze.Game.Framework
{
    public interface IGameContext
    {
        Rect SurfaceBounds { get; }
        IGameInput Input { get; }

        /// <summary>
        /// 定义每真实单位米有多少屏幕像素。这个值越接近真实比例，玩起来越真实。
        /// </summary>
        float PixelsPerMetre { get; }
    }
}
