using MakingSense.AspNetCore.Abstractions;
using Microsoft.AspNetCore.Http;
using System;

namespace MakingSense.AspNetCore.HypermediaApi.ExceptionHandling
{
	public interface IContextProblemDetectionHandler
	{
		Maybe<Problem> CheckForProblem(HttpContext context);
	}
}
