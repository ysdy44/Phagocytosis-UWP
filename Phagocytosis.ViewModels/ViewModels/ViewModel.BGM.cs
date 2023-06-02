using System;
using System.ComponentModel;
using Windows.Media.Playback;

namespace Phagocytosis.ViewModels
{
    /// <summary> 
    /// Represents a ViewModel that contains some methods of the application
    /// </summary>
    public partial class ViewModel : INotifyPropertyChanged, IDisposable
    {

        private bool isMuted;
        public bool IsMuted
        {
            get => this.isMuted;
            set
            {
                this.IsMutedCore = value;
                this.isMuted = value;
                this.OnPropertyChanged(nameof(IsMuted)); // Notify
            }
        }

        private bool IsMutedCore
        {
            set
            {
                this.BackgroundBGM.IsMuted = value;
                this.ForegroundBGM.IsMuted = value;
            }
        }

        readonly MediaPlayer BackgroundBGM = new MediaPlayer
        {
            AutoPlay = true,
            IsLoopingEnabled = true,
            Source = Windows.Media.Core.MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/12710.wav"))
        };

        readonly MediaPlayer ForegroundBGM = new MediaPlayer
        {
            AutoPlay = false,
            IsLoopingEnabled = false,
            Source = Windows.Media.Core.MediaSource.CreateFromUri(new Uri("ms-appx:///Sounds/14439.trim.mp3"))
        };

        public void PlayForegroundBGM()
        {
            this.ForegroundBGM.Play();
        }

        public void Dispose()
        {
            this.BackgroundBGM.Dispose();
            this.ForegroundBGM.Dispose();
        }

    }
}