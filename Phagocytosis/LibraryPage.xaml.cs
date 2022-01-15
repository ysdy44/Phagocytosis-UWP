using Phagocytosis.ViewModels;
using Windows.ApplicationModel.Resources;
using Windows.Globalization;
using Windows.UI.Xaml;
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
            this.ConstructFlowDirection();
            this.ConstructStrings();
            this.BackButton.Click += (s, e) =>
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            };
            this.FlipView.SelectionChanged += (s, e) => this.CanvasControl.Index = this.FlipView.SelectedIndex;
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
                this.CellRun.Text = resource.GetString("Cell");
                this.BacteriaRun.Text = resource.GetString("Bacteria");
                this.VirusRun.Text = resource.GetString("Virus");
                this.ParameciumRun.Text = resource.GetString("Paramecium");
                this.LeukocyteRun.Text = resource.GetString("Leukocyte");
            }

            {
                this.Speed001Run.Text =
                this.Speed002Run.Text =
                this.Speed003Run.Text =
                this.Speed004Run.Text =
                this.Speed005Run.Text =
                    resource.GetString("Speed");

                this.Level001Run.Text =
                this.Level002Run.Text =
                this.Level003Run.Text =
                this.Level004Run.Text =
                this.Level005Run.Text =
                    resource.GetString("Level");

                this.Count001Run.Text =
                this.Count002Run.Text =
                this.Count003Run.Text =
                this.Count004Run.Text =
                this.Count005Run.Text =
                    resource.GetString("Count");
            }

            this.Cell001Run.Text = resource.GetString("Cell001");
            this.Cell002Run.Text = resource.GetString("Cell002");
            this.Cell003Run.Text = resource.GetString("Cell003");
            this.Cell004Run.Text = resource.GetString("Cell004");
            this.Cell005Run.Text = resource.GetString("Cell005");

            this.Bacteria001Run.Text = resource.GetString("Bacteria001");
            this.Bacteria002Run.Text = resource.GetString("Bacteria002");
            this.Bacteria003Run.Text = resource.GetString("Bacteria003");
            this.Bacteria004Run.Text = resource.GetString("Bacteria004");
            this.Bacteria005Run.Text = resource.GetString("Bacteria005");

            this.Virus001Run.Text = resource.GetString("Virus001");
            this.Virus002Run.Text = resource.GetString("Virus002");
            this.Virus003Run.Text = resource.GetString("Virus003");
            this.Virus004Run.Text = resource.GetString("Virus004");
            this.Virus005Run.Text = resource.GetString("Virus005");

            this.Paramecium001Run.Text = resource.GetString("Paramecium001");
            this.Paramecium002Run.Text = resource.GetString("Paramecium002");
            this.Paramecium003Run.Text = resource.GetString("Paramecium003");
            this.Paramecium004Run.Text = resource.GetString("Paramecium004");
            //  this.Paramecium005Run.Text = resource.GetString("Paramecium005");

            this.Leukocyte001Run.Text = resource.GetString("Leukocyte001");
            this.Leukocyte002Run.Text = resource.GetString("Leukocyte002");
            this.Leukocyte003Run.Text = resource.GetString("Leukocyte003");
            this.Leukocyte004Run.Text = resource.GetString("Leukocyte004");
            this.Leukocyte005Run.Text = resource.GetString("Leukocyte005");
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