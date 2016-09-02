namespace MakingSense.AspNetCore.HypermediaApi.Problems
{
	public class AuthenticationErrorProblem : AuthenticationProblem
	{
		private readonly string _detail;

		public override string title => "Authentication error";
		public override string detail => _detail;
		public override int errorCode => 2;

		public AuthenticationErrorProblem(string detail)
		{
			_detail = detail;
		}
	}
}
