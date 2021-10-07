using System;
using System.Linq;
using CoreGraphics;
using VaxCheckNS.Mobile.iOS.Renderers;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Button), typeof(CustomButtonRenderer))]
namespace VaxCheckNS.Mobile.iOS.Renderers
{
    public class CustomButtonRenderer : ButtonRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                Control.ImageView.TranslatesAutoresizingMaskIntoConstraints = false;

                // Contraint the center of the buttons y-axis and the button left side (+ some extra space).
                NSLayoutConstraint.Create(Control.ImageView, NSLayoutAttribute.Left, NSLayoutRelation.Equal, Control.ImageView.Superview, NSLayoutAttribute.LeftMargin, 1.0f, 10f).Active = true;
                NSLayoutConstraint.Create(Control.ImageView, NSLayoutAttribute.CenterY, NSLayoutRelation.Equal, Control.ImageView.Superview, NSLayoutAttribute.CenterY, 1.0f, 0f).Active = true;
            }
        }
    }
}
