using System;
using System.Threading.Tasks;
using MakingSense.AspNetCore.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Linq;
using MakingSense.AspNetCore.HypermediaApi.Metadata;
using MakingSense.AspNetCore.HypermediaApi.Linking;

namespace MakingSense.AspNetCore.HypermediaApi.ExceptionHandling
{
	public class ApiErrorHandlerMiddleware
	{
		const string PROBLEM_MEDIATYPE = "application/problem+json";
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
				await DumpProblemAsync(context, problem.Value);
			}
		}

		private async Task DumpProblemAsync(HttpContext context, Problem problem)
		{
			PrepareProblem(context, problem);

			WriteResponseHeaders(problem, context.Request, context.Response);

			// TODO: If request ACCEPT header accepts HTML and does not accept JSON and
			// path begins with `docs/`, render error as HTML
			// See DocumentationMiddleware TODO note

			var objectResultExecutor = context.RequestServices.GetService<ObjectResultExecutor>()
				?? _objectResultExecutor;

			await WriteResponseBodyAsync(context, problem, objectResultExecutor);
		}

		private static void PrepareProblem(HttpContext context, Problem problem)
		{
			var linkHelper = context.RequestServices.GetService<ILinkHelper>();

			problem.InjectContext(context);

			if (linkHelper != null)
			{
				// Only add self link in GET errors
				if ("GET".Equals(context.Request.Method, StringComparison.OrdinalIgnoreCase))
				{
					problem._links.Add(linkHelper.ToSelf());
				}

				// TODO: Add better links for errors, and allow to customize them
				// For example, in non-GET errors add a self link to the related resource (GET relation)
				problem._links.Add(linkHelper.ToHomeAccount());
			}
		}

		private static async Task WriteResponseBodyAsync(HttpContext context, Problem problem, ObjectResultExecutor objectResultExecutor)
		{
			var actionContext = new ActionContext(context, new RouteData(), new ActionDescriptor());
			var objectResult = new ObjectResult(problem);
			await objectResultExecutor.ExecuteAsync(actionContext, objectResult);
		}

		private static void WriteResponseHeaders(Problem problem, HttpRequest request, HttpResponse response)
		{
			// Only set the right content-type (application/problem+json) if it is accepted
			// otherwise return application/json + schema
			var acceptsProblemType = request.Headers[HeaderNames.Accept].Contains(PROBLEM_MEDIATYPE);

			response.OnStarting((o) =>
			{
				response.ContentType = acceptsProblemType ? PROBLEM_MEDIATYPE
					: response.ContentType + $"; profile={SchemaAttribute.Path}problem.json";
				return Task.FromResult(0);
			}, null);


			foreach (var pair in problem.GetCustomHeaders())
			{
				response.Headers[pair.Key] = pair.Value;
			}

			// Set problem status code
			response.StatusCode = problem.status;
		}
	}
}
