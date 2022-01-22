using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Phagocytosis.Sprites;
using Phagocytosis.ViewModels;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Input;
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

        public EditSelectionMode Mode { get; set; }

        EditMoveType MoveType;
        Vector2 Point;
        Vector2 Offset;
        Spriter Sprite;
        Rect2 Rect;
        int Index;

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
                if (this.Mode != EditSelectionMode.None) return;

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
                if (this.Mode != EditSelectionMode.None) return;

                Vector2 position = e.Position.ToVector2();
                this.MoveType = this.Started(position);
            };
            base.ManipulationDelta += (s, e) =>
            {
                if (this.Mode != EditSelectionMode.None) return;

                Vector2 position = e.Position.ToVector2();
                this.Delta(position, e.Cumulative);
            };
            base.ManipulationCompleted += (s, e) =>
            {
                if (this.Mode != EditSelectionMode.None) return;

                Vector2 position = e.Position.ToVector2();
                this.Delta(position, e.Cumulative);

                this.MoveType = EditMoveType.Move;
            };


            this.CanvasControl.CreateResources += (sender, args) =>
            {
                base.Transform = this.GetTransform();
                this.Transform2 = this.GetTransform2();
                base.FriendSprites.Add(new Spriter(this.ResourceCreator, SpriteType.Player, new Vector2(400), Spriter.GetLevel(SpriteType.Player)));
            };

            this.CanvasControl.Draw += (sender, args) =>
            {
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
                    if (item == this.Sprite) this.DrawSprite(args.DrawingSession, item, base.Transform, base.Scale2);
                }
                foreach (Spriter item in base.EnemySprites)
                {
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

        public void LoadFromProject(Chapter chapter)
        {
            base.Load(chapter);
            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasControl2.Invalidate(); // Invalidate
        }

        public void Delta(Vector2 position, ManipulationDelta cumulative)
        {
            Vector2 position2 = Vector2.Transform(position, this.Transform2);

            switch (this.MoveType)
            {
                case EditMoveType.Move:
                    base.ZoomDelta(cumulative);
                    base.Transform = this.GetTransform();
                    this.Transform2 = this.GetTransform2();
                    break;
                case EditMoveType.ResizeMap:
                    base.Map.Resize((int)position2.X, (int)position2.Y);
                    break;
                case EditMoveType.MoveRestricted:
                    if (this.Index < 0) break;
                    if (this.Index >= base.Map.Restricteds.Count) break;
                    base.Map.Restricteds[base.Map.Restricteds.Count - 1] = this.Rect.Offset(cumulative.Translation.ToVector2());
                    break;
                case EditMoveType.ResizeRestricted:
                    if (this.Index < 0) break;
                    if (this.Index >= base.Map.Restricteds.Count) break;
                    base.Map.Restricteds[base.Map.Restricteds.Count - 1] = new Rect2(this.Point, position2);
                    break;
                case EditMoveType.MoveSprite:
                    if (this.Sprite == null) break;
                    this.Sprite.Position = position2 - this.Offset;
                    break;
                default:
                    break;
            }

            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasControl2.Invalidate(); // Invalidate
        }

        public EditMoveType Started(Vector2 position)
        {
            Vector2 position2 = Vector2.Transform(position, this.Transform2);

            this.Index = -1;
            this.Sprite = null;

            if (this.IsNode(position, this.MapNode, base.Transform))
            {
                return EditMoveType.ResizeMap;
            }

            for (int i = 0; i < base.Map.Restricteds.Count; i++)
            {
                Rect2 item = base.Map.Restricteds[i];

                if (this.IsNode(position, item.LeftTop(), base.Transform))
                {
                    this.Point = item.RightBottm();
                    this.Index = i;
                    return EditMoveType.ResizeRestricted;
                }
                if (this.IsNode(position, item.RightBottm(), base.Transform))
                {
                    this.Point = item.LeftTop();
                    this.Index = i;
                    return EditMoveType.ResizeRestricted;
                }
                if (item.Contains(position2))
                {
                    this.Rect = item;
                    this.Index = i;
                    return EditMoveType.MoveRestricted;
                }
            }

            foreach (Spriter item in base.FriendSprites)
            {
                float distance = Vector2.Distance(position2, item.Position);
                if (item.Radius > distance)
                {
                    this.Offset = position2 - item.Position;
                    this.Sprite = item;
                    return EditMoveType.MoveSprite;
                }
            }
            foreach (Spriter item in base.EnemySprites)
            {
                float distance = Vector2.Distance(position2, item.Position);
                if (item.Radius > distance)
                {
                    this.Offset = position2 - item.Position;
                    this.Sprite = item;
                    return EditMoveType.MoveSprite;
                }
            }

            base.ZoomStarted();
            return EditMoveType.Move;
        }

        public EditType Select(Vector2 position)
        {
            Vector2 position2 = Vector2.Transform(position, this.Transform2);

            this.Index = -1;
            this.Sprite = null;

            for (int i = 0; i < base.Map.Restricteds.Count; i++)
            {
                Rect2 item = base.Map.Restricteds[i];
                if (item.Contains(position2))
                {
                    this.Index = i;

                    this.Position = this.GetVector2(item.Center());
                    base.Transform = this.GetTransform();
                    this.Transform2 = this.GetTransform2();
                    this.CanvasControl.Invalidate();
                    this.CanvasControl2.Invalidate();
                    return EditType.Restricted;
                }
            }

            foreach (Spriter item in base.FriendSprites)
            {
                float distance = Vector2.Distance(position2, item.Position);
                if (item.Radius > distance)
                {
                    this.Sprite = item;

                    this.Position = this.GetVector2(item.Position);
                    base.Transform = this.GetTransform();
                    this.Transform2 = this.GetTransform2();
                    this.CanvasControl.Invalidate();
                    this.CanvasControl2.Invalidate();
                    switch (item.Type)
                    {
                        case SpriteType.Player:
                            return EditType.Player;
                        default:
                            return EditType.Friend;
                    }
                }
            }
            foreach (Spriter item in base.EnemySprites)
            {
                float distance = Vector2.Distance(position2, item.Position);
                if (item.Radius > distance)
                {
                    this.Sprite = item;

                    this.Position = this.GetVector2(item.Position);
                    base.Transform = this.GetTransform();
                    this.Transform2 = this.GetTransform2();
                    this.CanvasControl.Invalidate();
                    this.CanvasControl2.Invalidate();
                    return EditType.Enemy;
                }
            }

            this.Position = this.GetVector2(position2);
            base.Transform = this.GetTransform();
            this.Transform2 = this.GetTransform2();
            this.CanvasControl.Invalidate();
            this.CanvasControl2.Invalidate();
            return EditType.None;
        }

        public void ZoomSprite2(bool isFriend, bool isZoomIn)
        {
            foreach (Spriter item in isFriend ? this.FriendSprites : this.EnemySprites)
            {
                int level = isZoomIn ? item.Level * 2 : item.Level / 2;
                item.Upgrade(level);
            }
            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasControl2.Invalidate(); // Invalidate
        }
        public void ZoomIn2()
        {
            if (this.Sprite == null) return;
            this.Sprite.Upgrade(this.Sprite.Level + Food.Level);
            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasControl2.Invalidate(); // Invalidate
        }
        public void ZoomOut2()
        {
            if (this.Sprite == null) return;
            this.Sprite.Upgrade(this.Sprite.Level - Food.Level);
            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasControl2.Invalidate(); // Invalidate
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
        public void Add(SpriteType type)
        {
            Vector2 positionTransform = this.GetVector(this.Position);

            switch (type)
            {
                case SpriteType.Player:
                    this.Map.Restricteds.Add(new Rect2((int)positionTransform.X - 20, (int)positionTransform.Y - 20, 20 + 20, 20 + 20));
                    break;
                case SpriteType.Cell:
                    base.FriendSprites.Add(new Spriter(this.ResourceCreator, SpriteType.Cell, positionTransform, Spriter.GetLevel(SpriteType.Cell)));
                    break;
                default:
                    base.EnemySprites.Add(new Spriter(this.ResourceCreator, type, positionTransform, Spriter.GetLevel(type)));
                    break;
            }

            this.CanvasControl.Invalidate(); // Invalidate
            this.CanvasControl2.Invalidate(); // Invalidate
        }

        private Vector2 MapNode => new Vector2(base.Map.Width, base.Map.Height);
        private bool IsNode(Vector2 a, Vector2 b, Matrix3x2 matrix) => Vector2.Distance(a, Vector2.Transform(b, matrix)) < 20;
        private void DrawNode(CanvasDrawingSession ds, Vector2 b, Matrix3x2 matrix) => ds.DrawCircle(Vector2.Transform(b, matrix), 20, Colors.White, 2);
        private void FillNode(CanvasDrawingSession ds, Vector2 b, Matrix3x2 matrix) => ds.FillCircle(Vector2.Transform(b, matrix), 20, Colors.White);
        private void DrawSprite(CanvasDrawingSession ds, Spriter item, Matrix3x2 matrix, float scale)
        {
            Vector2 position = Vector2.Transform(item.Position, matrix);
            float radius = item.Radius * scale;
            ds.DrawRectangle(position.X - radius, position.Y - radius, radius + radius, radius + radius, Colors.White, 2);
        }

        private Vector2 GetVector(Vector2 position) => base.Map.Center - position / base.Scale2;
        private Vector2 GetVector2(Vector2 position) => (base.Map.Center - position) * base.Scale2;

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