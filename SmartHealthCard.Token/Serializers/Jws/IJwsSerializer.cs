﻿using ProofOfVaccine.Token.Serializers.Json;
using ProofOfVaccine.Token.Support;
using System.Threading.Tasks;

namespace ProofOfVaccine.Token.Serializers.Jws
{
  public interface IJwsSerializer
  {
    /// <summary>
    /// Serialize an object to a JSON string byte[]
    /// </summary>
    Task<Result<byte[]>> SerializeAsync<T>(T Obj, bool Minified = true);

    /// <summary>
    /// De-serialize a JSON string to typed object.
    /// </summary>
    Task<Result<T>> DeserializeAsync<T>(byte[] bytes);
  }
}
