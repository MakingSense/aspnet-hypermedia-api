using MakingSense.AspNet.HypermediaApi.ExceptionHandling;
using Microsoft.AspNet.Http;

namespace MakingSense.AspNet.HypermediaApi.Problems
{
	public abstract class AuthenticationProblem : Problem
	{
		public override int status => StatusCodes.Status401Unauthorized;

		public AuthenticationProblem()
		{
			this.AddHeader("WWW-Authenticate", "Bearer");
		}
	}
}
