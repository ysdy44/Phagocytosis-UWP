using Phagocytosis.Sprites;
using Phagocytosis.ViewModels;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Input;
using Windows.UI.Xaml.Controls;
using Microsoft.Graphics.Canvas.UI.Xaml;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Base of <see cref="CanvasAnimatedControl"/>.
    /// </summary>
    public abstract class BaseCanvasControl : UserControl
    {

        //@Abstract
        public abstract ICanvasResourceCreatorWithDpi ResourceCreator { get; }

        private float StartingScale = 1;
        protected float Scale2 = 1;
        private Vector2 StartingPosition;
        protected Vector2 Position;
        protected Vector2 Center;
        protected Matrix3x2 Transform = Matrix3x2.Identity;

        protected FoodMap Map = new FoodMap(800, 800);

        protected readonly IList<Spriter> FriendSprites = new List<Spriter>();
        protected readonly IList<Spriter> EnemySprites = new List<Spriter>();

        //@Constructs
        /// <summary>
        /// Initialize a BaseCanvasControl.
        /// </summary>
        public BaseCanvasControl()
        {
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                this.Center = new Vector2
                {
                    X = (float)e.NewSize.Width / 2,
                    Y = (float)e.NewSize.Height / 2
                };
            };
        }

        public Chapter Save()
        {
            return new Chapter
            {
                Width = this.Map.Width,
                Height = this.Map.Height,
                MaximumFoods = 6,
                IncreaseFoods = 1,
                Restricteds = this.Map.Restricteds.ToList(),
                FriendSprites = this.FriendSprites.Select(e => e.Save()).ToList(),
                EnemySprites = this.EnemySprites.Select(e => e.Save()).ToList(),
            };
        }

        protected void Load(Chapter chapter)
        {
            this.Map = new FoodMap(chapter.Width, chapter.Height)
            {
                Maximum = chapter.MaximumFoods,
                Increase = chapter.IncreaseFoods,
            };
            if (chapter.Restricteds != null)
            {
                foreach (Rect2 item in chapter.Restricteds)
                {
                    this.Map.Restricteds.Add(item);
                }
            }

            this.FriendSprites.Clear();
            if (chapter.FriendSprites != null)
            {
                foreach (ISprite item in chapter.FriendSprites)
                {
                    if (Spriter.Load(this.ResourceCreator, item) is Spriter sprite)
                    {
                        this.FriendSprites.Add(sprite);
                    }
                }
            }

            this.EnemySprites.Clear();
            if (chapter.EnemySprites != null)
            {
                foreach (ISprite item in chapter.EnemySprites)
                {
                    if (Spriter.Load(this.ResourceCreator, item) is Spriter sprite)
                    {
                        this.EnemySprites.Add(sprite);
                    }
                }
            }
        }

        private float Clamp(float value) => Math.Max(0.1f, Math.Min(10f, value));
        public void ZoomIn() => this.Scale2 = this.Clamp(this.Scale2 * 1.1f);
        public void ZoomOut() => this.Scale2 = this.Clamp(this.Scale2 / 1.1f);

        protected void ZoomStarted()
        {
            this.StartingScale = this.Scale2;
            this.StartingPosition = this.Position;
        }
        protected void ZoomDelta(ManipulationDelta cumulative)
        {
            this.Scale2 = this.Clamp(this.StartingScale * cumulative.Scale);
            this.Position = this.StartingPosition + cumulative.Translation.ToVector2();
        }

        public void ZoomSprite(bool isFriend, bool isZoomIn)
        {
            foreach (Spriter item in isFriend ? this.FriendSprites : this.EnemySprites)
            {
                int level = isZoomIn ? item.Level * 2 : item.Level / 2;
                item.Upgrade(level);
            }
        }

    }
}