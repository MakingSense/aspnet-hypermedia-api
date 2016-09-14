using System;
using System.Text;

namespace MakingSense.AspNetCore.Authentication.Abstractions
{
	public class AuthenticationException : Exception
	{
		public AuthenticationException(string message)
			: base(message)
		{

		}
	}
}
