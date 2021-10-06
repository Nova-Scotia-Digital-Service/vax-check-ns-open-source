using System;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ProofOfVaccine.Mobile.AppShell), typeof(ProofOfVaccine.Mobile.iOS.Renderers.CustomShellRenderer))]
namespace ProofOfVaccine.Mobile.iOS.Renderers
{
    public class CustomShellRenderer : ShellRenderer
    {
        protected override IShellPageRendererTracker CreatePageRendererTracker()
        {
            return new CustomShellPageTrackerRenderer(this);
        }
    }
}
