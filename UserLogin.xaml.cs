using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Media.Protection.PlayReady;
using Windows.Security.Cryptography.Core;
using Windows.Security.Cryptography;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.ComponentModel;
using System.Threading.Tasks;
using Windows.Devices.Usb;
using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Jellyfin.Models;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Jellyfin
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UserLogin : Page
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        string Username;
        string Password;
        bool QuicConecctEnabled = false;
        public QuickConnectResult quickConnectResult;
        public UserLogin()
        {
            this.InitializeComponent();
            localSettings.Values["AuthHeader"] = this.GetHashedPassword((localSettings.Values["Address"] as string) + (localSettings.Values["Name"] as string) + this.Username);
            this.getQuickConnectCode();
        }
        public string GetHashedPassword(string text)
        {

            IBuffer input = CryptographicBuffer.ConvertStringToBinary(text, BinaryStringEncoding.Utf8);
            var hashAlgorithm = HashAlgorithmProvider.OpenAlgorithm(HashAlgorithmNames.Sha256);
            var hash = hashAlgorithm.HashData(input);

            return CryptographicBuffer.EncodeToBase64String(hash);
        }
        public async void getQuickConnectCode()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\"");
                
                var response = await client.GetAsync((localSettings.Values["Address"] as string) + "/QuickConnect/Initiate");
                try
                {
                    response.EnsureSuccessStatusCode();
                    this.QuicConecctEnabled = true;
                    var stringResponse =  await response.Content.ReadAsStringAsync();
                    this.quickConnectResult = JsonConvert.DeserializeObject<QuickConnectResult>(stringResponse);
                    this.ConnectionCodeText.Text = quickConnectResult.Code;
                    this.tryQuickConnectLogin();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this.QuicConecctEnabled = false;
                }
            }
        }
        async void tryQuickConnectLogin()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser DeviceId=\"" + this.GetHashedPassword((localSettings.Values["Address"] as string)+ (localSettings.Values["Name"] as string) + this.Username) + "\", Client=\"Android TV\"");
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\"");
                while (!this.quickConnectResult.Authenticated)
                {
                    var authenticationResponse = await client.GetAsync((localSettings.Values["Address"] as string) + "/QuickConnect/Connect?secret="+ this.quickConnectResult.Secret);
                    try
                    {
                        authenticationResponse.EnsureSuccessStatusCode();
                        var stringResponse = await authenticationResponse.Content.ReadAsStringAsync();
                        this.quickConnectResult = JsonConvert.DeserializeObject<QuickConnectResult>(stringResponse);
                        if (!this.quickConnectResult.Authenticated) {

                            await Task.Delay(5000);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.WriteLine(ex.Message);
                        await Task.Delay(1000);
                    }
                }
                var response = await client.PostAsJsonAsync((localSettings.Values["Address"] as string) + "/Users/AuthenticateWithQuickConnect", new
                {
                    Secret = this.quickConnectResult.Secret
                });
                try
                {
                    response.EnsureSuccessStatusCode();
                    this.QuicConecctEnabled = true;
                    var stringResponse = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<AuthenticationResult>(stringResponse,new JsonSerializerSettings() { });
                    localSettings.Values["User"] = JsonConvert.SerializeObject(result.User);
                    localSettings.Values["Session"] = JsonConvert.SerializeObject(result.SessionInfo);
                    localSettings.Values["AccessToken"] = result.AccessToken;
                    localSettings.Values["ServerId"] = result.ServerId;
                    Frame.Navigate(typeof(HomeView));
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    this.QuicConecctEnabled = false;
                }
            }
        }
        public async void login()
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser DeviceId=\"" + this.GetHashedPassword((localSettings.Values["Address"] as string)+ (localSettings.Values["Name"] as string) + this.Username) + "\", Client=\"Android TV\"");
                client.DefaultRequestHeaders.Add("X-Emby-Authorization", "MediaBrowser Client=\"Jellyfin Xbox\", Device=\"Xbox\", DeviceId=\"" + localSettings.Values["AuthHeader"] + "\", Version=\"10.8.13\"");
                var response = await client.PostAsJsonAsync((localSettings.Values["Address"] as string)+ "/Users/AuthenticateByName",new {
                    Username = this.Username,
                    Pw = this.Password
                });
                try
                {
                    response.EnsureSuccessStatusCode();
                    var stringResponse = await response.Content.ReadAsStringAsync();

                    var result = JsonConvert.DeserializeObject<AuthenticationResult>(stringResponse);
                    localSettings.Values["User"] = JsonConvert.SerializeObject(result.User);
                    localSettings.Values["Session"] = JsonConvert.SerializeObject(result.SessionInfo);
                    localSettings.Values["AccessToken"] = result.AccessToken;
                    localSettings.Values["ServerId"] = result.ServerId;
                    Frame.Navigate(typeof(HomeView));
                } catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
            }
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            this.login();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame.CanGoBack)
            {
                rootFrame.GoBack();
            }
        }
    }
   
}
