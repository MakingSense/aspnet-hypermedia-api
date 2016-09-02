using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.ApiMappers
{
	/// <summary>
	/// It helps to create a IQueryable friendly mapper.
	/// </summary>
	/// <remarks>
	/// TProjection cannot be the same type than TOut
	/// </remarks>
	public abstract class BaseQueryableMapper<TIn, TOut, TProjection> : IApiMapper<TIn, TOut>, IQueryableMapper<TIn, TOut>
		where TOut : new()
	{
		public void Fill(TIn input, TOut output) =>
			Fill(MapToProjection(Enumerable.Repeat(input, 1).AsQueryable()).First(), output);

		public virtual IEnumerable<TOut> Map(IQueryable<TIn> queriable) =>
			MapToProjection(queriable)
			.Select(x =>
			{
				var output = new TOut();
				Fill(x, output);
				return output;
			});

		protected abstract void Fill(TProjection input, TOut output);

		/// <summary>
		/// Maps queriable results to an enumerable of an intermediate representation (projection)
		/// </summary>
		/// <remarks>
		/// Take into account that queriable results could contains null values
		/// </remarks>
		protected abstract IEnumerable<TProjection> MapToProjection(IQueryable<TIn> queriable);
	}
}
