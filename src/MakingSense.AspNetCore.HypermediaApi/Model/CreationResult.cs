using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using MakingSense.AspNetCore.Abstractions;
using MakingSense.AspNetCore.HypermediaApi.Linking;
using MakingSense.AspNetCore.HypermediaApi.Linking.StandardRelations;
using MakingSense.AspNetCore.HypermediaApi.Metadata;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace MakingSense.AspNetCore.HypermediaApi.Model
{
	[Schema(typeof(CreationResult))]
	public class CreationResult : MessageResult, IActionResult
	{
		private Maybe<Link> _creationLink;
		private int _statusCode;

		public string createdResourceId { get; set; }

		public static CreationResult GetCreationResult<Tid>(Tid id, Func<Tid, Maybe<Link>> creationLink, string resultMessage)
		{
			var creationResult = new CreationResult()
			{
				_creationLink = creationLink(id).AddRel<RelatedRelation>(),
				_statusCode = StatusCodes.Status201Created,
				message = resultMessage,
				createdResourceId = id?.ToString()
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
				createdResourceId = id?.ToString()
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
				context.HttpContext.Response.Headers[HeaderNames.Location] = _creationLink.Value.Href;
			}
			return wrapper.ExecuteResultAsync(context);
		}
	}
}
