﻿using Microsoft.Graphics.Canvas;
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
                if (this.IsGamepadButtonsMenu)
                {
                    this.IsGamepadButtonsMenu = false;

                    PlayState state = this.CanvasControl.Paused ? PlayState.Playing : PlayState.Paused;
                    this.GameOver?.Invoke(this, state); // Delegate
                }

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
                            this.ZoomIn();
                        else
                            this.ZoomOut();
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
                this.FriendsCount = chapter.FriendSprites.Sum(a => a.Level);
                this.EnemysCount = chapter.EnemySprites.Sum(a => a.Level);

                base.Load(chapter);
                this.Player = this.FriendSprites.First(c => c.Type == SpriteType.Player).Rebirth();

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
                        this.IsGamepadButtonsMenu = true;
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

    }
}