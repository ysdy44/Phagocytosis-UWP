using Phagocytosis.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Phagocytosis
{
    /// <summary> 
    /// Represents a page for collecting.
    /// </summary>
    public sealed partial class LibraryPage : Page
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;

        //@Constructs
        /// <summary>
        /// Initialize a LibraryPage.
        /// </summary>
        public LibraryPage()
        {
            this.InitializeComponent();
            this.BackButton.Click += (s, e) =>
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            };
            this.FlipView.SelectionChanged += (s, e) => this.CanvasControl.Index = this.FlipView.SelectedIndex;
        }
    }

    public sealed partial class LibraryPage : Page
    {

        //@BackRequested
        /// <summary> The current page no longer becomes an active page. </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.CanvasControl.Stop();

            this.ViewModel.OnSuspending -= this.OnSuspending;
            this.ViewModel.OnResuming -= this.OnResuming;
        }
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ViewModel.PlayForegroundBGM();

            this.CanvasControl.Restart();

            this.ViewModel.OnSuspending += this.OnSuspending;
            this.ViewModel.OnResuming += this.OnResuming;
        }

        private void OnSuspending()
        {
            this.CanvasControl.Stop();
        }
        private void OnResuming()
        {
            this.CanvasControl.Play();
        }

    }
}