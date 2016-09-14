using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.AspNetCore.Http;

namespace MakingSense.AspNetCore.HypermediaApi.Problems
{
	public class NotImplementedProblem : Problem
	{
		readonly string _detail;

		public override string title => "Not implemented";
		public override string detail => _detail;
		public override int status => StatusCodes.Status501NotImplemented;
		public override int errorCode => 0;

		public NotImplementedProblem()
			: this("This functionality has not been implemented yet.")
		{

		}

		public NotImplementedProblem(string detail)
		{
			_detail = detail;
        }
	}
}
