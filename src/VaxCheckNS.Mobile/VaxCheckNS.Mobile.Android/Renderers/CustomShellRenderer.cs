using Android.Content;
using VaxCheckNS.Mobile;
using VaxCheckNS.Mobile.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(AppShell), typeof(CustomShellRenderer))]
namespace VaxCheckNS.Mobile.Droid.Renderers
{
    public class CustomShellRenderer : ShellRenderer
    {
        public CustomShellRenderer(Context context) : base(context)
        {

        }

        protected override IShellToolbarAppearanceTracker CreateToolbarAppearanceTracker()
        {
            return new CustomShellToolbarAppearanceTracker(this);
        }
    }
}
