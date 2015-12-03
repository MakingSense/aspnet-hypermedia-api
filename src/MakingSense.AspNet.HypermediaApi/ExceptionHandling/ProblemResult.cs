using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MakingSense.AspNet.HypermediaApi.Linking;
using MakingSense.AspNet.HypermediaApi.Metadata;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.DependencyInjection;

namespace MakingSense.AspNet.HypermediaApi.ExceptionHandling
{
	public class ProblemResult : IActionResult
	{
		const string PROBLEM_MEDIATYPE = "application/problem+json";

		private readonly Problem _problem;

		public ProblemResult(Problem problem)
		{
			_problem = problem;
		}

		public async Task ExecuteResultAsync(ActionContext context)
		{
			_problem.InjectContext(context.HttpContext);

			//TODO: If request ACCEPT header accepts HTML and does not accept JSON and
			//path begins with `docs/`, render error as HTML
			//See DocumentationMiddleware TODO note

			// Only set the right content-type (application/problem+json) if it is accepted
			// otherwise return application/json + schema
			var acceptsProblemType = context.HttpContext.Request.Headers["Accept"].Contains(PROBLEM_MEDIATYPE);

			context.HttpContext.Response.OnStarting((o) =>
			{
				context.HttpContext.Response.ContentType = acceptsProblemType ? PROBLEM_MEDIATYPE
					: context.HttpContext.Response.ContentType + $"; profile={SchemaAttribute.Path}problem.json";
				return Task.FromResult(0);
			}, null);

			foreach (var pair in _problem.GetCustomHeaders())
			{
				context.HttpContext.Response.Headers[pair.Key] = pair.Value;
			}

			// Set problem status code
			context.HttpContext.Response.StatusCode = _problem.status;

			// Add a link to home page
			var linkHelper = context.HttpContext.RequestServices.GetService<ILinkHelper>();
			if (linkHelper != null)
			{
				// Only add self link in GET errors
				if ("GET".Equals(context.HttpContext.Request.Method, StringComparison.OrdinalIgnoreCase))
				{
					_problem._links.Add(linkHelper.ToSelf());
				}

				// TODO: Add better links for errors, and allow to customize them
				// For example, in non-GET errors add a self link to the related resource (GET relation)

				_problem._links.Add(linkHelper.ToHomeAccount());
			}

			// Execute wrapped result serialization
			await new ObjectResult(_problem).ExecuteResultAsync(context);
		}
	}
}
