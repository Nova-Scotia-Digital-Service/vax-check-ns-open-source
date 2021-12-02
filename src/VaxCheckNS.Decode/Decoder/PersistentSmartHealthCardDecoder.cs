using System;
using VaxCheckNS.Decode.Providers;
using VaxCheckNS.Decode.DataStore;
using VaxCheckNS.Token;

namespace VaxCheckNS.Decode.Decoder
{
    /// <summary>
    /// Class implements the <see cref="IDecoder"/> interface so it can be used in dependency injection.
    /// </summary>
    public class PersistentSmartHealthCardDecoder : SmartHealthCardDecoder, IDecoder
    {
        public PersistentSmartHealthCardDecoder(IPersistentJwksProvider<IJwksDataStore> persistentJwksProvider) : base(persistentJwksProvider){}
    }
}
