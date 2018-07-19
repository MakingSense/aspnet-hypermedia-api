using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using System;

namespace MakingSense.AspNetCore.HypermediaApi.Problems
{
	public class UnexpectedProblem : Problem
	{
		public override string title => "Unexpected error";
		public override int status => StatusCodes.Status500InternalServerError;
		public override int errorCode => 0;
		public override string detail { get; }

		public UnexpectedProblem(Exception exception)
		{
			detail = $"An unexpected error was thrown\r\n\tException Message: {exception.Message} \r\n\tException Type: {exception.GetType()}";
		}
	}
}
