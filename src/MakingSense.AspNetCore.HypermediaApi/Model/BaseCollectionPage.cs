using MakingSense.AspNetCore.HypermediaApi.Linking;
using MakingSense.AspNetCore.HypermediaApi.Linking.StandardRelations;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MakingSense.AspNetCore.HypermediaApi.Model
{
	public interface ICollectionPage
	{
		int pageSize { get; }
		int itemsCount { get; }
		int currentPage { get; }
		int pagesCount { get; }
		int lastPage { get; }
	}

	public abstract class BaseCollectionPage<T> : BaseModel, IModelCollection<T>, ICollectionPage
	{
		[Required]
		public List<T> items { get; } = new List<T>();

		public int pageSize { get; set; } = PaginationConstants.DEFAULT_PAGE_SIZE;
		public int itemsCount { get; set; } = 0;
		public int currentPage { get; set; } = PaginationConstants.FIRST_PAGE;
		public int pagesCount => pageSize > 0 ? (int)Math.Ceiling((double)itemsCount / pageSize) : 0;

		[JsonIgnore]
		public int lastPage => pagesCount + PaginationConstants.FIRST_PAGE - 1;
	}
}
