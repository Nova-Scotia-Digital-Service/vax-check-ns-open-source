using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace VaxCheckNS.Mobile.UITests
{
    [TestFixture(Platform.Android)]
    //[TestFixture(Platform.iOS)]
    public class ScanFlowTests : BaseTests
    {
        public ScanFlowTests(Platform platform) : base(platform) { }

        private bool NavigateToScanPage()
        {
            AcceptTermsOfUseAndPrivacyPolicy("TermsOfUsePage", "PrivacyPage");
            AcceptTermsOfUseAndPrivacyPolicy("PrivacyPage", "HomePage");
            
            if (IsOnPage("HomePage"))
                return CanTapNavigate("ScanButtonText", "ScanPage");
            else
                return false;
        }

        private bool AcceptTermsOfUseAndPrivacyPolicy(string currentPage, string nextPage)
        {
            if (IsOnPage(currentPage)) 
            {
                return CanTapNavigate("AcceptButtonText", nextPage);
            }
            else
                return false;
        }

        [Test]
        public void AcceptTermsOfUseTest()
        {
            Assert.IsTrue(AcceptTermsOfUseAndPrivacyPolicy("TermsOfUsePage", "PrivacyPage"));
        }

        [Test]
        public void AcceptPrivacyPolicyTest()
        {
            AcceptTermsOfUseAndPrivacyPolicy("TermsOfUsePage", "PrivacyPage");
            Assert.IsTrue(AcceptTermsOfUseAndPrivacyPolicy("PrivacyPage", "HomePage"));
        }

        [Test]
        public void ScanPageNavigatedTest()
        {
            Assert.IsTrue(NavigateToScanPage());
        }

        [Test]
        public void ScanPageTimeoutTest()
        {
            NavigateToScanPage();

            //Once in Scan Page
            Thread.Sleep(10000); //Wait 10 Secounds for app logic time out 
            Thread.Sleep(500); //Small buffer time incase

            //Check if we navigated back to home page
            Assert.IsTrue(IsOnPage("HomePage"));
        }

        [Test]
        public void ScanPagePressCancelTest()
        {
            NavigateToScanPage();
            Assert.IsTrue(CanTapNavigate("CancelButtonText", "HomePage"));
        }

    }
}
