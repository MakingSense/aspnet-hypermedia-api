using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.Utilities
{
	public interface ICurrentTimeProvider
	{
		DateTimeOffset GetCurrent();
	}

	public class CurrentTimeProvider : ICurrentTimeProvider
	{
		public virtual DateTimeOffset GetCurrent() => DateTimeOffset.Now;
	}
}

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CurrentTimeProviderServiceCollectionExtensions
	{
		/// <summary>
		/// Register default instance of ICurrentTimeProvider to generate current DateTimeOffset values
		/// </summary>
		/// <param name="services"></param>
		/// <returns></returns>
		public static IServiceCollection AddCurrentTimeProvider([Framework.Internal.NotNull] this IServiceCollection services)
		{
			return services.AddSingleton<MakingSense.AspNetCore.HypermediaApi.Utilities.ICurrentTimeProvider>(x =>
				new MakingSense.AspNetCore.HypermediaApi.Utilities.CurrentTimeProvider());
		}
	}
}
