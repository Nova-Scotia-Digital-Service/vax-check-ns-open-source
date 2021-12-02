using System;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.Animations
{
    public class RotationAnimation : IAnimation<double>
    {
        private readonly string _name = nameof(RotationAnimation);
        private double _rotation;

        public double FromRotation { get; }
        public double ToRotation { get; }
        public TimeSpan Duration { get; }

        public Easing Easing { get; }

        public RotationAnimation(double fromRotation,
            double toRotation,
            TimeSpan duration,
            Easing easing)
        {
            _rotation = fromRotation;

            FromRotation = fromRotation;
            ToRotation = toRotation;
            Duration = duration;
            Easing = easing;
        }

        public void Start(Action<double> onValueCallback, Action onComplete = null)
        {
            Animation animation = new Animation(ValueChangedEventArgs
                => onValueCallback(_rotation), finished: onComplete)
            {
                { 0, 1, new Animation(v => _rotation = v, FromRotation, ToRotation, Easing) }
            };
            animation.Commit(owner: Application.Current.MainPage,
                name: _name,
                length: (uint)Duration.TotalMilliseconds,
                finished: (d, b) => onComplete?.Invoke());
        }
    }
}
