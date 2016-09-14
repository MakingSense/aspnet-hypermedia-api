using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.Utilities
{
	public static class MappingHelpers
	{
		public static bool? TrueOrNull(this bool value)
		{
			return value ? true : (bool?)null;
		}

		public static T? NullIfDefault<T>(this T value)
			where T : struct
		{
			return value.Equals(default(T)) ? null : new T?(value);
		}

		public static T? NullIfDefault<T>(this T? value)
			where T : struct
		{
			return value.GetValueOrDefault().Equals(default(T)) ? null : value;
		}

		public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
		{
			return enumerable == null ? Enumerable.Empty<T>() : enumerable;
		}
	}
}
