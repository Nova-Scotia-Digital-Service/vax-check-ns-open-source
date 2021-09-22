using ProofOfVaccine.Token.JwsToken;
using ProofOfVaccine.Token.Model.Shc;
using ProofOfVaccine.Token.Support;

namespace ProofOfVaccine.Token.Validators
{
  public class SmartHealthCardHeaderValidator : IJwsHeaderValidator
  {   
    public Result Validate<T>(T Obj)
    {
      if (Obj is SmartHealthCareJWSHeaderModel SmartHealthCareJWSHeaderModel)
      {
        //Validate the JWS Header is a valid Smart Health Care JWS Header
        return SmartHealthCareJWSHeaderModel.Validate();
      }     
      else
      {
        throw new System.InvalidCastException($"{typeof(SmartHealthCardHeaderValidator)} can only validate instances of type {typeof(SmartHealthCareJWSHeaderModel).Name}.");
      }
    }

  }
}
