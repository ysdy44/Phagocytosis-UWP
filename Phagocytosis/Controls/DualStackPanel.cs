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
            int i = 0;

            foreach (FrameworkElement child in base.Children)
            {
                child.Arrange(new Rect(i * X, i % 2 * Y, W, H));
                i++;
            }

            return new Size(i * X + X, Y + H);
        }
        protected override Size MeasureOverride(Size availableSize)
        {
            int i = 0;

            foreach (FrameworkElement child in base.Children)
            {
                child.Measure(new Size(W, H));
                i++;
            }

            return new Size(i * X + X, Y + H);
        }
    }
}