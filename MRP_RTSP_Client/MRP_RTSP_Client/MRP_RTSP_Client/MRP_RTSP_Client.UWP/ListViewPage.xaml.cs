using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mqtt;
using System.Text;
using System.Text.RegularExpressions;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MRP_RTSP_Client.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ListViewPage : Page
    {
        private List<string> listContent = new List<string>();
        private string listHeader = "";
        private static string urlLink = "";
        private string rightClickedItem = "";
        private static MqttConfiguration configuration;
        private static IMqttClient client;
        private static string playingIpAddress;
        private static string playingPort;


        public ListViewPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is List<string>)
            {
                List<string> param = ((List<string>)e.Parameter);
                if(param != null)
                {
                    ((TextBlock)this.FindName("TitleBlock")).Text = param[0];
                    listHeader = param[0];
                    ((ListView)this.FindName("ItemList")).Items.Clear();
                    foreach(string item in param.Skip(1))
                        ((ListView)this.FindName("ItemList")).Items.Add(item);
                    listContent.AddRange(param.ToArray());

                    if (listHeader == "Network Devices")
                    {

                        ((MenuFlyoutItem)this.FindName("MFISubMenu1")).Text = "Add";
                    }

                    if (listHeader == "RTSP Servers")
                    {
                        ((MenuFlyoutItem)this.FindName("MFISubMenu1")).Text = "Delete";
                    }
                }
            }
            base.OnNavigatedTo(e);
        }

        private void ListView1_ItemClick(object sender, ItemClickEventArgs e)
        {
            string clickedItemText = e.ClickedItem.ToString();
            if (listHeader == "Network Devices")
            {
                string ipAddress = (clickedItemText.Split('\n')[1].Replace("tcp://", "")).Split(':')[0];
                string port = new Regex(@"[^\d]").Replace((clickedItemText.Split('\n')[1].Replace("tcp://", "")).Split(':')[1], "");
                playingIpAddress = ipAddress;
                playingPort = port;
                SendCommandToDevice(ipAddress, port, "url_request", "stream_url");
            }

            if (listHeader == "RTSP Servers")
            {
                MainPage.MainPageFrame.Navigate(typeof(MediaPlayerPage), clickedItemText);
            }
        }

        private void MFISubMenu1_Click(object sender, RoutedEventArgs e)
        {

            if (listHeader == "RTSP Servers")
            {
                DeleteData(rightClickedItem);
                ((ListView)this.FindName("ItemList")).Items.Clear();
                listContent.RemoveAt(listContent.IndexOf(rightClickedItem));
                foreach (string item in listContent.Skip(1))
                    ((ListView)this.FindName("ItemList")).Items.Add(item);
            }
        }

        private void ItemList_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            ListView listView = (ListView)sender;
            string text = ((FrameworkElement)e.OriginalSource).DataContext.ToString();
            MenuFlyoutContext.ShowAt(listView, e.GetPosition(listView));
            rightClickedItem = text;

        }


        private static void DeleteData(string inputText)
        {
            using (SqliteConnection db = new SqliteConnection("Filename=sqliteSample.db"))
            {
                db.Open();

                SqliteCommand deleteCommand = new SqliteCommand();
                deleteCommand.Connection = db;

                deleteCommand.CommandText = "DELETE FROM url_list WHERE url=@Entry";
                deleteCommand.Parameters.AddWithValue("@Entry", inputText);

                deleteCommand.ExecuteReader();

                db.Close();
            }
        }

        private async static void SendCommandToDevice(string ipAddress, string port, string topic, string command)
        {
            if (client != null && client.IsConnected)
            {
                await client.DisconnectAsync();
            }
            configuration = new MqttConfiguration {Port = Convert.ToInt32(port)};
            client = await MqttClient.CreateAsync(ipAddress, configuration);
            var sessionState = await client.ConnectAsync(new MqttClientCredentials(clientId: "mrpclient"));
            await client.SubscribeAsync(topic + "_client", MqttQualityOfService.AtLeastOnce);
            client.MessageStream.Subscribe(msg => OnMessageReceived(msg.Topic, System.Text.Encoding.UTF8.GetString(msg.Payload)));
            await client.PublishAsync(new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(command)), MqttQualityOfService.AtLeastOnce);
        }

        private async static void OnMessageReceived(string topic, string message)
        {
            if(topic == "url_request_client")
            {
                urlLink = message;
                await client.SubscribeAsync("event_client", MqttQualityOfService.AtLeastOnce);
                await client.PublishAsync(new MqttApplicationMessage("event", Encoding.UTF8.GetBytes("camera_server_on")), MqttQualityOfService.AtLeastOnce);
            }
            if(topic == "event_client")
            {
                if (message == "Camera Server Ready")
                {
                    if (client != null && client.IsConnected)
                    {
                        await client.UnsubscribeAsync(topic);
                        await client.DisconnectAsync();
                    }
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                         await MainPage.DepObject.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => MainPage.MainPageFrame.Navigate(typeof(MediaPlayerPage), urlLink + "`" + playingIpAddress + ":" + playingPort));
                    });
                }
            }
        }
    }
}
