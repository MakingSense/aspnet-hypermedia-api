using System;
using Microsoft.Framework.Internal;
using System.Linq;
using System.Reflection;
using MakingSense.AspNet.HypermediaApi.ApiMappers;
using MakingSense.AspNet.HypermediaApi.Linking;

namespace Microsoft.Framework.DependencyInjection
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
