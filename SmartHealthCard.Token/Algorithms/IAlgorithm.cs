using ProofOfVaccine.Token.Support;

namespace ProofOfVaccine.Token.Algorithms
{
  public interface IAlgorithm
  {
    static string? Name { get; }
    static string? KeyTypeName { get; }
    static string? CurveName { get; }
    Result<bool> Verify(byte[] bytesToSign, byte[] signature);
  }
}