using MakingSense.AspNet.HypermediaApi.Linking;
using MakingSense.AspNet.HypermediaApi.Linking.StandardRelations;
using MakingSense.AspNet.HypermediaApi.Metadata;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Http;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MakingSense.AspNet.Abstractions;

namespace MakingSense.AspNet.HypermediaApi.Model
{
	[Schema(typeof(CreationResult))]
	public class CreationResult : MessageModel, IActionResult
	{
		private Maybe<Link> _creationLink;
		private int _statusCode;

		public string createdResourceId { get; set; } = string.Empty;

		public static CreationResult GetCreationResult<Tid>(Tid id, Func<Tid, Maybe<Link>> creationLink, string resultMessage)
		{
			var creationResult = new CreationResult()
			{
				_creationLink = creationLink(id).AddRel<RelatedRelation>(),
				_statusCode = StatusCodes.Status201Created,
				message = resultMessage,
				createdResourceId = id.ToString()
			};
			creationResult._links.Add(creationResult._creationLink);
			return creationResult;
		}

		public static CreationResult GetAcceptedResult<Tid>(Tid id, Func<Tid, Maybe<Link>> creationLink, string resultMessage)
		{
			var creationResult = new CreationResult()
			{
				_creationLink = creationLink(id).AddRel<RelatedRelation>(),
				_statusCode = StatusCodes.Status202Accepted,
				message = resultMessage,
				createdResourceId = id.ToString()
			};
			creationResult._links.Add(creationResult._creationLink);
			return creationResult;
		}


		Task IActionResult.ExecuteResultAsync(ActionContext context)
		{
			var wrapper = new ObjectResult(this)
			{
				StatusCode = _statusCode
			};
			if (_creationLink.HasValue)
			{
				context.HttpContext.Response.Headers.Set(HeaderNames.Location, _creationLink.Value.Href);
			}
			return wrapper.ExecuteResultAsync(context);
		}
	}
}
