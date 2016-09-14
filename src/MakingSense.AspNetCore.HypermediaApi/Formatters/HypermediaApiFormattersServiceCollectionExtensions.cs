using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MakingSense.AspNetCore.HypermediaApi.Formatters.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class MvcJsonMvcCoreBuilderExtensions
	{
		public static IMvcBuilder SetHypermediaApiFormatters(this IMvcBuilder builder)
		{
			if (builder == null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			builder.Services.TryAddEnumerable(
				ServiceDescriptor.Transient<IConfigureOptions<MvcOptions>, HypermediaApiMvcOptionsSetup>());
			return builder;
		}
	}
}
