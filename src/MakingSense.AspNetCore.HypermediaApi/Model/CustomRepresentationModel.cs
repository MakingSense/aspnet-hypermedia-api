using MakingSense.AspNetCore.HypermediaApi.Linking;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.Model
{
	/// <summary>
	/// Represents models with custom representation.
	/// </summary>
	/// <remarks>
	/// It is required to register CustomRepresentationModelBinder on MVC options.
	/// </remarks>
	public abstract class CustomRepresentationModel : BaseModel, ICustomRepresentationModel
	{
		public abstract string ContentType { get; }

		public virtual string[] AlternativeContentTypes => null;

		public virtual bool AcceptEmptyContentType => true;

		public virtual bool CanRead(HttpContext context)
		{
			var requestContentType = context.Request.ContentType;
			if (string.IsNullOrEmpty(requestContentType))
			{
				return AcceptEmptyContentType;
			}
			else
			{
				// TODO: improve it based on ASP.NET code after RC2 see https://github.com/aspnet/Mvc/issues/3138
				return (ContentType != null && requestContentType.IndexOf(ContentType, StringComparison.OrdinalIgnoreCase) >= 0)
				|| (AlternativeContentTypes != null && AlternativeContentTypes.Any(x => requestContentType.IndexOf(x, StringComparison.OrdinalIgnoreCase) >= 0));
			}
		}

		public async Task ExecuteResultAsync(ActionContext context)
		{
			context.HttpContext.Response.OnStarting(() => {
				SetResponseLinks(context.HttpContext.Response);
				SetResponseContentType(context.HttpContext.Response);
				return Task.FromResult(0);
			});

			await WriteContentAsync(context.HttpContext.Response);
		}

		protected virtual void SetResponseContentType(HttpResponse response) =>
			response.ContentType = ContentType;

		protected virtual string FormatLinkHeader(Link link) => string.IsNullOrEmpty(link.Description)
			? $"<{ link.Href }>; rel=\"{ link.Relation.RelationName }\""
			: $"<{ link.Href }>; rel=\"{ link.Relation.RelationName }\"; title=\"{ link.Description }\"";

		protected virtual void SetResponseLinks(HttpResponse response) =>
			response.Headers.AppendCommaSeparatedValues("Link", _links.Select(FormatLinkHeader).ToArray());

		protected abstract Task WriteContentAsync(HttpResponse response);

		public abstract Task SetContentAsync(HttpRequest request);
	}
}
