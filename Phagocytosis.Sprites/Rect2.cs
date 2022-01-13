using System;
using System.Numerics;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Rect
    /// </summary>
    public struct Rect2
    {

        public readonly int X;
        public readonly int Y;
        public readonly int Width;
        public readonly int Height;

        public readonly int Left;
        public readonly int Top;
        public readonly int Right;
        public readonly int Bottom;

        //@Constructs
        /// <summary>
        /// Initialize a Rect2.
        /// </summary>
        /// <param name="position"> The position. </param>
        public Rect2(Vector2 position)
            : this((int)position.X, (int)position.Y, 2, 2)
        {
        }
        /// <summary>
        /// Initialize a Rect2.
        /// </summary>
        /// <param name="pointA"> The point A. </param>
        /// <param name="pointB"> The oint B. </param>
        public Rect2(Vector2 pointA, Vector2 pointB)
            : this((int)Math.Min(pointA.X, pointB.X),
                  (int)Math.Min(pointA.Y, pointB.Y),
                  (int)Math.Max(2, Math.Abs(pointA.X - pointB.X)),
                  (int)Math.Max(2, Math.Abs(pointA.Y - pointB.Y)))
        {
        }
        /// <summary>
        /// Initialize a Rect2.
        /// </summary>
        /// <param name="x"> The x. </param>
        /// <param name="y"> The y. </param>
        /// <param name="width"> The width. </param>
        /// <param name="height"> The height. </param>
        public Rect2(int x, int y, int width, int height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
            this.Left = x;
            this.Top = y;
            this.Right = x + width;
            this.Bottom = y + height;
        }

        public Vector2 LeftTop() => new Vector2(this.Left, this.Top);
        public Vector2 RightBottm() => new Vector2(this.Right, this.Bottom);

        public Vector2 Range(Vector2 position)
        {
            float left = position.X - this.Left;
            float right = this.Right - position.X;
            float top = position.Y - this.Top;
            float bottom = this.Bottom - position.Y;

            float min = (float)Math.Min(Math.Min(left, right), Math.Min(top, bottom));
            if (min <= 0) return position;

            if (min == left) position.X -= left;
            else if (min == right) position.X += right;
            else if (min == top) position.Y -= top;
            else if (min == bottom) position.Y += bottom;

            return position;
        }

        public bool Contains(Vector2 vector)
        {
            if (this.Left > vector.X) return false;
            if (this.Top > vector.Y) return false;
            if (this.Right < vector.X) return false;
            if (this.Bottom < vector.Y) return false;
            return true;
        }

    }
}