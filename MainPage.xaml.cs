﻿using Jellyfin.Models;
using Jellyfin.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Jellyfin.Views
{
    public class DiscoveryResponse
    {
        public string Address;
        public string Id;
        public string Name;
        public string EndpointAddress;
    }
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ServerSelectionView : Page
    {
        
        public ObservableCollection<DiscoveryResponse> discoveryResult { get; }
        = new ObservableCollection<DiscoveryResponse>();
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        DatagramSocket listenersocket = null;
        const string port = "7359";
        public Frame AppFrame { get { return this.Frame; } }
        public ServerSelectionView()
        {
            
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().BackRequested +=
                SystemNavigationManager_BackRequested;
            this.StartServerInitialization();

            var hostnames = NetworkInformation.GetHostNames();
            if (hostnames.Count() > 0 && !hostnames[0].ToString().Contains("192"))
            {
                localSettings.Values["DeviceName"] = hostnames[0].ToString();
            }
            else
            {
                localSettings.Values["DeviceName"] = "XBOX";
            }
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

        private async void StartServerInitialization()
        {
            DatagramSocket socket = await this.listen();
            this.send();
        }
        public JsonObject messagereceived(DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args)
        {
            DataReader reader = args.GetDataReader();
            uint len = reader.UnconsumedBufferLength;
            string msg = reader.ReadString(len);

            string remotehost = args.RemoteAddress.DisplayName;
            reader.Dispose();

            Debug.Write(msg);
            JsonObject response = JsonObject.Parse(msg);
           Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () => {
                this.discoveryResult.Add(new DiscoveryResponse() { Address = response["Address"].ToString().Replace("\"",""), Id = response["Id"].ToString().Replace("\"", ""), Name = response["Name"].ToString().Replace("\"", ""), EndpointAddress = response["EndpointAddress"].ToString().Replace("\"", "") });
                this.NoServersFoundLabel.Visibility = Visibility.Collapsed;

            });
            
            return response;
        }
        public async Task<DatagramSocket> listen()
        {
            listenersocket = new DatagramSocket();
            //listenersocket.messagereceived += (x, y) =>
            //{
            //    var a = "2";
            //};
            listenersocket.MessageReceived += (DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args) => this.messagereceived(socket, args);
            listenersocket.BindServiceNameAsync(port);
            return listenersocket;
        }

        public async void send()
        {
            IOutputStream outputstream;
            string localipstring = "192.168.31.12";
            IPAddress localip = IPAddress.Parse(localipstring);
            string subnetmaskstring = "255.255.255.0";
            IPAddress subnetip = IPAddress.Parse(subnetmaskstring);
            HostName remotehostname = new HostName(localip.ToString());
            outputstream = await listenersocket.GetOutputStreamAsync(remotehostname, port);

            using (DataWriter writer = new DataWriter(outputstream))
            {
                writer.WriteString("who is JellyfinServer?");
                await writer.StoreAsync();
            }


        }

        private void DiscoveredList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            localSettings.Values["Address"] = ((DiscoveryResponse)(sender as ListView).SelectedValue).Address;
            Frame.Navigate(typeof(UserLogin));
        }
        private async void CheckServerAddress()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\"");
                
                
                try
                {
                    var response = await client.GetAsync(ServeraddressInput.Text + "/System/Info/Public");
                    response.EnsureSuccessStatusCode();
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    localSettings.Values["Address"] = ServeraddressInput.Text;
                    Frame.Navigate(typeof(UserLogin));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    ErrorMessage.Visibility = Visibility.Visible;
                }
            }
        }
        private async void AddServerButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ServeraddressInput.Text.StartsWith("http://") && !ServeraddressInput.Text.StartsWith("https://"))
            {
                ServeraddressInput.Text = "http://" + ServeraddressInput.Text;
            }
            CheckServerAddress();
        }

        private void ServeraddressInput_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (!ServeraddressInput.Text.StartsWith("http://") && !ServeraddressInput.Text.StartsWith("https://"))
                {
                    ServeraddressInput.Text = "http://" + ServeraddressInput.Text;
                }
                CheckServerAddress();
            }
        }
    }
}
