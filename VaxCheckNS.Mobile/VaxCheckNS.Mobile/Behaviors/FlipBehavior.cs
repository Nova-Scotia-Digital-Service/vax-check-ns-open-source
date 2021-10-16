using System;
using VaxCheckNS.Mobile.Animations;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.Behaviors
{
    public static class FlipBehavior
    {
        public static readonly BindableProperty IsFlippedBehaviorProperty =
            BindableProperty.CreateAttached(
                "FlipBehavior",
                typeof(bool),
                typeof(FlipBehavior),
                false,
                propertyChanged: OnIsFlippedChanged);

        public static bool GetIsFlippedBehavior(BindableObject view)
        {
            return (bool)view.GetValue(IsFlippedBehaviorProperty);
        }

        public static void SetIsFlippedBehavior(BindableObject view, bool value)
        {
            view.SetValue(IsFlippedBehaviorProperty, value);
        }

        static void OnIsFlippedChanged(BindableObject view, object oldValue, object newValue)
        {
            if (!(view is View flipView))
            {
                return;
            }

            if (!(newValue is  bool isFlipped))
            {
                return;
            }

            flipView.AbortAnimation(nameof(RotationAnimation));
            if (isFlipped)
            {
                var flipAnimation = new RotationAnimation(flipView.RotationX, 180, TimeSpan.FromSeconds(0.5), Easing.SpringOut);
                flipAnimation.Start(r => flipView.RotationX = r);
            }
            else
            {
                var unFlipAnimation = new RotationAnimation(flipView.RotationX, 0, TimeSpan.FromSeconds(0.5), Easing.SpringOut);
                unFlipAnimation.Start(r => flipView.RotationX = r);
            }
        }
    }
}
