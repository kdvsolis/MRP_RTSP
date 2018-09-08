using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MRP_RTSP_Client.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadingDialogPage : Page
    {
        public LoadingDialogPage()
        {
            this.InitializeComponent();

            LoadDialogPageExecute();
        }

        public async void LoadDialogPageExecute()
        { 
            await Task.Delay(1000); // 1 sec delay  
            pr_ProgressRing1.IsActive = true; // start progress ring  
            for (int i = 5; i >= 1; i--) // Keep running progress Ring for 5 seconds  
            {
                await Task.Delay(1000);
            }
        }
    }
}
