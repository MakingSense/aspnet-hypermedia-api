using MakingSense.AspNet.Abstractions;
using Microsoft.AspNet.Http;
using System;

namespace MakingSense.AspNet.HypermediaApi.ExceptionHandling
{
	public interface IContextProblemDetectionHandler
	{
		Maybe<Problem> CheckForProblem(HttpContext context);
	}
}
