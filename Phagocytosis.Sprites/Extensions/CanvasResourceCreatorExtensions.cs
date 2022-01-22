using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using System;
using System.Numerics;
using Windows.UI;

namespace Phagocytosis.Sprites
{
    /// <summary>
    /// Extensions of <see cref = "ICanvasResourceCreator" />.
    /// </summary>
    public static partial class CanvasResourceCreatorExtensions
    {

        private static CellRenderTarget RenderCellCore(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin, CanvasGradientStop[] nuvleusStops, CanvasGradientStop[] cytoplasmStops)
        {
            float radius1 = radius - 1;
            float radius2 = 2 * (float)Math.Pow(level, 0.333333333333333333333333333333333f);

            CanvasRenderTarget nuvleus = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = nuvleus.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius1, new CanvasRadialGradientBrush(resourceCreator, nuvleusStops)
                {
                    Center = origin,
                    RadiusX = radius1,
                    RadiusY = radius1,
                });
                ds.DrawCircle(origin, radius1, Colors.White, 2);
            }

            CanvasRenderTarget cytoplasm = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = cytoplasm.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius2, new CanvasRadialGradientBrush(resourceCreator, cytoplasmStops)
                {
                    Center = origin,
                    RadiusX = radius2,
                    RadiusY = radius2,
                });
                ds.DrawCircle(origin, radius2, Colors.White, 2);
            }

            return new CellRenderTarget
            {
                Nuvleus = nuvleus,
                Cytoplasm = cytoplasm
            };
        }


        public static CellRenderTarget RenderCell(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin) =>
            resourceCreator.RenderCellCore(level, radius, diameter, origin,
                CanvasResourceCreatorExtensions.CellNuvleus, CanvasResourceCreatorExtensions.CellCytoplasm);


        public static CellRenderTarget RenderBacteria(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin) =>
            resourceCreator.RenderCellCore(level, radius, diameter, origin,
                CanvasResourceCreatorExtensions.BacteriaNuvleus, CanvasResourceCreatorExtensions.BacteriaCytoplasm);


        public static CellRenderTarget RenderVirus(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin)
        {
            float radius2 = 2 * (float)Math.Pow(level, 0.433333333333333333333333333333333f);

            CanvasRenderTarget nuvleus = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = nuvleus.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius2, new CanvasRadialGradientBrush(resourceCreator, Colors.Black, Colors.Red)
                {
                    Center = origin,
                    RadiusX = radius2,
                    RadiusY = radius2,
                });
            }

            CanvasRenderTarget cytoplasm = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = cytoplasm.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius - 2, Color.FromArgb(150, 0, 0, 0));
                ds.DrawCircle(origin, radius - 2, Colors.Black, 2);
                ds.DrawCircle(origin, radius2, Color.FromArgb(150, 255, 0, 0), 2);
            }

            return new CellRenderTarget
            {
                Nuvleus = nuvleus,
                Cytoplasm = cytoplasm
            };
        }


        public static CellRenderTarget RenderParamecium(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin)
        {
            float radius2 = 2 * (float)Math.Pow(level, 0.4633333333333333333333333333333333f);
            float offset = radius - radius2;

            float radius3 = radius2 - offset;

            float strokeWidth = offset / 4 + 6;
            float strokeWidth2 = offset / 12;

            CanvasRenderTarget nuvleus = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = nuvleus.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius, Color.FromArgb(54, 196, 230, 143));
                ds.DrawCircle(origin, radius2, Color.FromArgb(188, 196, 230, 143), strokeWidth);
                ds.DrawCircle(origin, radius2, Colors.White, strokeWidth2, new CanvasStrokeStyle
                {
                    DashStyle = CanvasDashStyle.Dash
                });
            }

            CanvasRenderTarget cytoplasm = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = cytoplasm.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius3, new CanvasRadialGradientBrush(resourceCreator, CanvasResourceCreatorExtensions.ParameciumCytoplasm)
                {
                    Center = origin,
                    RadiusX = radius3,
                    RadiusY = radius3,
                });
            }

            return new CellRenderTarget
            {
                Nuvleus = nuvleus,
                Cytoplasm = cytoplasm
            };
        }


        public static CellRenderTarget RenderLeukocyte(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin)
        {
            float radius1 = radius - 1;
            float radius2 = 2 * (float)Math.Pow(level, 0.333333333333333333333333333333333f);

            CanvasRenderTarget nuvleus = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = nuvleus.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius1, new CanvasRadialGradientBrush(resourceCreator, CanvasResourceCreatorExtensions.CellNuvleus)
                {
                    Center = origin,
                    RadiusX = radius1,
                    RadiusY = radius1,
                });

                ds.DrawCircle(origin, radius1, Color.FromArgb(222, 255, 255, 255), 2);
            }

            CanvasRenderTarget cytoplasm = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = cytoplasm.CreateDrawingSession())
            using (CanvasGeometry geometry = CanvasGeometry.CreateCircle(resourceCreator, origin, radius1))
            using (ds.CreateLayer(1, geometry))
            {
                ds.DrawImage(new InvertEffect
                {
                    Source = CanvasResourceCreatorExtensions.SpiderWebEffect(radius2, diameter)
                });
            }

            return new CellRenderTarget
            {
                Nuvleus = nuvleus,
                Cytoplasm = cytoplasm
            };
        }


        public static CellRenderTarget RenderPrion(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin)
        {
            float radius2 = 2 * (float)Math.Pow(level, 0.333333333333333333333333333333333f);

            CanvasRenderTarget nuvleus = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = nuvleus.CreateDrawingSession())
            using (CanvasGeometry geometry = CanvasGeometry.CreateCircle(resourceCreator, origin, radius))
            using (ds.CreateLayer(1, geometry))
            {
                ds.DrawImage(new OpacityEffect
                {
                    Opacity = 0.2f,
                    Source = new InvertEffect
                    {
                        Source = CanvasResourceCreatorExtensions.SpiderWebEffect2(radius2, diameter)
                    }
                });

                ds.FillCircle(origin, radius, new CanvasRadialGradientBrush(resourceCreator, CanvasResourceCreatorExtensions.PrionCytoplasm)
                {
                    Center = origin,
                    RadiusX = radius,
                    RadiusY = radius,
                });

                ds.DrawCircle(origin, radius, Colors.White, 2);
            }

            CanvasRenderTarget cytoplasm = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = cytoplasm.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius2, new CanvasRadialGradientBrush(resourceCreator, Color.FromArgb(0, 0, 0, 255), Color.FromArgb(255, 100, 220, 255))
                {
                    Center = origin,
                    RadiusX = radius2,
                    RadiusY = radius2,
                });

                ds.DrawCircle(origin, radius2, Colors.White, 2);
            }

            return new CellRenderTarget
            {
                Nuvleus = nuvleus,
                Cytoplasm = cytoplasm
            };
        }


        public static CellRenderTarget RenderCancer(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin)
        {
            float radius2 = 2 * (float)Math.Pow(level, 0.433333333333333333333333333333333f);

            CanvasRenderTarget nuvleus = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = nuvleus.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius - 2, Color.FromArgb(150, 0, 0, 0));
                ds.DrawCircle(origin, radius - 2, Colors.Black, 2);
            }

            CanvasRenderTarget cytoplasm = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = cytoplasm.CreateDrawingSession())
            {
                ds.DrawCircle(origin, radius2, Color.FromArgb(150, 255, 0, 0), 2);
            }

            return new CellRenderTarget
            {
                Nuvleus = nuvleus,
                Cytoplasm = cytoplasm
            };
        }


        public static CellRenderTarget RenderStaphylococcusAureus(this ICanvasResourceCreatorWithDpi resourceCreator, int level, float radius, float diameter, Vector2 origin)
        {
            float radius2 = 2 * (float)Math.Pow(level, 0.333333333333333333333333333333333f);

            CanvasRenderTarget nuvleus = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = nuvleus.CreateDrawingSession())
            {
                ds.FillCircle(origin, radius, new CanvasRadialGradientBrush(resourceCreator, CanvasResourceCreatorExtensions.StaphylococcusAureusNuvleus)
                {
                    Center = origin,
                    RadiusX = radius,
                    RadiusY = radius,
                });

                ds.DrawCircle(origin, radius, Color.FromArgb(222, 255, 255, 255), 2);
            }

            CanvasRenderTarget cytoplasm = new CanvasRenderTarget(resourceCreator, diameter, diameter);
            using (CanvasDrawingSession ds = cytoplasm.CreateDrawingSession())
            using (CanvasGeometry geometry = CanvasGeometry.CreateCircle(resourceCreator, origin, radius))
            using (ds.CreateLayer(1, geometry))
            {
                ds.DrawImage(new OpacityEffect
                {
                    Opacity = 0.8f,
                    Source = new InvertEffect
                    {
                        Source = CanvasResourceCreatorExtensions.SpiderWebEffect(radius2, diameter)
                    }
                });
            }

            return new CellRenderTarget
            {
                Nuvleus = nuvleus,
                Cytoplasm = cytoplasm
            };
        }


        private static ICanvasImage SpiderWebEffect(float radius2, float diameter)
        {
            return new LuminanceToAlphaEffect
            {
                Source = new EdgeDetectionEffect
                {
                    Amount = 1,
                    Source = new DiscreteTransferEffect
                    {
                        Source = new TurbulenceEffect
                        {
                            Octaves = 4, // 4
                            Frequency = new Vector2(1f / radius2), // 0.02
                            Size = new Vector2(diameter) // 222
                        }
                    }
                }
            };
        }
        private static ICanvasImage SpiderWebEffect2(float radius2, float diameter)
        {
            return new EdgeDetectionEffect
            {
                Amount = 1,
                Source = new DiscreteTransferEffect
                {
                    Source = new TurbulenceEffect
                    {
                        Octaves = 4, // 4
                        Frequency = new Vector2(1f / radius2), // 0.02
                        Size = new Vector2(diameter) // 222
                    }
                }
            };
        }


    }
}