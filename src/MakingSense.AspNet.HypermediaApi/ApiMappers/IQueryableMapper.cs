using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNet.HypermediaApi.ApiMappers
{
	public interface IQueryableMapper<TIn, TOut>
	{
		IEnumerable<TOut> Map(IQueryable<TIn> queriable);
	}
}
