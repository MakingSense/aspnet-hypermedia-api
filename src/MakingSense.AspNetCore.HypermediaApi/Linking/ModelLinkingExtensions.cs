using MakingSense.AspNetCore.Abstractions;
using MakingSense.AspNetCore.HypermediaApi.Linking;
using MakingSense.AspNetCore.HypermediaApi.Linking.StandardRelations;
using MakingSense.AspNetCore.HypermediaApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public static class ModelLinkingExtensions
	{
		public static T AddLinks<T>(this T model, params Link[] links)
			where T : BaseModel
		{
			model._links.AddRange(links);
			return model;
		}

		public static T AddLinks<T>(this T model, IEnumerable<Link> links)
			where T : BaseModel
		{
			model._links.AddRange(links);
			return model;
		}

		public static T AddLink<T>(this T model, Link link)
			where T : BaseModel
		{
			model._links.Add(link);
			return model;
		}
		public static T AddLinks<T>(this T model, params Maybe<Link>[] links)
			where T : BaseModel
		{
			model._links.AddRange(links);
			return model;
		}

		public static T AddLinks<T>(this T model, IEnumerable<Maybe<Link>> links)
			where T : BaseModel
		{
			model._links.AddRange(links);
			return model;
		}

		public static T AddLink<T>(this T model, Maybe<Link> link)
			where T : BaseModel
		{
			model._links.Add(link);
			return model;
		}

		public static IModelCollection<TItem> AddItemsLinks<TItem>(this IModelCollection<TItem> page, Func<TItem, IEnumerable<Maybe<Link>>> linkGenerator)
			where TItem : BaseModel
		{
			foreach (var item in page.items)
			{
				item.AddLinks(linkGenerator(item));
			}
			return page;
		}

		public static IModelCollection<TItem> AddItemsLinks<TItem>(this IModelCollection<TItem> page, Func<TItem, Maybe<Link>> linkGenerator)
			where TItem : BaseModel
		{
			return page.AddItemsLinks(item => new Maybe<Link>[] { linkGenerator(item) });
		}

		// Note, This method is a dirty patch to avoid issues because it does not work:
		// public static TPage AddItemsLinks<TPage, TItem>(this TPage page, Func<TItem, IEnumerable<Link>> linkGenerator)
		//     where TPage : BaseCollectionPage<TItem>
		//     where TItem : BaseModel
		// Code:
		//    new ListsCollectionPage().AddItemsLinks(item => new[] { ....
		// Error:
		//    The type arguments for method 'AddLinksHelpers.AddItemsLinks<TPage, TItem>(TPage, Func<TItem, IEnumerable<Link>>)' cannot be inferred from the usage.Try specifying the type arguments explicitly.
		public static T Cast<T>(this IModelCollection collection)
			where T : IModelCollection
		{
			return (T)collection;
		}

		public static TPage AddPageLinks<TPage>(this TPage page, Func<TemplateParameter<int?>, TemplateParameter<int?>, Maybe<Link>> linkFactory)
			where TPage : BaseModel, ICollectionPage
		{
			return page.AddLinks(GeneratePageLinks(page, linkFactory));
		}


		private static IEnumerable<Link> GeneratePageLinks<TPage>(TPage page, Func<TemplateParameter<int?>, TemplateParameter<int?>, Maybe<Link>> linkFactory)
			where TPage : ICollectionPage
		{
			var perPageParameter = page.pageSize == PaginationConstants.DEFAULT_PAGE_SIZE
				? TemplateParameter.Force<int?>(null)
				: TemplateParameter.Force<int?>(page.pageSize);

			var currentPageParameter = page.currentPage == PaginationConstants.FIRST_PAGE
				? TemplateParameter.Force<int?>(null)
				: TemplateParameter.Force<int?>(page.currentPage);

			var previousPage = page.currentPage - 1;
			var nextPage = page.currentPage + 1;

			var selfLink = linkFactory(currentPageParameter, perPageParameter);

			var firstLink =
				PaginationConstants.FIRST_PAGE == page.currentPage ? selfLink
				: linkFactory(TemplateParameter.Force<int?>(null), perPageParameter);

			var lastLink =
				page.lastPage == page.currentPage ? selfLink
				: page.lastPage == PaginationConstants.FIRST_PAGE ? firstLink
				: linkFactory(TemplateParameter.Force<int?>(page.lastPage), perPageParameter);

			var nextLink =
				nextPage > page.lastPage ? Maybe.None<Link>()
				: nextPage == page.lastPage ? lastLink
				: linkFactory(TemplateParameter.Force<int?>(nextPage), perPageParameter);

			var previousLink =
				previousPage < PaginationConstants.FIRST_PAGE ? Maybe.None<Link>()
				: previousPage == PaginationConstants.FIRST_PAGE ? firstLink
				: previousPage > page.lastPage ? null
				: previousPage == page.lastPage ? lastLink
				: linkFactory(TemplateParameter.Force<int?>(previousPage), perPageParameter);

			var specificLink =
				PaginationConstants.FIRST_PAGE == page.lastPage ? null
				: linkFactory(TemplateParameter.Create<int?>(), perPageParameter).SetDescription("Specific page");

			selfLink.SetSelf();
			firstLink.AddRel<FirstRelation>();
			lastLink.AddRel<LastRelation>();
			nextLink.AddRel<NextRelation>();
			previousLink.AddRel<PreviousRelation>();

			return new[] { specificLink, selfLink, firstLink, lastLink, nextLink, previousLink }
				.Where(x => x.HasValue)
				.Select(x => x.Value)
				.Distinct()
				.Select(x => SetDescription(x, page.currentPage, page.lastPage));
		}

		private static Link SetDescription(Link link, int currentPage, int lastPage)
		{
			if (link.Relation.Contains<FirstRelation>() && link.Relation.Contains<LastRelation>())
			{
				link.SetDescription("Single page");
			}
			else if (link.Relation.Contains<SelfRelation>() && currentPage > lastPage)
			{
				link.SetDescription("Current page (invalid)");
			}
			else if (link.Relation.Contains<SelfRelation>())
			{
				link.SetDescription("Current page");
			}
			else if (link.Relation.Contains<FirstRelation>() && link.Relation.Contains<PreviousRelation>())
			{
				link.SetDescription("First/Previous page");
			}
			else if (link.Relation.Contains<FirstRelation>())
			{
				link.SetDescription("First page");
			}
			else if (link.Relation.Contains<PreviousRelation>())
			{
				link.SetDescription("Previous page");
			}
			else if (link.Relation.Contains<LastRelation>() && link.Relation.Contains<NextRelation>())
			{
				link.SetDescription("Last/Next page");
			}
			else if (link.Relation.Contains<LastRelation>())
			{
				link.SetDescription("Last page");
			}
			else if (link.Relation.Contains<NextRelation>())
			{
				link.SetDescription("Next page");
			}
			return link;
		}

	}
}
