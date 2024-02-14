using Jellyfin.Models;
using Jellyfin.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Core;
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
    public sealed partial class MediaPlayerView : Page
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        BaseItemDto MediaLibrary;
        UserDto User;
        SessionInfo SessionInfo;
        MediaPlayer mediaPlayer;
        Task timer;
        CancellationTokenSource cancellationToken;
        PlaybackProgressInfo playbackProgressInfo = new PlaybackProgressInfo();
        string playbackSessionId;
        long token;
        bool reportingActive = false;
        WebsocketService websocketService = WebsocketService.Instance;
        public MediaPlayerView()
        {
            this.InitializeComponent();
            this.User = JsonConvert.DeserializeObject<UserDto>(localSettings.Values["User"] as string);
            this.SessionInfo = JsonConvert.DeserializeObject<SessionInfo>(localSettings.Values["Session"] as string);
            
        }
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.MediaLibrary = (BaseItemDto)e.Parameter;
            await getPlaybackInfo();
            mediaPlayer = new MediaPlayer();
            MainMediaPlayer.SetMediaPlayer(mediaPlayer);
            var videoSource = new MediaPlaybackItem(MediaSource.CreateFromUri(new Uri((localSettings.Values["Address"] as string) + "/Videos/" + this.MediaLibrary.Id + "/stream?container=mkv&static=true&subtitleMethod=1")));
            mediaPlayer.Source = videoSource;

            mediaPlayer.CommandManager.IsEnabled = true;
            MediaItemDisplayProperties props = videoSource.GetDisplayProperties();
            props.Type = Windows.Media.MediaPlaybackType.Video;
            props.VideoProperties.Title = "Video title";
            props.VideoProperties.Subtitle = "Video subtitle";
            props.VideoProperties.Genres.Add("Documentary");
            videoSource.ApplyDisplayProperties(props);
            mediaPlayer.CommandManager.PauseBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            
            mediaPlayer.CommandManager.RewindBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            Debug.WriteLine(MainMediaPlayer.MediaPlayer.PlaybackSession.CanSeek.ToString());
            //mediaPlayer.CommandManager.PositionReceived += (MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPositionReceivedEventArgs args) => { MainMediaPlayer.MediaPlayer.CanSeek };
            mediaPlayer.CommandManager.PauseReceived += (MediaPlaybackCommandManager cm, MediaPlaybackCommandManagerPauseReceivedEventArgs pauseEvent) => { MainMediaPlayer.MediaPlayer.Pause();playbackProgressInfo.IsPaused = true; };
            mediaPlayer.CommandManager.PlayReceived += (MediaPlaybackCommandManager cm, MediaPlaybackCommandManagerPlayReceivedEventArgs pauseEvent) => { MainMediaPlayer.MediaPlayer.Play(); playbackProgressInfo.IsPaused = false; };
            mediaPlayer.Play();
            Windows.UI.ViewManagement.ApplicationViewScaling.TrySetDisableLayoutScaling(true);
            startHideControlsTimer();
            Frame navigationFrame = Window.Current.Content as Frame;
            token = navigationFrame.RegisterPropertyChangedCallback(
               Frame.ContentProperty,
               ContentChanged);
            await sendPlayBackStart();
            if (this.MediaLibrary.PlayFromResumeState)
            {
                MainMediaPlayer.MediaPlayer.PlaybackSession.Position = TimeSpan.FromMilliseconds(this.MediaLibrary.UserData.PlaybackPositionTicks/10000);
            }
            repeatPlaybackReporting();
            websocketService.Close(true);
            websocketService.messageWebSocket.MessageReceived += this.WebSocket_PlayingMessageReceived;
            websocketService.ConnectWebsocket();
        }
        private async void ContentChanged(DependencyObject sender, DependencyProperty dp)
        {
            await sendPlayBackStopped();
            Frame navigationFrame = Window.Current.Content as Frame;
            //do something when Content changes
            navigationFrame.UnregisterPropertyChangedCallback(Frame.ContentProperty, token);
            try
            {
                if (timer != null && timer.Status != TaskStatus.RanToCompletion)
                {
                    cancellationToken.Cancel();
                }
                reportingActive = false;
                MainMediaPlayer.MediaPlayer.Pause();
                MainMediaPlayer.MediaPlayer.Source = null;
                Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
            } catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
        private async void repeatPlaybackReporting()
        {
            reportingActive = true;
            while (reportingActive) {
                sendPlayBackProgressInfo();
                await Task.Delay(5000);
            }
        }
        public async Task getPlaybackInfo()
        {
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                string parentId;
                //if (this.MediaLibrary.DisplayPreferencesId != null) { parentId = this.MediaLibrary.DisplayPreferencesId; } else { parentId = this.MediaLibrary.Id.ToString(); }
                if(this.MediaLibrary.Id != null) { parentId = this.MediaLibrary.Id.ToString(); } else { parentId = this.MediaLibrary.DisplayPreferencesId; }
                var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/Items/" + parentId + "/PlaybackInfo?UserId=" + this.User.Id);
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                    var parsedResponse= JsonConvert.DeserializeObject<PlaybackInfoResponse>(await authenticationResponse.Content.ReadAsStringAsync());
                    this.playbackSessionId = parsedResponse.PlaySessionId;
                    this.playbackProgressInfo.PlaySessionId = parsedResponse.PlaySessionId;
                    playbackProgressInfo.PlaybackRate = 1;
                    playbackProgressInfo.CanSeek = true;
                    playbackProgressInfo.PlayMethod = "Transcode";
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
                }
            }
        }

        public async Task sendPlayBackStart()
        {
            playbackProgressInfo.ItemId = MediaLibrary.Id;
            playbackProgressInfo.MediaSourceId = MediaLibrary.Id.ToString();
            playbackProgressInfo.PositionTicks = 0;
            playbackProgressInfo.PlaybackStartTimeTicks = ((long)MainMediaPlayer.MediaPlayer.PlaybackSession.NaturalDuration.TotalMilliseconds);
            playbackProgressInfo.EventName = null;
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");

                var authenticationResponse = await client.PostAsync((localSettings.Values["Address"] as string) + "/Sessions/Playing", new StringContent(JsonConvert.SerializeObject(this.playbackProgressInfo), System.Text.Encoding.UTF8, "application/json"));
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return;
        }
        public async void sendPlayBackProgressInfo()
        {
            playbackProgressInfo.ItemId = MediaLibrary.Id;
            playbackProgressInfo.MediaSourceId = MediaLibrary.Id.ToString();
            playbackProgressInfo.PositionTicks = ((long)this.MainMediaPlayer.MediaPlayer.PlaybackSession.Position.TotalMilliseconds)*10000;
            playbackProgressInfo.PlaybackStartTimeTicks = ((long)MainMediaPlayer.MediaPlayer.PlaybackSession.NaturalDuration.TotalMilliseconds);
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");

                var authenticationResponse = await client.PostAsync((localSettings.Values["Address"] as string) + "/Sessions/Playing/Progress", new StringContent(JsonConvert.SerializeObject(this.playbackProgressInfo), System.Text.Encoding.UTF8, "application/json"));
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return;
        }
        public async Task sendPlayBackStopped()
        {
            playbackProgressInfo.IsPaused = true;
            playbackProgressInfo.ItemId = MediaLibrary.Id;
            playbackProgressInfo.MediaSourceId = MediaLibrary.Id.ToString();
            playbackProgressInfo.PositionTicks = ((long)this.MainMediaPlayer.MediaPlayer.PlaybackSession.Position.TotalMilliseconds) * 10000;
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");

                var authenticationResponse = await client.PostAsync((localSettings.Values["Address"] as string) + "/Sessions/Playing/Stopped", new StringContent(JsonConvert.SerializeObject(this.playbackProgressInfo), System.Text.Encoding.UTF8, "application/json"));
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
            return;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            playbackProgressInfo.IsPaused = false;
            MainMediaPlayer.MediaPlayer.PlaybackSession.Position = TimeSpan.FromMinutes(2);
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            playbackProgressInfo.IsPaused = true;
            MainMediaPlayer.MediaPlayer.Pause();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame navigationFrame = Window.Current.Content as Frame;
            if (navigationFrame.CanGoBack)
            {
                navigationFrame.GoBack();
            }
        }
        private async void startHideControlsTimer()
        {
            try
            {
                if (timer != null && timer.Status != TaskStatus.RanToCompletion)
                {
                    cancellationToken.Cancel();
                }
                BackButton.Visibility = Visibility.Visible;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 0);
                });
                cancellationToken = new CancellationTokenSource();
                timer = Task.Run(async () => { await Task.Delay(4000); this.cancellationToken.Token.ThrowIfCancellationRequested(); }, cancellationToken.Token);
                await timer;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            if (timer != null && timer.Status == TaskStatus.RanToCompletion)
            {
                BackButton.Visibility = Visibility.Collapsed;
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    Windows.UI.Xaml.Window.Current.CoreWindow.PointerCursor = null;
                });
            }
        }
        private async void RelativePanel_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            startHideControlsTimer();
        }

        private void MainMediaPlayer_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            startHideControlsTimer();
        }
        private void WebSocket_PlayingMessageReceived(Windows.Networking.Sockets.MessageWebSocket sender, Windows.Networking.Sockets.MessageWebSocketMessageReceivedEventArgs args)
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
                    if (tmp.Equals("Playstate"))
                    {
                        var tmp2 = data["Data"].ToString();
                        JsonObject.TryParse(data["Data"].ToString(), out var payload);
                        var command = payload["Command"].ToString();
                        if (command.ToString().Replace("\"","").Equals("PlayPause"))
                        {
                            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                if (this.MainMediaPlayer.MediaPlayer.PlaybackSession.PlaybackState != MediaPlaybackState.Paused)
                                {
                                    this.MainMediaPlayer.MediaPlayer.Pause();
                                }
                                else
                                {
                                    this.MainMediaPlayer.MediaPlayer.Play();
                                }
                            });
                        } else if (command.ToString().Replace("\"", "").Equals("Seek"))
                        {
                            Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                            {
                                if (payload.ContainsKey("SeekPositionTicks"))
                                {
                                    MainMediaPlayer.MediaPlayer.PlaybackSession.Position = TimeSpan.FromMilliseconds(Int64.Parse(payload["SeekPositionTicks"].ToString()) / 10000);
                                }
                            });
                            
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Windows.Web.WebErrorStatus webErrorStatus = Windows.Networking.Sockets.WebSocketError.GetStatus(ex.GetBaseException().HResult);
                Debug.WriteLine(ex.Message);
            }
        }
    }
    }

