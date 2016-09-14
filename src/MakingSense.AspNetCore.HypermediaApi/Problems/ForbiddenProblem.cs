using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.AspNetCore.Http;

namespace MakingSense.AspNetCore.HypermediaApi.Problems
{
	public class ForbiddenProblem : Problem
	{
		private string _detail = "Authentication is done, but you do not have enough privileges";

		public override int status => StatusCodes.Status403Forbidden;
		public override string title => "Forbidden, not enough privileges";
		public override string detail => _detail;
		public override int errorCode => 0;

		public ForbiddenProblem()
		{
		}

		public ForbiddenProblem(string detail)
		{
			_detail = detail;
		}
	}
}
