using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Phagocytosis.Sprites;
using System;
using System.Numerics;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Background for <see cref="CanvasAnimatedControl"/>.
    /// </summary>
    public sealed class BackgroundCanvasControl : UserControl
    {

        //@Converter
        public Vector2 RotateConverter(double angle) => new Vector2
        {
            X = this.BugMap.Width * (float)(0.5d - Math.Cos(angle) / 2),
            Y = this.BugMap.Height * (float)(0.5d + Math.Sin(angle) / 2)
        };

        private Spriter Player;
        readonly BugMap BugMap = new BugMap(200, 200);

        public bool IsResizing { get; set; }
        CanvasAnimatedControl CanvasControl = new CanvasAnimatedControl
        {
            Paused = false,
        };

        //@Constructs
        /// <summary>
        /// Initialize a BackgroundCanvasControl.
        /// </summary>
        public BackgroundCanvasControl()
        {
            // Initialize
            base.Content = this.CanvasControl;
            base.SizeChanged += (s, e) =>
            {
                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                int width = (int)base.ActualWidth;
                int height = (int)base.ActualHeight;

                bool can = this.BugMap.CanResize(width, height);
                if (can == false) return;

                this.BugMap.Resize(width, height);
            };


            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Player = new Spriter(this.CanvasControl, SpriteType.Player, this.RotateConverter(0), 192 * 25 * 5);
            };


            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.IsResizing) return;
                if (this.BugMap.IsResizing) return;
                if (this.Player == null) return;

                CanvasCommandList renderTarget = new CanvasCommandList(this.CanvasControl);
                using (CanvasDrawingSession ds = renderTarget.CreateDrawingSession())
                {
                    foreach (Bug bug in this.BugMap)
                    {
                        ds.FillCircle(bug.Position, 12, CanvasDrawingSessionExtensions.LowColor);
                    }
                    ds.DrawImage(this.Player.Nuvleus, this.Player.Offset);
                }
                args.DrawingSession.DrawDisplacementNuvleus(renderTarget);
                args.DrawingSession.DrawImage(this.Player.Cytoplasm, this.Player.Offset);
            };


            this.CanvasControl.Update += (sender, args) =>
            {
                if (this.IsResizing) return;
                if (this.BugMap.IsResizing) return;
                if (this.Player == null) return;

                float totalTime = (float)args.Timing.TotalTime.TotalMilliseconds;
                float radians = totalTime / 360f / 16f;
                this.Player.Position = this.RotateConverter(radians);

                float elapsedTime = (float)args.Timing.ElapsedTime.TotalMilliseconds;
                this.BugMap.Update(elapsedTime);
            };
        }
        ~BackgroundCanvasControl()
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