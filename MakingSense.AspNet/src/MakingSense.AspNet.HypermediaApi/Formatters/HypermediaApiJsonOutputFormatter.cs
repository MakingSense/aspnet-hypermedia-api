using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
