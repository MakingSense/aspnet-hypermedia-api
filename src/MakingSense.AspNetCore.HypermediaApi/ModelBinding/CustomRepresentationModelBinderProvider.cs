using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MakingSense.AspNetCore.HypermediaApi.Model;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MakingSense.AspNetCore.HypermediaApi.ModelBinding
{
	/// <summary>
	/// Adds support to ICustomRepresentationModel model binding
	/// </summary>
	/// <remarks>
	/// IMPORTANT: This class has been updated as part of upgrading to ASP.NET Core v1.0 and has not been tested.
	/// </remarks>
	public class CustomRepresentationModelBinderProvider : IModelBinderProvider
	{
		private readonly TypeInfo CustomRepresentationModelInterfaceTypeInfo = typeof(ICustomRepresentationModel).GetTypeInfo();

		public IModelBinder GetBinder(ModelBinderProviderContext context)
		{
			var modelTypeTypeInfo = context.Metadata.ModelType.GetTypeInfo();
			if (!CustomRepresentationModelInterfaceTypeInfo.IsAssignableFrom(modelTypeTypeInfo))
			{
				return new CustomRepresentationModelBinder();
			}

			return null;
		}
	}
}
