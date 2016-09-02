using System.Collections.Generic;

namespace MakingSense.AspNetCore.HypermediaApi.Model
{
	public interface IModelCollection
	{
	}

	public interface IModelCollection<T> : IModelCollection
	{
		List<T> items { get; }
	}
}
