using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Formatters;

namespace MakingSense.AspNet.HypermediaApi.Formatters
{
	public class HypermediaApiJsonInputFormatter : JsonInputFormatter
	{
		public bool AcceptEmptyContentType { get; set; } = true;

		public bool AcceptAnyContentType { get; set; } = false;

		public List<string> AcceptedContentTypes { get; } = new List<string>() {
			"application/json",
			"application/javascript",
			"text/json",
			"text/javascript",
			"application/x-javascript",
			"text/x-javascript",
			"text/x-json"
		};

		public HypermediaApiJsonInputFormatter()
		{
			//TODO: add a setting to strict case sensitive de-serialization for properties
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
				// TODO: improve it based on ASP.NET code after RC2 see https://github.com/aspnet/Mvc/issues/3138
				return AcceptedContentTypes.Any(x => requestContentType.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0);
			}
		}
	}
}
