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
using Windows.Media.Protection.PlayReady;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Jellyfin.Views;
using Windows.Storage.Streams;
using System.Net.WebSockets;
using Windows.Networking.Sockets;
using Windows.Security.ExchangeActiveSyncProvisioning;
using Windows.System.Profile;
using System.Collections;
using Windows.System;
using Windows.Networking.Connectivity;
using Jellyfin.Services;

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
        ObservableCollection<BaseItemDto> ResumeMediaList = new ObservableCollection<BaseItemDto>();
        WebsocketService websocketService = WebsocketService.Instance;
        public HomeView()
        {

            this.InitializeComponent();

            var hostnames = NetworkInformation.GetHostNames();
            if (hostnames.Count() > 0 && !hostnames[0].ToString().Contains("192"))
            {
                localSettings.Values["DeviceName"] = hostnames[0].ToString();
            }
            else
            {
                localSettings.Values["DeviceName"] = "XBOX";
            }
            this.User = JsonConvert.DeserializeObject<UserDto>(localSettings.Values["User"] as string);
            this.SessionInfo = JsonConvert.DeserializeObject<SessionInfo>(localSettings.Values["Session"] as string);
            SystemNavigationManager.GetForCurrentView().BackRequested +=
                SystemNavigationManager_BackRequested;
            sendDeviceCapabilities();
            getSystemInfo();
            getUserViews();
            getResumeMedia();

            Windows.UI.ViewManagement.ApplicationViewScaling.TrySetDisableLayoutScaling(false);
        }

        private void SystemNavigationManager_BackRequested(
            object sender,
            BackRequestedEventArgs e)
        {
            if (!e.Handled)
            {
                e.Handled = this.BackRequested();
            }
        }

        

        public Frame AppFrame { get { return this.Frame; } }


        private bool BackRequested()
        {
            // Get a hold of the current frame so that we can inspect the app back stack
            if (this.AppFrame == null)
                return false;

            // Check to see if this is the top-most page on the app back stack
            if (this.AppFrame.CanGoBack)
            {
                // If not, set the event to handled and go back to the previous page in the
                // app.
                this.AppFrame.GoBack();
                return true;
            }
            return false;
        }
        public async void getUserViews()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"" + localSettings.Values["DeviceName"] + "\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Users/" + this.User.Id + "/Views");
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var stringResponse = JsonObject.Parse(await authenticationResponse.Content.ReadAsStringAsync());
                    foreach (var item in JsonConvert.DeserializeObject<ObservableCollection<BaseItemDto>>(stringResponse["Items"].ToString()))
                    {
                        this.ViewList.Add(item);
                    };
                    MediaLibraryListView.ContainerContentChanging += focusListItem;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }
        public async void getResumeMedia()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"" + localSettings.Values["DeviceName"] + "\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Users/" + this.User.Id + "/Items/Resume?Recursive=true&Fields=PrimaryImageAspectRatio%2CBasicSyncInfo&ImageTypeLimit=1&EnableImageTypes=Primary%2CBackdrop%2CThumb&EnableTotalRecordCount=false&MediaTypes=Video");
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var stringResponse = JsonObject.Parse(await authenticationResponse.Content.ReadAsStringAsync());
                    foreach (var item in JsonConvert.DeserializeObject<ObservableCollection<BaseItemDto>>(stringResponse["Items"].ToString()))
                    {
                        this.ResumeMediaList.Add(item);
                        ContinueWatchingTitle.Visibility = Visibility.Visible;
                    };
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }
        public async Task sendDeviceCapabilities()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser DeviceId=\"" + this.GetHashedPassword((localSettings.Values["Address"] as string)+ (localSettings.Values["Name"] as string) + this.Username) + "\", Client=\"Android TV\"");
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var response = await client.PostAsync((localSettings.Values["Address"] as string) + "/Sessions/Capabilities/Full", new StringContent("{\"PlayableMediaTypes\":[\"Audio\",\"Video\"],\"SupportedCommands\":[\"MoveUp\",\"MoveDown\",\"MoveLeft\",\"MoveRight\",\"PageUp\",\"PageDown\",\"PreviousLetter\",\"NextLetter\",\"ToggleOsd\",\"ToggleContextMenu\",\"Select\",\"Back\",\"SendKey\",\"SendString\",\"GoHome\",\"GoToSettings\",\"VolumeUp\",\"VolumeDown\",\"Mute\",\"Unmute\",\"ToggleMute\",\"SetVolume\",\"SetAudioStreamIndex\",\"SetSubtitleStreamIndex\",\"DisplayContent\",\"GoToSearch\",\"DisplayMessage\",\"SetRepeatMode\",\"SetShuffleQueue\",\"ChannelUp\",\"ChannelDown\",\"PlayMediaSource\",\"PlayTrailers\"],\"SupportsPersistentIdentifier\":false,\"SupportsMediaControl\":true}", System.Text.Encoding.UTF8, "application/json"));
                try
                {
                    response.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }
        public async void getSystemInfo()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/System/Info");
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var stringResponse = await authenticationResponse.Content.ReadAsStringAsync();
                    var parsedResponse = JsonObject.Parse(await authenticationResponse.Content.ReadAsStringAsync());
                    websocketService.messageWebSocket.MessageReceived += WebSocket_MessageReceived;
                    await websocketService.ConnectWebsocket();
                    localSettings.Values["System"] = stringResponse;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }

        

        public void focusListItem(ListViewBase sender, ContainerContentChangingEventArgs args)
        {
            if (args.ItemContainer is GridViewItem)
            {
                args.ItemContainer.Focus(FocusState.Programmatic);
                this.MediaLibraryListView.ContainerContentChanging -= focusListItem;
            }
        }


        private void MediaLibraries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Frame.Navigate(typeof(MediaItemsBrowser), (sender as GridView).SelectedValue);

        }
        private void ResumeMedia_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ((sender as GridView).SelectedValue as BaseItemDto).PlayFromResumeState = true;
            Frame.Navigate(typeof(MediaPlayerView), (sender as GridView).SelectedValue);

        }
        private async void WebSocket_MessageReceived(Windows.Networking.Sockets.MessageWebSocket sender, Windows.Networking.Sockets.MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader dataReader = args.GetDataReader())
                {
                    dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    Debug.WriteLine("Message received from MessageWebSocket: " + message);
                    JsonObject.TryParse(message, out var data);
                    var tmp = data["MessageType"].ToString().Replace("\"", "");
                    if (tmp.Equals("Play"))
                    {
                        var tmp2 = data["Data"].ToString();
                        JsonObject.TryParse(data["Data"].ToString(), out var payload);
                        var tmp3 = JsonArray.Parse(payload["ItemIds"].ToString());
                        var mediaItem = await getMediaInfo(tmp3.First().ToString().Replace("\"", ""));
                        if (payload.ContainsKey("StartPositionTicks"))
                        {
                            mediaItem.PlayFromResumeState = true;
                            mediaItem.UserData.PlaybackPositionTicks = Int64.Parse(payload["StartPositionTicks"].ToString());
                        }
                        await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                            Frame.Navigate(typeof(MediaPlayerView), mediaItem);
                        });

                    }
                }
            }
            catch (Exception ex)
            {
                Windows.Web.WebErrorStatus webErrorStatus = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(ex.Message);
            }
        }
        public async Task<BaseItemDto> getMediaInfo(string mediaId)
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Users/" + User.Id + "/Items/" + mediaId + "?EnableImageTypes=Primary%2CBackdrop%2CBanner%2CThumb");
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var stringResponse = JsonObject.Parse(await authenticationResponse.Content.ReadAsStringAsync());
                    return JsonConvert.DeserializeObject<BaseItemDto>(stringResponse.ToString());

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    return null;
                }
            }
        }
    }
}
