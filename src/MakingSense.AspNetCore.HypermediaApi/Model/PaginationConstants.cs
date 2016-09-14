using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.Model
{
	public static class PaginationConstants
	{
		public const int MAX_PAGE_SIZE = 200;
		public const int MIN_PAGE_SIZE = 5;
		public const int DEFAULT_PAGE_SIZE = 20;
		public const int FIRST_PAGE = 1;

		public const string PARAMETER_PAGE_NAME = "page";
		public const string PARAMETER_PER_PAGE_NAME = "per_page";
	}
}
