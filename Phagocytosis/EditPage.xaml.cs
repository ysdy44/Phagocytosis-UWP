﻿using Phagocytosis.Controls;
using Phagocytosis.Sprites;
using Phagocytosis.ViewModels;
using System;
using System.Xml.Linq;
using Windows.ApplicationModel.Resources;
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
            this.ConstructStrings();
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
                if (e.ClickedItem is TextBlock textBlock)
                {
                    switch (textBlock.Name)
                    {
                        case "BacteriaTextBlock":
                            this.CanvasControl.SpriteType = SpriteType.Bacteria;
                            break;
                        case "VirusTextBlock":
                            this.CanvasControl.SpriteType = SpriteType.Virus;
                            break;
                        case "ParameciumTextBlock":
                            this.CanvasControl.SpriteType = SpriteType.Paramecium;
                            break;
                        case "LeukocyteTextBlock":
                            this.CanvasControl.SpriteType = SpriteType.Leukocyte;
                            break;
                        default:
                            break;
                    }
                }
            };
            this.EditListView.ItemClick += (s, e) =>
            {
                if (e.ClickedItem is TextBlock textBlock)
                {
                    switch (textBlock.Name)
                    {
                        case "MoveTextBlock":
                            this.CanvasControl.EditType = EditType.Move;
                            break;
                        case "AddRestrictedTextBlock":
                            this.CanvasControl.EditType = EditType.AddRestricted;
                            break;
                        case "AddFriendTextBlock":
                            this.CanvasControl.EditType = EditType.AddFriend;
                            break;
                        case "AddEnemyTextBlock":
                            this.CanvasControl.EditType = EditType.AddEnemy;
                            break;
                        default:
                            break;
                    }
                }
            };

            this.ZoomInButton.Click += (s, e) => this.CanvasControl.ZoomIn2();
            this.ZoomOutButton.Click += (s, e) => this.CanvasControl.ZoomOut2();
            this.DeleteButton.Click += (s, e) => this.CanvasControl.Delete();
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

            this.BacteriaTextBlock.Text = resource.GetString("Bacteria");
            this.VirusTextBlock.Text = resource.GetString("Virus");
            this.ParameciumTextBlock.Text = resource.GetString("Paramecium");
            this.LeukocyteTextBlock.Text = resource.GetString("Leukocyte");

            this.MoveTextBlock.Text = resource.GetString("Move");
            this.AddRestrictedTextBlock.Text = resource.GetString("AddRestricted");
            this.AddFriendTextBlock.Text = resource.GetString("AddFriend");
            this.AddEnemyTextBlock.Text = resource.GetString("AddEnemy");
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