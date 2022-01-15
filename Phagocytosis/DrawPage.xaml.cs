using Phagocytosis.Elements;
using Phagocytosis.ViewModels;
using System.Xml.Linq;
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
            this.CanvasControl.GameOver += (s, value) =>
            {
                this.ViewModel.PlayForegroundBGM();
                this.Recorder.State = value;

                switch (value)
                {
                    case PlayState.Winner:
                        this.ViewModel.Pass();
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


            this.Controller.GamepadBChanged += (s, value) =>
            {
                if (value == false) return;
                if (this.CanvasControl.Player == null) return;
                this.CanvasControl.Player.Dividing();
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
            this.ZoomListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is SymbolIcon symbol)
                {
                    switch (symbol.Symbol)
                    {
                        case Symbol.ZoomIn: this.CanvasControl.ZoomSprite(symbol.IsHitTestVisible, true); break;
                        case Symbol.ZoomOut: this.CanvasControl.ZoomSprite(symbol.IsHitTestVisible, false); break;
                        default: break;
                    }
                }
            };


            this.GamepadButton.Click += (s, e) => this.Controller.CheckGamepad();
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
        }

        private void OnSuspending()
        {
            this.CanvasControl.Pause();
            this.Recorder.Pause();
        }

    }
}