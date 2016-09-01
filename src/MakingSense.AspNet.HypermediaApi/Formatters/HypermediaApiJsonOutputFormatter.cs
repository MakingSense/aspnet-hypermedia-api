using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Newtonsoft.Json;

namespace MakingSense.AspNet.HypermediaApi.Formatters
{
	public class HypermediaApiJsonOutputFormatter : JsonOutputFormatter
	{
		public HypermediaApiJsonOutputFormatter(JsonSerializerSettings serializerSettings, ArrayPool<char> charPool)
			: base(serializerSettings, charPool)
		{
			SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
		}
	}
}
