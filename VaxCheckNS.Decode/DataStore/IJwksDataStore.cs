using System.Threading.Tasks;
using VaxCheckNS.Token.Providers;

namespace VaxCheckNS.Decode.DataStore
{
    public interface IJwksDataStore
    {
        Task<IJwksCache> LoadJWKS();
        Task<bool> StoreJWKS(IJwksCache cache);
    }
}
