﻿using System.Linq;
using System;
using System.Collections.Generic;
using System.Reflection;
using MakingSense.AspNet.HypermediaApi.Problems;
using MakingSense.AspNet.HypermediaApi.ExceptionHandling;
using MakingSense.AspNet.HypermediaApi.Metadata;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.Framework.Internal;

namespace MakingSense.AspNet.HypermediaApi.ValidationFilters
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
				var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
				if (actionDescriptor == null)
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
			if (relationAttribute == null || relationAttribute.InputModel == null)
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
				var schemaAttribute = relationAttribute?.InputModel?.GetTypeInfo().GetCustomAttribute<SchemaAttribute>(true);
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
