using System;

using Xamarin.Forms;

namespace ProofOfVaccine.Mobile.Animations
{
    /// <summary>
    /// An animation that can be used to change the opacity of a view from one opacity level to another.
    /// </summary>
    public class FadeAnimation : IAnimation<double>
    {
        private readonly string _name = nameof(FadeAnimation);
        private double _opacity;

        /// <summary>
        /// The readonly opacity that the view starts with.
        /// </summary>
        public double FromOpacity { get; }

        /// <summary>
        /// The readonly opacity that the view ends with.
        /// </summary>
        public double ToOpacity { get; }

        /// <summary>
        /// The readonly duration of the animation.
        /// </summary>
        public TimeSpan Duration { get; }

        /// <summary>
        /// The readonly easing function the animation uses as part of the transformation.
        /// </summary>
        public Easing Easing { get; }

        /// <summary>
        /// Constructor used to create the animation.
        /// </summary>
        /// <param name="fromOpacity">Used to set <seealso cref="FromOpacity"/> of the animation. </param>
        /// <param name="toOpacity">Used to set <seealso cref="ToOpacity"/> of the animation.</param>
        /// <param name="duration">Used to set <seealso cref="Duration"/> of the animation.</param>
        /// <param name="easing">Used to set <seealso cref="Easing"/> of the animation.</param>
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

        /// <summary>
        /// The method that triggers the animation start. When called the Action passed in will be used to change from the <see cref="FromOpacity"/> to the <seealso cref="ToOpacity"/> over the defined <seealso cref="Duration"/> using the <seealso cref="Easing"/> function.
        /// This can be cancelled using the <seealso cref="AnimationExtensions.AbortAnimation(IAnimatable, string)"/> with the owner being the <seealso cref="Application.Current.MainPage"/> using the handle of the animation class.
        /// </summary>
        /// <param name="onValueCallback">This Action is a callback used to modify the Opacity property of a view. The Opacity passed back is transformed from <seealso cref="FromOpacity"/> to <seealso cref="ToOpacity"/> over the <seealso cref="Duration"/> using the <seealso cref="Easing"/> function.</param>
        /// <param name="onComplete">Called when the animation is completed.</param>
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
