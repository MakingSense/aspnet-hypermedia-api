using MakingSense.AspNet.Abstractions;
using System;

namespace MakingSense.AspNet.HypermediaApi.ExceptionHandling
{
	public interface IExceptionProblemDetectionHandler
	{
		Problem ExtractProblem(Exception exception);
	}
}
