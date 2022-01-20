using Phagocytosis.ViewModels;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Phagocytosis
{
    /// <summary>
    /// Represents a page used to display chapter.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;

        //@Converter
        private Symbol BooleanToMuteConverter(bool value) => value ? Symbol.Mute : Symbol.Volume;

        //@Construct
        /// <summary>
        /// Initializes a MainPage. 
        /// </summary>
        public MainPage()
        {
            this.InitializeComponent();
            this.ConstructFlowDirection();
            this.ConstructStrings();
            base.Loaded += async (s, e) =>
            {
                if (this.ViewModel.Chapters.Count != 0) return;
                IEnumerable<Chapter> chapters = await XML.ConstructChaptersFile();
                this.ViewModel.LoadFromProject(chapters);

                this.ViewModel.UnregisteLayout();
                this.ViewModel.RegisteLayout();
            };

            this.BackButton.Click += (s, e) => this.PlayButton.Flyout.Hide();
            this.MuteButton.Click += (s, e) => this.ViewModel.IsMuted = !this.ViewModel.IsMuted;
            this.AboutButton.Click += (s, e) => base.Frame.Navigate(typeof(AboutPage));
            this.LibraryButton.Click += (s, e) => base.Frame.Navigate(typeof(LibraryPage));
            this.EditButton.Click += (s, e) => base.Frame.Navigate(typeof(EditPage));
            this.LocalFolderButton.Click += async (s, e) =>
            {
                IStorageFolder folder = ApplicationData.Current.LocalFolder;
                await Launcher.LaunchFolderAsync(folder);
            };

            this.ListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is ChapterViewItem item)
                {
                    if (item.IsEnabled)
                    {
                        if (item.Index == this.ListView.SelectedIndex)
                        {
                            this.PlayButton.Flyout.Hide();
                            base.Frame.Navigate(typeof(DrawPage), item.Chapter);
                        }
                    }
                }
            };
        }

        // FlowDirection
        private void ConstructFlowDirection()
        {
            bool isRightToLeft = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            base.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        // Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            {
                this.DeveloperModeTextBlock.Text = resource.GetString("DeveloperMode");
                this.Click001Run.Text = resource.GetString("Click");
                this.ToEditRun.Text = resource.GetString("ToEdit");
                this.Click002Run.Text = resource.GetString("Click");
                this.ToOpenRun.Text = resource.GetString("ToOpen");
            }
        }

    }

    public sealed partial class MainPage : Page
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

            this.PlayButton.Focus(FocusState.Keyboard);

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