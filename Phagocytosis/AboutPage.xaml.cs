using Phagocytosis.Elements;
using Phagocytosis.ViewModels;
using System;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Phagocytosis
{
    /// <summary> 
    /// Represents a page for display about.
    /// </summary>
    public sealed partial class AboutPage : Page
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;

        readonly TrackScrollViewer TrackScrollViewer;

        //@Constructs
        /// <summary>
        /// Initialize a AboutPage.
        /// </summary>
        public AboutPage()
        {
            this.InitializeComponent();
            this.TrackScrollViewer = new TrackScrollViewer(this.ScrollViewer);
            base.Unloaded += (s, e) => this.TrackScrollViewer.IsPlaying = false;
            base.Loaded += (s, e) => this.TrackScrollViewer.IsPlaying = true;
            this.BackButton.Click += (s, e) =>
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            };
            this.DeveloperButton.Click += (s, e) => this.ViewModel.Developer = Visibility.Visible;
            this.DeveloperButton2.Click += (s, e) => this.ViewModel.Developer = Visibility.Collapsed;
            this.EditButton.Click += (s, e) => base.Frame.Navigate(typeof(EditPage));
            this.LocalFolderButton.Click += async (s, e) =>
            {
                IStorageFolder folder = ApplicationData.Current.LocalFolder;
                await Launcher.LaunchFolderAsync(folder);
            };
        }
    }

    public sealed partial class AboutPage : Page
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