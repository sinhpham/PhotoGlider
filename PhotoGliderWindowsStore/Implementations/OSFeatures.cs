using PhotoGliderPCL.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhotoGliderWindowsStore.Implementations
{
    class OSFeatures : IOSFeatures
    {
        public void OpenLink(string uri)
        {
            Windows.System.Launcher.LaunchUriAsync(new Uri(uri));
        }


        public void CopyToClipboard(string str)
        {
            var dp = new Windows.ApplicationModel.DataTransfer.DataPackage();
            dp.SetText(str);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dp);
        }
    }
}
