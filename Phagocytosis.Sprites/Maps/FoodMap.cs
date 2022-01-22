using System;
using System.Collections.Generic;
using System.Numerics;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Map of <see cref = "Food" />s.
    /// </summary>
    public sealed class FoodMap : List<Food>
    {

        readonly TimeSpan Span = TimeSpan.FromMilliseconds(4000);
        TimeSpan Last = TimeSpan.Zero;

        /// <summary> Gets map's maximum. </summary>
        public int Maximum { get; set; } = 6;
        /// <summary> Gets map's increase. </summary>
        public int Increase { get; set; } = 1;

        /// <summary> Gets map's width. </summary>
        public int Width => this.Restricteds.Width;

        /// <summary> Gets map's height. </summary>
        public int Height => this.Restricteds.Height;

        /// <summary> Gets map's center. </summary>
        public Vector2 Center => this.Restricteds.Center;

        /// <summary>
        /// Map of <see cref = "Rect2" />s.
        /// </summary>
        public readonly RectMap Restricteds;

        //@Constructs
        /// <summary>
        /// Initialize a FoodMap.
        /// </summary>
        /// <param name="width"> The width. </param>
        /// <param name="height"> The height. </param>
        public FoodMap(int width, int height)
        {
            this.Restricteds = new RectMap(width, height);
            for (int i = 0; i < this.Increase; i++)
            {
                this.Add();
            }
        }

        /// <summary>
        /// Resize.
        /// </summary>
        /// <param name="width"> The width. </param>
        /// <param name="height"> The height. </param>
        public void Resize(int width, int height)
        {
            this.Restricteds.Resize(width, height);

            base.Clear();
            for (int i = 0; i < this.Increase; i++)
            {
                this.Add();
            }
        }

        /// <summary>
        /// Update.
        /// </summary>
        /// <param name="elapsedTime"> The elapsed time. </param>
        public bool Update(TimeSpan totalTime)
        {
            if (base.Count != 0)
            {
                if (base.Count >= this.Maximum) return false;
                if (this.Last >= totalTime - this.Span) return false;
                this.Last = totalTime;
            }

            for (int i = 0; i < this.Increase; i++)
            {
                this.Add();
            }
            return true;
        }

        /// <summary>
        /// Add.
        /// </summary>
        public void Add()
        {
            Vector2 position;
            {
                do
                {
                    position = this.Restricteds.BugMap.RandomPosition();
                    foreach (Rect2 item in this.Restricteds)
                    {
                        if (item.Contains(position))
                        {
                            position = Vector2.Zero;
                            break;
                        }
                    }
                } while (position == Vector2.Zero);
            }
            base.Add(new Food(position));
        }

    }
}