using MakingSense.AspNet.HypermediaApi.ExceptionHandling;
using Microsoft.AspNet.Http;

namespace MakingSense.AspNet.HypermediaApi.Problems
{
	public class ForbiddenProblem : Problem
	{
		public override int status => StatusCodes.Status403Forbidden;
		public override string title => "Forbidden, not enough privileges";
		public override string detail => "Authentication is done, but you do not have enough privileges";
		public override int errorCode => 0;
	}
}
