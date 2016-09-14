using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.AspNetCore.Http;

namespace MakingSense.AspNetCore.HypermediaApi.Problems
{
	public abstract class AuthenticationProblem : Problem
	{
		public override int status => StatusCodes.Status401Unauthorized;

		public AuthenticationProblem()
		{
			AddHeader("WWW-Authenticate", "Bearer");
		}
	}
}
