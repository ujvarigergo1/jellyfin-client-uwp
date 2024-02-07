using Jellyfin.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Jellyfin
{
    public sealed partial class JellyfinAppBar : UserControl
    {
        ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
        UserDto User;
        SessionInfo SessionInfo;
        public bool ShowBackArrow
        {
            get { return (bool)GetValue(ShowBackArrowProperty); }
            set { SetValue(ShowBackArrowProperty, value); if (value)
                {
                    BackButton.Visibility = Visibility.Visible;
                }
                else { BackButton.Visibility = Visibility.Collapsed; }
            }
        }
        public static readonly DependencyProperty ShowBackArrowProperty =
        DependencyProperty.Register("ShowBackArrow", typeof(bool), typeof(JellyfinAppBar), new PropertyMetadata(false));
        ObservableCollection<BaseItemDto> ViewList = new ObservableCollection<BaseItemDto>();
        public JellyfinAppBar()
        {
            this.InitializeComponent();
            this.User = JsonConvert.DeserializeObject<UserDto>(localSettings.Values["User"] as string);
            this.SessionInfo = JsonConvert.DeserializeObject<SessionInfo>(localSettings.Values["Session"] as string);
            UsernameTextBlock.Text = User.Name;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame navigationFrame = Window.Current.Content as Frame;
            if (navigationFrame.CanGoBack)
            {
                navigationFrame.GoBack();
            }
        }
    }
}
