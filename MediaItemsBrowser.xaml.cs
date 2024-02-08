using Jellyfin.Models;
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
using System.Xml.Linq;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MediaItemsBrowser : Page
    {

        ObservableCollection<BaseItemDto> UserMedia = new ObservableCollection<BaseItemDto>();

        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        BaseItemDto MediaLibrary;
        UserDto User;
        SessionInfo SessionInfo;
        public MediaItemsBrowser()
        {
            this.InitializeComponent();

            this.User = JsonConvert.DeserializeObject<UserDto>(localSettings.Values["User"] as string);
            this.SessionInfo = JsonConvert.DeserializeObject<SessionInfo>(localSettings.Values["Session"] as string);
            Windows.UI.ViewManagement.ApplicationViewScaling.TrySetDisableLayoutScaling(true);

        }

        public async void getUserMedia()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                string parentId;
                if(this.MediaLibrary.DisplayPreferencesId != null) {parentId = this.MediaLibrary.DisplayPreferencesId; } else { parentId = this.MediaLibrary.Id.ToString(); }
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Users/" + this.User.Id + "/Items?ParentId=" + parentId + (this.MediaLibrary.CollectionType == CollectionType.tvshows? "&Recursive=true&IncludeItemTypes=Series": ""));
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var stringResponse = JsonObject.Parse(await authenticationResponse.Content.ReadAsStringAsync());
                    var items = JsonConvert.DeserializeObject<ObservableCollection<BaseItemDto>>(stringResponse["Items"].ToString());
                    foreach (var item in items)
                    {
                        this.UserMedia.Add(item);
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.MediaLibrary = (BaseItemDto)e.Parameter;
            getUserMedia();
        }
        private void MediaLibraries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if((((sender as GridView).SelectedValue) as BaseItemDto).Type == BaseItemKind.Series)
            {
                Frame.Navigate(typeof(EpisodeSelectionView), (sender as GridView).SelectedValue);
            } else if((((sender as GridView).SelectedValue) as BaseItemDto).Type == BaseItemKind.BoxSet)
            {
                Frame.Navigate(typeof(MediaItemsBrowser), (sender as GridView).SelectedValue);
            } else
            {

                Frame.Navigate(typeof(MediaPlayerView), (sender as GridView).SelectedValue);
            }
        }
        private void MediaLibrary_SelectionChanged(object sender, ItemClickEventArgs e)
        {
            Frame.Navigate(typeof(MediaPlayerView), e.ClickedItem);

        }
        }
    }
