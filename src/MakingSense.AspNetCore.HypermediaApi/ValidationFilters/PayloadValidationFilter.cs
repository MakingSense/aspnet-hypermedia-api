using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using MakingSense.AspNetCore.HypermediaApi.Metadata;
using MakingSense.AspNetCore.HypermediaApi.Problems;
using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Framework.Internal;

namespace MakingSense.AspNetCore.HypermediaApi.ValidationFilters
{
	public class PayloadValidationFilter : IActionFilter
	{
		public void OnActionExecuted([NotNull]ActionExecutedContext context)
		{
			// No action
		}

		public void OnActionExecuting([NotNull]ActionExecutingContext context)
		{
			if (!context.ModelState.IsValid)
			{
				var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
				var relationAttribute = actionDescriptor?.MethodInfo.GetCustomAttribute<ActionRelationAttribute>(true);
				var schemaAttribute = relationAttribute?.InputModel?.GetSchemaAttribute();

				var errors = context.ModelState
						.Where(x => x.Value.ValidationState == Microsoft.AspNetCore.Mvc.ModelBinding.ModelValidationState.Invalid)
						.SelectMany(x => x.Value.Errors.Select(y => new ValidationProblem.ErrorItem()
						{
							key = x.Key,
							detail = string.IsNullOrWhiteSpace(y.ErrorMessage) ? y.Exception?.Message : y.ErrorMessage
						}));

				throw new ApiException(new ValidationProblem(relationAttribute, schemaAttribute, errors));
			}
		}
	}
}
