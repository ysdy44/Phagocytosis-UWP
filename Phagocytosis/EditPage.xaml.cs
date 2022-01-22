using Phagocytosis.Controls;
using Phagocytosis.Sprites;
using Phagocytosis.ViewModels;
using System;
using System.Numerics;
using System.Xml.Linq;
using Windows.Foundation;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Phagocytosis
{
    /// <summary> 
    /// Represents a page used to edit map.
    /// </summary>
    public sealed partial class EditPage : Page
    {

        //@ViewModel
        ViewModel ViewModel => App.ViewModel;

        //@Constructs
        /// <summary>
        /// Initialize a EditPage.
        /// </summary>
        public EditPage()
        {
            this.InitializeComponent();
            this.ConstructFlowDirection();
            this.BackButton.Click += (s, e) =>
            {
                if (base.Frame.CanGoBack)
                {
                    base.Frame.GoBack();
                }
            };
            this.PlayButton.Click += (s, e) =>
            {
                if (this.CanvasControl.Save() is Chapter chapter)
                {
                    this.ViewModel.SelectedIndex = -1;
                    base.Frame.Navigate(typeof(DrawPage), chapter);
                }
            };

            this.CanvasControl.Tapped += (s, e) =>
            {
                Point position = e.GetPosition(this.CanvasControl);
                this.Select(this.CanvasControl.Mode, position.ToVector2());
                e.Handled = true;
            };

            this.ExportButton.Click += (s, e) =>
            {
                if (this.CanvasControl.Save() is Chapter chapter)
                {
                    this.TextBox.TextChanged -= this.TextBox_TextChanged;
                    XElement element = Phagocytosis.ViewModels.XML.SaveChapter("Chapter", chapter);
                    this.TextBox.Text = element.ToString();
                    this.TextBox.TextChanged += this.TextBox_TextChanged;
                }
            };
            this.LocalFolderButton.Click += async (s, e) =>
            {
                IStorageFolder folder = ApplicationData.Current.LocalFolder;
                await Launcher.LaunchFolderAsync(folder);
            };
            this.ZoomInFriendButton.Click += (s, e) => this.CanvasControl.ZoomSprite2(true, true);
            this.ZoomOutFriendButton.Click += (s, e) => this.CanvasControl.ZoomSprite2(true, false);
            this.ZoomInEnemyButton.Click += (s, e) => this.CanvasControl.ZoomSprite2(false, true);
            this.ZoomOutEnemyButton.Click += (s, e) => this.CanvasControl.ZoomSprite2(false, false);

            this.SpriteListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is FrameworkElement element)
                {
                    this.ViewModel.PlayForegroundBGM();
                    this.CanvasControl.Add((SpriteType)(int)element.Tag);
                    this.Select(EditSelectionMode.Add);
                }
            };
            this.EditListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is SymbolIcon symbolIcon)
                {
                    switch (symbolIcon.Symbol)
                    {
                        case Symbol.ZoomIn:
                            this.CanvasControl.ZoomIn2();
                            break;
                        case Symbol.ZoomOut:
                            this.CanvasControl.ZoomOut2();
                            break;
                        case Symbol.Delete:
                            this.CanvasControl.Delete();
                            this.Select(EditSelectionMode.Select);
                            break;
                        default:
                            break;
                    }
                }
            };

            this.IncreaseFood(null);
            this.DecreaseButton.Click += (s, e) => this.IncreaseFood(false);
            this.IncreaseButton.Click += (s, e) => this.IncreaseFood(true);
        }

        // FlowDirection
        private void ConstructFlowDirection()
        {
            bool isRightToLeft = System.Globalization.CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft;

            base.FlowDirection = isRightToLeft ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.TextBox.Text is string text)
            {
                try
                {
                    XElement element = XElement.Parse(text);
                    Chapter chapter = Phagocytosis.ViewModels.XML.LoadChapter(element);
                    this.CanvasControl.LoadFromProject(chapter);
                }
                catch (System.Exception)
                {
                }
            }
        }

        private void IncreaseFood(bool? increase)
        {
            if (increase.HasValue)
            {
                if (increase.Value)
                    this.CanvasControl.Map.Increase++;
                else
                    this.CanvasControl.Map.Increase--;
            }

            this.CanvasControl.Map.Maximum = this.CanvasControl.Map.Increase * 6;
            this.FoodTextBlock.Text = $"{this.CanvasControl.Map.Increase}";
        }

        private void Select(EditSelectionMode mode, Vector2 position = new Vector2())
        {
            EditType type =
                (mode != EditSelectionMode.None || position == Vector2.Zero) ?
                EditType.None :
                this.CanvasControl.Select(position);

            this.ZoomInItem.Visibility = this.ZoomOutItem.Visibility =
                (type == EditType.None || type == EditType.Restricted) ?
                Visibility.Collapsed : Visibility.Visible;

            this.DeleteItem.Visibility =
                (type == EditType.None || type == EditType.Player) ?
                Visibility.Collapsed : Visibility.Visible;

            switch (mode)
            {
                case EditSelectionMode.None:
                    this.EllipseStoryboard.Begin(); // Storyboard
                    this.ShowStoryboard.Begin(); // Storyboard
                    if (type != EditType.None)
                    {
                        this.ShowEditStoryboard.Begin(); // Storyboard
                        this.CanvasControl.Mode = EditSelectionMode.Select;
                    }
                    else
                    {
                        this.ShowSpriteStoryboard.Begin(); // Storyboard
                        this.CanvasControl.Mode = EditSelectionMode.Add;
                    }
                    break;
                case EditSelectionMode.Add:
                    this.EllipseStoryboard.Stop(); // Storyboard
                    this.HideStoryboard.Begin(); // Storyboard

                    this.HideSpriteStoryboard.Begin(); // Storyboard
                    this.CanvasControl.Mode = EditSelectionMode.None;
                    break;
                case EditSelectionMode.Select:
                    this.EllipseStoryboard.Stop(); // Storyboard
                    this.HideStoryboard.Begin(); // Storyboard

                    this.HideEditStoryboard.Begin(); // Storyboard
                    this.CanvasControl.Mode = EditSelectionMode.None;
                    break;
                default:
                    break;
            }
        }

    }

    public sealed partial class EditPage : Page
    {

        //@BackRequested
        /// <summary> The current page no longer becomes an active page. </summary>
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
        }
        /// <summary> The current page becomes the active page. </summary>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.ViewModel.PlayForegroundBGM();
        }

    }
}