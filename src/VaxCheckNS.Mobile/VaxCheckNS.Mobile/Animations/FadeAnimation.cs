using System;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.Animations
{
    public class FadeAnimation : IAnimation<double>
    {
        private readonly string _name = nameof(FadeAnimation);
        private double _opacity;

        public double FromOpacity { get; }
        public double ToOpacity { get; }
        public TimeSpan Duration { get; }

        public Easing Easing { get; }

        public FadeAnimation(double fromOpacity,
            double toOpacity,
            TimeSpan duration,
            Easing easing)
        {
            FromOpacity = fromOpacity;
            ToOpacity = toOpacity;
            Duration = duration;
            Easing = easing;

            _opacity = fromOpacity;
        }

        public void Start(Action<double> onValueCallback, Action onComplete = null)
        {

            Animation animation = new Animation(ValueChangedEventArgs
                => onValueCallback(_opacity), finished: onComplete)
            {
                { 0, 1, new Animation(v => _opacity = v, FromOpacity, ToOpacity, Easing) }
            };
            animation.Commit(owner: Application.Current.MainPage,
                name: _name,
                length: (uint)Duration.TotalMilliseconds,
                finished: (d, b) => onComplete?.Invoke());
        }
    }
}
