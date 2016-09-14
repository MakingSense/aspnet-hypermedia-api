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
	/// Capture binding of ICustomRepresentation models and use a proper binding mechanism
	/// </summary>
	/// <remarks>
	/// IMPORTANT: This class has been updated as part of upgrading to ASP.NET Core v1.0 and has not been tested.
	/// </remarks>
	public class CustomRepresentationModelBinder : IModelBinder
	{
		public async Task BindModelAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			try
			{
				var model = (ICustomRepresentationModel)Activator.CreateInstance(bindingContext.ModelType);
				if (!model.CanRead(bindingContext.HttpContext))
				{
					// TODO: Consider to add reference to Model documentation
					bindingContext.ModelState.TryAddModelError(
						bindingContext.ModelName,
						"Imposible to parse request body, verify Content-Type header.");
					// This model binder is the only handler for ICustomRepresentationModel binding source and it cannot run
					// twice. Always tell the model binding system to skip other model binders and never to fall back i.e.
					// indicate a fatal error.
					return;
				}
				await model.SetContentAsync(bindingContext.HttpContext.Request);
				bindingContext.Result = ModelBindingResult.Success(model);
			}
			catch (Exception exception)
			{
				bindingContext.ModelState.TryAddModelError(
					bindingContext.ModelName,
					exception,
					bindingContext.ModelMetadata);
			}
		}
	}
}
