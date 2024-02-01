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
    public sealed partial class EpisodeSelectionView : Page
    {


        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        BaseItemDto MediaLibrary;
        BaseItemDto mediaItem;
        BaseItemDto selectedSeason = new BaseItemDto();
        ObservableCollection<BaseItemDto> seasonItems = new ObservableCollection<BaseItemDto>();
        ObservableCollection<BaseItemDto> episodeItems = new ObservableCollection<BaseItemDto>();
        UserDto User;
        SessionInfo SessionInfo;

        public EpisodeSelectionView()
        {
            this.InitializeComponent();
            this.User = JsonConvert.DeserializeObject<UserDto>(localSettings.Values["User"] as string);
            this.SessionInfo = JsonConvert.DeserializeObject<SessionInfo>(localSettings.Values["Session"] as string);
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.MediaLibrary = (BaseItemDto)e.Parameter;
            getSeriesInfo();
        }
        public async void getSeriesInfo()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Users/" + User.Id + "/Items/" + this.MediaLibrary.Id + "?EnableImageTypes=Primary%2CBackdrop%2CBanner%2CThumb");
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var stringResponse = JsonObject.Parse(await authenticationResponse.Content.ReadAsStringAsync());
                    this.MediaLibrary = JsonConvert.DeserializeObject<BaseItemDto>(stringResponse.ToString());
                    if(MediaLibrary.Overview != null)
                    {
                        PageOverview.Text = MediaLibrary.Overview;
                    }
                    
                    getSeasons();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }
        public async void getSeasons()
        {
            seasonItems.Clear();
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Shows/" + this.MediaLibrary.Id + "/Seasons?UserId=" + this.User.Id +
                    "&Fields=ItemCounts%2CPrimaryImageAspectRatio%2CBasicSyncInfo%2CCanDelete%2CMediaSourceCount%2CChildCount" + (this.MediaLibrary.CollectionType == CollectionType.tvshows ? "&Recursive=true&IncludeItemTypes=Series" : ""));
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var stringResponse = JsonObject.Parse(await authenticationResponse.Content.ReadAsStringAsync());
                    foreach (var item in JsonConvert.DeserializeObject<ObservableCollection<BaseItemDto>>(stringResponse["Items"].ToString()))
                    {
                        this.seasonItems.Add(item);
                    };
                    this.selectedSeason = this.seasonItems[0];
                    getEpisodes();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }
        public async void getEpisodes()
        {
            episodeItems.Clear();
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var episodeResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Shows/" + this.selectedSeason.SeriesId + "/Episodes?UserId=" + this.User.Id + "&SeasonId=" + this.selectedSeason.Id +
                    "&Fields=ItemCounts%2CPrimaryImageAspectRatio%2CBasicSyncInfo%2CCanDelete%2CMediaSourceCount%2COverview"  + (this.MediaLibrary.CollectionType == CollectionType.tvshows ? "&Recursive=true&IncludeItemTypes=Series" : ""));
                try
                {
                    episodeResponse.EnsureSuccessStatusCode();
                    var stringResponse = JsonObject.Parse(await episodeResponse.Content.ReadAsStringAsync());
                    foreach (var item in JsonConvert.DeserializeObject<ObservableCollection<BaseItemDto>>(stringResponse["Items"].ToString()))
                    {
                        this.episodeItems.Add(item);
                    };
                    var tmp = MediaLibrary.getImageUrl(ImageType.Backdrop);
                    Debug.WriteLine(tmp);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }

        private void Seasons_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.selectedSeason = (((sender as ListView).SelectedItem) as BaseItemDto);
            getEpisodes();
        }

        private void Episode_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Frame.Navigate(typeof(MediaPlayerView), (sender as ListView).SelectedItem);
        }

        private void SeasonListItem_GotFocus(object sender, RoutedEventArgs e)
        {
                var item = ((GridViewItem)sender).DataContext;
                var container = (ListViewItem)SeasonListViewItem.ContainerFromItem(item);

                container.IsSelected = true;
        }
    }
}
