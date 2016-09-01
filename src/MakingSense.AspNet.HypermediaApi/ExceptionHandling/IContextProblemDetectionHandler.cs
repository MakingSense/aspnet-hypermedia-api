using MakingSense.AspNet.Abstractions;
using Microsoft.AspNetCore.Http;
using System;

namespace MakingSense.AspNet.HypermediaApi.ExceptionHandling
{
	public interface IContextProblemDetectionHandler
	{
		Maybe<Problem> CheckForProblem(HttpContext context);
	}
}
