using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Net.Http;
using T1708E_UWP.Entity;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Composition;
using System.Drawing;
using Windows.UI.Xaml.Media;
using Windows.UI;



// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1708E_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplitView : Page
    {
        private ObservableCollection<Song> _listSong;
        public SplitView()
        {
            this.InitializeComponent();
        }

        public void OnActiveAsync(object sender, RoutedEventArgs e)
        {          
            this.MainFrame.Navigate(typeof(Views.SongList));
        }


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.MySplitView.IsPaneOpen = !this.MySplitView.IsPaneOpen;
            if (!this.MySplitView.IsPaneOpen)
            {
                this.StackIcon.Margin = new Thickness(10, 50, 0, 0);
                this.MainFrame.Margin = new Thickness(0, 0, 0, 0);
            }
            else
            {
                this.StackIcon.Margin = new Thickness(50, 50, 0, 0);
                this.MainFrame.Margin = new Thickness(250, 0, 0, 0);
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            switch (radio.Tag.ToString())
            {
                case "Search":
                    this.MySplitView.IsPaneOpen = true;
                    this.search_box.Focus(FocusState.Programmatic);
                    break;
                case "Home":
                    this.MainFrame.Navigate(typeof(Views.SongList));
                    break;
                case "Account":
                    this.MainFrame.Navigate(typeof(UserInfo));
                    break;
                case "CreateSong":
                    this.MainFrame.Navigate(typeof(Views.SongForm));
                    break;
                default:
                    break;
            }

        }

        private void MySplitView_PaneClosed(Windows.UI.Xaml.Controls.SplitView sender, object args)
        {
            this.MainFrame.Margin = new Thickness(0, 0, 0, 0);
        }

        private void search_box_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
           
        }
    }
}
