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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1708E_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SongList : Page
    {
        private static string API_GETSONG = "http://2-dot-backup-server-002.appspot.com//_api/v2/songs";
        public SongList()
        {
            this.InitializeComponent();
        }

        private async void ClickMe_Click(object sender, RoutedEventArgs e)
        {
            Windows.Storage.StorageFolder storageFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
            Windows.Storage.StorageFile sampleFile = await storageFolder.GetFileAsync("token.txt");
            string text = await Windows.Storage.FileIO.ReadTextAsync(sampleFile);
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(text);
            HttpClient client2 = new HttpClient();
            client2.DefaultRequestHeaders.Add("Authorization", "Basic " + token.token);
            var resp = client2.GetAsync(API_GETSONG).Result;
            var respContent = await resp.Content.ReadAsStringAsync();
            var songs = JsonConvert.DeserializeObject<List<Song>>(respContent);
            Debug.WriteLine(songs[2].name);
        }

        //private void PlayMedia()
        //{
        //    Uri manifestUri = new Uri("C:/Users/VuPhuc/Downloads/New folder/03. Sparkle (English ver.).mp3");
        //    mediaPlayer.Source = MediaSource.CreateFromUri(manifestUri);
        //}

        private void mediaPlayer_Click(object sender, RoutedEventArgs e)
        {
            Uri manifestUri = new Uri("https://aredir.nixcdn.com/NhacCuaTui957/UnravelTokyoGhoulOpening-TKLingTositeSigure-3252204.mp3");
            MediaPlayer mediaPlayer = new MediaPlayer();
            mediaPlayer.AutoPlay = false;
            mediaPlayer.Source = MediaSource.CreateFromUri(manifestUri);
            mediaPlayer.Play();
            if(Play.Content == "Pause")
            {

            }
        }

        private void LoadMediaFromString()
        {
            try
            {
                Uri pathUri = new Uri("https://aredir.nixcdn.com/NhacCuaTui957/UnravelTokyoGhoulOpening-TKLingTositeSigure-3252204.mp3");
                mediaSimple.Source = MediaSource.CreateFromUri(pathUri);
            }
            catch (Exception ex)
            {
                if (ex is FormatException)
                {
                    // handle exception.
                    // For example: Log error or notify user problem with file
                }
            }
        }
    }
}
