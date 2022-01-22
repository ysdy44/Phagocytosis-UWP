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
        public SpriteType SpriteType { get; set; } = SpriteType.Bacteria;

        EditMoveType MoveType;
        Vector2 Point;
        Spriter Sprite;
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
                    base.ZoomIn();
                else
                    base.ZoomOut();

                base.Transform = this.GetTransform();
                this.Transform2 = this.GetTransform2();
                this.CanvasControl.Invalidate(); // Invalidate
                this.CanvasControl2.Invalidate(); // Invalidate
            };
            base.ManipulationStarted += (s, e) =>
            {
                this.IsManipulation = true;
                Vector2 point = e.Position.ToVector2();
                Vector2 pointTransform = Vector2.Transform(point, this.Transform2);

                this.Index = -1;
                this.Sprite = null;

                switch (this.EditType)
                {
                    case EditType.Move:
                        if (this.IsNode(point, this.MapNode, base.Transform))
                        {
                            this.MoveType = EditMoveType.MoveMap;
                            return;
                        }

                        for (int i = 0; i < base.Map.Restricteds.Count; i++)
                        {
                            Rect2 item = base.Map.Restricteds[i];

                            if (this.IsNode(point, item.LeftTop(), base.Transform))
                            {
                                this.Point = item.RightBottm();
                                this.Index = i;
                                this.MoveType = EditMoveType.MoveRestricted;
                                return;
                            }
                            if (this.IsNode(point, item.RightBottm(), base.Transform))
                            {
                                this.Point = item.LeftTop();
                                this.Index = i;
                                this.MoveType = EditMoveType.MoveSprite;
                                return;
                            }
                        }

                        foreach (Spriter item in base.FriendSprites)
                        {
                            if (this.IsNode(point, item.Position, base.Transform))
                            {
                                this.Sprite = item;
                                this.MoveType = EditMoveType.MoveSprite;
                                return;
                            }
                        }
                        foreach (Spriter item in base.EnemySprites)
                        {
                            if (this.IsNode(point, item.Position, base.Transform))
                            {
                                this.Sprite = item;
                                this.MoveType = EditMoveType.MoveSprite;
                                return;
                            }
                        }

                        base.ZoomStarted();
                        this.MoveType = EditMoveType.Move;
                        return;
                        break;
                    case EditType.AddRestricted:
                        this.Point = pointTransform;
                        this.Map.Restricteds.Add(new Rect2(this.Point));
                        this.Index = base.Map.Restricteds.Count - 1;
                        break;
                    case EditType.AddFriend:
                        base.FriendSprites.Add(new Spriter(this.CanvasControl, SpriteType.Cell, pointTransform, Spriter.GetLevel(SpriteType.Cell)));
                        this.Sprite = base.FriendSprites.LastOrDefault();
                        break;
                    case EditType.AddEnemy:
                        base.EnemySprites.Add(new Spriter(this.CanvasControl, this.SpriteType, pointTransform, Spriter.GetLevel(this.SpriteType)));
                        this.Sprite = base.EnemySprites.LastOrDefault();
                        break;
                    default:
                        break;
                }
            };
            base.ManipulationDelta += (s, e) =>
            {
                this.IsManipulation = true;
                {
                    Vector2 point = e.Position.ToVector2();
                    Vector2 pointTransform = Vector2.Transform(point, this.Transform2);

                    EditMoveType type = this.GetType(this.EditType, this.MoveType);
                    switch (type)
                    {
                        case EditMoveType.Move:
                            base.ZoomDelta(e.Cumulative);
                            base.Transform = this.GetTransform();
                            this.Transform2 = this.GetTransform2();
                            break;
                        case EditMoveType.MoveMap:
                            base.Map = new FoodMap((int)pointTransform.X, (int)pointTransform.Y);
                            break;
                        case EditMoveType.MoveRestricted:
                            if (this.Index >= 0)
                            {
                                if (this.Index < base.Map.Restricteds.Count)
                                {
                                    base.Map.Restricteds[base.Map.Restricteds.Count - 1] = new Rect2(this.Point, pointTransform);
                                }
                            }
                            break;
                        case EditMoveType.MoveSprite:
                            if (this.Sprite != null)
                            {
                                this.Sprite.Position = pointTransform;
                            }
                            break;
                        default:
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
                    Vector2 pointTransform = Vector2.Transform(point, this.Transform2);

                    EditMoveType type = this.GetType(this.EditType, this.MoveType);
                    switch (type)
                    {
                        case EditMoveType.Move:
                            base.ZoomDelta(e.Cumulative);
                            base.Transform = this.GetTransform();
                            this.Transform2 = this.GetTransform2();
                            break;
                        case EditMoveType.MoveMap:
                            base.Map = new FoodMap((int)pointTransform.X, (int)pointTransform.Y);
                            break;
                        case EditMoveType.MoveRestricted:
                            if (this.Index >= 0)
                            {
                                if (this.Index < base.Map.Restricteds.Count)
                                {
                                    base.Map.Restricteds[base.Map.Restricteds.Count - 1] = new Rect2(this.Point, pointTransform);
                                }
                            }
                            break;
                        case EditMoveType.MoveSprite:
                            if (this.Sprite != null)
                            {
                                this.Sprite.Position = pointTransform;
                            }
                            break;
                        default:
                            break;
                    }
                }
                this.IsManipulation = false;

                this.CanvasControl.Invalidate(); // Invalidate
                this.CanvasControl2.Invalidate(); // Invalidate
            };


            this.CanvasControl.CreateResources += (sender, args) =>
            {
                base.Transform = this.GetTransform();
                this.Transform2 = this.GetTransform2();
                base.FriendSprites.Add(new Spriter(this.CanvasControl, SpriteType.Player, new Vector2(400), 192 * 4));
            };

            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.IsManipulation) return;

                args.DrawingSession.Transform = base.Transform;

                args.DrawingSession.DrawBugMap(base.Map.Restricteds.BugMap);
                args.DrawingSession.DrawRectMap(base.Map.Restricteds);
                args.DrawingSession.DrawMap(base.Map);

                using (CanvasSpriteBatch sb = args.DrawingSession.CreateSpriteBatch())
                {
                    foreach (Spriter item in base.FriendSprites)
                    {
                        Vector2 offset = item.Offset;
                        sb.Draw(item.Nuvleus, offset);
                        sb.Draw(item.Cytoplasm, offset);
                    }
                    foreach (Spriter item in base.EnemySprites)
                    {
                        Vector2 offset = item.Offset;
                        sb.Draw(item.Nuvleus, offset);
                        sb.Draw(item.Cytoplasm, offset);
                    }
                }
            };

            this.CanvasControl2.Draw += (sender, args) =>
            {
                foreach (Spriter item in base.FriendSprites)
                {
                    this.DrawNode(args.DrawingSession, item.Position, base.Transform);

                    if (item == this.Sprite) this.DrawSprite(args.DrawingSession, item, base.Transform, base.Scale2);
                }
                foreach (Spriter item in base.EnemySprites)
                {
                    this.DrawNode(args.DrawingSession, item.Position, base.Transform);

                    if (item == this.Sprite) this.DrawSprite(args.DrawingSession, item, base.Transform, base.Scale2);
                }

                for (int i = 0; i < base.Map.Restricteds.Count; i++)
                {
                    Rect2 item = base.Map.Restricteds[i];

                    if (i == this.Index)
                    {
                        this.FillNode(args.DrawingSession, item.LeftTop(), base.Transform);
                        this.FillNode(args.DrawingSession, item.RightBottm(), base.Transform);
                    }
                    else
                    {
                        this.DrawNode(args.DrawingSession, item.LeftTop(), base.Transform);
                        this.DrawNode(args.DrawingSession, item.RightBottm(), base.Transform);
                    }
                }

                this.DrawNode(args.DrawingSession, this.MapNode, base.Transform);
            };
        }
        ~EditCanvasControl()
        {
            this.CanvasControl.RemoveFromVisualTree();
            this.CanvasControl = null;
            this.CanvasControl2.RemoveFromVisualTree();
            this.CanvasControl2 = null;
        }

        private EditMoveType GetType(EditType editType, EditMoveType moveType)
        {
            switch (editType)
            {
                case EditType.Move:
                    return moveType;
                case EditType.AddRestricted:
                    return EditMoveType.MoveRestricted;
                case EditType.AddFriend:
                case EditType.AddEnemy:
                    return EditMoveType.MoveSprite;
                default:
                    return EditMoveType.Move;
            }
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
                case Symbol.ZoomIn: base.ZoomSprite(isFriend, true); break;
                case Symbol.ZoomOut: base.ZoomSprite(isFriend, false); break;
                default: break;
            }
            this.CanvasControl.Invalidate(); // Invalidate
        }
        public void ZoomIn2()
        {
            if (this.Sprite == null) return;
            this.Sprite.Upgrade(this.Sprite.Level + Food.Level);
            this.CanvasControl.Invalidate();
        }
        public void ZoomOut2()
        {
            if (this.Sprite == null) return;
            this.Sprite.Upgrade(this.Sprite.Level - Food.Level);
            this.CanvasControl.Invalidate();
        }
        public void Delete()
        {
            if (this.Index >= 0)
            {
                if (this.Index < base.Map.Restricteds.Count)
                {
                    base.Map.Restricteds.RemoveAt(this.Index);
                    this.Index = -1;
                }
            }

            if (this.Sprite != null)
            {
                switch (this.Sprite.Type)
                {
                    case SpriteType.Player:
                        break;
                    case SpriteType.Cell:
                        base.FriendSprites.Remove(this.Sprite);
                        this.Sprite = null;
                        break;
                    default:
                        base.EnemySprites.Remove(this.Sprite);
                        this.Sprite = null;
                        break;
                }
            }

            this.CanvasControl.Invalidate();
            this.CanvasControl2.Invalidate();
        }

        private Vector2 MapNode => new Vector2(base.Map.Width, base.Map.Height);

        private bool IsNode(Vector2 a, Vector2 b, Matrix3x2 matrix) => Vector2.Distance(a, Vector2.Transform(b, matrix)) < 20;
        private void DrawNode(CanvasDrawingSession ds, Vector2 b, Matrix3x2 matrix) => ds.DrawCircle(Vector2.Transform(b, matrix), 20, Colors.Red, 2);
        private void FillNode(CanvasDrawingSession ds, Vector2 b, Matrix3x2 matrix) => ds.FillCircle(Vector2.Transform(b, matrix), 20, Colors.Red);
        private void DrawSprite(CanvasDrawingSession ds, Spriter item, Matrix3x2 matrix, float scale)
        {
            Vector2 position = Vector2.Transform(item.Position, matrix);
            float radius = item.Radius * scale;
            ds.DrawRectangle(position.X - radius, position.Y - radius, radius + radius, radius + radius, Colors.Red, 2);
            ds.FillCircle(position, 20, Colors.Red);
        }

        private Matrix3x2 GetTransform()
        {
            return
                 Matrix3x2.CreateTranslation(-base.Map.Center) *
                 Matrix3x2.CreateScale(base.Scale2) *
                 Matrix3x2.CreateTranslation(base.Center) *
                 Matrix3x2.CreateTranslation(base.Position);
        }
        private Matrix3x2 GetTransform2()
        {
            return
                 Matrix3x2.CreateTranslation(-base.Position) *
                 Matrix3x2.CreateTranslation(-base.Center) *
                 Matrix3x2.CreateScale(1 / base.Scale2) *
                 Matrix3x2.CreateTranslation(base.Map.Center);
        }

    }
}