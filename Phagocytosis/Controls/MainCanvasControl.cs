using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Phagocytosis.Elements;
using Phagocytosis.Sprites;
using Phagocytosis.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Input;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Main of <see cref="CanvasAnimatedControl"/>.
    /// </summary>
    public sealed class MainCanvasControl : BaseCanvasControl
    {

        //@Override
        public override ICanvasResourceCreatorWithDpi ResourceCreator => this.CanvasControl;

        //@Delegate
        /// <summary> Occurs when game over. </summary>
        public event EventHandler<PlayState> GameOver;
        /// <summary> Occurs when scored. </summary>
        public event EventHandler<ScoredEventArgs> Scored;
        /// <summary> Occurs when record. </summary>
        public event EventHandler<RecordEventArgs> Record;

        private bool CanVelocity;
        private Vector2 Velocity;

        public Spriter Player { get; set; }
        private readonly IList<Spriter> RemoveFriendSprites = new List<Spriter>();
        private readonly IList<Spriter> AddFriendSprites = new List<Spriter>();
        private readonly IList<Spriter> RemoveEnemySprites = new List<Spriter>();
        private readonly IList<Spriter> AddEnemySprites = new List<Spriter>();

        public PlayState State { get; private set; }
        private float Duration;
        private bool HasCreateResources;
        private bool LoadingFromProject = true;
        private Chapter Chapter;
        private readonly DispatcherTimer PausedTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(600)
        };

        #region DependencyProperty


        /// <summary> Gets or set the count for <see cref="MainCanvasControl"/>'s Friends. </summary>
        public int FriendsCount
        {
            get => (int)base.GetValue(FriendsCountProperty);
            set => base.SetValue(FriendsCountProperty, value);
        }
        /// <summary> Identifies the <see cref = "MainCanvasControl.FriendsCount" /> dependency property. </summary>
        public static readonly DependencyProperty FriendsCountProperty = DependencyProperty.Register(nameof(FriendsCount), typeof(int), typeof(MainCanvasControl), new PropertyMetadata(0));


        /// <summary> Gets or set the count for <see cref="MainCanvasControl"/>'s Enemys. </summary>
        public int EnemysCount
        {
            get => (int)base.GetValue(EnemysCountProperty);
            set => base.SetValue(EnemysCountProperty, value);
        }
        /// <summary> Identifies the <see cref = "MainCanvasControl.EnemysCount" /> dependency property. </summary>
        public static readonly DependencyProperty EnemysCountProperty = DependencyProperty.Register(nameof(EnemysCount), typeof(int), typeof(MainCanvasControl), new PropertyMetadata(0));


        #endregion

        readonly CanvasStopwatch Stopwatch = new CanvasStopwatch();
        CanvasAnimatedControl CanvasControl = new CanvasAnimatedControl
        {
            Paused = false,
        };

        //@Constructs
        /// <summary>
        /// Initialize a MainCanvasControl.
        /// </summary>
        public MainCanvasControl() : base()
        {
            // Initialize
            base.ManipulationMode = ManipulationModes.Scale | ManipulationModes.TranslateX | ManipulationModes.TranslateY;
            base.Content = this.CanvasControl;
            this.Pause();

            this.PausedTimer.Tick += (s, e) =>
            {
                switch (this.State)
                {
                    case PlayState.Playing:
                        // Delegate
                        this.Scored?.Invoke(this, new ScoredEventArgs
                        {
                            FriendSpritesSumLevel = this.FriendSprites.Sum(a => a.Level),
                            EnemySpritesSumLevel = this.EnemySprites.Sum(a => a.Level)
                        });
                        // Delegate
                        this.Record?.Invoke(this, new RecordEventArgs
                        {
                            FriendSpritesMaxLevel = this.FriendSprites.Max(a => a.Level),
                            FriendSpritesCount = this.FriendSprites.Count,
                            TotalTime = this.Stopwatch.TotalTime()
                        });
                        break;
                    case PlayState.Loser:
                        this.FriendsCount = 0;
                        this.EnemysCount = this.EnemySprites.Sum(a => a.Level);
                        this.Pause();
                        this.GameOver?.Invoke(this, PlayState.Loser); // Delegate
                        break;
                    case PlayState.Winner:
                        this.FriendsCount = this.FriendSprites.Sum(a => a.Level);
                        this.EnemysCount = 0;
                        this.Pause();
                        this.GameOver?.Invoke(this, PlayState.Winner); // Delegate
                        break;
                }
            };
            this.PausedTimer.Start();


            base.PointerWheelChanged += (s, e) =>
            {
                switch (this.State)
                {
                    case PlayState.Playing:
                        float space = e.GetCurrentPoint(this).Properties.MouseWheelDelta;

                        if (space > 0)
                            this.ZoomIn();
                        else
                            this.ZoomOut();
                        break;
                }
            };
            base.ManipulationStarted += (s, e) => base.ZoomStarted();
            base.ManipulationDelta += (s, e) => base.ZoomDelta(e.Cumulative);
            base.ManipulationCompleted += (s, e) => base.ZoomDelta(e.Cumulative);


            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.HasCreateResources = true;
                this.LoadFromProject(this.Chapter);
            };


            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.HasCreateResources == false) return;
                if (this.LoadingFromProject) return;
                args.DrawingSession.Transform = this.Transform;


                CanvasCommandList renderTargetNuvleus = new CanvasCommandList(this.ResourceCreator);
                using (CanvasDrawingSession ds = renderTargetNuvleus.CreateDrawingSession())
                {
                    ds.DrawBugMap(this.Map.Restricteds.BugMap);
                    ds.DrawRectMap(this.Map.Restricteds);
                    ds.DrawMap(this.Map);

                    using (CanvasSpriteBatch sb = ds.CreateSpriteBatch())
                    {
                        foreach (Spriter item in this.FriendSprites)
                        {
                            switch (item.State)
                            {
                                case SpriteState.Dead:
                                    break;
                                case SpriteState.Infected:
                                    sb.Draw(item.Nuvleus, item.Offset, new Vector4(0, 0, 0, 1));
                                    break;
                                case SpriteState.Dividing:
                                    sb.DrawFromSpriteSheet(item.Nuvleus, item.DividingTransform, item.DividingRect);
                                    sb.DrawFromSpriteSheet(item.Nuvleus, item.DividingTransformClone, item.DividingRect);
                                    break;
                                default:
                                    sb.Draw(item.Nuvleus, item.Offset);
                                    break;
                            }
                        }
                        foreach (Spriter item in this.EnemySprites)
                        {
                            switch (item.State)
                            {
                                case SpriteState.Dead:
                                    break;
                                case SpriteState.Infected:
                                    sb.Draw(item.Nuvleus, item.Offset, new Vector4(0, 0, 0, 1));
                                    break;
                                case SpriteState.Dividing:
                                    sb.DrawFromSpriteSheet(item.Nuvleus, item.DividingTransform, item.DividingRect);
                                    sb.DrawFromSpriteSheet(item.Nuvleus, item.DividingTransformClone, item.DividingRect);
                                    break;
                                default:
                                    sb.Draw(item.Nuvleus, item.Offset);
                                    break;
                            }
                        }
                    }
                }
                args.DrawingSession.DrawDisplacementNuvleus(renderTargetNuvleus);


                CanvasCommandList renderTargetCytoplasm = new CanvasCommandList(this.ResourceCreator);
                using (CanvasDrawingSession ds = renderTargetCytoplasm.CreateDrawingSession())
                {
                    using (CanvasSpriteBatch sb = ds.CreateSpriteBatch())
                    {
                        foreach (Spriter item in this.FriendSprites)
                        {
                            switch (item.State)
                            {
                                case SpriteState.Dead:
                                    break;
                                case SpriteState.Infected:
                                    sb.Draw(item.Cytoplasm, item.Offset, new Vector4(1, 0, 0, 0.7f));
                                    break;
                                case SpriteState.Dividing:
                                    sb.DrawFromSpriteSheet(item.Cytoplasm, item.DividingTransform, item.DividingRect);
                                    sb.DrawFromSpriteSheet(item.Cytoplasm, item.DividingTransformClone, item.DividingRect);
                                    break;
                                default:
                                    sb.Draw(item.Cytoplasm, item.Offset);
                                    break;
                            }
                        }
                        foreach (Spriter item in this.EnemySprites)
                        {
                            switch (item.State)
                            {
                                case SpriteState.Dead:
                                    break;
                                case SpriteState.Infected:
                                    sb.Draw(item.Cytoplasm, item.Offset, new Vector4(1, 0, 0, 0.7f));
                                    break;
                                case SpriteState.Dividing:
                                    sb.DrawFromSpriteSheet(item.Cytoplasm, item.DividingTransform, item.DividingRect);
                                    sb.DrawFromSpriteSheet(item.Cytoplasm, item.DividingTransformClone, item.DividingRect);
                                    break;
                                default:
                                    sb.Draw(item.Cytoplasm, item.Offset);
                                    break;
                            }
                        }
                    }
                }
                args.DrawingSession.DrawDisplacementCytoplasm(renderTargetCytoplasm);


                switch (this.Player.State)
                {
                    case SpriteState.Rebirth:
                        args.DrawingSession.DrawFlag("ↆ", this.Player, Colors.White);
                        break;
                    case SpriteState.Infected:
                        args.DrawingSession.DrawFlag("💀", this.Player, Colors.Black);
                        break;
                }
            };


            this.CanvasControl.Update += (sender, args) =>
            {
                if (this.HasCreateResources == false) return;
                if (this.LoadingFromProject) return;

                TimeSpan totalTime = args.Timing.TotalTime;
                float elapsedTime = (float)args.Timing.ElapsedTime.TotalMilliseconds;

                this.Stopwatch.Update(totalTime);
                this.Map.Restricteds.BugMap.Update(elapsedTime);
                bool isAdd = this.Map.Update(totalTime);


                // FriendSprites
                {
                    foreach (Spriter item in this.FriendSprites)
                    {
                        item.Update(this.EnemySprites, this.Map, elapsedTime);
                        switch (item.State)
                        {
                            case SpriteState.Dead:
                            case SpriteState.Cancerous:
                                this.RemoveFriendSprites.Add(item);
                                break;
                            case SpriteState.Divided:
                                this.AddFriendSprites.Add(item.Divided());
                                switch (item.Type)
                                {
                                    case SpriteType.Player:
                                        this.Position.X += item.Radius;
                                        break;
                                }
                                break;
                        }
                    }

                    if (this.RemoveFriendSprites.Count > 0)
                    {
                        foreach (Spriter item in this.RemoveFriendSprites)
                        {

                            switch (item.Type)
                            {
                                case SpriteType.Player:
                                    switch (item.State)
                                    {
                                        case SpriteState.Dead:
                                        case SpriteState.Cancerous:
                                            foreach (Spriter item2 in this.FriendSprites)
                                            {
                                                switch (item2.State)
                                                {
                                                    case SpriteState.Dead:
                                                    case SpriteState.Cancerous:
                                                        break;
                                                    default:
                                                        this.NewCamera(this.Player.Position, item2.Position, item2.Radius);
                                                        this.Player = item2.Rebirth();
                                                        break;
                                                }
                                            }
                                            break;
                                    }
                                    break;
                            }

                            switch (item.State)
                            {
                                case SpriteState.Cancerous:
                                    this.AddEnemySprites.Add(item.Prions());
                                    break;
                            }

                            this.FriendSprites.Remove(item);
                        }
                        this.RemoveFriendSprites.Clear();
                    }

                    if (this.AddFriendSprites.Count > 0)
                    {
                        foreach (Spriter item in this.AddFriendSprites)
                        {
                            this.FriendSprites.Add(item);
                        }
                        this.AddFriendSprites.Clear();
                    }
                }


                // EnemySprites
                {
                    foreach (Spriter item in this.EnemySprites)
                    {
                        item.Update(this.FriendSprites, this.Map, elapsedTime);
                        switch (item.State)
                        {
                            case SpriteState.Dead:
                                this.RemoveEnemySprites.Add(item);
                                break;
                            case SpriteState.Divided:
                                this.AddEnemySprites.Add(item.Divided());
                                break;
                        }
                    }

                    if (this.RemoveEnemySprites.Count > 0)
                    {
                        foreach (Spriter item in this.RemoveEnemySprites)
                        {
                            this.EnemySprites.Remove(item);
                        }
                        this.RemoveEnemySprites.Clear();
                    }

                    if (this.AddEnemySprites.Count > 0)
                    {
                        foreach (Spriter item in this.AddEnemySprites)
                        {
                            this.EnemySprites.Add(item);
                        }
                        this.AddEnemySprites.Clear();
                    }
                }


                if (this.CanVelocity) this.Position += this.Velocity * elapsedTime / 4f;

                switch (this.State)
                {
                    case PlayState.Playing:
                        this.Transform = this.GetTransform();
                        if (this.FriendSprites.Count == 0)
                            this.State = PlayState.Losing;
                        if (this.EnemySprites.Count == 0)
                            this.State = PlayState.Winning;
                        break;
                    case PlayState.Losing:
                        this.State = PlayState.Loser;
                        break;
                    case PlayState.Winning:
                        float duration = 800;
                        this.Duration += elapsedTime;
                        float progress = Math.Max(0, Math.Min(1, this.Duration / duration));
                        this.Transform = this.GetTransform(progress);
                        if (this.Duration >= duration) // 800 ms
                            this.State = PlayState.Winner;
                        break;
                }
            };
        }
        ~MainCanvasControl()
        {
            this.CanvasControl.RemoveFromVisualTree();
            this.CanvasControl = null;
        }


        public void LoadFromProject(Chapter chapter)
        {
            if (chapter == null)
                chapter = this.Chapter;
            else
                this.Chapter = chapter;

            if (this.HasCreateResources == false) return;

            this.LoadingFromProject = true;
            {
                this.FriendsCount = chapter.FriendSprites.Sum(a => a.Level);
                this.EnemysCount = chapter.EnemySprites.Sum(a => a.Level);

                base.Load(chapter);
                this.Player = this.FriendSprites.First(c => c.Type == SpriteType.Player).Rebirth();

                this.Stopwatch.Restart();
                this.Play();
            }
            this.LoadingFromProject = false;
        }
        public void Stop() => this.PausedTimer.Stop();
        public void Start() => this.PausedTimer.Start();
        public void Play()
        {
            this.Stopwatch.Play();
            this.State = PlayState.Playing;
            this.Duration = 0;
            this.CanvasControl.Paused = false;
        }
        public void Pause()
        {
            this.Stopwatch.Pause();
            this.State = PlayState.Paused;
            this.Duration = 0;
            this.CanvasControl.Paused = true;
        }

        public void Move(Vector2 velocity)
        {
            this.CanVelocity = velocity != Vector2.Zero;
            this.Velocity = velocity;
        }

        private void NewCamera(Vector2 position1, Vector2 position2, float radius2)
        {
            Vector2 screenPosition1 = Vector2.Transform(position1, this.Transform);
            Vector2 screenPosition2 = Vector2.Transform(position2, this.Transform);
            float screenRadius = radius2 * this.Scale2;

            if (screenPosition2.X > screenRadius)
            {
                if (screenPosition2.Y > screenRadius)
                {
                    if (screenPosition2.X < this.Center.X * 2 - screenRadius)
                    {
                        if (screenPosition2.Y < this.Center.Y * 2 - screenRadius)
                        {
                            this.Position -= screenPosition1 - screenPosition2;
                            return;
                        }
                    }
                }
            }

            this.Position = Vector2.Zero;
        }

        private Matrix3x2 GetTransform()
        {
            return
                 Matrix3x2.CreateTranslation(-this.Player.Position) *
                 Matrix3x2.CreateScale(this.Scale2) *
                 Matrix3x2.CreateTranslation(this.Center) *
                 Matrix3x2.CreateTranslation(this.Position);
        }
        private Matrix3x2 GetTransform(float progress)
        {
            float noprogress = 1 - progress;
            float scale = Math.Min(this.Center.X * 2 / (this.Map.Width + 20), this.Center.Y * 2 / (this.Map.Height + 20));
            Vector2 center = new Vector2(this.Map.Width, this.Map.Height) / 2;

            return
                 Matrix3x2.CreateTranslation(-center * progress) *
                 Matrix3x2.CreateTranslation(-this.Player.Position * noprogress) *
                 Matrix3x2.CreateScale(this.Scale2 * noprogress + scale * progress) *
                 Matrix3x2.CreateTranslation(this.Center) *
                 Matrix3x2.CreateTranslation(this.Position * noprogress);
        }

    }
}