using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Windows.Networking.Sockets;
using Windows.Networking;
using Windows.Storage.Streams;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls;
using System.Diagnostics;
using Windows.Data.Json;
using Windows.Foundation;

namespace Jellyfin.Services
{
    public class LocalDiscovery
    {
        public LocalDiscovery(TextBox address)
        {
            this.Address = address;
        }
        TextBox Address;
            DatagramSocket listenersocket = null;
            const string port = "7359";
            public class DiscoveryResponse
        {
            string Address;
            string Id;
            string name;
            string EndpontAddress;
        }

            public async Task<DatagramSocket> listen(Func<DatagramSocket, DatagramSocketMessageReceivedEventArgs, TextBox, JsonObject> messagereceived)
            {
                listenersocket = new DatagramSocket();
            //listenersocket.messagereceived += (x, y) =>
            //{
            //    var a = "2";
            //};
            listenersocket.MessageReceived += (DatagramSocket socket, DatagramSocketMessageReceivedEventArgs args) =>messagereceived(socket, args, Address);
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
        
    }
}
