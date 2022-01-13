using System;
using System.ComponentModel;
using Windows.UI.Xaml;

namespace Phagocytosis.ViewModels
{
    /// <summary> 
    /// Represents a ViewModel that contains some methods of the application
    /// </summary>
    public partial class ViewModel : INotifyPropertyChanged, IDisposable
    {

        //@Delegate
        public event Action OnSuspending;
        public event Action OnResuming;

        public void Suspend()
        {
            this.IsMutedCore = true;
            this.OnSuspending?.Invoke(); // Delegate
        }
        public void Resumed()
        {
            this.IsMutedCore = this.IsMuted;
            this.OnResuming?.Invoke(); // Delegate
        }


        public Visibility Developer
        {
            get => this.developer;
            set
            {
                this.developer = value;
                this.OnPropertyChanged(nameof(Developer)); // Notify
            }
        }
        private Visibility developer = Visibility.Collapsed;

    }
}