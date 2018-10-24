using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using T1708E_UWP.Entity;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace T1708E_UWP.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserInfo : Page
    {
        private static string API_USER_INFOMATION = "http://2-dot-backup-server-002.appspot.com/_api/v2/members/information";
        public UserInfo()
        {
            this.InitializeComponent();
        }

        private async void UserInfo_Loaded(object sender, RoutedEventArgs e)
        {
            Run run = new Run();
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file_token = await storageFolder.GetFileAsync("token.txt");
            TokenResponse token = JsonConvert.DeserializeObject<TokenResponse>(await FileIO.ReadTextAsync(file_token));
            HttpClient client2 = new HttpClient();
            client2.DefaultRequestHeaders.Add("Authorization", "Basic " + token.token);
            var resp = client2.GetAsync(API_USER_INFOMATION).Result;
            var respContent = await resp.Content.ReadAsStringAsync();
            var user_info = JsonConvert.DeserializeObject<Member>(respContent);
            this.name.Text = user_info.firstName + " " + user_info.lastName;
            this.email.Text = user_info.email;
            this.phone.Text = user_info.phone;
            this.address.Text = user_info.address;
            this.email.Text = user_info.email;
            run.Text = user_info.introduction;
            this.introduction_content.Inlines.Add(run);
        }

        private async void BtnLogOut_Click(object sender, RoutedEventArgs e)
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file_token = await storageFolder.GetFileAsync("token.txt");
            StorageFile file_loginStatus = await storageFolder.GetFileAsync("loginStatus.txt");
            await file_token.DeleteAsync(StorageDeleteOption.Default);
            await file_loginStatus.DeleteAsync(StorageDeleteOption.Default);
            this.Frame.Navigate(typeof(LoginForm));
        }
    }
}
