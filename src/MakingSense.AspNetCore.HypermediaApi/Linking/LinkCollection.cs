using MakingSense.AspNetCore.Abstractions;
using Microsoft.Framework.Internal;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public class LinkCollection : ICollection<Link>
	{
		private List<Link> _innerList = new List<Link>();
		public int Count => _innerList.Count;
		public bool IsReadOnly => false;

		public LinkCollection()
		{

		}

		public LinkCollection([NotNull] IEnumerable<Link> links)
		{
			AddRange(links);
		}

		public LinkCollection(params Link[] links)
			: this((IEnumerable<Link>)links)
		{

		}

		public LinkCollection([NotNull] IEnumerable<Maybe<Link>> links)
		{
			AddRange(links);
		}

		public LinkCollection(params Maybe<Link>[] links)
			: this((IEnumerable<Maybe<Link>>)links)
		{
		}

		public LinkCollection AddRange([NotNull] IEnumerable<Link> links)
		{
			foreach (var link in links)
			{
				Add(link);
			}
			return this;
		}

		public LinkCollection AddRange([NotNull] IEnumerable<Maybe<Link>> links)
		{
			foreach (var link in links)
			{
				Add(link);
			}
			return this;
		}

		public LinkCollection Add([NotNull] Link item)
		{
			if (!Contains(item))
			{
				_innerList.Add(item);
			}
			return this;
		}

		public LinkCollection Add(Maybe<Link> item)
		{
			if (item.HasValue)
			{
				Add(item.Value);
			}
			return this;
		}

		void ICollection<Link>.Add([NotNull] Link item)
		{
			Add(item);
		}

		public LinkCollection Clear()
		{
			_innerList.Clear();
			return this;
		}

		void ICollection<Link>.Clear()
		{
			Clear();
		}

		public bool Contains([NotNull] Link item)
		{
			var sameHrefRelations = _innerList.Where(x => x.Href == item.Href).Select(x => x.Relation);
			return sameHrefRelations.Any(x => x.Contains(item.Relation));
		}

		void ICollection<Link>.CopyTo(Link[] array, int arrayIndex)
		{
			_innerList.CopyTo(array, arrayIndex);
		}

		public IEnumerator<Link> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		bool ICollection<Link>.Remove([NotNull] Link item)
		{
			return _innerList.Remove(item);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return ((IEnumerable)_innerList).GetEnumerator();
		}
	}
}
