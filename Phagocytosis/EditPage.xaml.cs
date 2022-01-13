using Phagocytosis.Controls;
using Phagocytosis.Sprites;
using Phagocytosis.ViewModels;
using System.Xml.Linq;
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

            this.ZoomListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is SymbolIcon symbol)
                {
                    this.CanvasControl.ZoomSprite(symbol.IsHitTestVisible, symbol.Symbol);
                }
            };

            this.EditListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is SymbolIcon symbol)
                {
                    switch (symbol.Symbol)
                    {
                        case Symbol.MapPin:
                            this.CanvasControl.EditType = EditType.View;
                            break;
                        case Symbol.Delete:
                            this.CanvasControl.EditType = EditType.Clear;
                            break;

                        case Symbol.World:
                            this.CanvasControl.EditType = EditType.AddCell;
                            this.CanvasControl.SpriteType = SpriteType.Cell;
                            break;
                        case Symbol.Globe:
                            this.CanvasControl.EditType = EditType.AddCell;
                            this.CanvasControl.SpriteType = SpriteType.Bacteria;
                            break;
                        case Symbol.Target:
                            this.CanvasControl.EditType = EditType.AddCell;
                            this.CanvasControl.SpriteType = SpriteType.Virus;
                            break;
                        case Symbol.Account:
                            this.CanvasControl.EditType = EditType.AddCell;
                            this.CanvasControl.SpriteType = SpriteType.Paramecium;
                            break;
                        case Symbol.Bold:
                            this.CanvasControl.EditType = EditType.AddCell;
                            this.CanvasControl.SpriteType = SpriteType.Leukocyte;
                            break;
                        case Symbol.Italic:
                            this.CanvasControl.EditType = EditType.AddCell;
                            this.CanvasControl.SpriteType = SpriteType.Prion;
                            break;
                        case Symbol.Favorite:
                            this.CanvasControl.EditType = EditType.AddCell;
                            this.CanvasControl.SpriteType = SpriteType.Cancer;
                            break;
                        case Symbol.Crop:
                            this.CanvasControl.EditType = EditType.CursorRestricted;
                            break;

                        case Symbol.ZoomIn:
                            this.CanvasControl.EditType = EditType.ZoomIn;
                            break;
                        case Symbol.ZoomOut:
                            this.CanvasControl.EditType = EditType.ZoomOut;
                            break;
                        default:
                            break;
                    }
                }
            };
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