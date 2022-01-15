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