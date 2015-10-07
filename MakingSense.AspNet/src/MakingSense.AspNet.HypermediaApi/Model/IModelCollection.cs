using System.Collections.Generic;

namespace MakingSense.AspNet.HypermediaApi.Model
{
	public interface IModelCollection
	{
	}

	public interface IModelCollection<T> : IModelCollection
	{
		List<T> items { get; }
	}
}
