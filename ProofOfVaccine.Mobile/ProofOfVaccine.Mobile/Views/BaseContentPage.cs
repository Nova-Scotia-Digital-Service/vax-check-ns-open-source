using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.Views
{
    public abstract class BaseContentPage : ContentPage
    {
        protected string PageName { get; set; }
        public BaseContentPage(string pageName)
        {
            PageName = pageName;
        }
    }
}
