using System;

namespace VaxCheckNS.Token.Exceptions
{
  public abstract class SmartHealthCardException : Exception
  {
    public SmartHealthCardException(string Message)
        : base(Message) { }
  }
}
