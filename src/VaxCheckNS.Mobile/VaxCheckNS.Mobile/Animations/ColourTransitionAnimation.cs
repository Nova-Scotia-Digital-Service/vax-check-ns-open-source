using System;
using Xamarin.Forms;

namespace VaxCheckNS.Mobile.Animations
{
    public class ColourTransitionAnimation : IAnimation<Color>
    {
        private readonly string _name = nameof(ColourTransitionAnimation);
        private double _r;
        private double _g;
        private double _b;
        private double _a;

        public Color FromColor { get; }
        public Color ToColor { get; }
        public TimeSpan Duration { get; }
        public Easing Easing { get; }

        public ColourTransitionAnimation(Color fromColor,
            Color toColor,
            TimeSpan duration,
            Easing easing)
        {
            _r = FromColor.R;
            _g = FromColor.G;
            _b = FromColor.B;
            _a = FromColor.A;

            FromColor = fromColor;
            ToColor = toColor;
            Duration = duration;
            Easing = easing;
        }

        public void Start(Action<Color> onValueCallback, Action onComplete = null)
        {
            Animation animation = new Animation(ValueChangedEventArgs
                => onValueCallback(new Color(_r, _g, _b, _a)));

            animation.Add(0, 1, new Animation(v => _r = v, FromColor.R, ToColor.R, Easing));
            animation.Add(0, 1, new Animation(v => _g = v, FromColor.G, ToColor.G, Easing));
            animation.Add(0, 1, new Animation(v => _b = v, FromColor.B, ToColor.B, Easing));
            animation.Add(0, 1, new Animation(v => _a = v, FromColor.A, ToColor.A, Easing));

            animation.Commit(owner: Application.Current.MainPage,
                name: _name,
                length: (uint)Duration.TotalMilliseconds);
        }
    }
}
