using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Phagocytosis.Sprites;
using Phagocytosis.ViewModels;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Edit of <see cref="CanvasAnimatedControl"/>.
    /// </summary>
    public sealed class EditCanvasControl : BaseCanvasControl
    {

        //@Override
        public override ICanvasResourceCreatorWithDpi ResourceCreator => this.CanvasControl;

        public EditType EditType { get; set; }
        public SpriteType SpriteType { get; set; }
        Spriter Sprite;
        Vector2 Point;
        int Index;
        bool IsManipulation;

        Matrix3x2 Transform2 = Matrix3x2.Identity;
        CanvasControl CanvasControl = new CanvasControl();
        CanvasControl CanvasControl2 = new CanvasControl();

        //@Constructs
        /// <summary>
        /// Initialize a EditCanvasControl.
        /// </summary>
        public EditCanvasControl() : base()
        {
            // Initialize
            base.ManipulationMode = ManipulationModes.Scale | ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            base.Content = new Grid
            {
                Children =
                {
                    this.CanvasControl,
                    this.CanvasControl2,
                }
            };

            base.PointerWheelChanged += (s, e) =>
            {
                float space = e.GetCurrentPoint(this).Properties.MouseWheelDelta;

                if (space > 0)
                    this.ZoomIn();
                else
                    this.ZoomOut();

                this.Transform = this.GetTransform();
                this.Transform2 = this.GetTransform2();
                this.CanvasControl.Invalidate(); // Invalidate
                this.CanvasControl2.Invalidate(); // Invalidate
            };
            base.ManipulationStarted += (s, e) =>
            {
                this.IsManipulation = true;
                Vector2 point = e.Position.ToVector2();
                Vector2 point2 = Vector2.Transform(point, this.Transform2);

                switch (this.EditType)
                {
                    case EditType.Clear:
                        {
                            this.Index = -1;
                            foreach (Rect2 item in this.Map.Restricteds)
                            {
                                if (this.IsNode(point, item.LeftTop(), this.Transform))
                                {
                                    this.Index = this.Map.Restricteds.IndexOf(item);
                                    break;
                                }
                                if (this.IsNode(point, item.RightBottm(), this.Transform))
                                {
                                    this.Index = this.Map.Restricteds.IndexOf(item);
                                    break;
                                }
                            }
                            if (this.Index >= 0) this.Map.Restricteds.RemoveAt(this.Index);


                            this.Sprite = null;
                            foreach (Spriter item in this.FriendSprites)
                            {
                                if (item.Type != SpriteType.Player)
                                    if (this.IsNode(point, item.Position, this.Transform))
                                    {
                                        this.Sprite = item;
                                        break;
                                    }
                            }
                            if (this.Sprite != null) this.FriendSprites.Remove(this.Sprite);

                            this.Sprite = null;
                            foreach (Spriter item in this.EnemySprites)
                            {
                                if (item.Type != SpriteType.Player)
                                    if (this.IsNode(point, item.Position, this.Transform))
                                    {
                                        this.Sprite = item;
                                        break;
                                    }
                            }
                            if (this.Sprite != null) this.EnemySprites.Remove(this.Sprite);

                            return;
                        }
                    case EditType.AddCell:
                        {
                            switch (this.SpriteType)
                            {
                                case SpriteType.Cell:
                                    this.FriendSprites.Add(new Spriter(this.CanvasControl, SpriteType.Cell, point2, Spriter.GetLevel(SpriteType.Cell)));
                                    break;
                                case SpriteType.Bacteria:
                                    this.EnemySprites.Add(new Spriter(this.CanvasControl, SpriteType.Bacteria, point2, Spriter.GetLevel(SpriteType.Bacteria)));
                                    break;
                                case SpriteType.Virus:
                                    this.EnemySprites.Add(new Spriter(this.CanvasControl, SpriteType.Virus, point2, Spriter.GetLevel(SpriteType.Virus)));
                                    break;
                                case SpriteType.Paramecium:
                                    this.EnemySprites.Add(new Spriter(this.CanvasControl, SpriteType.Paramecium, point2, Spriter.GetLevel(SpriteType.Paramecium)));
                                    break;
                                case SpriteType.Leukocyte:
                                    this.EnemySprites.Add(new Spriter(this.CanvasControl, SpriteType.Leukocyte, point2, Spriter.GetLevel(SpriteType.Leukocyte)));
                                    break;
                                case SpriteType.Prion:
                                    this.EnemySprites.Add(new Spriter(this.CanvasControl, SpriteType.Prion, point2, Spriter.GetLevel(SpriteType.Prion)));
                                    break;
                                case SpriteType.Cancer:
                                    this.EnemySprites.Add(new Spriter(this.CanvasControl, SpriteType.Cancer, point2, Spriter.GetLevel(SpriteType.Cancer)));
                                    break;
                            }
                            this.Sprite = this.EnemySprites.Last();
                        }
                        return;
                    case EditType.AddRestricted:
                        this.Point = point2;
                        this.Map.Restricteds.Add(new Rect2(this.Point));
                        return;
                    case EditType.ZoomIn:
                        foreach (Spriter item in this.FriendSprites)
                        {
                            if (this.IsNode(point, item.Position, this.Transform))
                            {
                                item.Upgrade(item.Level + Food.Level);
                                break;
                            }
                        }

                        foreach (Spriter item in this.EnemySprites)
                        {
                            if (this.IsNode(point, item.Position, this.Transform))
                            {
                                item.Upgrade(item.Level + Food.Level);
                                break;
                            }
                        }
                        return;
                    case EditType.ZoomOut:
                        foreach (Spriter item in this.FriendSprites)
                        {
                            if (this.IsNode(point, item.Position, this.Transform))
                            {
                                item.Upgrade(item.Level - Food.Level);
                                break;
                            }
                        }

                        foreach (Spriter item in this.EnemySprites)
                        {
                            if (this.IsNode(point, item.Position, this.Transform))
                            {
                                item.Upgrade(item.Level - Food.Level);
                                break;
                            }
                        }
                        return;
                }

                if (this.IsNode(point, this.MapNode, this.Transform))
                {
                    this.EditType = EditType.Crop;
                    return;
                }

                foreach (Rect2 item in this.Map.Restricteds)
                {
                    if (this.IsNode(point, item.LeftTop(), this.Transform))
                    {
                        this.Point = item.RightBottm();
                        this.Index = this.Map.Restricteds.IndexOf(item);
                        this.EditType = EditType.CursorRestricted;
                        return;
                    }
                    if (this.IsNode(point, item.RightBottm(), this.Transform))
                    {
                        this.Point = item.LeftTop();
                        this.Index = this.Map.Restricteds.IndexOf(item);
                        this.EditType = EditType.CursorRestricted;
                        return;
                    }
                }

                foreach (Spriter item in this.FriendSprites)
                {
                    if (this.IsNode(point, item.Position, this.Transform))
                    {
                        this.Sprite = item;
                        this.EditType = EditType.CursorCell;
                        return;
                    }
                }
                foreach (Spriter item in this.EnemySprites)
                {
                    if (this.IsNode(point, item.Position, this.Transform))
                    {
                        this.Sprite = item;
                        this.EditType = EditType.CursorCell;
                        return;
                    }
                }

                this.EditType = EditType.View;
                base.ZoomStarted();
            };
            base.ManipulationDelta += (s, e) =>
            {
                this.IsManipulation = true;
                {
                    Vector2 point = e.Position.ToVector2();
                    Vector2 point2 = Vector2.Transform(point, this.Transform2);

                    switch (this.EditType)
                    {
                        case EditType.View:
                            base.ZoomDelta(e.Cumulative);
                            this.Transform = this.GetTransform();
                            this.Transform2 = this.GetTransform2();
                            break;
                        case EditType.Crop:
                            this.Map = new FoodMap((int)point2.X, (int)point2.Y);
                            break;
                        case EditType.CursorCell:
                        case EditType.AddCell:
                            this.Sprite.Position = point2;
                            break;
                        case EditType.AddRestricted:
                            this.Map.Restricteds[this.Map.Restricteds.Count - 1] = new Rect2(this.Point, point2);
                            break;
                        case EditType.CursorRestricted:
                            this.Map.Restricteds[this.Index] = new Rect2(this.Point, point2);
                            break;
                    }
                }
                this.IsManipulation = false;
                this.CanvasControl.Invalidate(); // Invalidate
                this.CanvasControl2.Invalidate(); // Invalidate
            };
            base.ManipulationCompleted += (s, e) =>
            {
                this.IsManipulation = true;
                {
                    Vector2 point = e.Position.ToVector2();
                    Vector2 point2 = Vector2.Transform(point, this.Transform2);

                    switch (this.EditType)
                    {
                        case EditType.View:
                            base.ZoomDelta(e.Cumulative);
                            this.Transform = this.GetTransform();
                            this.Transform2 = this.GetTransform2();
                            break;
                        case EditType.Crop:
                            this.Map = new FoodMap((int)point2.X, (int)point2.Y);
                            break;
                        case EditType.CursorCell:
                        case EditType.AddCell:
                            this.Sprite.Position = point2;
                            break;
                        case EditType.AddRestricted:
                            this.Map.Restricteds[this.Map.Restricteds.Count - 1] = new Rect2(this.Point, point2);
                            break;
                        case EditType.CursorRestricted:
                            this.Map.Restricteds[this.Index] = new Rect2(this.Point, point2);
                            break;
                    }
                }
                this.IsManipulation = false;
                this.CanvasControl.Invalidate(); // Invalidate
                this.CanvasControl2.Invalidate(); // Invalidate
            };


            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.Transform = this.GetTransform();
                this.Transform2 = this.GetTransform2();
                this.FriendSprites.Add(new Spriter(this.CanvasControl, SpriteType.Player, new Vector2(400), 192 * 4));
            };

            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.IsManipulation) return;

                args.DrawingSession.Transform = this.Transform;

                args.DrawingSession.DrawBugMap(this.Map.Restricteds.BugMap);
                args.DrawingSession.DrawRectMap(this.Map.Restricteds);
                args.DrawingSession.DrawMap(this.Map);

                using (CanvasSpriteBatch sb = args.DrawingSession.CreateSpriteBatch())
                {
                    foreach (Spriter item in this.FriendSprites)
                    {
                        Vector2 offset = item.Offset;
                        sb.Draw(item.Nuvleus, offset);
                        sb.Draw(item.Cytoplasm, offset);
                    }
                    foreach (Spriter item in this.EnemySprites)
                    {
                        Vector2 offset = item.Offset;
                        sb.Draw(item.Nuvleus, offset);
                        sb.Draw(item.Cytoplasm, offset);
                    }
                }
            };

            this.CanvasControl2.Draw += (sender, args) =>
            {
                foreach (Spriter item in this.FriendSprites)
                {
                    this.DrawNode(args.DrawingSession, item.Position, this.Transform);
                }
                foreach (Spriter item in this.EnemySprites)
                {
                    this.DrawNode(args.DrawingSession, item.Position, this.Transform);
                }
                foreach (Rect2 item in this.Map.Restricteds)
                {
                    this.DrawNode(args.DrawingSession, item.LeftTop(), this.Transform);
                    this.DrawNode(args.DrawingSession, item.RightBottm(), this.Transform);
                }

                this.DrawNode(args.DrawingSession, this.MapNode, this.Transform);
            };
        }
        ~EditCanvasControl()
        {
            this.CanvasControl.RemoveFromVisualTree();
            this.CanvasControl = null;
            this.CanvasControl2.RemoveFromVisualTree();
            this.CanvasControl2 = null;
        }

        public void LoadFromProject(Chapter chapter)
        {
            base.Load(chapter);
            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasControl2.Invalidate(); // Invalidate
        }

        public void ZoomSprite(bool isFriend, Symbol symbol)
        {
            switch (symbol)
            {
                case Symbol.ZoomIn: this.ZoomSprite(isFriend, true); break;
                case Symbol.ZoomOut: this.ZoomSprite(isFriend, false); break;
                default: break;
            }
            this.CanvasControl.Invalidate(); // Invalidate
        }

        private Vector2 MapNode => new Vector2(this.Map.Width, this.Map.Height);

        private bool IsNode(Vector2 a, Vector2 b, Matrix3x2 matrix) => Vector2.Distance(a, Vector2.Transform(b, matrix)) < 20;
        private void DrawNode(CanvasDrawingSession ds, Vector2 b, Matrix3x2 matrix) => ds.DrawCircle(Vector2.Transform(b, matrix), 20, Colors.Red);

        private Matrix3x2 GetTransform()
        {
            return
                 Matrix3x2.CreateTranslation(-this.Map.Center) *
                 Matrix3x2.CreateScale(this.Scale2) *
                 Matrix3x2.CreateTranslation(this.Center) *
                 Matrix3x2.CreateTranslation(this.Position);
        }
        private Matrix3x2 GetTransform2()
        {
            return
                 Matrix3x2.CreateTranslation(-this.Position) *
                 Matrix3x2.CreateTranslation(-this.Center) *
                 Matrix3x2.CreateScale(1 / this.Scale2) *
                 Matrix3x2.CreateTranslation(this.Map.Center);
        }

    }
}