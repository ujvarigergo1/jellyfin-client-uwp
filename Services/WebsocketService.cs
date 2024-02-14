using Jellyfin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Networking.Sockets;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System;

namespace Jellyfin.Services
{
    internal class WebsocketService
    {
        private static WebsocketService instance = null;
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        public MessageWebSocket messageWebSocket { get; private set; }
        public static WebsocketService Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WebsocketService();
                }
                return instance;
            }
        }
        WebsocketService()
        {

            messageWebSocket = new Windows.Networking.Sockets.MessageWebSocket();

            // In this example, we send/receive a string, so we need to set the MessageType to Utf8.
            this.messageWebSocket.Control.MessageType = Windows.Networking.Sockets.SocketMessageType.Utf8;
            this.messageWebSocket.Closed += WebSocket_Closed;
        }
        public async Task ConnectWebsocket()
        {
            string address = (localSettings.Values["Address"] as string);
            string websocketUrl = address.Contains("https") ? address.Replace("https", "wss") : address.Replace("http", "ws");
            await messageWebSocket.ConnectAsync(new Uri(websocketUrl + "/socket?api_key=" + localSettings.Values["AccessToken"] + "&deviceId=" + localSettings.Values["AuthHeader"]));
        }

        private void WebSocket_Closed(Windows.Networking.Sockets.IWebSocket sender, Windows.Networking.Sockets.WebSocketClosedEventArgs args)
        {
            Debug.WriteLine("WebSocket_Closed; Code: " + args.Code + ", Reason: \"" + args.Reason + "\"");
            // Add additional code here to handle the WebSocket being closed.
            if(!args.Reason.Equals("Manually closed"))
            {
                ConnectWebsocket();
            }
        }
        public void Close(bool recreate = false)
        {
            messageWebSocket.Close(1000, "Manually closed");
            messageWebSocket.Dispose();
            if(recreate)
            {
                messageWebSocket = new Windows.Networking.Sockets.MessageWebSocket();

                // In this example, we send/receive a string, so we need to set the MessageType to Utf8.
                this.messageWebSocket.Control.MessageType = Windows.Networking.Sockets.SocketMessageType.Utf8;
                this.messageWebSocket.Closed += WebSocket_Closed;
            }
        }

        
    }
        
}
