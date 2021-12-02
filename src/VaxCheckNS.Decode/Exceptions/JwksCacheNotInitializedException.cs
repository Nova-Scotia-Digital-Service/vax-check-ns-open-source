using System;
using VaxCheckNS.Decode.Providers;
namespace VaxCheckNS.Decode.Exceptions
{
    public class JwksCacheNotInitializedException<T> : ApplicationException
    {
        public JwksCacheNotInitializedException(): base($"{nameof(IPersistentJwksProvider<T>.TryInitializeJwksAsync)} must be called before attempting to get JWKS.")
        {
        }
    }
}
