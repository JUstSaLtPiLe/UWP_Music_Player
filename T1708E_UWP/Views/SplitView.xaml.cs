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
        private static string API_GETSONG = "http://2-dot-backup-server-002.appspot.com//_api/v2/songs";
        private ObservableCollection<Song> _listSong;
        internal ObservableCollection<Song> ListSongs { get => _listSong; set => _listSong = value; }
        DispatcherTimer dispatcherTimer;
        private bool PlayingStatus = true;
        private int _currentIndex;
        public SplitView()
        {
            this.InitializeComponent();
            this.ListSongs = new ObservableCollection<Song>();
        }

        public async void OnActiveAsync(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("token.txt");
            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(text);
            HttpClient client2 = new HttpClient();
            client2.DefaultRequestHeaders.Add("Authorization", "Basic " + token.token);
            var resp = client2.GetAsync(API_GETSONG).Result;
            var respContent = await resp.Content.ReadAsStringAsync();
            var songs = JsonConvert.DeserializeObject<ObservableCollection<Song>>(respContent);
            foreach(var song in songs)
            {
                ListSongs.Add(song);
            }
        }

        private void PlayCurrentSong(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            StackPanel currentPanel = sender as StackPanel;
            _currentIndex = this.MusicView.SelectedIndex;
            Uri songLink = new Uri(this.ListSongs[_currentIndex].link);
            this.myMediaElement.Source = songLink;
            //var currentButton = (AppBarButton)currentPanel.Children[2];
            //currentButton.Icon = new SymbolIcon(Symbol.Pause);
            //currentButton.Label = "Pause";
            PlayButton.Icon = new SymbolIcon(Symbol.Pause);
            DispatcherTimerSetup();
            Debug.WriteLine(_currentIndex);
        }

        //private void PlayButton_Click_1(object sender, RoutedEventArgs e)
        //{
        //    if (sender is AppBarButton appBarButton)
        //    {
        //        if (appBarButton.Tag is Song mySong)
        //        {
        //            Uri songLink = new Uri(mySong.link);
        //            this.myMediaElement.Source = songLink;
        //            OnMouseDownPlayMedia();
        //            appBarButton.Icon = new SymbolIcon(Symbol.Pause);
        //            PlayButton.Icon = new SymbolIcon(Symbol.Pause);
        //        }
        //    }
        //    //AppBarButton currentButton = sender as AppBarButton;
        //    //if (PlayingStatus == false)
        //    //{
        //    //    this.myMediaElement.Play();
        //    //    currentButton.Icon = new SymbolIcon(Symbol.Pause);
        //    //    currentButton.Label = "Pause";
        //    //    PlayButton.Icon = new SymbolIcon(Symbol.Pause);
        //    //    PlayingStatus = true;
        //    //    DispatcherTimerSetup();
        //    //}
        //    //else if (PlayingStatus == true)
        //    //{
        //    //    this.myMediaElement.Pause();
        //    //    currentButton.Icon = new SymbolIcon(Symbol.Play);
        //    //    PlayButton.Icon = new SymbolIcon(Symbol.Play);
        //    //    currentButton.Label = "Play";
        //    //    PlayingStatus = false;
        //    //    DispatcherTimerSetup();
        //    //}
        //}

        private void OnMouseDownPlayMedia()
        {
            if (PlayingStatus == false)
            {
                this.myMediaElement.Play();
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
                PlayingStatus = true;
                DispatcherTimerSetup();
            }
            else if (PlayingStatus == true)
            {
                this.myMediaElement.Pause();
                PlayButton.Icon = new SymbolIcon(Symbol.Play);
                PlayingStatus = false;
                DispatcherTimerSetup();
            }
        }

        private void OnMouseDownPauseMedia(object sender, RoutedEventArgs e)
        {
            this.myMediaElement.Pause();
        }

        private void OnMouseDownStopMedia(object sender, RoutedEventArgs e)
        {
            this.myMediaElement.Stop();
        }

        //private void ChangeMediaVolume(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        //{
        //    myMediaElement.Volume = (double)volumeSlider.Value;
        //}

        private void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            this.myMediaElement.Stop();
        }

        public void DispatcherTimerSetup()
        {
            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += dispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);
            dispatcherTimer.Start();
        }

        void dispatcherTimer_Tick(object sender, object e)
        {
            this.timelineSlider.Value = this.myMediaElement.Position.TotalMilliseconds;
        }

        private void Element_MediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = this.myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
        }

        void changeSlider()
        {
            this.timelineSlider.Value = this.myMediaElement.Position.TotalMilliseconds;
        }
        private void SeekToMediaPosition(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            int SliderValue = (int)timelineSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            this.myMediaElement.Position = ts;
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
                case "Search":
                    this.MySplitView.IsPaneOpen = true;
                    break;
                case "Home":
                    this.MainFrame.Navigate(typeof(Views.SplitView.Content));
                    break;
                case "Account":
                    this.MainFrame.Navigate(typeof(MainPage));
                    break;
                case "CreateSong":
                    this.MainFrame.Navigate(typeof(Views.SongForm));
                    break;
                default:
                    break;
            }
            
        }

        private void ShowButton(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //StackPanel currentPanel = sender as StackPanel;
            //var button = (AppBarButton)currentPanel.Children[2];
            //button.Visibility = Visibility.Visible;
        }

        private void HideButton(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            //StackPanel currentPanel = sender as StackPanel;
            //var button = (AppBarButton)currentPanel.Children[2];
            //button.Visibility = Visibility.Collapsed;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if(this.myMediaElement.Source == null)
            {
                Uri songLink = new Uri(ListSongs[0].link);
                this.myMediaElement.Source = songLink;
                OnMouseDownPlayMedia();
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
            }
            else
            {
                OnMouseDownPlayMedia();
            }
        }

        private void OnMouseDownPreviousMedia(object sender, RoutedEventArgs e)
        {
            if(_currentIndex == 0)
            {
                this.myMediaElement.Stop();
                Uri songLink = new Uri(ListSongs[ListSongs.Count - 1].link);
                this.myMediaElement.Source = songLink;
                OnMouseDownPlayMedia();
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
                _currentIndex = ListSongs.Count - 1;
                this.MusicView.SelectedIndex = _currentIndex;
            }
            else
            {
                this.myMediaElement.Stop();
                Uri songLink = new Uri(ListSongs[_currentIndex - 1].link);
                this.myMediaElement.Source = songLink;
                OnMouseDownPlayMedia();
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
                _currentIndex = _currentIndex - 1;
                this.MusicView.SelectedIndex = _currentIndex;

            }
        }

        private void OnMouseDownNextMedia(object sender, RoutedEventArgs e)
        {
            if(_currentIndex == ListSongs.Count - 1)
            {
                this.myMediaElement.Stop();
                Uri songLink = new Uri(ListSongs[0].link);
                this.myMediaElement.Source = songLink;
                OnMouseDownPlayMedia();
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
                _currentIndex = 0;
                this.MusicView.SelectedIndex = _currentIndex;

            }
            else
            {
                this.myMediaElement.Stop();
                Uri songLink = new Uri(ListSongs[_currentIndex + 1].link);
                this.myMediaElement.Source = songLink;
                OnMouseDownPlayMedia();
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
                _currentIndex = _currentIndex + 1;
                this.MusicView.SelectedIndex = _currentIndex;
            }
        }
    }
}
