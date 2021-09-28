using System;
namespace ProofOfVaccine.Mobile.Animations
{
    public interface IAnimation<T>
    {
        void Start(Action<T> onValueCallback, Action onComplete = null);
    }
}
