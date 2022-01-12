using System.Numerics;
using Windows.UI.Xaml;

namespace Phagocytosis.Elements
{
    internal class ControllerVelocity
    {
        public bool IsLeftFrist;
        public bool IsLeft;
        public bool IsRight;

        public bool IsTopFrist;
        public bool IsTop;
        public bool IsBottom;

        public Vector2 GetVector() => this.GetVector((this.IsLeft == false && this.IsRight == false) ? HorizontalAlignment.Center : IsLeft == false ? HorizontalAlignment.Right : this.IsRight == false ? HorizontalAlignment.Left : this.IsLeftFrist ? HorizontalAlignment.Left : HorizontalAlignment.Right, (IsTop == false && this.IsBottom == false) ? VerticalAlignment.Center : this.IsTop == false ? VerticalAlignment.Bottom : IsBottom == false ? VerticalAlignment.Top : this.IsTopFrist ? VerticalAlignment.Top : VerticalAlignment.Bottom);
        public Vector2 GetVector(HorizontalAlignment x, VerticalAlignment y)
        {
            switch (x)
            {
                case HorizontalAlignment.Left:
                    switch (y)
                    {
                        case VerticalAlignment.Top:
                            return new Vector2(-0.70710678118654752440084436210485f, -0.70710678118654752440084436210485f);
                        case VerticalAlignment.Center:
                            return new Vector2(-1, 0);
                        case VerticalAlignment.Bottom:
                            return new Vector2(-0.70710678118654752440084436210485f, 0.70710678118654752440084436210485f);
                        default:
                            return Vector2.Zero;
                    }
                case HorizontalAlignment.Center:
                    switch (y)
                    {
                        case VerticalAlignment.Top:
                            return new Vector2(0, -1);
                        case VerticalAlignment.Center:
                            return Vector2.Zero;
                        case VerticalAlignment.Bottom:
                            return new Vector2(0, 1);
                        default:
                            return Vector2.Zero;
                    }
                case HorizontalAlignment.Right:
                    switch (y)
                    {
                        case VerticalAlignment.Top:
                            return new Vector2(0.70710678118654752440084436210485f, -0.70710678118654752440084436210485f);
                        case VerticalAlignment.Center:
                            return new Vector2(1, 0);
                        case VerticalAlignment.Bottom:
                            return new Vector2(0.70710678118654752440084436210485f, 0.70710678118654752440084436210485f);
                        default:
                            return Vector2.Zero;
                    }
                default:
                    return Vector2.Zero;
            }
        }
    }
}