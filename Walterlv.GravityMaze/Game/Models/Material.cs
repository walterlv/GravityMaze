namespace Walterlv.GravityMaze.Game.Models
{
    public class Material
    {
        public static Material Default = new Material("默认", 0.005f, 0.4f);

        public Material(string name, float resistanceAcceleration, float collisionLoss)
        {
            Name = name;
            ResistanceAcceleration = resistanceAcceleration;
            CollisionLoss = collisionLoss;
        }

        public string Name { get; }

        /// <summary>
        /// 阻力加速度，单位 m/s²。
        /// </summary>
        public float ResistanceAcceleration { get; }

        /// <summary>
        /// 碰撞损失，单位百分比；表示每次碰撞后剩余冲量的百分比。
        /// </summary>
        public float CollisionLoss { get; }
    }
}
