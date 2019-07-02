using System;
using System.Linq;
using System.Reflection;
using MakingSense.AspNetCore.HypermediaApi.SuitableValidators;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Framework.Internal;

namespace Microsoft.Extensions.DependencyInjection
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

		/// <summary>
		/// Register existent suitable validators in caller assembly to be available during link generation and before execute actions.
		/// </summary>
		public static IServiceCollection AddSuitableValidators([NotNull] this IServiceCollection services)
		{
			return services.AddSuitableValidators(Assembly.GetCallingAssembly());
		}
	}
}
