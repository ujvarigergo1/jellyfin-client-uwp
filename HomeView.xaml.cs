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

        private void WebSocket_MessageReceived(Windows.Networking.Sockets.MessageWebSocket sender, Windows.Networking.Sockets.MessageWebSocketMessageReceivedEventArgs args)
        {
            try
            {
                using (DataReader dataReader = args.GetDataReader())
                {
                    dataReader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    string message = dataReader.ReadString(dataReader.UnconsumedBufferLength);
                    Debug.WriteLine("Message received from MessageWebSocket: " + message);
                }
            }
            catch (Exception ex)
            {
                Windows.Web.WebErrorStatus webErrorStatus = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.GetBaseException().HResult);
                // Add additional code here to handle exceptions.
            }
        }

        private void WebSocket_Closed(Windows.Networking.Sockets.IWebSocket sender, Windows.Networking.Sockets.WebSocketClosedEventArgs args)
        {
            Debug.WriteLine("WebSocket_Closed; Code: " + args.Code + ", Reason: \"" + args.Reason + "\"");
            // Add additional code here to handle the WebSocket being closed.
        }
        public Frame AppFrame { get { return this.Frame; } }

        public MessageWebSocket messageWebSocket { get; private set; }

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
                    messageWebSocket = new Windows.Networking.Sockets.MessageWebSocket();
                    string address = (localSettings.Values["Address"] as string);
                    string websocketUrl = address.Contains("https") ? address.Replace("https", "wss") : address.Replace("http", "ws");
                    
                    // In this example, we send/receive a string, so we need to set the MessageType to Utf8.
                    this.messageWebSocket.Control.MessageType = Windows.Networking.Sockets.SocketMessageType.Utf8;

                    this.messageWebSocket.MessageReceived += WebSocket_MessageReceived;
                    this.messageWebSocket.Closed += WebSocket_Closed;
                    await messageWebSocket.ConnectAsync(new Uri(websocketUrl + "/socket?api_key=" + localSettings.Values["AccessToken"] + "&deviceId=" + localSettings.Values["AuthHeader"]));
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MediaLibraryListView.ContainerFromIndex(0);
        }
    }
    }
