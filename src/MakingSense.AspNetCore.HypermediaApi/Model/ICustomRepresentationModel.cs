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
	/// `IActionResult.ExecuteResultAsync()` implementation is used to generate HTTP response.
	/// `CanRead` and `SetContentAsync()` implementations are used to parse HTTP request.
	/// </remarks>
	public interface ICustomRepresentationModel : IActionResult
	{
		Task SetContentAsync(HttpRequest request);
		bool CanRead(HttpContext context);
	}
}
