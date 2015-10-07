using System;
using Microsoft.Framework.Internal;
using MakingSense.AspNet.HypermediaApi.ExceptionHandling;

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
