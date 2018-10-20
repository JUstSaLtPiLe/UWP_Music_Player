using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;


// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1708E_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SplitView : Page
    {
        public SplitView()
        {
            this.InitializeComponent();
            this.ListViewControl.ItemsSource = new List<string> { "one", "tow", "three", "four", "five", "six", "seven", "eight", "nine" };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.MySplitView.IsPaneOpen = !this.MySplitView.IsPaneOpen;
            if (!this.MySplitView.IsPaneOpen)
            {
                this.StackIcon.Margin = new Thickness(10, 50, 0, 0);
            }
            else {
                this.StackIcon.Margin = new Thickness(50, 50, 0, 0);
            }
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            RadioButton radio = sender as RadioButton;
            switch (radio.Tag.ToString()) {
                case "Register":
                    this.MainFrame.Navigate(typeof(MainPage));
                    break;
                case "CreateSong":
                    this.MainFrame.Navigate(typeof(Views.SongForm));
                    break;
                default:
                    break;
            }
            
        }
    }
}
