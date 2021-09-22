using System;
using System.Threading.Tasks;
using ProofOfVaccine.Token.Model.Shc;

namespace ProofOfVaccine.Decode.Decoder
{
    public interface IDecoder
    {
        Task<SmartHealthCardModel> DecodeAsync(string Token, bool Verify = true);
    }
}
