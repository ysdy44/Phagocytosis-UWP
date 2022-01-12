using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Phagocytosis.Elements
{
    /// <summary>
    /// Represents an auto-scrolling ScrollViewer.
    /// </summary>
    public sealed class TrackScrollViewer
    {

        bool IsDirectFreedom;
        bool IsDirectSliderScale;
        bool IsDirectTouchScale;
        bool IsDirectWheelMove;
        bool IsDirectTouchMove;

        readonly ScrollViewer ScrollViewer;
        readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(25)
        };


        /// <summary> Gets or sets the state of playback. </summary>
        public bool IsPlaying
        {
            get => this.isPlaying;
            set
            {
                this.isPlaying = value;
                this.IsPlayingCore = value;
            }
        }
        private bool isPlaying;

        /// <summary> Gets or sets the state of playback. </summary>
        private bool IsPlayingCore
        {
            get => this.isPlayingCore;
            set
            {
                if (value)
                {
                    this.Timer.Start();
                }
                else
                {
                    this.Timer.Stop();
                }

                this.isPlayingCore = value;
            }
        }
        private bool isPlayingCore;


        //@Construct
        /// <summary>
        /// Initializes a TrackScrollViewer. 
        /// </summary>
        /// <param name="scrollViewer"> The scrollViewer. </param>
        public TrackScrollViewer(ScrollViewer scrollViewer)
        {
            this.ScrollViewer = scrollViewer;
            this.ScrollViewer.Tapped += (s, e) => this.IsPlaying = !this.IsPlaying;
            this.ScrollViewer.RightTapped += (s, e) => this.IsPlaying = !this.IsPlaying;
            this.ScrollViewer.Holding += (s, e) => this.IsPlaying = !this.IsPlaying;

            // Wheel Move
            // Wheel Scale
            // Touch Move
            // Touch Scale
            // Slider Scale
            this.Timer.Tick += (s, e) =>
            {
                if (this.IsDirectFreedom) return;
                if (this.IsDirectSliderScale) return;
                if (this.IsDirectTouchScale) return;
                if (this.IsDirectWheelMove) return;
                if (this.IsDirectTouchMove) return;

                double offset = this.ScrollViewer.VerticalOffset;
                if (offset < this.ScrollViewer.ScrollableHeight - 12)
                {
                    bool disableAnimation = true;
                    this.ScrollViewer.ChangeView(null, 1 + offset, null, disableAnimation);
                }
                else
                {
                    this.IsPlaying = false;
                }
            };


            this.ScrollViewer.SizeChanged += (s, e) =>
            {
                if (this.IsDirectSliderScale) return;
                if (this.IsDirectTouchScale) return;

                if (e.NewSize == Size.Empty) return;
                if (e.NewSize == e.PreviousSize) return;

                if (this.ScrollViewer.Content is FrameworkElement element)
                {
                    element.Margin = new Thickness(0, e.NewSize.Height / 2, 0, e.NewSize.Height / 2);
                }
            };

            // Wheel Move
            // Wheel Scale
            // Touch Move
            // Touch Scale
            // Slider Scale
            this.ScrollViewer.ViewChanging += (s, e) =>
            {
                if (this.IsDirectFreedom) return;
                if (this.IsDirectSliderScale) return;
                if (this.IsDirectTouchScale) return;
                if (this.IsDirectTouchMove) return;

                this.IsDirectWheelMove = true;
            };
            // Wheel Move
            // Wheel Scale
            // Touch Move
            // Touch Scale
            // Slider Scale
            this.ScrollViewer.ViewChanged += (s, e) =>
            {
                if (this.IsDirectFreedom) return;
                if (this.IsDirectSliderScale) return;
                if (this.IsDirectTouchScale) return;

                if (this.IsDirectTouchMove)
                {
                }
                else
                {
                    this.IsPlayingCore = this.IsPlaying;
                    this.IsDirectWheelMove = false;
                }
            };

            // Touch Move
            this.ScrollViewer.DirectManipulationStarted += (s, e) =>
            {
                if (this.IsDirectFreedom) return;
                if (this.IsDirectSliderScale) return;
                if (this.IsDirectTouchScale) return;

                this.IsDirectTouchMove = true;
                this.IsPlayingCore = false;
            };
            // Touch Move
            // Touch Scale
            this.ScrollViewer.DirectManipulationCompleted += (s, e) =>
            {
                if (this.IsDirectFreedom) return;
                if (this.IsDirectSliderScale) return;
                if (this.IsDirectTouchScale) return;

                if (this.IsDirectTouchMove)
                {
                    this.IsDirectTouchMove = false;
                    this.IsPlayingCore = this.IsPlaying;
                }
            };


            // Touch Scale
            // this.TrackCanvas.ManipulationStarted += (s, e) =>
            // {
            //     this.IsDirectTouchScale = this.TrackCanvas.ManipulationMode.HasFlag(ManipulationModes.Scale);
            // };
            // // Touch Scale
            // this.TrackCanvas.ManipulationDelta += (s, e) =>
            // {
            //     if (this.IsDirectTouchScale)
            //     {
            //     }
            // };
            // // Touch Scale
            // this.TrackCanvas.ManipulationCompleted += (s, e) =>
            // {
            //     this.IsDirectTouchScale = false;
            // };
            //
            //
            //  // Slider Scale
            //  this.ScaleSlider.ValueChangedStarted += (s, e) =>
            //  {
            //      this.IsPlayingCore = false;
            //      this.IsDirectSliderScale = true;
            //  };
            //  // Slider Scale
            //  this.ScaleSlider.ValueChangedDelta += (s, e) =>
            //  {
            //      if (this.IsDirectFreedom) return;
            //
            //  };
            //  // Slider Scale
            //  this.ScaleSlider.ValueChangedCompleted += (s, e) =>
            //  {
            //      this.IsPlayingCore = this.IsPlaying;
            //      this.IsDirectSliderScale = false;
            //  };
        }

    }
}