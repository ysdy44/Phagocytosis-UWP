using System;
using System.ComponentModel;
using Windows.Foundation;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Phagocytosis.ViewModels
{
    /// <summary> 
    /// Represents a ViewModel that contains some methods of the application
    /// </summary>
    public partial class ViewModel : INotifyPropertyChanged, IDisposable
    {

        Size LayoutSize = new Size(1920, 1080);
        readonly DispatcherTimer LayoutTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(250)
        };

        /// <summary>
        /// Registe the layout.
        /// </summary>
        public void RegisteLayout()
        {
            Rect rect = Window.Current.Bounds;
            this.LayoutSize.Width = rect.Width;
            this.LayoutSize.Height = rect.Height;
            this.OnPropertyChanged(nameof(this.WindowWidth)); // Notify 
            this.OnPropertyChanged(nameof(this.WindowHeight)); // Notify 

            Window.Current.SizeChanged += this.Window_SizeChanged;
            this.LayoutTimer.Tick += this.LayoutTimer_Tick;
        }
        /// <summary>
        /// Unregiste the layout.
        /// </summary>
        public void UnregisteLayout()
        {
            Window.Current.SizeChanged -= this.Window_SizeChanged;
            this.LayoutTimer.Tick -= this.LayoutTimer_Tick;
        }

        private void Window_SizeChanged(object sender, WindowSizeChangedEventArgs e)
        {
            this.LayoutSize = e.Size;
            this.IsResizing = true;
            this.OnPropertyChanged(nameof(IsResizing)); // Notify 

            this.LayoutTimer.Stop();
            this.LayoutTimer.Start();
        }
        private void LayoutTimer_Tick(object sender, object e)
        {
            if (this.IsResizing)
            {
                this.IsResizing = false;
                this.OnPropertyChanged(nameof(WindowWidth)); // Notify 
                this.OnPropertyChanged(nameof(WindowHeight)); // Notify 
            }
            else
            {
                this.LayoutTimer.Stop();
                this.OnPropertyChanged(nameof(IsResizing)); // Notify 
            }
        }



        /// <summary> Gets windows's state for sizing. </summary>
        public bool IsResizing { get; private set; }

        /// <summary> Gets or sets the windows width. </summary>
        public double WindowWidth => this.LayoutSize.Width;

        /// <summary> Gets or sets the windows height. </summary>
        public double WindowHeight => this.LayoutSize.Height;

    }
}