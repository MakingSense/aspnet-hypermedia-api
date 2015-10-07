using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNet.Utilities
{
	public static class MappingHelpers
	{
		public static bool? TrueOrNull(this bool value)
		{
			return value ? true : (bool?)null;
		}
	}
}
