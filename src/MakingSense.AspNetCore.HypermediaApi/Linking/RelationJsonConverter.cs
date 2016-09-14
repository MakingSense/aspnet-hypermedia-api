using Newtonsoft.Json;
using System;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public class RelationJsonConverter : JsonConverter
	{
		private class DummyRelation : IRelation
		{
			public Type InputModel => null;
			public bool IsVirtual => false;
			public HttpMethod? Method => null;
			public Type OutputModel => null;
			public string RelationName { get; set; }
		}

		public override bool CanConvert(Type objectType)
		{
			return objectType == typeof(ComposedRelation)
				|| objectType == typeof(IRelation);
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			// TODO: parse relation correctly
			// It is a workaround to allow Relation deserialization in tests,
			// we do not need to deserialize relations for anything else.
			var relationWithName = new DummyRelation()
			{
				RelationName = (string)reader.Value
			};

			if (objectType == typeof(IRelation))
			{
				return relationWithName;
			}
			else
			{
				var composedRelation = existingValue as ComposedRelation ?? new ComposedRelation();
				composedRelation.Add(relationWithName);
				return composedRelation;
			}
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			writer.WriteValue(((IRelation)value).RelationName);
		}
	}
}
