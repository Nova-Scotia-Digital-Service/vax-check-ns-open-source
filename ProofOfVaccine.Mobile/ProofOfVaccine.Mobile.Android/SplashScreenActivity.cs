using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProofOfVaccine.Mobile.Droid
{
    [Activity(MainLauncher = false, Theme = "@style/Theme.Splash", NoHistory = true, Label = "Proof of Vaccination")]
    public class SplashScreenActivity : Activity
    {
        public override void OnCreate(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            base.OnCreate(savedInstanceState, persistentState);
            //StartActivity(typeof(MainActivity));
        }

        protected override void OnResume()
        {
            base.OnResume();
            StartActivity(new Intent(Application.Context, typeof(MainActivity)));
        }


    }
}