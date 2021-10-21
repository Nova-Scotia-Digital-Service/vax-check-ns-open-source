using Foundation;
using UIKit;
using VaxCheckNS.Mobile.Helpers;
using VaxCheckNS.Mobile.iOS.Helpers;
using Xamarin.Forms;

[assembly: Dependency(typeof(OpeniOSAppSettingsHelper))]
namespace VaxCheckNS.Mobile.iOS.Helpers
{
    public class OpeniOSAppSettingsHelper : IOpenPlatformAppSettingsHelper
    {
        public void OpenAppSettings()
        {
            var url = new NSUrl($"app-settings:");
            UIApplication.SharedApplication.OpenUrl(url);
        }
    }
}
