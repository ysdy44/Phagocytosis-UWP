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
    public sealed partial class MainCanvasControl : BaseCanvasControl
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

        public bool IsResizing { get; set; }
        public FlowDirection Direction { private get; set; }
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
            this.ConstructPausedTimer();
            this.ConstructManipulation();
            this.Pause();

            this.CanvasControl.CreateResources += (sender, args) =>
            {
                this.HasCreateResources = true;
                this.LoadFromProject(this.Chapter);
            };


            this.CanvasControl.Draw += (sender, args) =>
            {
                if (this.IsResizing) return;
                if (this.HasCreateResources == false) return;
                if (this.LoadingFromProject) return;

                args.DrawingSession.Transform = base.Transform;


                CanvasCommandList renderTargetNuvleus = new CanvasCommandList(this.ResourceCreator);
                using (CanvasDrawingSession ds = renderTargetNuvleus.CreateDrawingSession())
                {
                    ds.DrawBugMap(base.Map.Restricteds.BugMap);
                    ds.DrawRectMap(base.Map.Restricteds);
                    ds.DrawMap(base.Map);

                    using (CanvasSpriteBatch sb = ds.CreateSpriteBatch())
                    {
                        foreach (Spriter item in base.FriendSprites)
                        {
                            this.DrawNuvleus(sb, item);
                        }
                        foreach (Spriter item in base.EnemySprites)
                        {
                            this.DrawNuvleus(sb, item);
                        }
                    }
                }
                args.DrawingSession.DrawDisplacementNuvleus(renderTargetNuvleus);


                CanvasCommandList renderTargetCytoplasm = new CanvasCommandList(this.ResourceCreator);
                using (CanvasDrawingSession ds = renderTargetCytoplasm.CreateDrawingSession())
                {
                    using (CanvasSpriteBatch sb = ds.CreateSpriteBatch())
                    {
                        foreach (Spriter item in base.FriendSprites)
                        {
                            this.DrawCytoplasm(sb, item);
                        }
                        foreach (Spriter item in base.EnemySprites)
                        {
                            this.DrawCytoplasm(sb, item);
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
                if (this.IsResizing) return;
                if (this.HasCreateResources == false) return;
                if (this.LoadingFromProject) return;


                // Gamepad
                this.Gamepad();


                TimeSpan totalTime = args.Timing.TotalTime;
                float elapsedTime = (float)args.Timing.ElapsedTime.TotalMilliseconds;

                this.Stopwatch.Update(totalTime);
                base.Map.Restricteds.BugMap.Update(elapsedTime);
                bool isAdd = base.Map.Update(totalTime);


                // FriendSprites
                {
                    foreach (Spriter item in base.FriendSprites)
                    {
                        item.Update(base.EnemySprites, base.Map, elapsedTime);
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
                                        base.Position.X += item.Radius;
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
                                            foreach (Spriter item2 in base.FriendSprites)
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

                            base.FriendSprites.Remove(item);
                        }
                        this.RemoveFriendSprites.Clear();
                    }

                    if (this.AddFriendSprites.Count > 0)
                    {
                        foreach (Spriter item in this.AddFriendSprites)
                        {
                            base.FriendSprites.Add(item);
                        }
                        this.AddFriendSprites.Clear();
                    }
                }


                // EnemySprites
                {
                    foreach (Spriter item in base.EnemySprites)
                    {
                        item.Update(base.FriendSprites, base.Map, elapsedTime);
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
                            base.EnemySprites.Remove(item);
                        }
                        this.RemoveEnemySprites.Clear();
                    }

                    if (this.AddEnemySprites.Count > 0)
                    {
                        foreach (Spriter item in this.AddEnemySprites)
                        {
                            base.EnemySprites.Add(item);
                        }
                        this.AddEnemySprites.Clear();
                    }
                }


                if (this.CanVelocity) base.Position += this.Velocity * elapsedTime / 4f;

                switch (this.State)
                {
                    case PlayState.Playing:
                        base.Transform = this.GetTransform();
                        if (base.FriendSprites.Count == 0)
                            this.State = PlayState.Losing;
                        if (base.EnemySprites.Count == 0)
                            this.State = PlayState.Winning;
                        break;
                    case PlayState.Losing:
                        this.State = PlayState.Loser;
                        break;
                    case PlayState.Winning:
                        float duration = 800;
                        this.Duration += elapsedTime;
                        float progress = Math.Max(0, Math.Min(1, this.Duration / duration));
                        base.Transform = this.GetTransform(progress);
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

        private Matrix3x2 GetTransform()
        {
            return
                 Matrix3x2.CreateTranslation(-this.Player.Position) *
                 Matrix3x2.CreateScale(base.Scale2) *
                 Matrix3x2.CreateTranslation(base.Center) *
                 Matrix3x2.CreateTranslation(base.Position);
        }
        private Matrix3x2 GetTransform(float progress)
        {
            float noprogress = 1 - progress;
            float scale = Math.Min(base.Center.X * 2 / (base.Map.Width + 20), base.Center.Y * 2 / (base.Map.Height + 20));
            Vector2 center = new Vector2(base.Map.Width, base.Map.Height) / 2;

            return
                 Matrix3x2.CreateTranslation(-center * progress) *
                 Matrix3x2.CreateTranslation(-this.Player.Position * noprogress) *
                 Matrix3x2.CreateScale(base.Scale2 * noprogress + scale * progress) *
                 Matrix3x2.CreateTranslation(base.Center) *
                 Matrix3x2.CreateTranslation(base.Position * noprogress);
        }

    }
}