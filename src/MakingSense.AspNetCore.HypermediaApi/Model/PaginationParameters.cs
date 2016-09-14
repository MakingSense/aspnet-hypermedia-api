using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using MakingSense.AspNetCore.HypermediaApi.Problems;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace MakingSense.AspNetCore.HypermediaApi.Model
{
	[System.Diagnostics.DebuggerStepThrough]
	public class PaginationParameters
	{
		[Range(PaginationConstants.FIRST_PAGE, double.MaxValue, ErrorMessage = "Value for `{0}` should be `{1}` or greater.")]
		[Display(Name = PaginationConstants.PARAMETER_PAGE_NAME)]
		public int page { get; private set; } = PaginationConstants.FIRST_PAGE;

		[Range(PaginationConstants.MIN_PAGE_SIZE, PaginationConstants.MAX_PAGE_SIZE, ErrorMessage = "Value for `{0}` must be between `{1}` and `{2}`.")]
		[Display(Name = PaginationConstants.PARAMETER_PER_PAGE_NAME)]
		public int per_page { get; private set; } = PaginationConstants.DEFAULT_PAGE_SIZE;

		public int offset => (page - PaginationConstants.FIRST_PAGE) * per_page;

		public PaginationParameters(int? page, int? perPage)
		{
			if (page.HasValue)
			{
				this.page = page.Value;
			}

			if (perPage.HasValue)
			{
				this.per_page = perPage.Value;
			}

			var validationResults = new List<ValidationResult>();
			if (!Validator.TryValidateObject(this, new ValidationContext(this), validationResults, true))
			{
				throw new ApiException(new ValidationProblem(validationResults.SelectMany(x => x.MemberNames.Select(y => new ValidationProblem.ErrorItem()
				{
					key = y,
					detail = x.ErrorMessage
				}))));
			}
		}

		public override bool Equals(object obj)
		{
			var other = obj as PaginationParameters;
			return other != null
				&& other.page == page
				&& other.per_page == per_page
				&& other.offset == offset;
		}

		public override int GetHashCode()
		{
			return page.GetHashCode() * per_page.GetHashCode() * offset.GetHashCode();
		}
	}
}
