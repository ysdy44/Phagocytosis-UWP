using System;
using System.Numerics;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Food
    /// </summary>
    public sealed class Food
    {

        //@Static
        public static int Level = 32;
        public static float Radius = 2 * (float)Math.Sqrt(Food.Level);

        public readonly Vector2 Position;
        public readonly bool IsDouble;

        public bool IsDead;
        public byte ShowProgress;

        //@Constructs
        /// <summary>
        /// Initialize a Food.
        /// </summary>
        /// <param name="position"> The position. </param>
        public Food(Vector2 position)
        {
            this.Position = position;
            int count = (int)position.LengthSquared();
            this.IsDouble = count % 2 == 0;
        }

    }
}