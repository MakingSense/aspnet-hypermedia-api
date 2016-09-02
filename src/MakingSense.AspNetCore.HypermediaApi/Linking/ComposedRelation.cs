using System;
using System.Collections.Generic;
using System.Linq;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public class ComposedRelation : IRelation
	{
		private List<IRelation> _innerRelations;
		private HashSet<string> _innerRelationNames;

		private static IEnumerable<string> GetNames(IRelation relation)
		{
			return relation.RelationName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		}

		private bool ContainsAllNames(IEnumerable<string> names)
		{
			return names.All(x => _innerRelationNames.Contains(x));
		}

		public bool Contains(IRelation relation)
		{
			return ContainsAllNames(GetNames(relation));
		}

		public bool Contains<T>()
			where T : IRelation, new()
		{
			return Contains(new T());
		}

		public ComposedRelation(params IRelation[] innerRelations)
		{
			_innerRelations = innerRelations.ToList();
			_innerRelationNames = new HashSet<string>(innerRelations.SelectMany(x => GetNames(x)));
		}

		public ComposedRelation Add(IRelation relation)
		{
			var names = GetNames(relation);
			if (!ContainsAllNames(names))
			{
				_innerRelations.Add(relation);
				foreach (var name in names)
				{
					_innerRelationNames.Add(name);
				}
			}
			return this;
		}

		public ComposedRelation Add<T>()
			where T : IRelation, new()
		{
			Add(new T());
			return this;
		}

		public ComposedRelation Add(params IRelation[] relations)
		{
			foreach (var rel in relations)
			{
				Add(rel);
			}
			return this;
		}

		private T SingleNotNull<T>(Func<IRelation, T> selector) =>
			_innerRelations.Select(selector).Where(x => x != null).Distinct().SingleOrDefault();

		private bool? AllTrues(Func<IRelation, bool?> selector)
		{
			var values = _innerRelations.Select(selector).Where(x => x != null).Distinct().ToArray();
			if (values.Length == 0)
			{
				return null;
			}
			else if (values.Length > 1)
			{
				return false;
			}
			else
			{
				return values[0];
			}
		}

		public Type InputModel => SingleNotNull(x => x.InputModel);

		public Type OutputModel => SingleNotNull(x => x.InputModel);

		public HttpMethod? Method => SingleNotNull(x => x.Method);

		public string RelationName => string.Join(" ", _innerRelations.Where(x => !x.IsVirtual).SelectMany(x => x.RelationName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)).Distinct().OrderBy(x => x));

		public bool IsVirtual => false;
	}
}
