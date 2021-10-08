using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace VaxCheckNS.Mobile.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class AppLaunchTests
    {
        IApp app;
        Platform platform;

        public AppLaunchTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            app = AppInitializer.StartApp(platform);
        }

        [Test]
        public void LaunchTest()
        {
            AppResult[] results = app.WaitForElement(c => c.Marked("AppLogo"));
            Assert.IsTrue(results.Any());
        }
    }
}
