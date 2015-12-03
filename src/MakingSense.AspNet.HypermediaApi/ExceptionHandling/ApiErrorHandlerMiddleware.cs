using System;
using System.Threading.Tasks;
using MakingSense.AspNet.Abstractions;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.Extensions.Logging;

namespace MakingSense.AspNet.HypermediaApi.ExceptionHandling
{
	public class ApiErrorHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;
		private readonly IContextProblemDetectionHandler _contextProblemDetectionHandler;
		private readonly IExceptionProblemDetectionHandler _exceptionProblemDetectionHandler;

		public ApiErrorHandlerMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
		{
			_next = next;
			_logger = loggerFactory.CreateLogger<ApiErrorHandlerMiddleware>();
			var defaultProblemDetectionHandler = new DefaultProblemDetectionHandler(loggerFactory.CreateLogger<DefaultProblemDetectionHandler>());
            _contextProblemDetectionHandler = defaultProblemDetectionHandler;
			_exceptionProblemDetectionHandler = defaultProblemDetectionHandler;
		}

		public ApiErrorHandlerMiddleware(RequestDelegate next, ILogger<ApiErrorHandlerMiddleware> logger, IContextProblemDetectionHandler contextProblemDetectionHandler, IExceptionProblemDetectionHandler exceptionProblemDetectionHandler)
		{
			_next = next;
			_logger = logger;
			_contextProblemDetectionHandler = contextProblemDetectionHandler;
			_exceptionProblemDetectionHandler = exceptionProblemDetectionHandler;
		}

		public async Task Invoke(HttpContext context)
		{
			Maybe<Problem> problem;
			try
			{
				await _next(context);
				problem = _contextProblemDetectionHandler.CheckForProblem(context);
			}
			catch (Exception exception)
			{
				problem = _exceptionProblemDetectionHandler.ExtractProblem(exception);
			}

			if (problem.HasValue)
			{
				await ExecuteProblemResultAsync(context, problem.Value);
			}
		}

		private static async Task ExecuteProblemResultAsync(HttpContext context, Problem problem)
		{
			var result = new ProblemResult(problem);
			var actionContext = new ActionContext(context, null, null);
			await result.ExecuteResultAsync(actionContext);
		}

	}
}
