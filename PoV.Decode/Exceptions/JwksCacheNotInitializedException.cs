using System;
using PoV.Decode.Providers;
namespace PoV.Decode.Exceptions
{
    public class JwksCacheNotInitializedException<T> : ApplicationException
    {
        public JwksCacheNotInitializedException(): base($"{nameof(IPersistentJwksProvider<T>.TryInitializeJwksAsync)} must be called before attempting to get JWKS.")
        {
        }
    }
}
