using Microsoft.AspNet.Mvc;
using Microsoft.Framework.Internal;
using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using MakingSense.AspNet.HypermediaApi.Metadata;
using MakingSense.AspNet.HypermediaApi.Problems;
using MakingSense.AspNet.HypermediaApi.ExceptionHandling;

namespace MakingSense.AspNet.HypermediaApi.ValidationFilters
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
				var schemaAttribute = relationAttribute?.InputModel?.GetTypeInfo().GetCustomAttribute<SchemaAttribute>(true);

				var errors = context.ModelState
						.Where(x => x.Value.ValidationState == Microsoft.AspNet.Mvc.ModelBinding.ModelValidationState.Invalid)
						.SelectMany(x => x.Value.Errors.Select(y => new ValidationProblem.ErrorItem()
						{
							Key = x.Key,
							Detail = string.IsNullOrWhiteSpace(y.ErrorMessage) ? y.Exception?.Message : y.ErrorMessage
						}));

				throw new ApiException(new ValidationProblem(relationAttribute, schemaAttribute, errors));
			}
		}
	}
}
