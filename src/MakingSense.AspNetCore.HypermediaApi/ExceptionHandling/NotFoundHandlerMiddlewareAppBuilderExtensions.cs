using System;
using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNetCore.Builder
{
	public static class NotFoundHandlerMiddlewareAppBuilderExtensions
	{
		/// <summary>
		/// This middleware capture all request an throws a NotFound exception.
		/// </summary>
		public static IApplicationBuilder UseNotFoundHandler([NotNull] this IApplicationBuilder app)
		{
			return app.UseMiddleware<NotFoundHandlerMiddleware>();
		}
	}
}
