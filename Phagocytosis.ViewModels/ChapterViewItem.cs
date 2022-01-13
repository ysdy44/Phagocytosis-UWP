using System.ComponentModel;
using Windows.UI.Xaml;

namespace Phagocytosis.ViewModels
{
    /// <summary> 
    /// Item of <see cref="Phagocytosis.ViewModels.Chapter"/>. 
    /// </summary>
    public class ChapterViewItem : INotifyPropertyChanged
    {

        public bool IsEnabled => this.GroupIndex >= this.Index;
        public bool IsCurrent => this.GroupIndex == this.Index;
        public Visibility UnlockVisibility => this.IsEnabled ? Visibility.Visible : Visibility.Collapsed;
        public Visibility LockVisibility => this.IsEnabled ? Visibility.Collapsed : Visibility.Visible;
        public Visibility CurrentVisibility => this.IsCurrent ? Visibility.Visible : Visibility.Collapsed;

        public int Index
        {
            get => this.index;
            set
            {
                this.index = value;
                this.OnPropertyChanged(nameof(Index)); // Notify
                this.OnPropertyChanged(nameof(IsEnabled)); // Notify
                this.OnPropertyChanged(nameof(IsCurrent)); // Notify
                this.OnPropertyChanged(nameof(UnlockVisibility)); // Notify
                this.OnPropertyChanged(nameof(LockVisibility)); // Notify
                this.OnPropertyChanged(nameof(CurrentVisibility)); // Notify
            }
        }
        private int index;


        public int GroupIndex
        {
            get => this.groupIndex;
            set
            {
                this.groupIndex = value;
                this.OnPropertyChanged(nameof(GroupIndex)); // Notify
                this.OnPropertyChanged(nameof(IsCurrent)); // Notify
                this.OnPropertyChanged(nameof(UnlockVisibility)); // Notify
                this.OnPropertyChanged(nameof(LockVisibility)); // Notify
                this.OnPropertyChanged(nameof(CurrentVisibility)); // Notify
            }
        }
        private int groupIndex;


        public Chapter Chapter { get; set; }


        //@Notify 
        /// <summary> Multicast event for property change notifications. </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName"> Name of the property used to notify listeners. </param>
        protected void OnPropertyChanged(string propertyName) => this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}