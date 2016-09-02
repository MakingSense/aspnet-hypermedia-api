using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.Abstractions
{
	public static class Maybe
	{
		public static Maybe<T> From<T>(T value) => new Maybe<T>(value);
		public static Maybe<T> None<T>() => default(Maybe<T>);
	}

	public struct Maybe<T> : IEnumerable<T>
	{
		readonly bool _hasValue;
		public bool HasValue => _hasValue;

		readonly T _value;
		public T Value
		{
			get
			{
				if (!_hasValue)
				{
					throw new InvalidOperationException();
				}
				return _value;
			}
		}

		internal Maybe(T value)
		{
			_hasValue = !ReferenceEquals(value, null);
			_value = value;
		}

		public IEnumerator<T> GetEnumerator()
		{
			if (_hasValue)
			{
				yield return _value;
			}
			yield break;
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public static implicit operator Maybe<T>(T value) => new Maybe<T>(value);
	}
}
