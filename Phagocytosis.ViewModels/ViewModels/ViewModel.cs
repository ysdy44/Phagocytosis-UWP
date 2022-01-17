using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Phagocytosis.ViewModels
{
    /// <summary> 
    /// Represents a ViewModel that contains some methods of the application
    /// </summary>
    public partial class ViewModel : INotifyPropertyChanged, IDisposable
    {

        public int SelectedIndex
        {
            get => this.selectedIndex;
            set
            {
                this.selectedIndex = value;
                this.OnPropertyChanged(nameof(SelectedIndex)); // Notify
            }
        }
        private int selectedIndex;


        public int Count => this.Chapters.Count;
        public ChapterViewItem CurrentChapter => this.Chapters[this.SelectedIndex];

        public readonly IList<ChapterViewItem> Chapters = new List<ChapterViewItem>();


        public bool Next()
        {
            int index = this.SelectedIndex;
            if (index == -1) return false;

            index++;
            if (index >= this.Chapters.Count) return false;

            if (this.SelectedIndex > this.GetGroupIndex()) return false;
            this.SelectedIndex = index;
            return true;
        }

        public bool Pass()
        {
            int index = this.SelectedIndex;
            if (index == -1) return false;

            index++;
            if (index >= this.Chapters.Count) return false;

            if (this.GetGroupIndex() < index) this.SetGroupIndex(index);
            return true;
        }


        public void LoadFromProject(IEnumerable<Chapter> chapters)
        {
            this.Chapters.Clear();

            int groupIndex = this.GetGroupIndex();
            foreach (Chapter item in chapters)
            {
                this.Chapters.Add(new ChapterViewItem
                {
                    Chapter = item,
                    GroupIndex = groupIndex
                });
            }

            for (int i = 0; i < this.Chapters.Count; i++)
            {
                ChapterViewItem chapter = this.Chapters[i];
                chapter.Index = i;
                chapter.Chapter.IsGuider = i == 0;
            }

            this.selectedIndex = groupIndex;
            this.OnPropertyChanged(nameof(SelectedIndex)); // Notify
            this.OnPropertyChanged(nameof(Count)); // Notify
        }


        public int GetGroupIndex()
        {
            Windows.Storage.ApplicationDataContainer container = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (container.Values.ContainsKey("ChaptersGroupIndex"))
            {
                if (container.Values["ChaptersGroupIndex"] is int value)
                {
                    return value;
                }
            }
            return 0;
        }

        public void SetGroupIndex(int value)
        {
            Windows.Storage.ApplicationDataContainer container = Windows.Storage.ApplicationData.Current.LocalSettings;
            container.Values["ChaptersGroupIndex"] = value;

            foreach (ChapterViewItem item in this.Chapters)
            {
                item.GroupIndex = value;
            }
        }


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