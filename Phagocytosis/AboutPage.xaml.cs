using Phagocytosis.Elements;
using Phagocytosis.ViewModels;
using System;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;
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
            this.ConstructFlowDirection();
            this.ConstructStrings();
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

            this.MeRun001.Text =
            this.MeRun002.Text =
            this.MeRun003.Text =
            this.MeRun004.Text =
            this.MeRun005.Text =
                resource.GetString("Me");

            this.NoneRun001.Text =
            this.NoneRun002.Text =
            this.NoneRun003.Text =
            this.NoneRun004.Text =
            this.NoneRun005.Text =
            this.NoneRun006.Text =
            this.NoneRun007.Text =
                resource.GetString("None");

            this.VersionTipRun.Text = resource.GetString("Version");
            this.VersionRun.Text = resource.GetString("$Version");

            {
                this.ProductionRun.Text = resource.GetString("Production");
                this.DesignRun.Text = resource.GetString("Design");
                this.ArtRun.Text = resource.GetString("Art");
                this.DevelopmentRun.Text = resource.GetString("Development");
                this.TestRun.Text = resource.GetString("Test");
                this.ManagementRun.Text = resource.GetString("Management");
                this.PromoteRun.Text = resource.GetString("Promote");
                this.LegalRun.Text = resource.GetString("Legal");
                this.LocalRun.Text = resource.GetString("Local");
                this.PublishingRun.Text = resource.GetString("Publishing");
                this.CommunityRun.Text = resource.GetString("Community");
                this.ThanksRun.Text = resource.GetString("Thanks");
            }

            {
                this.OpenSourceRun.Text = resource.GetString("OpenSource");
                string githubLink = resource.GetString("$GithubLink");
                this.OpenSourceHyperlink.NavigateUri = new Uri(githubLink);

                this.ReferenceRun.Text = resource.GetString("Reference");

                this.FeedbackRun.Text = resource.GetString("Feedback");
                string feedbackLink = resource.GetString("$FeedbackLink");
                this.FeedbackHyperlinkRun.Text = feedbackLink;
                this.FeedbackHyperlink.NavigateUri = new Uri("mailto:" + feedbackLink);
            }

            this.NoMoreRun.Text = resource.GetString("noMore");

            {
                this.DeveloperModeTextBlock.Text = resource.GetString("DeveloperMode");
                this.Click001Run.Text = resource.GetString("Click");
                this.ToEditRun.Text = resource.GetString("ToEdit");
                this.Click002Run.Text = resource.GetString("Click");
                this.ToOpenRun.Text = resource.GetString("ToOpen");
            }
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