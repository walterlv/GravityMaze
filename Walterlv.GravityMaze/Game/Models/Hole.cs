namespace Walterlv.GravityMaze.Game
{
    public readonly struct Hole
    {
        public float Column { get; }
        public float Row { get; }

        public Hole(float column, float row)
        {
            Column = column;
            Row = row;
        }
    }
}
