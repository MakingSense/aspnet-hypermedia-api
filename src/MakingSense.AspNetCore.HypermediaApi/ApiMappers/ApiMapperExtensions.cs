using MakingSense.AspNetCore.Abstractions;
using MakingSense.AspNetCore.HypermediaApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.ApiMappers
{
	[System.Diagnostics.DebuggerStepThrough]
	public static class ApiMapperExtensions
	{
		public static TPage MapPage<TIn, TOut, TPage>(this IApiMapper<TIn, TOut> mapper, PaginationParameters pagination, IQueryable<TIn> query)
			where TOut : class, new()
			where TPage : BaseCollectionPage<TOut>, new()
		{
			return mapper.MapPage<TIn, TOut, TPage>(
				pagination,
				query.Skip(pagination.offset).Take(pagination.per_page),
				query.Count());
		}

		public static TPage MapPage<TIn, TOut, TPageItem, TPage>(this IApiMapper<TIn, TOut> mapper, PaginationParameters pagination, IQueryable<TIn> query)
			where TOut : class, TPageItem, new()
			where TPage : BaseCollectionPage<TPageItem>, new()
		{
			return mapper.MapPage<TIn, TOut, TPageItem, TPage>(
				pagination,
				query.Skip(pagination.offset).Take(pagination.per_page),
				query.Count());
		}

		public static TOut MapAndTakeFirstOrDefault<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, IQueryable<TIn> query)
			where TOut : class, new()
		{
			return mapper.Map(query.Take(1)).FirstOrDefault();
		}

		public static Maybe<TOut> MapAndMaybeTakeFirst<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, IQueryable<TIn> query)
			where TOut : class, new()
		{
			var result = mapper.MapAndTakeFirstOrDefault(query);
			return result == null ? Maybe.None<TOut>() : Maybe.From(result);
		}

		public static TOut Map<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, TIn input)
			where TOut : class, new()
		{
			if (input == null)
			{
				return null;
			}

			var output = new TOut();
			mapper.Fill(input, output);
			return output;
		}

		public static IEnumerable<TOut> Map<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, IEnumerable<TIn> inputEnumerable)
			where TOut : class, new()
		{
			return
				mapper is IQueryableMapper<TIn, TOut> queryableProyector ? queryableProyector.Map(inputEnumerable.AsQueryable())
				: inputEnumerable.Select(x => mapper.Map(x));
		}

		public static IEnumerable<TOut> Map<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, IQueryable<TIn> inputQueryable)
			where TOut : class, new()
		{
			return
				mapper is IQueryableMapper<TIn, TOut> queryableProyector ? queryableProyector.Map(inputQueryable)
				: inputQueryable.AsEnumerable().Select(x => mapper.Map(x));
		}

		public static void Add<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, IEnumerable<TIn> inputEnumerable, List<TOut> outputList)
			where TOut : class, new()
		{
			outputList.AddRange(mapper.Map(inputEnumerable));
		}

		public static void Add<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, IEnumerable<TIn> inputEnumerable, ICollection<TOut> outputCollection)
			where TOut : class, new()
		{
			foreach (var item in mapper.Map(inputEnumerable))
			{
				outputCollection.Add(item);
			}
		}

		public static void FillCollection<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, IEnumerable<TIn> input, IModelCollection<TOut> output)
			where TOut : class, new()
		{
			output.items.AddRange(mapper.Map(input));
		}

		public static TCollection MapCollection<TIn, TOut, TCollection>(this IApiMapper<TIn, TOut> mapper, IEnumerable<TIn> input)
			where TOut : class, new()
			where TCollection : IModelCollection<TOut>, new()
		{
			var result = new TCollection();
			mapper.FillCollection(input, result);
			return result;
		}

		public static void FillPage<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, PaginationParameters pagination, IEnumerable<TIn> inputItems, int totalItems, BaseCollectionPage<TOut> output)
			where TOut : class, new()
		{
			FillPage<TIn, TOut, TOut>(mapper, pagination, inputItems, totalItems, output);
		}

		private static void FillPage<TIn, TOut, TPageItem>(IApiMapper<TIn, TOut> mapper, PaginationParameters pagination, IEnumerable<TIn> inputItems, int totalItems, BaseCollectionPage<TPageItem> output)
			where TOut : class, TPageItem, new()
		{
			output.items.AddRange(mapper.Map(inputItems));
			output.pageSize = pagination.per_page;
			output.itemsCount = totalItems;
			output.currentPage = pagination.page;
		}

		// TODO: Make this API more friendly for consumers. It should not
		// be necessary to send all type parameters:
		//
		//     var mapped = _DtoSubscriber_To_Subscriber
		//        .MapPage<DtoSubscriber, Subscriber, SubscriberCollectionPage>(pagination, result.Items, result.TotalItemsFilter);
		//
		public static TPage MapPage<TIn, TOut, TPage>(this IApiMapper<TIn, TOut> mapper, PaginationParameters pagination, IEnumerable<TIn> input, int totalItems)
			where TOut : class, new()
			where TPage : BaseCollectionPage<TOut>, new()
		{
			var result = new TPage();
			mapper.FillPage(pagination, input, totalItems, result);
			return result;
		}

		public static void FillPage<TIn, TOut>(this IApiMapper<TIn, TOut> mapper, PaginationParameters pagination, IQueryable<TIn> inputQueryable, int totalItems, BaseCollectionPage<TOut> output)
			where TOut : class, new()
		{
			output.items.AddRange(mapper.Map(inputQueryable));
			output.pageSize = pagination.per_page;
			output.itemsCount = totalItems;
			output.currentPage = pagination.page;
		}

		public static TPage MapPage<TIn, TOut, TPage>(this IApiMapper<TIn, TOut> mapper, PaginationParameters pagination, IQueryable<TIn> inputQueriable, int totalItems)
			where TOut : class, new()
			where TPage : BaseCollectionPage<TOut>, new()
		{
			var result = new TPage();
			mapper.FillPage(pagination, inputQueriable, totalItems, result);
			return result;
		}

		public static TPage MapPage<TIn, TOut, TPageItem, TPage>(this IApiMapper<TIn, TOut> mapper, PaginationParameters pagination, IQueryable<TIn> inputQueriable, int totalItems)
			where TOut : class, TPageItem, new()
			where TPage : BaseCollectionPage<TPageItem>, new()
		{
			var result = new TPage();
			FillPage(mapper, pagination, inputQueriable, totalItems, result);
			return result;
		}
	}
}
