using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc.Formatters;

namespace MakingSense.AspNet.HypermediaApi.Formatters
{
	public class HypermediaApiJsonInputFormatter : JsonInputFormatter
	{
		public HypermediaApiJsonInputFormatter()
		{
			//TODO: add a setting to strict case sensitive de-serialization for properties
		}

		public override bool CanRead(InputFormatterContext context)
		{
			return true;
		}
	}
}
