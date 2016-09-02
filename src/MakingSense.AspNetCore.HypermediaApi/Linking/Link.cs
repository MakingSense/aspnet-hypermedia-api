using MakingSense.AspNetCore.HypermediaApi.Linking.StandardRelations;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public sealed class Link
	{
		[Required]
		[Url]
		[JsonProperty(PropertyName = "href")]
		public string Href { get; set; }

		[JsonProperty(PropertyName = "description", NullValueHandling = NullValueHandling.Ignore)]
		public string Description { get; set; }

		[JsonProperty(PropertyName = "rel", Required = Required.Always)]
		[JsonConverter(typeof(RelationJsonConverter))]
		public ComposedRelation Relation { get; set; } = new ComposedRelation();

		public Link SetDescription(string description)
		{
			Description = description;
			return this;
		}

		public Link ClearDescription()
		{
			Description = null;
			return this;
		}

		public Link AddRel(params IRelation[] relations)
		{
			Relation.Add(relations);
			return this;
		}

		public Link AddRel<T>()
			where T : IRelation, new()
		{
			Relation.Add<T>();
			return this;
		}

		public Link SetSelf() =>
			AddRel<SelfRelation>();
	}
}
