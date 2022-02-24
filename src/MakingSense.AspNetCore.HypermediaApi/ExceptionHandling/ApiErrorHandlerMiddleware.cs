using System;
using System.Threading.Tasks;
using MakingSense.AspNetCore.Abstractions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Net.Http.Headers;
using System.Linq;
using MakingSense.AspNetCore.HypermediaApi.Metadata;
using MakingSense.AspNetCore.HypermediaApi.Linking;
using Microsoft.AspNetCore.Mvc.Infrastructure;
#if NETFRAMEWORK
using Microsoft.AspNetCore.Mvc.Internal;
#endif

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

			if (context.Response.HasStarted)
			{
				// Response has started. We cannot write information, see https://github.com/aspnet/Diagnostics/issues/401
				// So, logging problem...
				_logger.LogError(
					"Error cannot be rendered because response already started. ErrorInformation: {0}",
					SerializeProblemInformation(problem, context));
			}
			else
			{
				try
				{
					WriteResponseHeaders(problem, context.Request, context.Response);

					// TODO: If request ACCEPT header accepts HTML and does not accept JSON and
					// path begins with `docs/`, render error as HTML
					// See DocumentationMiddleware TODO note

					var objectResultExecutor = context.RequestServices.GetService<ObjectResultExecutor>()
						?? _objectResultExecutor;

					await WriteResponseBodyAsync(context, problem, objectResultExecutor);

				}
				catch (Exception e)
				{
					_logger.LogError(
						new EventId(),
						e,
						"Error rendering problem. ErrorInformation: {0}",
						SerializeProblemInformation(problem, context));
				}
			}
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

		private static string SerializeProblemInformation(Problem problem, HttpContext context) =>
			Newtonsoft.Json.JsonConvert.SerializeObject(
				new
				{
					RequestPath = context.Request.Path,
					RequestQuery = context.Request.QueryString,
					ResponseContentLength = context.Response.ContentLength,
					ResponseStatusCode = context.Response.StatusCode,
					Problem = problem
				},
				Newtonsoft.Json.Formatting.Indented,
				new Newtonsoft.Json.JsonSerializerSettings()
				{
					ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
				});

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

			var keepUnquoted = request.Query.ContainsKey("keep-unquoted-profile");

			response.OnStarting((o) =>
			{
				response.ContentType = acceptsProblemType ? PROBLEM_MEDIATYPE
					: response.ContentType + CreateProfileContentType(keepUnquoted);
				return Task.FromResult(0);
			}, null);

			foreach (var pair in problem.GetCustomHeaders())
			{
				response.Headers[pair.Key] = pair.Value;
			}

			// Set problem status code
			response.StatusCode = problem.status;
		}

		private static String CreateProfileContentType(bool keepUnquoted = false)
		{
			var profile = keepUnquoted ? SchemaAttribute.Path + "problem.json"
				: $"\"{SchemaAttribute.Path?.Replace("\"", "\\\"")}" + "problem.json\"";
			return $"; profile={profile}";
		}
	}
}
