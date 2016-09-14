using MakingSense.AspNetCore.Abstractions;
using System;

namespace MakingSense.AspNetCore.HypermediaApi.ExceptionHandling
{
	public interface IExceptionProblemDetectionHandler
	{
		Problem ExtractProblem(Exception exception);
	}
}
