using System;
using Microsoft.Framework.Internal;
using System.Linq;
using MakingSense.AspNet.HypermediaApi.SuitableValidators;
using System.Reflection;
using Microsoft.AspNet.Mvc;

namespace Microsoft.Framework.DependencyInjection
{
	public static class DefaultSuitableValidatorsServiceCollectionExtensions
	{
		/// <summary>
		/// Register existent suitable validators in specified assembly to be available during link generation and before execute actions.
		/// </summary>
		public static IServiceCollection AddSuitableValidators([NotNull] this IServiceCollection services, Assembly mappersAssembly)
		{
			services.Configure<MvcOptions>(options =>
			{
				options.Filters.Add(new SuitableValidationFilter());
			});

			var serviceTypes = mappersAssembly.GetTypes()
				.Select(x => new
				{
					type = x,
					info = x.GetTypeInfo()
				})
				.Where(x => !x.info.IsAbstract
					&& !x.info.IsInterface
					&& x.info.IsPublic
					&& x.info.Namespace != null
					&& x.info.ImplementedInterfaces.Contains(typeof(ISuitableValidator)))
				.Select(x => x.type);

			foreach (var type in serviceTypes)
			{
				services.AddScoped(type);
			}

			return services;
		}

#if DNX451
		/// <summary>
		/// Register existent suitable validators in caller assembly to be available during link generation and before execute actions.
		/// </summary>
		public static IServiceCollection AddSuitableValidators([NotNull] this IServiceCollection services)
		{
			return services.AddSuitableValidators(Assembly.GetCallingAssembly());
		}
#endif

	}
}
