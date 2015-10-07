using System;
using System.Text;

namespace MakingSense.AspNet.Authentication.Abstractions
{
	public class AuthenticationException : Exception
	{
		public AuthenticationException(string message)
			: base(message)
		{

		}
	}
}
