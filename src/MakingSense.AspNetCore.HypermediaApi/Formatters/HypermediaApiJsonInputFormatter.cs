using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;

namespace MakingSense.AspNetCore.HypermediaApi.Formatters
{
	public class HypermediaApiJsonInputFormatter : JsonInputFormatter
	{
		public bool AcceptEmptyContentType { get; set; } = true;

		public bool AcceptAnyContentType { get; set; } = false;

		public HypermediaApiJsonInputFormatter(ILogger logger, JsonSerializerSettings serializerSettings, ArrayPool<char> charPool, ObjectPoolProvider objectPoolProvider)
			: base(logger, serializerSettings, charPool, objectPoolProvider)
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
				// TODO: verify it
				//// TODO: improve it based on ASP.NET code after RC2 see https://github.com/aspnet/Mvc/issues/3138
				//// return AcceptedContentTypes.Any(x => requestContentType.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
				return base.CanRead(context);
			}
		}
	}
}
