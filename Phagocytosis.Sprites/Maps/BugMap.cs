using System;
using System.Collections.Generic;
using System.Numerics;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Map of <see cref = "Bug" />s.
    /// </summary>
    public sealed class BugMap : List<Bug>
    {

        private readonly Random Random = new Random(21688);

        /// <summary> Gets map's width. </summary>
        public int Width { get; private set; }

        /// <summary> Gets map's height. </summary>
        public int Height { get; private set; }

        /// <summary> Gets map's center. </summary>
        public Vector2 Center { get; private set; }

        /// <summary> Gets map's rect. </summary>
        public Rect2 Rect { get; private set; }

        /// <summary> Gets map's state for sizing. </summary>
        public bool IsResizing { get; private set; }

        //@Constructs
        /// <summary>
        /// Initialize a BugMap.
        /// </summary>
        /// <param name="width"> The width. </param>
        /// <param name="height"> The height. </param>
        public BugMap(int width, int height)
        {
            this.ResizeCore(width, height);
        }

        /// <summary>
        /// Returns whether resizing is allowed
        /// </summary>
        /// <param name="width"> The width. </param>
        /// <param name="height"> The height. </param>
        /// <returns> Return **true** if resizing is allowed, otherwise **false**. </returns>
        public bool CanResize(int width, int height)
        {
            if (Math.Abs(width - this.Width) > 20) return true;
            if (Math.Abs(height - this.Height) > 20) return true;
            return false;
        }

        /// <summary>
        /// Resize.
        /// </summary>
        /// <param name="width"> The width. </param>
        /// <param name="height"> The height. </param>
        public void Resize(int width, int height)
        {
            this.IsResizing = true;
            base.Clear();
            this.ResizeCore(width, height);
            this.IsResizing = false;
        }
        private void ResizeCore(int width, int height)
        {
            this.Width = width;
            this.Height = height;
            this.Center = new Vector2(width / 2f, height / 2f);
            this.Rect = new Rect2(0, 0, width, height);

            int bugs = Math.Max(10, width * height / 128 / 128);
            for (int i = 0; i < bugs; i++)
            {
                base.Add(new Bug(width, height, this.Random));
            }
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="elapsedTime"> The elapsed time. </param>
        public void Update(float elapsedTime)
        {
            if (this.IsResizing) return;

            foreach (Bug item in this)
            {
                item.Update(this.Width, this.Height, elapsedTime);
            }
        }

        /// <summary>
        /// Returns a point contained within the range.
        /// </summary>
        /// <param name="point"> The point. </param>
        /// <returns> Returns a point contained within the range. </returns>
        public Vector2 Range(Vector2 point)
        {
            point.X = Math.Max(0, Math.Min(this.Width, point.X));
            point.Y = Math.Max(0, Math.Min(this.Height, point.Y));
            return point;
        }

        /// <summary>
        /// Returns a random point.
        /// </summary>
        /// <returns> Returns a random point. </returns>
        public Vector2 RandomPosition()
        {
            return new Vector2(this.Random.Next(0, this.Width), this.Random.Next(0, this.Height));
        }

    }
}