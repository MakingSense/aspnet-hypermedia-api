using MakingSense.AspNetCore.HypermediaApi.Problems;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.ExceptionHandling
{
	/// <summary>
	/// This middleware capture all request an throws a NotFound exception.
	/// </summary>
	public class NotFoundHandlerMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly ILogger _logger;

		public NotFoundHandlerMiddleware(RequestDelegate next, ILogger<NotFoundHandlerMiddleware> logger)
		{
			_next = next;
			_logger = logger;
		}

		public Task Invoke(HttpContext context)
		{
			throw new ApiException(new RouteNotFoundProblem());
		}
	}
}
