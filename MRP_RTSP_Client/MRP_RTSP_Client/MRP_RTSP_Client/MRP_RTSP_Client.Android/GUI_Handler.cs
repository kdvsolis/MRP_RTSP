using System;
using System.Collections.Generic;
using System.Net.Mqtt;
using System.Text;
using System.Text.RegularExpressions;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using static Android.Widget.TabHost;
using LibVLCSharp.Shared;
using VideoView = LibVLCSharp.Platforms.Android.VideoView;
using System.Threading.Tasks;

namespace MRP_RTSP_Client.Droid
{
    class GUI_Handler
    {
        private TabHost tabHost;
        private TabSpec spec1;
        private TabSpec spec2;
        private ListView deviceListView;
        private ListView rtspListView;
        private ArrayAdapter<string> deviceListAdapter;
        private ArrayAdapter<string> rtspListAdapter;
        private List<string> deviceList = new List<string>();
        private List<string> rtspList = new List<string>();
        private MainActivity mActivity;
        private AlertDialog.Builder loadingDialogBuilder;
        private AlertDialog loadingDialog;
        private AlertDialog.Builder inputDialogBuilder;
        private AlertDialog inputDialog;
        private LayoutInflater inputDialogLayoutInflater;
        private View inputDialogView;
        private string inputValue = "";
        private static string urlLink = "";
        private static MqttConfiguration configuration;
        private static IMqttClient client;
        private static string playingIpAddress;
        private static string playingPort;
        private delegate void SendCommandToDeviceCB(string ipAddress, string port, string topic, string command, bool isPublishOnly=false);
        private static SendCommandToDeviceCB sendCommandToDevice;

        [Activity(MainLauncher = false,
          Theme = "@android:style/Theme.Black.NoTitleBar.Fullscreen",
          ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
        public class VLCActivity : Activity
        {
            private VideoView _videoView;
            private string source_link = "";

            protected override void OnCreate(Bundle savedInstanceState)
            {
                base.OnCreate(savedInstanceState);
                SetContentView(Resource.Layout.vlc_view);
                string link = Intent.GetStringExtra("link");
                source_link = Intent.GetStringExtra("source_link");
                _videoView = new VideoView(this);
                AddContentView(_videoView, new LinearLayout.LayoutParams(ViewGroup.LayoutParams.WrapContent, ViewGroup.LayoutParams.WrapContent));
                var media = new Media(_videoView.LibVLC, link, Media.FromType.FromLocation);
                var configuration = new MediaConfiguration();
                configuration.EnableHardwareDecoding();
                media.AddOption(configuration);
                _videoView.MediaPlayer.Play(media);
            }

            public override void OnBackPressed()
            {
                _videoView.MediaPlayer.Stop();
                while (_videoView.MediaPlayer.IsPlaying) ;
                if(source_link =="network")
                    Task.Run(() => sendCommandToDevice(playingIpAddress, playingPort, "event", "camera_server_off", true)).Wait();
                base.OnBackPressed();
            }

            protected override void OnStop()
            {
                base.OnStop();
            }
        }

        public enum DialogChoice
        {
            LOADING = 1,
            INPUT_TEXT
        }

        private GUI_Handler()
        {

        }

        private static GUI_Handler instance = null;

        private void InitializeDialogHandlers()
        {
            loadingDialogBuilder = new AlertDialog.Builder(this.mActivity);
            loadingDialogBuilder.SetCancelable(true);
            loadingDialogBuilder.SetView(Resource.Layout.loading_dialog_box);
            loadingDialog = loadingDialogBuilder.Create();

            inputDialogLayoutInflater = LayoutInflater.From(this.mActivity);
            inputDialogView = inputDialogLayoutInflater.Inflate(Resource.Layout.user_input_dialog_box, null);
            inputDialogBuilder = new AlertDialog.Builder(this.mActivity);
            inputDialogBuilder.SetView(inputDialogView);
            inputDialogBuilder.SetCancelable(true)
                              .SetPositiveButton("Add", (EventHandler<DialogClickEventArgs>)null)
                              .SetNegativeButton("Cancel", (EventHandler<DialogClickEventArgs>)null);
            inputDialog = inputDialogBuilder.Create();
        }

        private void InitializeTabs()
        {
            tabHost = this.mActivity.FindViewById<TabHost>(Resource.Id.tabHost);
            tabHost.Setup();

            View tabIndicator = this.mActivity.LayoutInflater.Inflate(Resource.Layout.tab_indicator, tabHost.TabWidget, false);
            (tabIndicator.FindViewById<TextView>(Android.Resource.Id.Title)).Text = "Network Devices";
            (tabIndicator.FindViewById<ImageView>(Android.Resource.Id.Icon)).SetImageResource(Resource.Mipmap.baseline_router_black_48);

            View tabIndicator2 = this.mActivity.LayoutInflater.Inflate(Resource.Layout.tab_indicator, tabHost.TabWidget, false);
            (tabIndicator2.FindViewById<TextView>(Android.Resource.Id.Title)).Text = "RTSP Servers";
            (tabIndicator2.FindViewById<ImageView>(Android.Resource.Id.Icon)).SetImageResource(Resource.Mipmap.baseline_playlist_play_black_48);

            spec1 = tabHost.NewTabSpec("Tab1");
            spec1.SetContent(Resource.Id.tab1);
            spec1.SetIndicator(tabIndicator);
            spec2 = tabHost.NewTabSpec("Tab2");
            spec2.SetIndicator(tabIndicator2);
            spec2.SetContent(Resource.Id.tab2);
            tabHost.AddTab(spec1);
            tabHost.AddTab(spec2);
        }

        private void InitializeListView()
        {
            deviceList = new List<string>();
            deviceListView = this.mActivity.FindViewById<ListView>(Resource.Id.listView1);
            deviceListAdapter = new ArrayAdapter<string>(this.mActivity, Resource.Layout.listviewtemplate, Resource.Id.lbltransparent, deviceList);
            deviceListView.Adapter = deviceListAdapter;
            deviceListView.AddFooterView(new View(this.mActivity));
            deviceListView.ItemClick += deviceListView_ItemClick;
            this.mActivity.RegisterForContextMenu(deviceListView);

            rtspList = new List<string>();
            rtspListView = this.mActivity.FindViewById<ListView>(Resource.Id.listView2);
            rtspListAdapter = new ArrayAdapter<string>(this.mActivity, Resource.Layout.listviewtemplate, Resource.Id.lbltransparent, rtspList);
            rtspListView.Adapter = rtspListAdapter;
            rtspListView.AddFooterView(new View(this.mActivity));
            rtspListView.ItemClick += rtspListView_ItemClick;
            this.mActivity.RegisterForContextMenu(rtspListView);

        }

        public void GUI_Handler_Initialize(MainActivity _mActivity)
        {
            this.mActivity = _mActivity;
            sendCommandToDevice = SendCommandToDevice;
            InitializeDialogHandlers();
            InitializeTabs();
            InitializeListView();
        }

        public static GUI_Handler Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GUI_Handler();
                }
                return instance;
            }
        }

        public void FillDeviceListView(List<string> _deviceList)
        {
            if (_deviceList != null)
            {
                deviceList = _deviceList;
                this.mActivity.RunOnUiThread(() =>
                {
                    deviceListAdapter = new ArrayAdapter<string>(this.mActivity, Resource.Layout.listviewtemplate, Resource.Id.lbltransparent, deviceList);
                    deviceListView.Adapter = deviceListAdapter;
                    tabHost.SetCurrentTabByTag("Tab1");
                });
            }
        }

        public void FillRtspListView(List<string> _rtspList)
        {
            if (_rtspList != null)
            {
                rtspList = _rtspList;
                this.mActivity.RunOnUiThread(() =>
                {
                    rtspListAdapter = new ArrayAdapter<string>(this.mActivity, Resource.Layout.listviewtemplate, Resource.Id.lbltransparent, rtspList);
                    rtspListView.Adapter = rtspListAdapter;
                    tabHost.SetCurrentTabByTag("Tab2");
                });
            }
            else
            {
                rtspListView.Adapter = null;
            }
        }

        public void ShowDialog(DialogChoice dialogChoice)
        {
            switch (dialogChoice)
            {
                case DialogChoice.LOADING:
                    loadingDialog.Show();
                    break;

                case DialogChoice.INPUT_TEXT:
                    inputDialog.Show();
                    inputDialog.GetButton((int)DialogButtonType.Positive).Click += inputText_AddClick;
                    inputDialog.GetButton((int)DialogButtonType.Negative).Click += inputText_CancelClick;
                    break;
            }
        }

        public bool GetDialogState(DialogChoice dialogChoice)
        {
            switch (dialogChoice)
            {
                case DialogChoice.LOADING:
                    return loadingDialog.IsShowing;
                case DialogChoice.INPUT_TEXT:
                    return inputDialog.IsShowing;
            }

            return false;
        }

        public void DismissDialog(DialogChoice dialogChoice)
        {
            switch (dialogChoice)
            {
                case DialogChoice.LOADING:
                    loadingDialog.Dismiss();
                    break;

                case DialogChoice.INPUT_TEXT:
                    inputDialog.Dismiss();
                    break;

            }
        }

        public string GetInputTextValue()
        {
            return inputValue;
        }

        private void deviceListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string item = deviceListView.Adapter.GetItem(e.Position).ToString();
            string ipAddress = (item.Split('\n')[1].Replace("tcp://", "")).Split(':')[0];
            string port = new Regex(@"[^\d]").Replace((item.Split('\n')[1].Replace("tcp://", "")).Split(':')[1], "");
            SendCommandToDevice(ipAddress, port, "url_request", "stream_url");
        }

        private void rtspListView_ItemClick(object sender, AdapterView.ItemClickEventArgs e)
        {
            string item = rtspListView.Adapter.GetItem(e.Position).ToString();
            ExecuteVLCPlayer(item, "list");
        }

        private void inputText_AddClick(object sender, EventArgs e)
        {
            inputValue = (inputDialogView.FindViewById<EditText>(Resource.Id.editText)).Text;
            if (!inputValue.ToLower().StartsWith("rtsp://") &&
                !inputValue.ToLower().StartsWith("http://") &&
                !inputValue.ToLower().StartsWith("https://") &&
                inputValue.Length < 10)
            {
                inputValue = "";
                (inputDialogView.FindViewById<EditText>(Resource.Id.editText)).Text = "";
                Toast.MakeText(Android.App.Application.Context, "Invalid input!", ToastLength.Short).Show();
            }
            else
            {
                inputDialog.Dismiss();
                InitializeDialogHandlers();
            }
        }

        private void inputText_CancelClick(object sender, EventArgs e)
        {
            inputValue = "";
            (inputDialogView.FindViewById<EditText>(Resource.Id.editText)).Text = "";
            inputDialog.Dismiss();
        }

        private async void SendCommandToDevice(string ipAddress, string port, string topic, string command, bool isPublishOnly=false)
        {
            if (client != null && client.IsConnected)
            {
                await client.DisconnectAsync();
            }
            playingIpAddress = ipAddress;
            playingPort = port;
            configuration = new MqttConfiguration { Port = Convert.ToInt32(port), WaitTimeoutSecs = 1};
            client = await MqttClient.CreateAsync(ipAddress, configuration);
            var sessionState = await client.ConnectAsync(new MqttClientCredentials(clientId: "mrpclient"));
            if (!isPublishOnly)
            {
                    
                await client.SubscribeAsync(topic + "_client", MqttQualityOfService.AtLeastOnce);
                client.MessageStream.Subscribe(msg => OnMessageReceived(msg.Topic, System.Text.Encoding.UTF8.GetString(msg.Payload)));
            }
            await client.PublishAsync(new MqttApplicationMessage(topic, Encoding.UTF8.GetBytes(command)), MqttQualityOfService.AtLeastOnce);

        }

        private async void OnMessageReceived(string topic, string message)
        {
            if (topic == "url_request_client")
            {
                urlLink = message;
                await client.SubscribeAsync("event_client", MqttQualityOfService.AtLeastOnce);
                await client.PublishAsync(new MqttApplicationMessage("event", Encoding.UTF8.GetBytes("camera_server_on")), MqttQualityOfService.AtLeastOnce);
                return;
            }
            if (topic == "event_client")
            {
                if (message == "Camera Server Ready")
                {
                    if (client != null && client.IsConnected)
                    {
                        await client.UnsubscribeAsync(topic);
                        await client.DisconnectAsync();
                        
                        ExecuteVLCPlayer(urlLink, "network");
                    }
                }
                return;
            }
        }

        private void ExecuteVLCPlayer(string link, string source_link)
        {
            this.mActivity.RunOnUiThread(() =>
            {
                Intent vlcActivity = new Intent(this.mActivity, typeof(VLCActivity));
                vlcActivity.PutExtra("link", link);
                vlcActivity.PutExtra("source_link", source_link);
                this.mActivity.StartActivity(vlcActivity);
            });
        }

    }
}