using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using MakingSense.AspNetCore.HypermediaApi.Problems;
using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using MakingSense.AspNetCore.HypermediaApi.Metadata;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Framework.Internal;

namespace MakingSense.AspNetCore.HypermediaApi.ValidationFilters
{
	public class RequiredPayloadFilter : IActionFilter
	{
		public void OnActionExecuted([NotNull]ActionExecutedContext context)
		{
			// No action
		}

		public void OnActionExecuting([NotNull]ActionExecutingContext context)
		{
			if (context.ModelState.IsValid)
			{
				if (!(context.ActionDescriptor is ControllerActionDescriptor actionDescriptor))
				{
					return;
				}
				ValidateByActionRelation(actionDescriptor, context.ActionArguments);
				ValidateByNotNullAttributtes(actionDescriptor, context.ActionArguments);
			}
		}

		public void ValidateByNotNullAttributtes([NotNull] ControllerActionDescriptor actionDescriptor, [NotNull] IDictionary<string, object> actionArguments)
		{
			var errors = actionDescriptor.MethodInfo
				.GetParameters()
				.Where(x => x.GetCustomAttributes().Any(y => y.GetType().Name.StartsWith("NotNull", StringComparison.Ordinal)))
				.Where(x => actionArguments.Where(y => y.Key == x.Name).Select(y => y.Value).FirstOrDefault() == null)
				.Select(x => new ValidationProblem.ErrorItem()
				{
					key = x.Name,
					detail = "Value is required."
				})
				.ToArray();

			if (errors.Length > 0)
			{
				throw new ApiException(new ValidationProblem(errors));
			}
		}

		public void ValidateByActionRelation([NotNull] ControllerActionDescriptor actionDescriptor, [NotNull] IDictionary<string, object> actionArguments)
		{
			var relationAttribute = actionDescriptor.MethodInfo.GetCustomAttribute<ActionRelationAttribute>(true);
			if (relationAttribute == null || relationAttribute.InputModel == null || relationAttribute.AllowEmptyInput)
			{
				return;
			}

			var parameter = actionDescriptor.Parameters.Where(x => x.ParameterType == relationAttribute.InputModel).FirstOrDefault();
			if (parameter == null)
			{
				return;
			}

			var argumentValue = actionArguments.Where(x => x.Key == parameter.Name).Select(x => x.Value).FirstOrDefault();
			if (argumentValue == null)
			{
				var schemaAttribute = relationAttribute?.InputModel?.GetSchemaAttribute();
				throw new ApiException(new ValidationProblem(relationAttribute, schemaAttribute, new[] {
						new ValidationProblem.ErrorItem()
						{
							key = parameter.Name,
							detail = "Value is required."
						}
					}));
			}
		}
	}
}
