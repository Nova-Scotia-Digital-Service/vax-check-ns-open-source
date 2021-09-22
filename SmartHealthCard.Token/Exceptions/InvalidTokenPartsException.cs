namespace ProofOfVaccine.Token.Exceptions
{
  public class InvalidTokenPartsException : SmartHealthCardException
  {   
    public InvalidTokenPartsException(string Message)
        : base(Message){ }
  }
}
