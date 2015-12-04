using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Formatters;

namespace MakingSense.AspNet.HypermediaApi.Formatters
{
	public class HypermediaApiJsonOutputFormatter : JsonOutputFormatter
	{
		public HypermediaApiJsonOutputFormatter()
		{
			SerializerSettings.Formatting = Newtonsoft.Json.Formatting.Indented;
		}
	}
}
