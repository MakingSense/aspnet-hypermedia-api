using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNet.HypermediaApi.Model
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
		Task SetContentAsync(Stream stream);
		bool CanRead(HttpContext context);
	}
}
