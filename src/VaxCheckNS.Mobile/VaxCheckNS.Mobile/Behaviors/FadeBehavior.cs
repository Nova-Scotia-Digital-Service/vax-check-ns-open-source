using System;
using VaxCheckNS.Mobile.Animations;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.Behaviors
{
    public static class FadeBehavior
    {
        public static readonly BindableProperty IsFadedInBehaviorProperty =
            BindableProperty.CreateAttached(
                "FadeBehavior",
                typeof(bool),
                typeof(ColourChangeBehavior),
                false,
                propertyChanged: FadeChanged);

        public static bool GetIsFadedInBehavior(BindableObject view)
        {
            return (bool)view.GetValue(IsFadedInBehaviorProperty);
        }

        public static void SetIsFadedInBehavior(BindableObject view, bool value)
        {
            view.SetValue(IsFadedInBehaviorProperty, value);
        }

        static void FadeChanged(BindableObject view, object oldValue, object newValue)
        {
            if (!(view is View fadeView))
            {
                return;
            }

            if (!(newValue is bool fadeIn))
            {
                return;
            }

            fadeView.AbortAnimation(nameof(FadeAnimation));
            if (fadeIn)
            {
                var fadeInAnimation = new FadeAnimation(fadeView.Opacity, 1, TimeSpan.FromSeconds(0.750), Easing.SinIn);
                fadeInAnimation.Start(o => fadeView.Opacity = o);
            }
            else
            {
                var fadeOutAnimation = new FadeAnimation(fadeView.Opacity, 0, TimeSpan.FromSeconds(0.1), Easing.SinIn);
                fadeOutAnimation.Start(o => fadeView.Opacity = o);
            }
        }
    }
}
