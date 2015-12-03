using System;
using MakingSense.AspNet.Abstractions;
using MakingSense.AspNet.Authentication.Abstractions;
using MakingSense.AspNet.HypermediaApi.Problems;
using Microsoft.AspNet.Http;
using Microsoft.Extensions.Logging;

namespace MakingSense.AspNet.HypermediaApi.ExceptionHandling
{
	public class DefaultProblemDetectionHandler : IContextProblemDetectionHandler, IExceptionProblemDetectionHandler
	{
		private readonly ILogger _logger;

		public DefaultProblemDetectionHandler(ILogger logger)
		{
			_logger = logger;
		}

		public Problem ExtractProblem(Exception exception)
		{
			var apiException = exception as ApiException;
			if (apiException != null)
			{
				return apiException.Problem;
			}

			var authException = exception as AuthenticationException;
			if (authException != null)
			{
				return new AuthenticationErrorProblem(exception.Message);
			}

			_logger.LogError("Unexpected error", exception);
			return new UnexpectedProblem(exception);
		}

		public Maybe<Problem> CheckForProblem(HttpContext context)
		{
			switch (context.Response.StatusCode)
			{
				case StatusCodes.Status401Unauthorized:
					return new UnauthorizedProblem();
				case StatusCodes.Status403Forbidden:
					return new ForbiddenProblem();
				case StatusCodes.Status404NotFound:
					return new RouteNotFoundProblem();
			}
			return Maybe.None<Problem>();
		}
	}
}
