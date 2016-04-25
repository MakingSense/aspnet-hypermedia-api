﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MakingSense.AspNet.Abstractions;
using MakingSense.AspNet.HypermediaApi.ExceptionHandling;
using MakingSense.AspNet.HypermediaApi.Linking;
using MakingSense.AspNet.HypermediaApi.Linking.VirtualRelations;
using MakingSense.AspNet.HypermediaApi.SuitableValidators;
using MakingSense.AspNet.Utilities;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.Framework.Internal;

namespace MakingSense.AspNet.HypermediaApi.Metadata
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
	public abstract class ActionRelationAttribute : Attribute, IRelation, IActionHttpMethodProvider, IRouteTemplateProvider, IResultFilter
	{
		public static string Path { get; set; } = "/docs/rels/";

		private string _simpleName;

		public string RelationName => Path + _simpleName;

		public abstract HttpMethod? Method { get; }

		public virtual Type InputModel => null;

		public abstract Type OutputModel { get; }

		public bool IsVirtual => false;

		public bool NotImplemented { get; set; }

		public bool IsExperimental { get; set; }

		public ActionRelationAttribute()
		{
			_simpleName = ExtractName();
		}

		private string ExtractName()
		{
			return SlugHelper.Slug(GetType(), removeSufix: "Relation");
		}

		//TODO: manage this attribute during link generation and maybe to generate automatic documentation

		private int? _order;

		public string Template { get; set; }

		public Type[] SuitableValidators { get; set; } = new Type[0];

		public abstract string Description { get; set; }

		public virtual string DocumentationDescription
		{
			get { return Description; }
		}

		public virtual string DocumentationNote
		{
			get { return null; }
		}

		public IEnumerable<string> HttpMethods => new[] { (Method ?? HttpMethod.GET).ToString() };

		public string Name { get; set; }

		public int Order
		{
			get { return _order ?? 0; }
			set { _order = value; }
		}

		int? IRouteTemplateProvider.Order => _order;

		public void OnResultExecuting([NotNull]ResultExecutingContext context)
		{
			var schemaAttribute =
				GetSchemaAttributeFromResultValue(context)
				?? GetSchemaAttributeFromActionResult(context)
				?? GetSchemaAttributeFromOutputModel();

			if (schemaAttribute != null)
			{
				context.HttpContext.Response.OnStarting((o) =>
				{
					context.HttpContext.Response.ContentType += $"; profile={schemaAttribute.SchemaFilePath}";
					return Task.FromResult(0);
				}, null);
			}
		}

		private SchemaAttribute GetSchemaAttributeFromOutputModel()
		{
			var attr = OutputModel?.GetTypeInfo().GetCustomAttribute<SchemaAttribute>(true);
			return attr == null || attr.NoSchema ? null : attr;
		}

		private static SchemaAttribute GetSchemaAttributeFromActionResult(ResultExecutingContext context)
		{
			var attr = (context.ActionDescriptor as ControllerActionDescriptor)?.MethodInfo.ReturnType?.GetTypeInfo().GetCustomAttribute<SchemaAttribute>(true);
			return attr == null || attr.NoSchema ? null : attr;
		}

		private static SchemaAttribute GetSchemaAttributeFromResultValue(ResultExecutingContext context)
		{
			var attr = (context.Result as ObjectResult)?.Value?.GetType()?.GetTypeInfo().GetCustomAttribute<SchemaAttribute>(true);
			return attr == null || attr.NoSchema ? null : attr;
		}

		public void OnResultExecuted([NotNull]ResultExecutedContext context)
		{
			//Do nothing
		}

		public virtual void UpdateLinkMethodInfo(Link link)
		{
			if (NotImplemented)
			{
				link.AddRel<NotImplementedRelation>();
			}
			if (IsExperimental)
			{
				link.AddRel<ExperimentalRelation>();
			}
			link.AddRel(this);
			link.SetDescription(Description);
		}

		public virtual Task<Maybe<Problem>> ExecuteSuitableValidationsAsync(IServiceProvider serviceProvider, IDictionary<string, object> values)
		{
			if (SuitableValidators.Length == 0)
			{
				return Task.FromResult(Maybe.None<Problem>());
			}

			var wrappedValues = values.ToDictionary(
				x => x.Key,
				x =>
				{
					var casted = x.Value as TemplateParameter;
					return casted ?? TemplateParameter.Force(x.Value);
				});

			return ExecuteSuitableValidationsAsync(serviceProvider, wrappedValues);
		}

		public virtual Task<Maybe<Problem>> ExecuteSuitableValidationsAsync(IServiceProvider serviceProvider, IDictionary<string, TemplateParameter> values)
		{
			if (SuitableValidators.Length == 0)
			{
				return Task.FromResult(Maybe.None<Problem>());
			}

			var problem = SuitableValidators
				.Select(x => serviceProvider.GetService(x) as ISuitableValidator)
				.Where(x => x != null)
				.Select(x => x.ValidateAsync(values).WaitAndGetValue())
				.FirstOrDefault(x => x.HasValue);

			return Task.FromResult(problem);
		}
	}
}
