using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(VaxCheckNS.Mobile.AppShell), typeof(VaxCheckNS.Mobile.iOS.Renderers.CustomShellRenderer))]
namespace VaxCheckNS.Mobile.iOS.Renderers
{
    public class CustomShellRenderer : ShellRenderer
    {
        protected override IShellPageRendererTracker CreatePageRendererTracker()
        {
            return new CustomShellPageTrackerRenderer(this);
        }
    }
}
