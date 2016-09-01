using System;
using MakingSense.AspNet.HypermediaApi.ExceptionHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.Framework.Internal;

namespace MakingSense.AspNet.HypermediaApi.Problems
{
	public abstract class NotFoundProblem : Problem
	{
		public override string title => "Not found";
		public override string detail => $"Path `{resourceNotFoundPath}` not found";
		public override int status => StatusCodes.Status404NotFound;

		public string resourceNotFoundPath { get; private set; }

		public override void InjectContext([NotNull] HttpContext context)
		{
			resourceNotFoundPath = context.Request.Path;
		}
	}
}
