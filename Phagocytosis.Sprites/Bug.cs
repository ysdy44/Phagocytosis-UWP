using System;
using System.Numerics;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Bug
    /// </summary>
    public sealed class Bug
    {

        public Vector2 Position;
        private Vector2 Velocity;

        //@Constructs
        /// <summary>
        /// Initialize a Bug.
        /// </summary>
        /// <param name="width"> The width. </param>
        /// <param name="height"> The height. </param>
        /// <param name="random"> The random. </param>
        public Bug(int width, int height, Random random)
        {
            this.Position = new Vector2(random.Next(0, width), random.Next(0, height));
            this.Velocity = new Vector2(random.Next(-100, 100) / 100.0f, random.Next(-100, 100) / 100.0f);
        }

        public void Update(float width, float height, float elapsedTime)
        {
            if (this.Position.X < 0)
            {
                this.Position.X = 1;
                this.Velocity.X = -this.Velocity.X;
            }
            else if (this.Position.X > width)
            {
                this.Position.X = width - 1;
                this.Velocity.X = -this.Velocity.X;
            }

            if (this.Position.Y < 0)
            {
                this.Position.Y = 1;
                this.Velocity.Y = -this.Velocity.Y;
            }
            else if (this.Position.Y > height)
            {
                this.Position.Y = height - 1;
                this.Velocity.Y = -this.Velocity.Y;
            }

            float speed = elapsedTime / 16f;
            this.Position = this.Position + this.Velocity * speed;
        }

    }
}