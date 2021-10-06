using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace ProofOfVaccine.Mobile.UITests
{
    public class BaseTests
    {
        protected static IApp app => concurrentApp.Value;
        protected Platform platform;

        private static ThreadLocal<IApp> concurrentApp = new ThreadLocal<IApp>();
        public static IApp GetApp()
        {
            return concurrentApp.Value;
        }

        public BaseTests(Platform platform)
        {
            this.platform = platform;
        }

        [SetUp]
        public void BeforeEachTest()
        {
            concurrentApp.Value = AppInitializer.StartApp(platform);
        }

        //TODO: add timeouts
        public bool IsOnPage(string pageId)
        {
            AppResult[] results = app.WaitForElement(e => e.Marked(pageId));
            return HasResults(results);
        }

        public bool CanTapButton(string elementId)
        {
            AppResult[] results = app.Query(e => e.Marked(elementId));

            if (results.Count() > 0)
            {
                app.Tap(e => e.Marked(elementId));
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CanTapNavigate(string tapElementId, string pageId)
        {
            var isTapped = CanTapButton(tapElementId);

            var results = app.WaitForElement(e => e.Marked(pageId));

            if (results.Count() > 0)
            {
                app.Tap(e => e.Marked(pageId));
                return true;
            }
            else
            {
                return false;
            }
        }


        private bool HasResults(AppResult[] results, Action method = null)
        {
            if (results.Count() > 0)
            {
                if(method != null)
                    method.Invoke();

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
