using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Phagocytosis.Elements
{
    /// <summary>
    /// Represents a guider.
    /// </summary>
    public sealed partial class Guider : UserControl
    {

        //@Construct
        /// <summary>
        /// Initializes a Guider. 
        /// </summary>
        public Guider()
        {
            this.InitializeComponent();
            this.ConstructFlowDirection();
            this.ConstructStrings();
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

            this.TextBlock001.Text = resource.GetString("Guider001");
            this.TextBlock002.Text = resource.GetString("Guider002");
            this.TextBlock003.Text = resource.GetString("Guider003");

            this.Click004Run.Text = resource.GetString("Click");
            this.ToDivide.Text = resource.GetString("Guider004");

            this.TextBlock005.Text = resource.GetString("Guider005");
            this.TextBlock006.Text = resource.GetString("Guider006");
        }

        /// <summary>
        /// Begin the story-board.
        /// </summary>
        public void Begin()
        {
            this.Storyboard.Begin();
        }

    }
}