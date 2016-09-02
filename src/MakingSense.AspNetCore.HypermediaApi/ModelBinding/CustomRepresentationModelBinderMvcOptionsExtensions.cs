using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MakingSense.AspNetCore.HypermediaApi.ModelBinding;
using Microsoft.AspNetCore.Mvc;

namespace Microsoft.Extensions.DependencyInjection
{
	public static class CustomRepresentationModelBinderMvcOptionsExtensions
	{
		/// <summary>
		/// Adds support to ICustomRepresentationModel model binding
		/// </summary>
		/// <remarks>
		/// IMPORTANT: These classes have been updated as part of upgrading to ASP.NET Core v1.0 and have not been tested.
		/// </remarks>
		public static void AddCustomRepresentationModelBinder(this MvcOptions mvcOptions)
		{
			mvcOptions.ModelBinderProviders.Add(new CustomRepresentationModelBinderProvider());
		}
	}
}
