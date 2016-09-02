using System;
using System.Linq;
using MakingSense.AspNetCore.HypermediaApi.Linking;
using Microsoft.Framework.Internal;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class LinkingServiceCollectionExtensions
	{
		/// <summary>
		/// Register a Link Helper class
		/// </summary>
		/// <remarks>
		/// Consider to create a concrete ILinkHelper class based on BaseLinkHelper.
		/// </remarks>
		public static IServiceCollection AddLinkHelper<T>([NotNull] this IServiceCollection services)
			where T : class, ILinkHelper
		{
			return services.AddScoped<ILinkHelper, T>();
		}
	}
}
