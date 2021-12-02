namespace VaxCheckNS.Token.Exceptions
{
	public class SmartHealthCardDecoderException : SmartHealthCardException
	{
		public SmartHealthCardDecoderException(string Message, string issuer = "")
		  : base(Message, issuer)
		{ }
	}
}
