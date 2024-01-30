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
using System.Security.Cryptography;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Core;
using Windows.Media.Playback;
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
    public sealed partial class MediaPlayerView : Page
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        BaseItemDto MediaLibrary;
        UserDto User;
        SessionInfo SessionInfo;
        MediaPlayer mediaPlayer;
        PlaybackProgressInfo playbackProgressInfo = new PlaybackProgressInfo();
        
        public MediaPlayerView()
        {
            this.InitializeComponent();
            this.User = JsonConvert.DeserializeObject<UserDto>(localSettings.Values["User"] as string);
            this.SessionInfo = JsonConvert.DeserializeObject<SessionInfo>(localSettings.Values["Session"] as string);
            
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.MediaLibrary = (BaseItemDto)e.Parameter;
            mediaPlayer = new MediaPlayer();
            MainMediaPlayer.SetMediaPlayer(mediaPlayer);
            mediaPlayer.Source = MediaSource.CreateFromUri(new Uri((localSettings.Values["Address"] as string) + "/Videos/" + this.MediaLibrary.Id + "/stream?static=false&startTimeTicks=50&subtitleMethod=1"));
            mediaPlayer.Play();
            mediaPlayer.CommandManager.PauseBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            
            mediaPlayer.CommandManager.RewindBehavior.EnablingRule = MediaCommandEnablingRule.Always;
            MainMediaPlayer.MediaPlayer.PlaybackSession.Position = MainMediaPlayer.MediaPlayer.PlaybackSession.Position.Add(new TimeSpan(0,0,20));
            Debug.WriteLine(MainMediaPlayer.MediaPlayer.PlaybackSession.CanSeek.ToString());
            //mediaPlayer.CommandManager.PositionReceived += (MediaPlaybackCommandManager sender, MediaPlaybackCommandManagerPositionReceivedEventArgs args) => { MainMediaPlayer.MediaPlayer.CanSeek };
            mediaPlayer.CommandManager.PauseReceived += (MediaPlaybackCommandManager cm, MediaPlaybackCommandManagerPauseReceivedEventArgs pauseEvent) => { MainMediaPlayer.MediaPlayer.Pause(); };
            mediaPlayer.CommandManager.PlayReceived += (MediaPlaybackCommandManager cm, MediaPlaybackCommandManagerPlayReceivedEventArgs pauseEvent) => { MainMediaPlayer.MediaPlayer.Play(); };
        }


        public async void sendPlayBackProgressInfo()
        {
            playbackProgressInfo.ItemId = MediaLibrary.Id;
            playbackProgressInfo.MediaSourceId = MediaLibrary.Id.ToString();
            using (var client = new HttpClient())
            {

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\", Token=\"" + localSettings.Values["AccessToken"] + "\"");
                var authenticationResponse = await client.PostAsJsonAsync((localSettings.Values["Address"] as string) + "/Sessions/Playing/Progress",this.playbackProgressInfo);
                try
                {
                    authenticationResponse.EnsureSuccessStatusCode();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    await Task.Delay(1000);
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
        private async void Page_Drop(object sender, DragEventArgs e)
        {
            var def = e.GetDeferral();
            var file = (StorageFile)(await e.DataView.GetStorageItemsAsync()).FirstOrDefault();
            MainMediaPlayer.Source = MediaSource.CreateFromStorageFile(file);

            //await AppCore.Instance.Play(new StorageFile[] { file });
            //EmbeddedPlayer.SetMediaPlayer(AppCore.Instance.MediaPlayer);

            MainMediaPlayer.MediaPlayer.Play();
            def.Complete();
        }
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if(e.Key == Windows.System.VirtualKey.GamepadB)
            {
                Frame rootFrame = Window.Current.Content as Frame;
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                }
            } else
            {
                base.OnKeyDown(e);
            }
            
        }

    }
    }

