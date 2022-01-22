using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Phagocytosis.Elements;
using Phagocytosis.Sprites;
using Phagocytosis.ViewModels;
using System.Linq;
using System.Numerics;
using Windows.UI.Xaml;

namespace Phagocytosis.Controls
{
    /// <summary>
    /// Main of <see cref="CanvasAnimatedControl"/>.
    /// </summary>
    public sealed partial class MainCanvasControl : BaseCanvasControl
    {

        public void ConstructPausedTimer()
        {
            this.PausedTimer.Tick += (s, e) =>
            {
                // Delegate
                this.Scored?.Invoke(this, new ScoredEventArgs
                {
                    FriendSpritesSumLevel = base.FriendSprites.Sum(a => a.Level),
                    EnemySpritesSumLevel = base.EnemySprites.Sum(a => a.Level)
                });
                // Delegate
                this.Record?.Invoke(this, new RecordEventArgs
                {
                    FriendSpritesMaxLevel = base.FriendSprites.Max(a => a.Level),
                    FriendSpritesCount = base.FriendSprites.Count,
                    TotalTime = this.Stopwatch.TotalTime()
                });

                switch (this.State)
                {
                    case PlayState.Playing:
                        break;
                    case PlayState.Loser:
                        this.Pause();
                        this.GameOver?.Invoke(this, PlayState.Loser); // Delegate
                        break;
                    case PlayState.Winner:
                        this.Pause();
                        this.GameOver?.Invoke(this, PlayState.Winner); // Delegate
                        break;
                }
            };
            this.PausedTimer.Start();
        }


        public void ConstructManipulation()
        {
            base.PointerWheelChanged += (s, e) =>
            {
                switch (this.State)
                {
                    case PlayState.Playing:
                        float space = e.GetCurrentPoint(this).Properties.MouseWheelDelta;

                        if (space > 0)
                            base.ZoomIn();
                        else
                            base.ZoomOut();
                        break;
                }
            };
            base.ManipulationStarted += (s, e) => base.ZoomStarted();
            base.ManipulationDelta += (s, e) => base.ZoomDelta(e.Cumulative);
            base.ManipulationCompleted += (s, e) => base.ZoomDelta(e.Cumulative);
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
                // Delegate
                this.Scored?.Invoke(this, new ScoredEventArgs
                {
                    FriendSpritesSumLevel = chapter.FriendSprites.Sum(a => a.Level),
                    EnemySpritesSumLevel = chapter.EnemySprites.Sum(a => a.Level)
                });

                base.Load(chapter);
                this.Player = base.FriendSprites.First(c => c.Type == SpriteType.Player).Rebirth();

                this.Stopwatch.Restart();
                this.Play();
            }
            this.LoadingFromProject = false;
        }


        private void DrawNuvleus(CanvasSpriteBatch spriteBatch, Spriter item)
        {
            switch (item.State)
            {
                case SpriteState.Dead:
                    break;
                case SpriteState.Infected:
                    spriteBatch.Draw(item.Nuvleus, item.Offset, new Vector4(0, 0, 0, 1));
                    break;
                case SpriteState.Dividing:
                    spriteBatch.DrawFromSpriteSheet(item.Nuvleus, item.DividingTransform, item.DividingRect);
                    spriteBatch.DrawFromSpriteSheet(item.Nuvleus, item.DividingTransformClone, item.DividingRect);
                    break;
                default:
                    spriteBatch.Draw(item.Nuvleus, item.Offset);
                    break;
            }
        }
        private void DrawCytoplasm(CanvasSpriteBatch spriteBatch, Spriter item)
        {
            switch (item.State)
            {
                case SpriteState.Dead:
                    break;
                case SpriteState.Infected:
                    spriteBatch.Draw(item.Cytoplasm, item.Offset, new Vector4(1, 0, 0, 0.7f));
                    break;
                case SpriteState.Dividing:
                    spriteBatch.DrawFromSpriteSheet(item.Cytoplasm, item.DividingTransform, item.DividingRect);
                    spriteBatch.DrawFromSpriteSheet(item.Cytoplasm, item.DividingTransformClone, item.DividingRect);
                    break;
                default:
                    spriteBatch.Draw(item.Cytoplasm, item.Offset);
                    break;
            }
        }


        private void Gamepad()
        {
            // Gamepad
            foreach (Windows.Gaming.Input.Gamepad item in Windows.Gaming.Input.Gamepad.Gamepads)
            {
                Windows.Gaming.Input.GamepadReading reading = item.GetCurrentReading();

                switch (reading.Buttons)
                {
                    case Windows.Gaming.Input.GamepadButtons.Menu:
                        this.State = PlayState.Paused;
                        break;
                    case Windows.Gaming.Input.GamepadButtons.X:
                        this.Player.Dividing();
                        break;
                    case Windows.Gaming.Input.GamepadButtons.LeftShoulder:
                        base.ZoomIn();
                        break;
                    case Windows.Gaming.Input.GamepadButtons.RightShoulder:
                        base.ZoomOut();
                        break;
                    default:
                        break;
                }


                float lx = (float)reading.LeftThumbstickX;
                float ly = (float)reading.LeftThumbstickY;
                float rx = (float)reading.RightThumbstickX;
                float ry = (float)reading.RightThumbstickY;
                switch (this.Direction)
                {
                    case FlowDirection.LeftToRight:
                        this.Player.Velocity = new Vector2(lx, -ly);
                        this.Move(new Vector2(-rx, ry));
                        break;
                    case FlowDirection.RightToLeft:
                        this.Player.Velocity = new Vector2(-lx, -ly);
                        this.Move(new Vector2(rx, ry));
                        break;
                }

                break;
            }
        }


        private void NewCamera(Vector2 position1, Vector2 position2, float radius2)
        {
            Vector2 screenPosition1 = Vector2.Transform(position1, base.Transform);
            Vector2 screenPosition2 = Vector2.Transform(position2, base.Transform);
            float screenRadius = radius2 * base.Scale2;

            if (screenPosition2.X > screenRadius)
            {
                if (screenPosition2.Y > screenRadius)
                {
                    if (screenPosition2.X < base.Center.X * 2 - screenRadius)
                    {
                        if (screenPosition2.Y < base.Center.Y * 2 - screenRadius)
                        {
                            base.Position -= screenPosition1 - screenPosition2;
                            return;
                        }
                    }
                }
            }

            base.Position = Vector2.Zero;
        }

    }
}