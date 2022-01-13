using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Windows.UI;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Extensions of <see cref = "ICanvasResourceCreator" />.
    /// </summary>
    public static partial class CanvasResourceCreatorExtensions
    {

        private static readonly CanvasGradientStop[] CellNuvleus = new CanvasGradientStop[]
        {
            new CanvasGradientStop
            {
                Position = 0,
                Color = Color.FromArgb(58, 241, 175, 190),
            },
            new CanvasGradientStop
            {
                Position = 0.2f,
                Color = Color.FromArgb(88, 210, 175, 190),
            },
            new CanvasGradientStop
            {
                Position = 0.5f,
                Color = Color.FromArgb(122, 80, 171, 171),
            },
            new CanvasGradientStop
            {
                Position = 0.8f,
                Color = Color.FromArgb(156, 1, 210, 244),
            },
            new CanvasGradientStop
            {
                Position = 0.95f,
                Color = Color.FromArgb(255, 189, 219, 232),
            },
            new CanvasGradientStop
            {
                Position = 1,
                Color = Colors.White
            },
        };
        private static readonly CanvasGradientStop[] CellCytoplasm = new CanvasGradientStop[]
        {
            new CanvasGradientStop
            {
                Position = 0.2f,
                Color = Color.FromArgb(255, 199, 231, 148),
            },
            new CanvasGradientStop
            {
                Position = 0.65f,
                Color = Color.FromArgb(255, 93, 231, 189),
            },
            new CanvasGradientStop
            {
                Position = 1,
                Color = Color.FromArgb(255, 167, 231, 207),
            },
        };

        private static readonly CanvasGradientStop[] BacteriaNuvleus = new CanvasGradientStop[]
        {
            new CanvasGradientStop
            {
                Position = 0,
                Color = Color.FromArgb(58, 255, 175, 255),
            },
            new CanvasGradientStop
            {
                Position = 0.2f,
                Color = Color.FromArgb(88, 255, 175, 255),
            },
            new CanvasGradientStop
            {
                Position = 0.5f,
                Color = Color.FromArgb(122, 255, 134, 255),
            },
            new CanvasGradientStop
            {
                Position = 0.8f,
                Color = Color.FromArgb(156, 255, 122, 255),
            },
            new CanvasGradientStop
            {
                Position = 0.95f,
                Color = Color.FromArgb(255, 255, 219, 255),
            },
            new CanvasGradientStop
            {
                Position = 1,
                Color = Colors.White
            },
        };
        private static readonly CanvasGradientStop[] BacteriaCytoplasm = new CanvasGradientStop[]
        {
            new CanvasGradientStop
            {
                Position = 0.2f,
                Color = Color.FromArgb(255, 231, 146, 168),
            },
            new CanvasGradientStop
            {
                Position = 0.65f,
                Color = Color.FromArgb(255, 231, 93, 189),
            },
            new CanvasGradientStop
            {
                Position = 1,
                Color = Color.FromArgb(255, 231, 167, 207),
            },
        };

        private static readonly CanvasGradientStop[] ParameciumCytoplasm = new CanvasGradientStop[]
        {
            new CanvasGradientStop
            {
                Position = 0,
                Color = Color.FromArgb(255, 8, 48, 11),
            },
            new CanvasGradientStop
            {
                Position = 0.22f,
                Color = Color.FromArgb(255, 12, 62, 11),
            },
            new CanvasGradientStop
            {
                Position = 0.39f,
                Color = Color.FromArgb(255, 24, 76, 13),
            },
            new CanvasGradientStop
            {
                Position = 0.71f,
                Color = Color.FromArgb(255, 69, 128, 20),
            },
            new CanvasGradientStop
            {
                Position = 1,
                Color = Color.FromArgb(255, 124, 168, 45),
            },
        };

        private static readonly CanvasGradientStop[] PrionCytoplasm = new CanvasGradientStop[]
        {
            new CanvasGradientStop
            {
                Position = 0,
                Color = Color.FromArgb(209, 255, 31, 31),
            },
            new CanvasGradientStop
            {
                Position = 0.25f,
                Color = Color.FromArgb(0, 254, 254, 246),
            },
            new CanvasGradientStop
            {
                Position = 0.81f,
                Color = Color.FromArgb(84, 251, 255, 255),
            },
            new CanvasGradientStop
            {
                Position = 1,
                Color = Color.FromArgb(189, 255, 255, 255),
             },
        };

        private static readonly CanvasGradientStop[] StaphylococcusAureusNuvleus = new CanvasGradientStop[]
        {
             new CanvasGradientStop
             {
                Position = 0,
                Color = Color.FromArgb(255 , 242, 211, 0),
             },
             new CanvasGradientStop
             {
                 Position = 0.3f,
                 Color = Color.FromArgb(255 , 242, 211, 0),
             },
             new CanvasGradientStop
             {
                 Position = 0.35f,
                 Color = Color.FromArgb(222, 237, 207, 0),
             },
             new CanvasGradientStop
             {
                 Position = 0.80f,
                     Color = Color.FromArgb(186 , 255, 231, 64),
             },
             new CanvasGradientStop
             {
                 Position = 0.85f,
                 Color = Color.FromArgb(255 , 255, 231, 0),
              },
             new CanvasGradientStop
             {
                 Position = 1,
                 Color = Color.FromArgb(255 , 255, 255, 255),
              },
        };

    }
}