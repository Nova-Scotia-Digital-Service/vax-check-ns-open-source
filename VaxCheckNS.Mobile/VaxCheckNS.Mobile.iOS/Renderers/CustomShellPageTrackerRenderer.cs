using System;
using System.Linq;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace VaxCheckNS.Mobile.iOS.Renderers
{
    public class CustomShellPageTrackerRenderer : ShellPageRendererTracker
    {
        public CustomShellPageTrackerRenderer(IShellContext context) : base(context)
        {
        }

        // HACK: The TitleView has unavoidable margin around it. To get around that,
        // we must add the view to the Navigations Subviews directly. 
        protected override void UpdateTitleView()
        {
            var titleView = Shell.GetTitleView(Page);

            if (titleView == null)
            {
                var view = ViewController.NavigationController.NavigationBar.Subviews.FirstOrDefault(v => v is TitleViewContainer);
                view?.Dispose();
                view?.RemoveFromSuperview();
                ViewController.NavigationItem.TitleView?.Dispose();
                ViewController.NavigationItem.TitleView = null;
            }
            else
            {
                var view = new CustomTitleViewContainer(titleView);
                view.UserInteractionEnabled = false;

                view.TranslatesAutoresizingMaskIntoConstraints = false;

                var subview = ViewController.NavigationController.NavigationBar.Subviews.FirstOrDefault(v => v is TitleViewContainer);
                if (subview == null)
                {
                    // Just add an empty view.
                    ViewController.NavigationItem.TitleView = new UIView();
                    // Add the actual view as a child so it fills the space available.
                    ViewController.NavigationController.NavigationBar.AddSubview(view);
                }
                else
                {
                    subview = view;
                }

                // NOTE: Using autoconstraints in place of changing the frame size to match parent.
                // Sometimes the NavigationBar wasn't fully loaded when resizing, resulting in a missing InfoBar.
                NSLayoutConstraint.Create(view, NSLayoutAttribute.Width, NSLayoutRelation.Equal, ViewController.NavigationController.NavigationBar, NSLayoutAttribute.Width, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(view, NSLayoutAttribute.Height, NSLayoutRelation.Equal, ViewController.NavigationController.NavigationBar, NSLayoutAttribute.Height, 1.0f, 0.0f).Active = true;
                NSLayoutConstraint.Create(view, NSLayoutAttribute.Leading, NSLayoutRelation.Equal, ViewController.NavigationController.NavigationBar, NSLayoutAttribute.Leading, 1.0f, 0.0f).Active = true;
            }
        }

        public class CustomTitleViewContainer : UIContainerView
        {
            public CustomTitleViewContainer(View view) : base(view)
            {
            }
        }
    }
}
