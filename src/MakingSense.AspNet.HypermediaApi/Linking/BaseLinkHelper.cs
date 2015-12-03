using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using MakingSense.AspNet.Abstractions;
using MakingSense.AspNet.HypermediaApi.Linking.VirtualRelations;
using MakingSense.AspNet.HypermediaApi.Metadata;
using MakingSense.AspNet.Utilities;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Mvc.Controllers;
using Microsoft.AspNet.Mvc.Infrastructure;
using Microsoft.AspNet.Mvc.Routing;
using Microsoft.Framework.Internal;
using Microsoft.Extensions.WebEncoders;

namespace MakingSense.AspNet.HypermediaApi.Linking
{
	public abstract class BaseLinkHelper : ILinkHelper
	{
		private readonly IUrlHelper _urlHelper;
		private readonly IActionContextAccessor _actionContextAccessor;
		private readonly IUrlEncoder _urlEncoder;

		protected HttpContext HttpContext { get; private set; }

		private IUrlHelper GetValidUrlHelper() => _actionContextAccessor?.ActionContext == null ? null : _urlHelper;
		private ControllerActionDescriptor GetActionDescriptor() => _actionContextAccessor?.ActionContext?.ActionDescriptor as ControllerActionDescriptor;

		public BaseLinkHelper([NotNull] IHttpContextAccessor httpContextAccessor, IActionContextAccessor actionContextAccessor, IUrlHelper urlHelper)
		{
			_urlHelper = urlHelper;
			_actionContextAccessor = actionContextAccessor;
			HttpContext = httpContextAccessor.HttpContext;
			_urlEncoder = UrlEncoder.Default;
		}

		private static string ExtractControllerName(MethodInfo methodInfo)
		{
			var controllerType = methodInfo.DeclaringType.GetTypeInfo();
			var controllerName = controllerType.Name.EndsWith("Controller") ? controllerType.Name.Substring(0, controllerType.Name.Length - "Controller".Length) : controllerType.Name;
			return controllerName;
		}

		public Maybe<Link> ToAction<T>(Expression<Func<T, Task>> expression)
			where T : Controller
		{
			var methodCallExpression = (MethodCallExpression)expression.Body;
			return ToAction(methodCallExpression.Method, methodCallExpression.Arguments);
		}

		public Maybe<Link> ToAction<T>(Expression<Action<T>> expression)
			where T : Controller
		{
			var methodCallExpression = (MethodCallExpression)expression.Body;
			return ToAction(methodCallExpression.Method, methodCallExpression.Arguments);
		}

		public Maybe<Link> ToAction(MethodInfo method, ReadOnlyCollection<Expression> arguments)
		{
			var parameters = method.GetParameters();
			var isTemplate = false;

			var wrappedValues = Enumerable.Range(0, arguments.Count).Select(x =>
			{
				var argument = arguments[x];
				var unaryExpression = argument as UnaryExpression;
				TemplateParameter value;

				if (unaryExpression != null && unaryExpression.Operand.Type.GetTypeInfo().IsSubclassOf(typeof(TemplateParameter)))
				{
					value = Expression.Lambda(unaryExpression.Operand).Compile().DynamicInvoke() as TemplateParameter;
				}
				else
				{
					value = TemplateParameter.Force(Expression.Lambda(argument).Compile().DynamicInvoke());
				}

				return new
				{
					Name = parameters[x].Name,
					Value = value
				};
			})
			.ToDictionary(x => x.Name, x => x.Value);

			var actionAttribute = ExtractActionAttribute(method);

			if (actionAttribute != null && HttpContext != null && HttpContext.RequestServices != null)
			{
				var task = actionAttribute.ExecuteSuitableValidationsAsync(HttpContext.RequestServices, wrappedValues);

				var problem = task.WaitAndGetValue();

				if (problem.HasValue)
				{
					return Maybe.None<Link>();
				}
			}

			var values = wrappedValues.ToDictionary(
				x => x.Key,
				x =>
				{
					if (x.Value.ForceValue)
					{
						return x.Value.ForcedValue;
					}
					else
					{
						isTemplate = true;
						return string.Format("{{{0}}}", x.Value.CustomText ?? x.Key);
					}
				});

			var link = new Link()
			{
				Href = ExtractUrl(method, values)
			};

			if (isTemplate)
			{
				link.Href = link.Href?.Replace("%7B", "{").Replace("%7D", "}");
				link.Relation.Add<TemplateRelation>();
			}

			if (actionAttribute != null)
			{
				actionAttribute.UpdateLinkMethodInfo(link);
			}

			return Maybe.From(link);
		}

		private string ExtractUrl(MethodInfo methodInfo, IDictionary<string, object> values)
		{
			values = values.ToDictionary(x => x.Key, x =>
			{
				if (x.Value != null && x.Value is DateTimeOffset)
				{
					return ((DateTimeOffset)x.Value).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.FFFFFFFZ");
				}
				else
				{
					return x.Value;
				}
			});

			var urlHelper = GetValidUrlHelper();
			if (urlHelper != null)
			{
				return GenerateUrlUsingUrlHelper(urlHelper, new UrlActionContext()
				{
					Action = methodInfo.Name,
					Controller = ExtractControllerName(methodInfo),
					Values = values
				});
			}
			else
			{
				//FALLBACK when _urlHelper is not available
				var actionAttribute = ExtractActionAttribute(methodInfo);
				var url = actionAttribute.Template;
				foreach (var key in values.Keys)
				{
					url = url.Replace("{" + key + "}", _urlEncoder.UrlEncode($"{values[key]}"));
				}
				return url;
			}
		}

		private string GenerateUrlUsingUrlHelper(IUrlHelper urlHelper, UrlActionContext urlActionContext)
		{
			var url = urlHelper.Action(urlActionContext);

			// Ugly patch in order to use "@" in place of "%40"
			// Ugly patch in order to use ":" in place of "%3A"
			url = url?.Replace("%40", "@").Replace("%3A", ":");

			return url;
		}

		private static ActionRelationAttribute ExtractActionAttribute(MethodInfo methodInfo)
		{
			return methodInfo?.GetCustomAttribute<ActionRelationAttribute>(false);
		}

		public Maybe<Link> ToSelf(object values = null)
		{
			var urlHelper = GetValidUrlHelper();
			var href = urlHelper != null ? GenerateUrlUsingUrlHelper(urlHelper, new UrlActionContext() { Values = values })
					: HttpContext.Request.Path.Value;

			if (href == null)
			{
				// Ignore the link
				return Maybe.None<Link>();
			}

			var link = new Link() { Href = href };

			var actionDescriptor = GetActionDescriptor();
			if (actionDescriptor != null)
			{
				ExtractActionAttribute(actionDescriptor.MethodInfo)?.UpdateLinkMethodInfo(link);
			}

			link.SetSelf();

			return Maybe.From(link);
		}

		public abstract Maybe<Link> ToHomeAccount();
	}
}
