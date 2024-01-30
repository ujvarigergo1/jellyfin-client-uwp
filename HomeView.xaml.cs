﻿using Jellyfin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Jellyfin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HomeView : Page
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        UserDto User;
        SessionInfo SessionInfo;
        ObservableCollection<BaseItemDto> ViewList = new ObservableCollection<BaseItemDto>();
        public HomeView()
        {

            this.InitializeComponent();

            this.User = JsonConvert.DeserializeObject<UserDto>(localSettings.Values["User"] as string);
            this.SessionInfo = JsonConvert.DeserializeObject<SessionInfo>(localSettings.Values["Session"] as string);
            UsernameTextbox.Text = User.Name;
            getUserViews();
        }
        public async void getUserViews()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Users/" + this.User.Id + "/Views");
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var stringResponse = JsonObject.Parse(await authenticationResponse.Content.ReadAsStringAsync());
                    foreach (var item in JsonConvert.DeserializeObject<ObservableCollection<BaseItemDto>>(stringResponse["Items"].ToString()))
                    {
                        this.ViewList.Add(item);
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            localSettings.Values.Remove("User");
            localSettings.Values.Remove("Session");
            localSettings.Values.Remove("ServerId");
            localSettings.Values.Remove("AccessToken");
            Frame.Navigate(typeof(MainPage));
        }
        public string getpictureUrl()
        {
            return "";
        }


        private void MediaLibraries_SelectionChanged(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(MediaItemsBrowser), e.ClickedItem);

        }
    }
}