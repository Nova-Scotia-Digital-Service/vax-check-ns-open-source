using System;

namespace ProofOfVaccine.Token.Exceptions
{
  public abstract class SmartHealthCardException : Exception
  {
    public SmartHealthCardException(string Message)
        : base(Message) { }
  }
}
