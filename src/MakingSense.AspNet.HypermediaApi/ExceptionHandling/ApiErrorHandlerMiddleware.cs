using System;
using System.Threading.Tasks;
using MakingSense.AspNet.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace MakingSense.AspNet.HypermediaApi.ExceptionHandling
{
	public class ApiErrorHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;
		private readonly ObjectResultExecutor _objectResultExecutor;
		private readonly IContextProblemDetectionHandler _contextProblemDetectionHandler;
		private readonly IExceptionProblemDetectionHandler _exceptionProblemDetectionHandler;

		public ApiErrorHandlerMiddleware(
			RequestDelegate next,
			ILoggerFactory loggerFactory,
			ObjectResultExecutor objectResultExecutor)
		{
			_next = next;
			_logger = loggerFactory.CreateLogger<ApiErrorHandlerMiddleware>();
			_objectResultExecutor = objectResultExecutor;
			var defaultProblemDetectionHandler = new DefaultProblemDetectionHandler(loggerFactory.CreateLogger<DefaultProblemDetectionHandler>());
			_contextProblemDetectionHandler = defaultProblemDetectionHandler;
			_exceptionProblemDetectionHandler = defaultProblemDetectionHandler;
		}

		public ApiErrorHandlerMiddleware(
			RequestDelegate next,
			ILogger<ApiErrorHandlerMiddleware> logger,
			ObjectResultExecutor objectResultExecutor,
			IContextProblemDetectionHandler contextProblemDetectionHandler,
			IExceptionProblemDetectionHandler exceptionProblemDetectionHandler)
		{
			_next = next;
			_logger = logger;
			_objectResultExecutor = objectResultExecutor;
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

		private async Task ExecuteProblemResultAsync(HttpContext context, Problem problem)
		{
			var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
			var executor = context.RequestServices.GetService<ObjectResultExecutor>() ?? _objectResultExecutor;
			var result = new ProblemResult(problem);
			await result.ExecuteResultAsync(executor, actionContext);
		}
	}
}
