using Android.Content;
using VaxCheckNS.Mobile.Helpers;
using VaxCheckNS.Mobile.Droid.Helpers;
using Xamarin.Forms;
using Application = Android.App.Application;

[assembly: Dependency(typeof(OpenAndroidAppSettingsHelper))]
namespace VaxCheckNS.Mobile.Droid.Helpers
{
    public class OpenAndroidAppSettingsHelper : IOpenPlatformAppSettingsHelper
    {
        public void OpenAppSettings()
        {
            var intent = new Intent(Android.Provider.Settings.ActionApplicationDetailsSettings);
            intent.AddFlags(ActivityFlags.NewTask);
            string package_name = Xamarin.Essentials.AppInfo.PackageName;
            var uri = Android.Net.Uri.FromParts("package", package_name, null);
            intent.SetData(uri);
            Application.Context.StartActivity(intent);
        }
    }
}
