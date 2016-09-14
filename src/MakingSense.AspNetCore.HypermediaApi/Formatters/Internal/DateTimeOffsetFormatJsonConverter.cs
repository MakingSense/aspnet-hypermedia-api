using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MakingSense.AspNetCore.HypermediaApi.Utilities;
using Newtonsoft.Json;

namespace MakingSense.AspNetCore.HypermediaApi.Formatters.Internal
{
	public class DateTimeOffsetFormatJsonConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var input = (string)reader.Value;
			if (!string.IsNullOrEmpty(input))
			{
				DateTimeOffset result;
				if (DateTimeHelper.TryParseAbsoluteDateTime(input, out result))
				{
					return result;
				}
				throw new Exception($"Could not convert string to DateTimeOffset: {input}. Path '{reader.Path}'.");
			}
			return null;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			// We asume that, at this point, value cannot be null
			var formattedValue = ((DateTimeOffset)value).ToApiAbsoluteDateFormat();
			writer.WriteValue(formattedValue);
		}
	}
}
