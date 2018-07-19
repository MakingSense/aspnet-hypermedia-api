using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;

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
			if (exception is AggregateException aggregateException)
			{
				detail = "An unexpected error was thrown"
					+ string.Join("", aggregateException.Flatten().InnerExceptions.Select(RenderException));
			}
			else
			{
				detail = "An unexpected error was thrown" + RenderException(exception);
			}
		}

		private string RenderException(Exception exception) =>
			$"\r\n\tException Message: {exception.Message} \r\n\tException Type: {exception.GetType()}";
	}
}
