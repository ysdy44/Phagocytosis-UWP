﻿using Phagocytosis.Elements;
using Phagocytosis.ViewModels;
using System;
using System.Xml.Linq;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Phagocytosis
{
    /// <summary> 
    /// Represents a page used to draw vector graphics.
    /// </summary>
    public sealed partial class DrawPage : Page
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;

        //@Converter
        private Visibility VisibilityConverter(Visibility value) => value == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;

        //@Constructs
        /// <summary>
        /// Initialize a DrawPage.
        /// </summary>
        public DrawPage()
        {
            this.InitializeComponent();
            this.ConstructFlowDirection();
            this.CanvasControl.GameOver += (s, value) =>
            {
                this.ViewModel.PlayForegroundBGM();
                this.Recorder.State = value;

                switch (value)
                {
                    case PlayState.Winner:
                        this.ViewModel.Pass();
                        break;
                    case PlayState.Playing:
                        this.Play();
                        break;
                    case PlayState.Paused:
                        this.Pause();
                        break;
                }
            };
            this.CanvasControl.Scored += (s, value) =>
            {
                this.Scorer.LeftCount = value.EnemySpritesSumLevel;
                this.Scorer.RightCount = value.FriendSpritesSumLevel;
            };
            this.CanvasControl.Record += (s, value) =>
            {
                this.Recorder.Record(value.FriendSpritesMaxLevel, value.FriendSpritesCount, value.TotalTime);
            };


            this.Controller.Paused += (s, value) =>
            {
                if (value == false) return;
                if (this.CanvasControl.Player == null) return;

                switch (this.CanvasControl.State)
                {
                    case PlayState.Playing:
                        this.Pause();
                        break;
                    case PlayState.Paused:
                        this.Play();
                        break;
                }
            };
            this.Controller.Divided += (s, value) =>
            {
                if (value == false) return;
                if (this.CanvasControl.Player == null) return;
                this.CanvasControl.Player.Dividing();
            };
            this.Controller.Zoom += (s, value) =>
            {
                if (value == null) return;
                if (this.CanvasControl.Player == null) return;

                switch (value.Value)
                {
                    case false:
                        this.CanvasControl.ZoomOut();
                        break;
                    case true:
                        this.CanvasControl.ZoomIn();
                        break;
                }
            };
            this.Controller.Moved += (s, value) => this.CanvasControl.Move(-value);
            this.Controller.VectorChanged += (s, value) =>
            {
                if (this.CanvasControl.Player == null) return;
                this.CanvasControl.Player.Velocity = value;
            };


            this.ExportButton.Click += (s, e) =>
            {
                if (this.CanvasControl.Save() is Chapter chapter)
                {
                    XElement element = Phagocytosis.ViewModels.XML.SaveChapter("Chapter", chapter);
                    this.TextBox.Text = element.ToString();
                }
            };
            this.LocalFolderButton.Click += async (s, e) =>
            {
                IStorageFolder folder = ApplicationData.Current.LocalFolder;
                await Launcher.LaunchFolderAsync(folder);
            };
            this.ZoomInFriendButton.Click += (s, e) => this.CanvasControl.ZoomSprite(true, true);
            this.ZoomOutFriendButton.Click += (s, e) => this.CanvasControl.ZoomSprite(true, false);
            this.ZoomInEnemyButton.Click += (s, e) => this.CanvasControl.ZoomSprite(false, true);
            this.ZoomOutEnemyButton.Click += (s, e) => this.CanvasControl.ZoomSprite(false, false);


            this.PauseButton.Click += (s, e) => this.Pause();
            this.SplitButton.Click += (s, e) =>
            {
                this.ViewModel.PlayForegroundBGM();

                if (this.CanvasControl.Player == null) return;
                this.CanvasControl.Player.Dividing();
            };


            this.Recorder.BackButtonClick += (s, e) =>
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            };
            this.Recorder.RestartButtonClick += (s, e) => this.Restart(null);
            this.Recorder.PlayButtonClick += (s, e) => this.Play();
            this.Recorder.NextButtonClick += (s, e) =>
            {
                bool result = this.ViewModel.Next();

                if (result == false)
                {
                    base.Frame.GoBack();
                    base.Frame.Navigate(typeof(AboutPage));
                }
                else if (this.ViewModel.CurrentChapter is ChapterViewItem chapter)
                {
                    this.Restart(chapter.Chapter);
                }
            };
        }

        // FlowDirection
        private void ConstructFlowDirection()
        {
            bool isRightToLeft = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            base.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;

            this.CanvasControl.Direction = base.FlowDirection;
        }

        private void Restart(Chapter chapter)
        {
            this.Recorder.Reset();
            if (chapter != null && chapter.IsGuider) this.Guider.Begin();

            this.Recorder.State = PlayState.Playing;
            this.CanvasControl.LoadFromProject(chapter);
            this.ViewModel.PlayForegroundBGM();
        }

        private void Play()
        {
            this.CanvasControl.Play();
            this.ViewModel.PlayForegroundBGM();
            this.Recorder.State = PlayState.Playing;
        }

        private void Pause()
        {
            this.CanvasControl.Pause();
            this.ViewModel.PlayForegroundBGM();
            this.Recorder.State = PlayState.Paused;
        }

    }

    public sealed partial class DrawPage : Page
    {

        //@BackRequested
        /// <summary> The current page no longer becomes an active page. </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.ViewModel.OnSuspending -= this.OnSuspending;
            this.CanvasControl.Stop();
        }
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            switch (e.NavigationMode)
            {
                case NavigationMode.New:
                case NavigationMode.Forward:
                    {
                        if (e.Parameter is Chapter item)
                        {
                            this.Restart(item);
                        }
                    }
                    break;
            }

            this.ViewModel.OnSuspending += this.OnSuspending;
            this.CanvasControl.Start();
        }

        private void OnSuspending()
        {
            this.CanvasControl.Pause();
            this.Recorder.Pause();
        }

    }
}