using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VaxCheckNS.Token.Model.Jwks;
using VaxCheckNS.Token.Providers;
using VaxCheckNS.Token.Support;

namespace VaxCheckNS.Decode.Providers
{
    /// <summary>
    /// A <see cref="IJwksProvider"/> that loads the JWKS in from a persistent store of type <typeparamref name="T"/>.
    /// This allows for the JWKS to be stored locally and loaded regardless of internet connectivity.
    /// </summary>
    /// <typeparam name="T">The type of data store that contains the <see cref="IJwksCache"/>.</typeparam>
    public interface IPersistentJwksProvider<T> : IJwksProvider
    {
        /// <summary>
        /// Tries to load in the white-listed well know uri frin the JWKS data saved persistently on the device.
        /// Failing that, attempts to retrieve the white-listed URI via HTTPS.
        /// </summary>
        /// <param name="WhiteList">The list of URI that have been white-listed in the app.</param>
        /// <param name="HasConnectivity">A flag that indicates whether there is network connectivity..</param>
        /// <returns>The results of attempting to load the JWKS.</returns>
        Task<IList<Result<JsonWebKeySet>>> TryInitializeJwksAsync(bool HasConnectivity);
    }
}
