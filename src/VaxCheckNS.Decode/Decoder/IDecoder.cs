using System;
using System.Threading.Tasks;
using VaxCheckNS.Token.Model.Shc;

namespace VaxCheckNS.Decode.Decoder
{
    public interface IDecoder
    {
        Task<SmartHealthCardModel> DecodeAsync(string Token, bool Verify = true);
    }
}
