using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Panel for Dual-Stack.
    /// </summary>
    public sealed class DualStackPanel : Panel
    {
        
        const int W = 120;
        const int H = 80;

        const int X = 100;
        const int Y = 150;

        protected override Size ArrangeOverride(Size finalSize)
        {
            int i = -1;

            foreach (FrameworkElement child in base.Children)
            {
                i++;
                child.Arrange(new Rect(i * X, i % 2 * Y, W, H));
            }

            return new Size(i * X + X + X, Y + H);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            int i = -1;

            foreach (FrameworkElement child in base.Children)
            {
                i++;
                child.Measure(new Size(W, H));
            }

            return new Size(i * X + X + X, Y + H);
        }
    }
}