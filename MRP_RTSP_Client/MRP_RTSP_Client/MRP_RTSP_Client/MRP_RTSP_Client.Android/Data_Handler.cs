using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Android.Database;
using Android.Database.Sqlite;
using Rssdp;
using SQLite;

namespace MRP_RTSP_Client.Droid
{
    class Data_Handler
    {
        private List<string> deviceInfoList;
        private GUI_Handler ui;
        private DBHelper db;
        private SQLiteDatabase sqliteDB;
        private List<string> streamLinkList = new List<string>();

        public class Stream_Link_List_Handler
        {
            [PrimaryKey]
            public int id { get; set; }
            public string stream_link { get; set; }
        }

        public class DBHelper : SQLiteOpenHelper
        {
            private static string DB_PATH = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
            private static string DB_NAME = "mrp_db.db";
            private static int VERSION = 1;
            private MainActivity context;
            public DBHelper(MainActivity context) : base(context, DB_NAME, null, VERSION)
            {
                this.context = context;
            }
            private string GetSQLiteDBPath()
            {
                return Path.Combine(DB_PATH, DB_NAME);
            }
            public override SQLiteDatabase WritableDatabase
            {
                get
                {
                    return CreateSQLiteDB();
                }
            }
            private SQLiteDatabase CreateSQLiteDB()
            {
                SQLiteDatabase sqliteDB = null;
                string path = GetSQLiteDBPath();
                Stream streamSQLite = null;
                FileStream streamWriter = null;
                Boolean isSQLiteInit = false;
                try
                {
                    if (File.Exists(path))
                        isSQLiteInit = true;
                    else
                    {
                        streamSQLite = context.Resources.OpenRawResource(Resource.Raw.mrp_db);
                        streamWriter = new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write);
                        if (streamSQLite != null && streamWriter != null)
                        {
                            if (CopySQLiteDB(streamSQLite, streamWriter))
                                isSQLiteInit = true;
                        }
                    }
                    if (isSQLiteInit)
                        sqliteDB = SQLiteDatabase.OpenDatabase(path, null, DatabaseOpenFlags.OpenReadwrite);
                }
                catch
                {

                }
                return sqliteDB;
            }
            private bool CopySQLiteDB(Stream streamSQLite, FileStream streamWriter)
            {
                bool isSuccess = false;
                int lenght = 256;
                Byte[] buffer = new Byte[lenght];
                try
                {
                    int bytesRead = streamSQLite.Read(buffer, 0, lenght);
                    while (bytesRead > 0)
                    {
                        streamWriter.Write(buffer, 0, bytesRead);
                        bytesRead = streamSQLite.Read(buffer, 0, lenght);
                    }
                    isSuccess = true;
                }
                catch { }
                finally
                {
                    streamSQLite.Close();
                    streamWriter.Close();
                }
                return isSuccess;
            }
            public override void OnCreate(SQLiteDatabase db)
            {
            }
            public override void OnUpgrade(SQLiteDatabase db, int oldVersion, int newVersion)
            {

            }
        }

        public Data_Handler(MainActivity activity)
        {
            ui = GUI_Handler.Instance;
            db = new DBHelper(activity);
            sqliteDB = db.WritableDatabase;
        }

        public async void LoadDeviceInfoList()
        {
            SsdpDeviceLocator deviceLocator = new SsdpDeviceLocator();
            List<string> tempDeviceInfoList = new List<string>();
            var foundDevicesTask = Task.Run(() => deviceLocator.SearchAsync(new TimeSpan(0, 0, 5)));
            string deviceInfo = "";

            ui.ShowDialog(GUI_Handler.DialogChoice.LOADING);

            await Task.Run(() =>
            {
                while (!deviceLocator.IsSearching || !ui.GetDialogState(GUI_Handler.DialogChoice.LOADING));
                while (deviceLocator.IsSearching && ui.GetDialogState(GUI_Handler.DialogChoice.LOADING)) ;

                if (!ui.GetDialogState(GUI_Handler.DialogChoice.LOADING))
                {
                    tempDeviceInfoList = null;
                    return;
                }

                tempDeviceInfoList = new List<string>();
                foreach (var foundDevice in foundDevicesTask.Result)
                {
                    try
                    {
                        deviceInfo = "";
                        var fullDevice = Task.Run(() => foundDevice.GetDeviceInfo()).Result;
                        deviceInfo += fullDevice.FriendlyName + "\n";
                        deviceInfo += fullDevice.PresentationUrl;
                        if (deviceInfo.Contains("tcp://"))
                            tempDeviceInfoList.Add(deviceInfo);
                    }
                    catch(System.AggregateException)
                    {

                    }
                }

                if (ui.GetDialogState(GUI_Handler.DialogChoice.LOADING))
                    ui.DismissDialog(GUI_Handler.DialogChoice.LOADING);

            });

            deviceInfoList = tempDeviceInfoList;
            if (deviceInfoList != null)
                this.ui.FillDeviceListView(deviceInfoList);
        }


        public void ReloadExistingDeviceInfoList()
        {
            if (deviceInfoList != null)
                this.ui.FillDeviceListView(deviceInfoList);
        }

        public List<string> GetStreamLinkList()
        {
            ICursor selectData = sqliteDB.RawQuery("SELECT * FROM stream_link_list", new string[] { });
            streamLinkList = null;
            if (selectData.Count > 0)
            {
                streamLinkList = new List<string>();
                selectData.MoveToFirst();
                do
                {
                    Stream_Link_List_Handler streamLinkListHandler = new Stream_Link_List_Handler();
                    streamLinkListHandler.id = selectData.GetInt(selectData.GetColumnIndex("Id"));
                    streamLinkListHandler.stream_link = selectData.GetString(selectData.GetColumnIndex("stream_link"));
                    streamLinkList.Add(streamLinkListHandler.stream_link);
                }
                while (selectData.MoveToNext());
                selectData.Close();
            }
            return streamLinkList;
        }

        public void AddStreamLink(string streamLink)
        {
            sqliteDB.ExecSQL("INSERT INTO stream_link_list(stream_link) VALUES('" + streamLink + "')");
        }

        public void DeleteStreamLink(string streamLink)
        {
            sqliteDB.ExecSQL("DELETE FROM stream_link_list WHERE stream_link = '" + streamLink + "'");
        }
    }
}