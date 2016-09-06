using System;
using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNetCore.Builder
{
	public static class ApiErrorHandlerAppBuilderExtensions
	{
		/// <summary>
		/// Adds a middleware render all exceptions as Problem Details (See https://tools.ietf.org/html/draft-ietf-appsawg-http-problem-01)
		/// </summary>
		/// <remarks>
		/// It catches:
		/// * Any unhandled exception and render them as UnexpectedProblem
		/// * Any AuthenticationException and render them as AuthenticationErrorProblem
		/// * Any ApiException and render the inner problem
		/// * Response.StatusCode == 401 (Unauthorized) and render an UnauthorizedProblem
		/// * Response.StatusCode == 403 (Forbidden) and render a ForbiddenProblem
		/// * Response.StatusCode == 404 (Not Found) and render a RouteNotFoundProblem
		/// </remarks>
		public static IApplicationBuilder UseHypermediaApiErrorHandler([NotNull] this IApplicationBuilder app)
		{
			return app.UseMiddleware<ApiErrorHandlerMiddleware>();
		}
	}
}
