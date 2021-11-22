using System;
namespace VaxCheckNS.Mobile.Animations
{
    public interface IAnimation<T>
    {
        void Start(Action<T> onValueCallback, Action onComplete = null);
    }
}
