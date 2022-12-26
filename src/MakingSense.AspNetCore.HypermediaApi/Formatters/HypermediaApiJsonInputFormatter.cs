using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using System;
using System.Buffers;

#if NET6_0_OR_GREATER
using MvcJsonOptions = Microsoft.AspNetCore.Mvc.MvcNewtonsoftJsonOptions;
using JsonInputFormatter = Microsoft.AspNetCore.Mvc.Formatters.NewtonsoftJsonInputFormatter;
#endif

namespace MakingSense.AspNetCore.HypermediaApi.Formatters
{
	// TODO: it is difficult to personalize it. Find an alternative.
	public class HypermediaApiJsonInputFormatter : JsonInputFormatter
	{
		public bool AcceptEmptyContentType { get; set; } = true;

		public bool AcceptAnyContentType { get; set; } = false;

		[Obsolete("This constructor is obsolete and will be removed in a future version.")]
		public HypermediaApiJsonInputFormatter(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider)
			: this(logger, serializerSettings, charPool, objectPoolProvider, null, null)
		{
		}

		public HypermediaApiJsonInputFormatter(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider, MvcOptions options, MvcJsonOptions jsonOptions)
#if NETFRAMEWORK
				: base(logger, serializerSettings, charPool, objectPoolProvider)
#else
				: base(logger, serializerSettings, charPool, objectPoolProvider, options, jsonOptions)
#endif
		{
			//TODO: add a setting to strict case sensitive de-serialization for properties

			SupportedMediaTypes.Clear();
			SupportedMediaTypes.Add("application/json");
			SupportedMediaTypes.Add("application/javascript");
			SupportedMediaTypes.Add("text/json");
			SupportedMediaTypes.Add("text/javascript");
			SupportedMediaTypes.Add("application/x-javascript");
			SupportedMediaTypes.Add("text/x-javascript");
			SupportedMediaTypes.Add("text/x-json");
		}

public override bool CanRead(InputFormatterContext context)
{
	var requestContentType = context.HttpContext.Request.ContentType;
	if (string.IsNullOrEmpty(requestContentType))
	{
		return AcceptEmptyContentType;
	}
	if (AcceptAnyContentType)
	{
		return true;
	}
	else
	{
		return base.CanRead(context);
	}
}
	}
}
