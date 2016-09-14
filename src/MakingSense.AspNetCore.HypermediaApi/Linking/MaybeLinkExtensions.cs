using MakingSense.AspNetCore.Abstractions;
using MakingSense.AspNetCore.HypermediaApi.Linking.StandardRelations;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public static class MaybeLinkExtensions
	{
		public static Maybe<Link> SetDescription(this Maybe<Link> link, string description)
		{
			if (link.HasValue)
			{
				link.Value.SetDescription(description);
			}
			return link;
		}

		public static Maybe<Link> ClearDescription(this Maybe<Link> link)
		{
			if (link.HasValue)
			{
				link.Value.ClearDescription();
			}
			return link;
		}

		public static Maybe<Link> AddRel(this Maybe<Link> link, params IRelation[] relations)
		{
			if (link.HasValue)
			{
				link.Value.AddRel(relations);
			}
			return link;
		}

		public static Maybe<Link> AddRel<T>(this Maybe<Link> link)
			where T : IRelation, new()
		{
			if (link.HasValue)
			{
				link.Value.AddRel<T>();
			}
			return link;
		}

		public static Maybe<Link> SetSelf(this Maybe<Link> link)
		{
			if (link.HasValue)
			{
				link.Value.SetSelf();
			}
			return link;
		}
	}
}
