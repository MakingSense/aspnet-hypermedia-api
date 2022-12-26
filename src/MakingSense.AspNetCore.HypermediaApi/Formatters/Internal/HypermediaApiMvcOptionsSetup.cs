using System;
using System.Buffers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

#if NET6_0_OR_GREATER
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions;
#endif

namespace MakingSense.AspNetCore.HypermediaApi.Formatters.Internal
{
	public class HypermediaApiMvcOptionsSetup : ConfigureOptions<MvcOptions>
	{
		public HypermediaApiMvcOptionsSetup(
			ILoggerFactory loggerFactory,
			IOptions<MvcJsonOptions> jsonOptions,
			ArrayPool<char> charPool,
			ObjectPoolProvider objectPoolProvider)
			: base((options) => ConfigureMvc(
				options,
				jsonOptions.Value.SerializerSettings,
				loggerFactory,
				charPool,
				objectPoolProvider,
				jsonOptions.Value))
		{
		}

		public static void ConfigureMvc(
			MvcOptions options,
			JsonSerializerSettings serializerSettings,
			ILoggerFactory loggerFactory,
			ArrayPool<char> charPool,
			ObjectPoolProvider objectPoolProvider,
			MvcJsonOptions jsonOptions = null)
		{
			serializerSettings.Formatting = Formatting.Indented;

			serializerSettings.Converters.Add(new DateTimeOffsetFormatJsonConverter());
			serializerSettings.DateParseHandling = DateParseHandling.None;

			options.OutputFormatters.Clear();

			var jsonOutputFormatter =
#if NET6_0_OR_GREATER
				new NewtonsoftJsonOutputFormatter(serializerSettings, charPool, options, jsonOptions);
#elif NETSTANDARD2_0 || NETFRAMEWORK
				new JsonOutputFormatter(serializerSettings, charPool);
#else
#error unknown target framework
#endif

			options.OutputFormatters.Add(jsonOutputFormatter);

			options.InputFormatters.Clear();
			var jsonInputLogger = loggerFactory.CreateLogger<HypermediaApiJsonInputFormatter>();
			options.InputFormatters.Add(new HypermediaApiJsonInputFormatter(
				jsonInputLogger,
				serializerSettings,
				charPool,
				objectPoolProvider,
				options,
				jsonOptions));
		}
	}
}
