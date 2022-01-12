using System;
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


        int Level;
        int LevelSpace;
        int LevelIndex;

        int Count;
        int CountSpace;
        int CountIndex;

        TimeSpan Time;
        TimeSpan TimeSpace;
        int TimeIndex;

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
            this.Timer.Tick += (s, e) =>
            {
                if (this.LevelIndex <= 0)
                {
                    if (this.CountIndex <= 0)
                    {
                        if (this.TimeIndex <= 0)
                        {
                            this.Timer.Stop();
                        }
                        else
                        {
                            this.TimeIndex--;
                            TimeSpan time = this.Time - TimeSpan.FromSeconds(this.TimeIndex * this.TimeSpace.TotalSeconds);
                            this.TimeTextBlock.Text = this.TimeSpanToTextConverter(time);
                        }
                    }
                    else
                    {
                        this.CountIndex--;
                        int count = this.Count - this.CountIndex * this.CountSpace;
                        this.CountTextBlock.Text = count.ToString();
                    }
                }
                else
                {
                    this.LevelIndex--;
                    int level = this.Level - this.LevelIndex * this.LevelSpace;
                    this.LevelTextBlock.Text = level.ToString();
                }
            };
        }

        public void Record(int level, int count, TimeSpan time)
        {
            if (this.Level < level) this.Level = level;
            if (this.Count < count) this.Count = count;
            if (this.Time < time) this.Time = time;
        }

        public void Reset()
        {
            this.Level = 0;
            this.Count = 0;
            this.Time = TimeSpan.Zero;
        }

        private void Start()
        {
            this.LevelTextBlock.Text = 0.ToString();
            if (this.Level > 100 * 10)
            {
                this.LevelSpace = this.Level / 10;
                this.LevelIndex = 10;
            }
            else
            {
                this.LevelSpace = 100;
                this.LevelIndex = this.Level / 100;
            }


            this.CountTextBlock.Text = 0.ToString();
            if (this.Count > 1 * 10)
            {
                this.CountSpace = this.Count / 10;
                this.CountIndex = 10;
            }
            else
            {
                this.CountSpace = 1;
                this.CountIndex = this.Count / 1;
            }


            this.TimeTextBlock.Text = this.TimeSpanToTextConverter();
            if (this.Time > TimeSpan.FromSeconds(1 * 10))
            {
                this.TimeSpace = TimeSpan.FromSeconds(this.Time.TotalSeconds / 10);
                this.TimeIndex = 10;
            }
            else
            {
                this.TimeSpace = TimeSpan.FromSeconds(1);
                this.TimeIndex = (int)(this.Time.TotalSeconds / 1);
            }


            this.Timer.Start();
        }

    }
}