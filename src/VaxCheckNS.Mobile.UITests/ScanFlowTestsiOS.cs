﻿using System;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Xamarin.UITest;
using Xamarin.UITest.Queries;

namespace VaxCheckNS.Mobile.UITests
{
    [TestFixture(Platform.iOS)]
    public class ScanFlowTestsiOS : BaseTests
    {
        public ScanFlowTestsiOS(Platform platform) : base(platform) { }

        private bool NavigateToScanPage()
        {
            //AcceptTermsOfUseAndPrivacyPolicy("TermsOfUsePage", "PrivacyPage");
            //AcceptTermsOfUseAndPrivacyPolicy("PrivacyPage", "HomePage");

            if (IsOnPage("HomePage"))
            {
                app.Tap("ScanButtonText");
                Thread.Sleep(10000);
                return IsOnPage("ScanPage");

                //return CanTapNavigate("ScanButtonText", "ScanPage");
            }
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
        public void AcceptPrivacyPolicyAndTermsOfUseTest()
        {
           
                Assert.IsTrue(AcceptTermsOfUseAndPrivacyPolicy("TermsOfUsePage", "PrivacyPage"));
                if (CanTapNavigate("AcceptButtonText", "PermissionsPage"))
                {
                    Assert.IsTrue(IsOnPage("PermissionsPage"));
                    Assert.IsTrue(CanTapNavigate("AcceptButtonText", "HomePage"));
                }
                else
                {
                    Assert.IsTrue(AcceptTermsOfUseAndPrivacyPolicy("PrivacyPage", "HomePage"));
                }
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
