using System;
using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.Framework.Internal;

namespace Microsoft.AspNet.Builder
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
