using Microsoft.Data.Sqlite;
using Rssdp;
using System;
using System.Collections.Generic;
using System.Net.Mqtt;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MRP_RTSP_Client.UWP
{
    public sealed partial class MainPage : Page
    {
        public Frame AppFrame { get { return Content; } }
        public static Frame MainPageFrame;
        public static DependencyObject DepObject;
        private static List<string> networkDevices = null;
        private static List<string> rtspList = null;
        private static int activeOption = 0;
        private static MqttConfiguration configuration;
        private static IMqttClient client;

        public MainPage()
        {
            this.InitializeComponent();
            MainPageFrame = MainFrame;
            InitializeDatabase();
            DepObject = this;
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            switch (activeOption)
            {
                case 1:
                    try
                    { 
                        if (client != null && client.IsConnected)
                        {
                            await client.DisconnectAsync();
                        }

                        configuration = new MqttConfiguration { Port = Convert.ToInt32(((string)e.Parameter).Split(':')[1]) };
                        client = await MqttClient.CreateAsync(((string)e.Parameter).Split(':')[0], configuration);
                        var sessionState = await client.ConnectAsync(new MqttClientCredentials(clientId: "mrpclient"));
                        await client.PublishAsync(new MqttApplicationMessage("event", Encoding.UTF8.GetBytes("camera_server_off")), MqttQualityOfService.AtLeastOnce);

                        AppFrame.Navigate(typeof(ListViewPage), networkDevices, new EntranceNavigationTransitionInfo());
                    }
                    catch (System.Net.Mqtt.MqttClientException)
                    {

                    }
                    ((RadioButton)this.FindName("Option1")).IsChecked = true;
                break;

                case 2:
                    AppFrame.Navigate(typeof(ListViewPage), rtspList, new EntranceNavigationTransitionInfo());
                    ((RadioButton)this.FindName("Option2")).IsChecked = true;
                break;
            }
            base.OnNavigatedTo(e);
        }

        private void Option1Button_Checked(object sender, RoutedEventArgs e)
        {
            activeOption = 1;
            if(networkDevices == null)
            {
                networkDevices = new List<string>();
                networkDevices.Add("Network Devices");
            }
            AppFrame.Navigate(typeof(ListViewPage), networkDevices, new EntranceNavigationTransitionInfo());
        }

        private void Option2Button_Checked(object sender, RoutedEventArgs e)
        {
            activeOption = 2;
            rtspList = GetData();
            rtspList.Insert(0, "RTSP Servers");
            AppFrame.Navigate(typeof(ListViewPage), rtspList, new EntranceNavigationTransitionInfo());
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationPane.IsPaneOpen = !NavigationPane.IsPaneOpen;

            ResizeOptions();
        }

        private void Shell_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            FocusNavigationDirection direction = FocusNavigationDirection.None;

            switch (e.Key)
            {
                // both Space and Enter will trigger navigation

                case Windows.System.VirtualKey.Space:
                case Windows.System.VirtualKey.Enter:
                    {
                        var control = FocusManager.GetFocusedElement() as Control;
                        var option = control as RadioButton;
                        if (option != null)
                        {
                            var automation = new RadioButtonAutomationPeer(option);
                            automation.Select();
                        }
                    }
                    return;

                // otherwise, find next focusable element in the appropriate direction

                case Windows.System.VirtualKey.Left:
                case Windows.System.VirtualKey.GamepadDPadLeft:
                case Windows.System.VirtualKey.GamepadLeftThumbstickLeft:
                case Windows.System.VirtualKey.NavigationLeft:
                    direction = FocusNavigationDirection.Left;
                    break;
                case Windows.System.VirtualKey.Right:
                case Windows.System.VirtualKey.GamepadDPadRight:
                case Windows.System.VirtualKey.GamepadLeftThumbstickRight:
                case Windows.System.VirtualKey.NavigationRight:
                    direction = FocusNavigationDirection.Right;
                    break;

                case Windows.System.VirtualKey.Up:
                case Windows.System.VirtualKey.GamepadDPadUp:
                case Windows.System.VirtualKey.GamepadLeftThumbstickUp:
                case Windows.System.VirtualKey.NavigationUp:
                    direction = FocusNavigationDirection.Up;
                    break;

                case Windows.System.VirtualKey.Down:
                case Windows.System.VirtualKey.GamepadDPadDown:
                case Windows.System.VirtualKey.GamepadLeftThumbstickDown:
                case Windows.System.VirtualKey.NavigationDown:
                    direction = FocusNavigationDirection.Down;
                    break;
            }

            if (direction != FocusNavigationDirection.None)
            {
                var control = FocusManager.FindNextFocusableElement(direction) as Control;
                if (control != null)
                {
                    control.Focus(FocusState.Programmatic);
                    e.Handled = true;
                }
            }
        }

        private void ResizeOptions()
        {
            // calculate the actual width of the navigation pane

            var width = NavigationPane.CompactPaneLength;
            if (NavigationPane.IsPaneOpen)
                width = NavigationPane.OpenPaneLength;

            // change the width of all control in the navigation pane

            HamburgerButton.Width = width;

            foreach (var option in NavigationMenu.Children)
            {
                var radioButton = (option as RadioButton);
                if (radioButton != null)
                    radioButton.Width = width;
            }
        }

        private async void ScanNetworkAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ((AppBarButton)this.FindName("ScanButton")).IsEnabled = false;
            ((AppBarButton)this.FindName("AddRTSPButton")).IsEnabled = false;
            activeOption = 1;
            AppFrame.Navigate(typeof(LoadingDialogPage), null, new EntranceNavigationTransitionInfo());
            networkDevices = new List<string>();
            networkDevices.Add("Network Devices");
            networkDevices.AddRange(await GetDeviceInfoList());
            AppFrame.Navigate(typeof(ListViewPage), networkDevices, new EntranceNavigationTransitionInfo());
            ((AppBarButton)this.FindName("ScanButton")).IsEnabled = true;
            ((AppBarButton)this.FindName("AddRTSPButton")).IsEnabled = true;
            ((RadioButton)this.FindName("Option1")).IsChecked = true;
        }

        private async void AddRTSPServerAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            activeOption = 2;
            var dialog1 = new ContentDialog1();
            var result = await dialog1.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                AddData(dialog1.Text);
                rtspList = GetData();
                rtspList.Insert(0, "RTSP Servers");
                AppFrame.Navigate(typeof(ListViewPage), rtspList, new EntranceNavigationTransitionInfo());
            }
            ((RadioButton)this.FindName("Option2")).IsChecked = true;

        }

        public void ListView1_ItemClick(object sender, ItemClickEventArgs e)
        {
            string clickedItemText = e.ClickedItem.ToString();
        }

        private async Task<List<string>> GetDeviceInfoList()
        {
            List<string> tempDeviceInfoList = new List<string>();
            string deviceInfo = "";
            using (var deviceLocator = new SsdpDeviceLocator())
            {
                var foundDevices = await deviceLocator.SearchAsync(new TimeSpan(0, 0, 10)); 
                foreach (var foundDevice in foundDevices)
                {
                    deviceInfo = "";
                    var fullDevice = Task.Run(() => foundDevice.GetDeviceInfo()).Result;
                    deviceInfo += fullDevice.FriendlyName + "\n";
                    deviceInfo += fullDevice.PresentationUrl;
                    if (deviceInfo.Contains("tcp://"))
                        tempDeviceInfoList.Add(deviceInfo);
                }
            }
            return tempDeviceInfoList;
        }

        public static void InitializeDatabase()
        {
            SQLitePCL.raw.SetProvider(new SQLitePCL.SQLite3Provider_winsqlite3());
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                String tableCommand = "CREATE TABLE IF NOT " +
                    "EXISTS url_list (Primary_Key INTEGER PRIMARY KEY, " +
                    "url NVARCHAR(2048) NULL)";

                SqliteCommand createTable = new SqliteCommand(tableCommand, db);

                createTable.ExecuteReader();
            }
        }

        public static void AddData(string inputText)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                SqliteCommand insertCommand = new SqliteCommand();
                insertCommand.Connection = db;

                insertCommand.CommandText = "INSERT INTO url_list VALUES (NULL, @Entry);";
                insertCommand.Parameters.AddWithValue("@Entry", inputText);

                insertCommand.ExecuteReader();

                db.Close();
            }
        }

        public static List<String> GetData()
        {
            List<String> entries = new List<string>();

            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                SqliteCommand selectCommand = new SqliteCommand
                    ("SELECT url from url_list", db);

                SqliteDataReader query = selectCommand.ExecuteReader();

                while (query.Read())
                {
                    entries.Add(query.GetString(0));
                }

                db.Close();
            }

            return entries;
        }

    }
}