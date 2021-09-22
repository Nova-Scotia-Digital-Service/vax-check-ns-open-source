using System.Threading.Tasks;
using ProofOfVaccine.Token.Providers;

namespace PoV.Decode.DataStore
{
    public interface IJwksDataStore
    {
        Task<IJwksCache> LoadJWKS();
        Task<bool> StoreJWKS(IJwksCache cache);
    }
}
