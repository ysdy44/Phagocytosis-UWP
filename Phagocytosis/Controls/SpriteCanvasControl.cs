using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Phagocytosis.Sprites;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Sprite for <see cref="CanvasAnimatedControl"/>.
    /// </summary>
    public sealed class SpriteCanvasControl : UserControl
    {

        //@Converter
        public Vector2 RotateConverter(double angle)
        {
            float radius = 128;
            return new Vector2
            {
                X = this.BugMap.Width / 4 + radius * (float)(-Math.Cos(angle) / 2),
                Y = this.BugMap.Height / 2 + radius * (float)(Math.Sin(angle) / 2)
            };
        }

        public int Index;
        private Spriter[] Sprites;
        readonly BugMap BugMap = new BugMap(200, 200);

        #region DependencyProperty


        /// <summary> Gets or set the library margin for <see cref="SpriteCanvasControl"/>. </summary>
        public Thickness LibraryMargin
        {
            get => (Thickness)base.GetValue(LibraryMarginProperty);
            set => base.SetValue(LibraryMarginProperty, value);
        }
        /// <summary> Identifies the <see cref = "SpriteCanvasControl.LibraryMargin" /> dependency property. </summary>
        public static readonly DependencyProperty LibraryMarginProperty = DependencyProperty.Register(nameof(LibraryMargin), typeof(Thickness), typeof(SpriteCanvasControl), new PropertyMetadata(new Thickness(12)));


        #endregion

        public bool IsResizing { get; set; }
        CanvasAnimatedControl CanvasControl = new CanvasAnimatedControl
        {
            Paused = false,
        };

        //@Constructs
        /// <summary>
        /// Initialize a SpriteCanvasControl.
        /// </summary>
        public SpriteCanvasControl()
        {
            // Initialize
            base.Content = this.CanvasControl;
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                int width = (int)this.ActualWidth;
                int height = (int)this.ActualHeight;

                bool can = this.BugMap.CanResize(width, height);
                if (can == false) return;

                this.BugMap.Resize(width, height);

                this.LibraryMargin = new Thickness(this.BugMap.Width / 3, 12, 12, 12);
            };


            this.CanvasControl.CreateResources += (sender, args) =>
            {
                Vector2 position = this.RotateConverter(0);
                this.Sprites = new Spriter[]
                {
                    new Spriter(this.CanvasControl, SpriteType.Cell, position, Spriter.GetLevel(SpriteType.Cell)),
                    new Spriter(this.CanvasControl, SpriteType.Bacteria, position, Spriter.GetLevel(SpriteType.Bacteria)),
                    new Spriter(this.CanvasControl, SpriteType.Virus, position, Spriter.GetLevel(SpriteType.Virus)),
                    new Spriter(this.CanvasControl, SpriteType.Paramecium, position, Spriter.GetLevel(SpriteType.Paramecium)),
                    new Spriter(this.CanvasControl, SpriteType.Leukocyte, position, Spriter.GetLevel(SpriteType.Leukocyte)),
                };
            };


            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.IsResizing) return;
                if (this.BugMap.IsResizing) return;

                Spriter sprite = this.Sprites[this.Index];
                CanvasCommandList renderTarget = new CanvasCommandList(this.CanvasControl);
                using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
                {
                    foreach (Bug bug in this.BugMap)
                    {
                        ds.FillCircle(bug.Position, 12, CanvasDrawingSessionExtensions.LowColor);
                    }
                    ds.DrawImage(sprite.Nuvleus, sprite.Offset);
                }
                args.DrawingSession.DrawDisplacementNuvleus(renderTarget);
                args.DrawingSession.DrawImage(sprite.Cytoplasm, sprite.Offset);
            };


            this.CanvasControl.Update += (sender, args) =>
            {
                if (this.IsResizing) return;
                if (this.BugMap.IsResizing) return;

                float totalTime = (float)args.Timing.TotalTime.TotalMilliseconds;
                float radians = totalTime / 360f / 4f;
                Vector2 position = this.RotateConverter(radians);
                foreach (Spriter item in this.Sprites)
                {
                    item.Position = position;
                }

                float elapsedTime = (float)args.Timing.ElapsedTime.TotalMilliseconds;
                this.BugMap.Update(elapsedTime);
            };
        }
        ~SpriteCanvasControl()
        {
            this.CanvasControl.RemoveFromVisualTree();
            this.CanvasControl = null;
        }


        public void Restart()
        {
            this.CanvasControl.ResetElapsedTime();

            this.Play();
        }
        public void Play()
        {
            this.CanvasControl.Paused = false;
        }
        public void Stop()
        {
            this.CanvasControl.Paused = true;
        }

    }
}