using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
