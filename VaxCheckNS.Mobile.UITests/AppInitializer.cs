using System;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace VaxCheckNS.Mobile.UITests
{
    public class AppInitializer
    {
        public static IApp StartApp(Platform platform)
        {
            if (platform == Platform.Android)
            {
                return ConfigureApp.Android
                    .InstalledApp("ca.novascotia.vaxcheckns.mobile")
                    .EnableLocalScreenshots()
                    .StartApp();
            }

            return ConfigureApp.iOS.StartApp();
        }
    }
}