using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Phagocytosis.Elements
{
    /// <summary>
    /// Represents a scorer.
    /// </summary>
    public sealed partial class Scorer : UserControl
    {

        #region DependencyProperty


        /// <summary> Gets or set the count for <see cref="Scorer"/>'s left. </summary>
        public int LeftCount
        {
            get => (int)base.GetValue(LeftCountProperty);
            set => base.SetValue(LeftCountProperty, value);
        }
        /// <summary> Identifies the <see cref = "Scorer.LeftCount" /> dependency property. </summary>
        public static readonly DependencyProperty LeftCountProperty = DependencyProperty.Register(nameof(LeftCount), typeof(int), typeof(Scorer), new PropertyMetadata(0, (sender, e) =>
        {
            Scorer control = (Scorer)sender;

            if (e.NewValue is int value)
            {
                control.Update(value, control.RightCount);
            }
        }));


        /// <summary> Gets or set the count for <see cref="Scorer"/>'s right. </summary>
        public int RightCount
        {
            get => (int)base.GetValue(RightCountProperty);
            set => base.SetValue(RightCountProperty, value);
        }
        /// <summary> Identifies the <see cref = "Scorer.RightCount" /> dependency property. </summary>
        public static readonly DependencyProperty RightCountProperty = DependencyProperty.Register(nameof(RightCount), typeof(int), typeof(Scorer), new PropertyMetadata(0, (sender, e) =>
        {
            Scorer control = (Scorer)sender;

            if (e.NewValue is int value)
            {
                control.Update(control.LeftCount, value);
            }
        }));


        #endregion

        //@Construct
        /// <summary>
        /// Initializes a Scorer. 
        /// </summary>
        public Scorer()
        {
            this.InitializeComponent();
        }

        private void Update(int left, int right)
        {
            double width = base.ActualWidth;
            bool leftIsZero = left <= 0;
            bool rightIsZero = right <= 0;

            if (leftIsZero && rightIsZero)
            {
                this.LeftRectangle.Width =
                this.RightRectangle.Width = width / 2;
            }
            else if (leftIsZero)
            {
                this.LeftRectangle.Width = 0;
                this.RightRectangle.Width = width;
            }
            else if (rightIsZero)
            {
                this.LeftRectangle.Width = width;
                this.RightRectangle.Width = 0;
            }
            else
            {
                int count = left + right;
                this.LeftRectangle.Width = width * left / count;
                this.RightRectangle.Width = width * right / count;
            }
        }

    }
}