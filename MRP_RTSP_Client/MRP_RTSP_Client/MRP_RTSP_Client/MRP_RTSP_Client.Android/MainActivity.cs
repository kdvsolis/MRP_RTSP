using Android.App;
using Android.OS;
using Android.Views;
using System.Threading;
using Android.Content.Res;
using static Android.Widget.AdapterView;
using Android.Widget;

namespace MRP_RTSP_Client.Droid
{
    [Activity(Label = "MRP RTSP Client", 
              MainLauncher = true,
              Theme = "@android:style/Theme.Material.Light.DarkActionBar",
              ConfigurationChanges = Android.Content.PM.ConfigChanges.KeyboardHidden | Android.Content.PM.ConfigChanges.Orientation | Android.Content.PM.ConfigChanges.ScreenSize)]
    public class MainActivity : Activity
    {
        private GUI_Handler ui;
        private Data_Handler dataHandler;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);
            dataHandler = new Data_Handler(this);
            ui = GUI_Handler.Instance;
            ui.GUI_Handler_Initialize(this);
            ui.FillRtspListView(dataHandler.GetStreamLinkList());
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        { 
            MenuInflater.Inflate(Resource.Menu.menu, menu);
            return base.OnPrepareOptionsMenu(menu);
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_scan:
                    dataHandler.LoadDeviceInfoList();
                    return true;

                case Resource.Id.action_add:
                    ui.ShowDialog(GUI_Handler.DialogChoice.INPUT_TEXT);
                    new Thread(() => 
                    {
                        while (ui.GetDialogState(GUI_Handler.DialogChoice.INPUT_TEXT)) ;
                        if(ui.GetInputTextValue() != "")
                        {
                            dataHandler.AddStreamLink(ui.GetInputTextValue());
                            ui.FillRtspListView(dataHandler.GetStreamLinkList());
                        }
                    }).Start();
                    return true;

            }
            return base.OnOptionsItemSelected(item);
        }

        public override void OnConfigurationChanged(Configuration newConfig)
        {
            dataHandler.ReloadExistingDeviceInfoList();
            base.OnConfigurationChanged(newConfig);
        }

        public override void OnCreateContextMenu(IContextMenu menu, View v, IContextMenuContextMenuInfo menuInfo)
        {
            base.OnCreateContextMenu(menu, v, menuInfo);

            /*if (v.Id == Resource.Id.listView1)
            {
                MenuInflater inflater = MenuInflater;
                inflater.Inflate(Resource.Menu.network_menu, menu);
            }*/
            if (v.Id == Resource.Id.listView2)
            {
                MenuInflater inflater = MenuInflater;
                inflater.Inflate(Resource.Menu.rtsp_menu, menu);
            }
        }

        public override bool OnContextItemSelected(IMenuItem item)
        {
            AdapterContextMenuInfo info = (AdapterContextMenuInfo)item.MenuInfo;
            switch (item.ItemId)
            {
                case Resource.Id.action_add_to_list:
                    //FindViewById<ListView>(Resource.Id.listView1).Adapter.GetItem(info.Position).ToString();
                    return true;
                case Resource.Id.action_delete_from_list:
                    dataHandler.DeleteStreamLink(FindViewById<ListView>(Resource.Id.listView2).Adapter.GetItem(info.Position).ToString());
                    ui.FillRtspListView(dataHandler.GetStreamLinkList());
                    return true;
            }
            return base.OnContextItemSelected(item);
        }

    }
}

