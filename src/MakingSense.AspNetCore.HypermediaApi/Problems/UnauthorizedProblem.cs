namespace MakingSense.AspNetCore.HypermediaApi.Problems
{
	public class UnauthorizedProblem : AuthenticationProblem
	{
		public override string title => "Unauthorized";
		public override string detail => "Authentication is required but no authentication information was sent";
		public override int errorCode => 0;
	}
}
