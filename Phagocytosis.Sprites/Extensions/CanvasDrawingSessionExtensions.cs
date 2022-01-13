using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Extensions of <see cref = "CanvasDrawingSession" />.
    /// </summary>
    public static class CanvasDrawingSessionExtensions
    {

        public static readonly Color LowColor = Color.FromArgb(12, 255, 255, 255);
        public static readonly Color MediumColor = Color.FromArgb(125, 255, 255, 255);

        private static readonly BorderEffect DisplacementNuvleus = new BorderEffect
        {
            ExtendX = CanvasEdgeBehavior.Mirror,
            ExtendY = CanvasEdgeBehavior.Mirror,
            Source = new TurbulenceEffect
            {
                Octaves = 15,
                Size = new Vector2(100, 100)
            }
        };
        private static readonly BorderEffect DisplacementCytoplasm = new BorderEffect
        {
            ExtendX = CanvasEdgeBehavior.Mirror,
            ExtendY = CanvasEdgeBehavior.Mirror,
            Source = new TurbulenceEffect
            {
                Octaves = 4,
                Size = new Vector2(100, 100)
            }
        };


        public static void DrawDisplacementNuvleus(this CanvasDrawingSession drawingSession, ICanvasImage players)
        {
            drawingSession.DrawImage(new DisplacementMapEffect
            {
                XChannelSelect = EffectChannelSelect.Red,
                YChannelSelect = EffectChannelSelect.Green,
                Amount = 32f,
                Displacement = DisplacementNuvleus,
                Source = new Transform2DEffect
                {
                    TransformMatrix = Matrix3x2.CreateTranslation(new Vector2(-10)),
                    Source = players
                }
            });
        }

        public static void DrawDisplacementCytoplasm(this CanvasDrawingSession drawingSession, ICanvasImage players)
        {
            drawingSession.DrawImage(new DisplacementMapEffect
            {
                XChannelSelect = EffectChannelSelect.Red,
                YChannelSelect = EffectChannelSelect.Green,
                Amount = 16f,
                Displacement = DisplacementCytoplasm,
                Source = new Transform2DEffect
                {
                    TransformMatrix = Matrix3x2.CreateTranslation(new Vector2(-4)),
                    Source = players
                }
            });
        }


        public static void DrawBugMap(this CanvasDrawingSession drawingSession, BugMap map)
        {
            Rect2 rect = map.Rect;
            drawingSession.DrawRectangle(rect.X, rect.Y, rect.Width, rect.Height, Colors.White, 2);

            foreach (Bug bug in map)
            {
                drawingSession.FillCircle(bug.Position, 12, CanvasDrawingSessionExtensions.LowColor);
            }
        }

        public static void DrawRectMap(this CanvasDrawingSession drawingSession, RectMap map)
        {
            foreach (Rect2 item in map)
            {
                drawingSession.FillRectangle(item.X, item.Y, item.Width, item.Height, CanvasDrawingSessionExtensions.LowColor);
                drawingSession.DrawRectangle(item.X, item.Y, item.Width, item.Height, Colors.White, 2);
            }
        }

        public static void DrawMap(this CanvasDrawingSession drawingSession, FoodMap map)
        {
            foreach (Food item in map)
            {
                drawingSession.FillCircle(item.Position, Food.Radius, CanvasDrawingSessionExtensions.MediumColor);
                drawingSession.DrawCircle(item.Position, Food.Radius, Colors.White, 2);
            }
        }

        public static void DrawFlag(this CanvasDrawingSession drawingSession, string text, Spriter sprite, Color color)
        {
            drawingSession.DrawText(text, sprite.Position.X - 10, sprite.Position.Y - 26 - sprite.Radius - 20 * sprite.Opacity, color);
        }

    }
}