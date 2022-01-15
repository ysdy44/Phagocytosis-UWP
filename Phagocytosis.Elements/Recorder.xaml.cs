using System;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Phagocytosis.Elements
{
    /// <summary>
    /// Represents a recorder.
    /// </summary>
    public sealed partial class Recorder : UserControl
    {

        //@Converter
        public string TimeSpanToTextConverter(TimeSpan value) => value.ToString("mm':'ss'.'ff");
        public string TimeSpanToTextConverter() => "00:00.00";


        //@Delegate
        public event RoutedEventHandler BackButtonClick
        {
            remove => this.BackButton.Click -= value;
            add => this.BackButton.Click += value;
        }
        public event RoutedEventHandler RestartButtonClick
        {
            remove => this.RestartButton.Click -= value;
            add => this.RestartButton.Click += value;
        }
        public event RoutedEventHandler PlayButtonClick
        {
            remove => this.PlayButton.Click -= value;
            add => this.PlayButton.Click += value;
        }
        public event RoutedEventHandler NextButtonClick
        {
            remove => this.NextButton.Click -= value;
            add => this.NextButton.Click += value;
        }


        #region DependencyProperty


        /// <summary> Gets or set the accent PlayState for <see cref="Recorder"/>. </summary>
        public PlayState State
        {
            get => (PlayState)base.GetValue(StateProperty);
            set => base.SetValue(StateProperty, value);
        }
        /// <summary> Identifies the <see cref = "Recorder.State" /> dependency property. </summary>
        public static readonly DependencyProperty StateProperty = DependencyProperty.Register(nameof(State), typeof(PlayState), typeof(Recorder), new PropertyMetadata(PlayState.Playing, (sender, e) =>
        {
            Recorder control = (Recorder)sender;

            if (e.NewValue is PlayState value)
            {
                control.StateChanged(value);
            }
        }));

        public void Pause() => this.StateChanged(PlayState.Paused);
        private void StateChanged(PlayState value)
        {
            switch (value)
            {
                case PlayState.Paused:
                case PlayState.Loser:
                case PlayState.Winner:
                    base.Visibility = Visibility.Visible;
                    this.Start();
                    break;
                default:
                    base.Visibility = Visibility.Collapsed;
                    break;
            }

            switch (value)
            {
                case PlayState.Loser:
                case PlayState.Winner:
                    this.LevelGrid.Visibility = Visibility.Visible;
                    this.CountGrid.Visibility = Visibility.Visible;
                    this.TimeGrid.Visibility = Visibility.Visible;
                    break;
                default:
                    this.LevelGrid.Visibility = Visibility.Collapsed;
                    this.CountGrid.Visibility = Visibility.Collapsed;
                    this.TimeGrid.Visibility = Visibility.Collapsed;
                    break;
            }

            this.LoserTextBlock.Visibility =
            this.LoserTipTextBlock.Visibility =
                value == PlayState.Loser ? Visibility.Visible : Visibility.Collapsed;

            this.WinnerTextBlock.Visibility =
            this.WinnerTipTextBlock.Visibility =
            this.NextButton.Visibility =
                value == PlayState.Winner ? Visibility.Visible : Visibility.Collapsed;

            this.PauserTextBlock.Visibility =
            this.PauserTipTextBlock.Visibility =
            this.PlayButton.Visibility =
                value == PlayState.Paused ? Visibility.Visible : Visibility.Collapsed;

            switch (value)
            {
                case PlayState.Winner:
                    this.NextButton.Focus(FocusState.Keyboard);
                    break;
                case PlayState.Loser:
                    this.RestartButton.Focus(FocusState.Keyboard);
                    break;
                case PlayState.Paused:
                    this.PlayButton.Focus(FocusState.Keyboard);
                    break;
                default:
                    break;
            }
        }


        #endregion


        int MaxLevel;
        int MaxLevelSpace;
        int MaxLevelIndex;

        int MaxCount;
        int MaxCountSpace;
        int MaxCountIndex;

        TimeSpan Duration;
        TimeSpan DurationSpace;
        int DurationIndex;

        readonly DispatcherTimer Timer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };


        //@Construct
        /// <summary>
        /// Initializes a Recorder. 
        /// </summary>
        public Recorder()
        {
            this.InitializeComponent();
            this.ConstructFlowDirection();
            this.ConstructStrings();
            this.Timer.Tick += (s, e) =>
            {
                if (this.MaxLevelIndex <= 0)
                {
                    if (this.MaxCountIndex <= 0)
                    {
                        if (this.DurationIndex <= 0)
                        {
                            this.Timer.Stop();
                        }
                        else
                        {
                            this.DurationIndex--;
                            TimeSpan time = this.Duration - TimeSpan.FromSeconds(this.DurationIndex * this.DurationSpace.TotalSeconds);
                            this.DurationTextBlock.Text = this.TimeSpanToTextConverter(time);
                        }
                    }
                    else
                    {
                        this.MaxCountIndex--;
                        int count = this.MaxCount - this.MaxCountIndex * this.MaxCountSpace;
                        this.MaxCountTextBlock.Text = count.ToString();
                    }
                }
                else
                {
                    this.MaxLevelIndex--;
                    int level = this.MaxLevel - this.MaxLevelIndex * this.MaxLevelSpace;
                    this.MaxLevelTextBlock.Text = level.ToString();
                }
            };
        }

        // FlowDirection
        private void ConstructFlowDirection()
        {
            bool isRightToLeft = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            base.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        // Strings
        private void ConstructStrings()
        {
            ResourceLoader resource = ResourceLoader.GetForCurrentView();

            this.WinnerTextBlock.Text = resource.GetString("Win");
            this.LoserTextBlock.Text = resource.GetString("Lose");
            this.PauserTextBlock.Text = resource.GetString("Pause");

            this.MaxLevelTipTextBlock.Text = resource.GetString("MaxLevel");
            this.MaxCountTipTextBlock.Text = resource.GetString("MaxCount");
            this.DurationTipTextBlock.Text = resource.GetString("Duration");

            this.Click001Run.Text = resource.GetString("Click");
            this.ToRestart.Text = resource.GetString("ToRestart");
            this.Click002Run.Text = resource.GetString("Click");
            this.ToNext.Text = resource.GetString("ToNext");
            this.Click003Run.Text = resource.GetString("Click");
            this.ToPlay.Text = resource.GetString("ToPlay");
        }

        public void Record(int level, int count, TimeSpan startingTime)
        {
            if (this.MaxLevel < level) this.MaxLevel = level;
            if (this.MaxCount < count) this.MaxCount = count;
            this.Duration = DateTime.Now.TimeOfDay - startingTime;
        }

        public void Reset()
        {
            this.MaxLevel = 0;
            this.MaxCount = 0;
            this.Duration = TimeSpan.Zero;
        }

        private void Start()
        {
            this.MaxLevelTextBlock.Text = 0.ToString();
            if (this.MaxLevel > 100 * 10)
            {
                this.MaxLevelSpace = this.MaxLevel / 10;
                this.MaxLevelIndex = 10;
            }
            else
            {
                this.MaxLevelSpace = 100;
                this.MaxLevelIndex = this.MaxLevel / 100;
            }


            this.MaxCountTextBlock.Text = 0.ToString();
            if (this.MaxCount > 1 * 10)
            {
                this.MaxCountSpace = this.MaxCount / 10;
                this.MaxCountIndex = 10;
            }
            else
            {
                this.MaxCountSpace = 1;
                this.MaxCountIndex = this.MaxCount / 1;
            }


            this.DurationTextBlock.Text = this.TimeSpanToTextConverter();
            if (this.Duration > TimeSpan.FromSeconds(1 * 10))
            {
                this.DurationSpace = TimeSpan.FromSeconds(this.Duration.TotalSeconds / 10);
                this.DurationIndex = 10;
            }
            else
            {
                this.DurationSpace = TimeSpan.FromSeconds(1);
                this.DurationIndex = (int)(this.Duration.TotalSeconds / 1);
            }


            this.Timer.Start();
        }

    }
}