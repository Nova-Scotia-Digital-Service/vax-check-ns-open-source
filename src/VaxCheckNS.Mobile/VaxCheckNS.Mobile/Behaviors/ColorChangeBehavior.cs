using System;
using VaxCheckNS.Mobile.Animations;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.Behaviors
{
    public static class ColourChangeBehavior
    {
        public static readonly BindableProperty ColourChangedBehaviorProperty =
            BindableProperty.CreateAttached(
                "ColourChangeBehavior",
                typeof(bool),
                typeof(ColourChangeBehavior),
                false,
                propertyChanged: ColourChanged);

        public static bool GetColourChangedBehavior(BindableObject view)
        {
            return (bool)view.GetValue(ColourChangedBehaviorProperty);
        }

        public static void SetColourChangedBehavior(BindableObject view, bool value)
        {
            view.SetValue(ColourChangedBehaviorProperty, value);
        }

        public static readonly BindableProperty OriginalColourProperty =
            BindableProperty.CreateAttached(
                "OriginalColour",
                typeof(Color),
                typeof(ColourChangeBehavior),
                Color.White);

        public static Color GetOriginalColour(BindableObject view)
        {
            return (Color)view.GetValue(OriginalColourProperty);
        }

        public static void SetOriginalColour(BindableObject view, Color value)
        {
            view.SetValue(OriginalColourProperty, value);
        }

        public static readonly BindableProperty ChangedColourProperty =
            BindableProperty.CreateAttached(
                "ChangedColour",
                typeof(Color),
                typeof(ColourChangeBehavior),
                Color.White);

        public static Color GetChangedColour(BindableObject view)
        {
            return (Color)view.GetValue(ChangedColourProperty);
        }

        public static void SetChangedColour(BindableObject view, Color value)
        {
            view.SetValue(ChangedColourProperty, value);
        }

        static void ColourChanged(BindableObject view, object oldValue, object newValue)
        {
            if (!(view is View colouredView))
            {
                return;
            }

            if (!(newValue is bool changeColor))
            {
                return;
            }

            colouredView.AbortAnimation(nameof(ColourTransitionAnimation));
            if (changeColor)
            {
                var toChangedColourAnimation = new ColourTransitionAnimation(colouredView.BackgroundColor, GetChangedColour(colouredView), TimeSpan.FromSeconds(0.5), Easing.Linear);
                toChangedColourAnimation.Start(c => colouredView.BackgroundColor = c);
            }
            else
            {
                var toOriginalColourAnimation = new ColourTransitionAnimation(colouredView.BackgroundColor, GetOriginalColour(colouredView), TimeSpan.FromSeconds(0.5), Easing.Linear);
                toOriginalColourAnimation.Start(c => colouredView.BackgroundColor = c);
            }
        }
    }
}
