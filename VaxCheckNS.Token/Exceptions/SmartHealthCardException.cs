using System;

namespace VaxCheckNS.Token.Exceptions
{
	public abstract class SmartHealthCardException : Exception
	{
		public SmartHealthCardException(string Message, string issuer = "")
			: base(Message)
		{
			IssuerURL = issuer;
		}

		public string IssuerURL { get; set; }
	}
}
