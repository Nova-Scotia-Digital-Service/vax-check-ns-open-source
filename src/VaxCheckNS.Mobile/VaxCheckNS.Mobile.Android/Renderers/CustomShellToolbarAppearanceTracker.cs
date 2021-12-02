using System;
using Android.Content;
using AndroidX.AppCompat.Widget;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

namespace VaxCheckNS.Mobile.Droid.Renderers
{
    public class CustomShellToolbarAppearanceTracker : ShellToolbarAppearanceTracker
    {
        public CustomShellToolbarAppearanceTracker(IShellContext context) : base(context)
        {
        }

        public override void SetAppearance(Toolbar toolbar, IShellToolbarTracker toolbarTracker, ShellAppearance appearance)
        {
            base.SetAppearance(toolbar, toolbarTracker, appearance);
            toolbar.SetPadding(toolbar.PaddingStart, 0, -toolbar.PaddingStart, 0);
        }
    }
}
