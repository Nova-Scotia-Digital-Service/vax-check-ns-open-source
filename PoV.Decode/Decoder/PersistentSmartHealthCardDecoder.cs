using System;
using PoV.Decode.Providers;
using PoV.Decode.DataStore;
using ProofOfVaccine.Token;

namespace ProofOfVaccine.Decode.Decoder
{
    /// <summary>
    /// Class implements the <see cref="IDecoder"/> interface so it can be used in dependency injection.
    /// </summary>
    public class PersistentSmartHealthCardDecoder : SmartHealthCardDecoder, IDecoder
    {
        public PersistentSmartHealthCardDecoder(IPersistentJwksProvider<IJwksDataStore> persistentJwksProvider) : base(persistentJwksProvider){}
    }
}
