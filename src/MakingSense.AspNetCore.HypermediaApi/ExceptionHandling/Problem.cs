using MakingSense.AspNetCore.HypermediaApi.Model;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;

namespace MakingSense.AspNetCore.HypermediaApi.ExceptionHandling
{
	public abstract class Problem : BaseModel
	{
		private readonly IDictionary<string, string> _customHeaders = new Dictionary<string, string>();

		public static string Path { get; set; } = "/docs/errors/";
		public abstract string title { get; }
		public abstract string detail { get; }
		public abstract int status { get; }
		public abstract int errorCode { get; }

		public virtual string type =>
			$"{Path}{status}.{errorCode}-{title.ToLower().Replace(", ", ",").Replace(",", " ").Replace(" ", "-")}";

		public void AddHeader(string header, string value) =>
			_customHeaders[header] = value;

		public IEnumerable<KeyValuePair<string, string>> GetCustomHeaders() =>
			_customHeaders.AsEnumerable();

		/// <summary>
		/// Called by ExecuteResultAsync before serialize problem
		/// </summary>
		/// <remarks>
		/// Allows sub-classes to change rendering information based on current HTTP context.
		/// </remarks>
		/// <param name="context"></param>
		public virtual void InjectContext(HttpContext context)
		{
			//No default action
		}
	}
}
