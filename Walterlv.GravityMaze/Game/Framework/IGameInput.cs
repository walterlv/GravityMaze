namespace Walterlv.GravityMaze.Game.Framework
{
    public interface IGameInput
    {
        bool Up { get; }
        bool Down { get; }
        bool Left { get; }
        bool Right { get; }
    }
}
