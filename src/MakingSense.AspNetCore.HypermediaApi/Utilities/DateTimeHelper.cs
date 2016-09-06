using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.Utilities
{
	public static class DateTimeHelper
	{
		public static readonly string PreferedUtcFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFZ";
		public static readonly string PreferedNonUtcFormat = "yyyy-MM-ddTHH:mm:ss.FFFFFFFzzz";
		public static readonly string[] IsoAbsoluteDateFormats =
			(
				from baseFormat in new[]
				{
					"yyyy-MM-ddTHH:mm:ss.FFFFFFF", // Up to seven digits of the seconds fraction (trailing zeros are not displayed)
					"yyyy-MM-ddTHH:mm:ss",
					"yyyy-MM-ddTHH:mm",
					"yyyy-MM-dd"
				}
				from baseFormatWithOffset in new[]
				{
					baseFormat + "Z", // Literal Z
					baseFormat + "zzz", // Offset as two digit hour with leading zero and minutes with digit hour with leading zero (for example -07:00 or +03:00)
					baseFormat + "zz", // Offset as two digit hour with leading zero (for example -07 or +03)
					baseFormat + "z" // Offset as hour without leading zero (for example -7 or +3)
				}
				from finalFormat in new[]
				{
					baseFormatWithOffset, // Simple base format with offset information
					// These values are not accepted by MVC parser:
					// baseFormatWithOffset.Replace("-", "").Replace(":", ""), // Short form without `-` nor `:`
					baseFormatWithOffset.Replace("T", " ") // Friendly format with space in place of `T`
				}
				select finalFormat
			).Union(new[] // Other custom formats
			{
				"R" // The RFC1123 Format Specifier (ddd, dd MMM yyyy HH':'mm':'ss 'GMT') https://msdn.microsoft.com/en-us/library/az4se3k1(v=vs.110).aspx#RFC1123
			})
			.ToArray();

		public static bool IsValidAbsoluteDateTimeFormat(string input)
		{
			DateTimeOffset discardMe;
			return TryParseAbsoluteDateTime(input, out discardMe);
		}

		public static bool TryParseAbsoluteDateTime(string input, out DateTimeOffset result)
		{
			return DateTimeOffset.TryParseExact(input, IsoAbsoluteDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result);
		}

		public static string ToApiAbsoluteDateFormat(this DateTimeOffset value)
		{
			return value.Offset == TimeSpan.Zero
				? value.ToString(PreferedUtcFormat, CultureInfo.InvariantCulture)
				: value.ToString(PreferedNonUtcFormat, CultureInfo.InvariantCulture);
		}

		public static string ToApiAbsoluteDateFormat(this DateTimeOffset? value)
		{
			return value.HasValue ? ToApiAbsoluteDateFormat(value.Value) : null;
		}
	}
}
