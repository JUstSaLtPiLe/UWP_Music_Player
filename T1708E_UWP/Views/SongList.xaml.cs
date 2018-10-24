using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using T1708E_UWP.Entity;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Media.Core;
using Windows.Media.Playback;
using System.Collections.ObjectModel;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1708E_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SongList : Page
    {
        private static string API_GETSONG = "http://2-dot-backup-server-002.appspot.com//_api/v2/songs";
        private ObservableCollection<Song> _listSong;
        internal ObservableCollection<Song> ListSongs { get => _listSong; set => _listSong = value; }
        DispatcherTimer dispatcherTimer;
        private bool PlayingStatus = true;
        private int _currentIndex;
        private string shuffle = "no shuffle";
        private List<int> played_songs = new List<int>();
        Random rnd_song_index = new Random();
        int shuffle_index;
        TimeSpan current_time;
        public SongList()
        {
            this.InitializeComponent();
            this.ListSongs = new ObservableCollection<Song>();
        }

        public async void OnActiveAsync(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file_token = await storageFolder.GetFileAsync("token.txt");
            string text = await FileIO.ReadTextAsync(file_token);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(text);
            HttpClient client2 = new HttpClient();
            client2.DefaultRequestHeaders.Add("Authorization", "Basic " + token.token);
            var resp = client2.GetAsync(API_GETSONG).Result;
            var respContent = await resp.Content.ReadAsStringAsync();
            var songs = JsonConvert.DeserializeObject<ObservableCollection<Song>>(respContent);
            foreach (var song in songs)
            {
                ListSongs.Add(song);
            }
            volumeSlider.Value = 100;

            string rootPath = ApplicationData.Current.LocalFolder.Path;
            string filePath = Path.Combine(rootPath, "appSettings.txt");
            if (!System.IO.File.Exists(filePath))
            {
                StorageFolder folder = ApplicationData.Current.LocalFolder;
                StorageFile file_appSetings = await folder.CreateFileAsync("appSettings.txt", CreationCollisionOption.ReplaceExisting);
            }
            else
            {
                StorageFile file_appSetings = await storageFolder.GetFileAsync("appSettings.txt");
                shuffle = await FileIO.ReadTextAsync(file_appSetings);
                if(shuffle == "shuffle")
                {
                    shuffle_btn.IsChecked = true;
                }
            }
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
            current_time = TimeSpan.FromMilliseconds(this.myMediaElement.Position.TotalMilliseconds);
            this.current_runtime.Text = string.Format("{0:D2}:{1:D2}", current_time.Minutes, current_time.Seconds);
            this.current_runtime.Visibility = Visibility.Visible;
        }

        private void Element_MediaOpened(object sender, RoutedEventArgs e)
        {
            timelineSlider.Maximum = this.myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds;
            myMediaElement.Volume = volumeSlider.Value/100;
            TimeSpan runtime = TimeSpan.FromMilliseconds(this.myMediaElement.NaturalDuration.TimeSpan.TotalMilliseconds);
            this.duration.Text = string.Format("{0:D2}:{1:D2}", runtime.Minutes, runtime.Seconds);
            this.duration.Visibility = Visibility.Visible;
        }

        private void Element_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (shuffle != "shuffle")
            {
                this.myMediaElement.Stop();
                Uri songLink = new Uri(ListSongs[_currentIndex + 1].link);
                this.myMediaElement.Source = songLink;
                OnMouseDownPlayMedia();
                PlayButton.Icon = new SymbolIcon(Symbol.Pause);
                _currentIndex = _currentIndex + 1;
                this.MusicView.SelectedIndex = _currentIndex;
            }
            else
            {
                do
                {
                    shuffle_index = rnd_song_index.Next(ListSongs.Count);
                } while (played_songs.Contains(shuffle_index));
                played_songs.Add(shuffle_index);
                this.myMediaElement.Stop();
                Uri songLink = new Uri(ListSongs[shuffle_index].link);
                this.myMediaElement.Source = songLink;
                _currentIndex = shuffle_index;
                OnMouseDownPlayMedia();
                this.MusicView.SelectedIndex = _currentIndex;
            }
        }

        private void PlayCurrentSong(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            StackPanel currentPanel = sender as StackPanel;
            _currentIndex = this.MusicView.SelectedIndex;
            Uri songLink = new Uri(this.ListSongs[_currentIndex].link);
            this.myMediaElement.Source = songLink;
            PlayButton.Icon = new SymbolIcon(Symbol.Pause);
            DispatcherTimerSetup();
            Debug.WriteLine(_currentIndex);
        }

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

        private void SeekToMediaPosition(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            int SliderValue = (int)timelineSlider.Value;
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, SliderValue);
            this.myMediaElement.Position = ts;
        }

        private void ChangeMediaVolume(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            myMediaElement.Volume = (double)(volumeSlider.Value/100);
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.myMediaElement.Source == null)
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
            if (_currentIndex == 0)
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
            if (_currentIndex == ListSongs.Count - 1)
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

        private void MusicView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MusicView.ScrollIntoView(MusicView.SelectedItem);
        }

        private void Repeat_Click(object sender, RoutedEventArgs e)
        {
            this.myMediaElement.Stop();
            this.myMediaElement.Play();
        }

        private async void shuffle_btn_Checked(object sender, RoutedEventArgs e)
        {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile file_appSetings = await storageFolder.GetFileAsync("appSettings.txt");
                await FileIO.WriteTextAsync(file_appSetings, "shuffle");
                shuffle = "shuffle";
        }

        private async void shuffle_btn_Unchecked(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file_appSetings = await storageFolder.GetFileAsync("appSettings.txt");
            await FileIO.WriteTextAsync(file_appSetings, "no shuffle");
            shuffle = "no shuffle";
        }
    }
}
