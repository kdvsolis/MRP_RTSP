using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Content Dialog item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MRP_RTSP_Client.UWP
{
    public sealed partial class ContentDialog1 : ContentDialog
    {
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(ContentDialog1), new PropertyMetadata(default(string)));

        public ContentDialog1()
        {
            InitializeComponent();
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private async void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            var deferral = args.GetDeferral();
            args.Cancel = await ValidateForm();
            deferral.Complete();
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
        }

        private async Task<bool> ValidateForm()
        {
            String rtspInput = ((TextBox)this.FindName("RTSPInput")).Text;

            if (!rtspInput.ToLower().StartsWith("rtsp://") && !rtspInput.ToLower().StartsWith("http://") && !rtspInput.ToLower().StartsWith("https://") || rtspInput.Length < 10)
            {
                var dialog = new MessageDialog("Invalid input!");
                await dialog.ShowAsync();
                return true;
            }
            return false;
        }
    }
}
