using MakingSense.AspNet.HypermediaApi.ExceptionHandling;
using Microsoft.AspNet.Http;

namespace MakingSense.AspNet.HypermediaApi.Problems
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
