using MakingSense.AspNet.HypermediaApi.Model;
using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MakingSense.AspNet.HypermediaApi.ModelBinding
{
	/// <summary>
	/// Capture binding of ICustomRepresentation models and use a proper binding mechanism
	/// </summary>
	public class CustomRepresentationModelBinder : IModelBinder
	{
		public Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
		{
			// This method is optimized to use cached tasks when possible and avoid allocating
			// using Task.FromResult. If you need to make changes of this nature, profile
			// allocations afterwards and look for Task<ModelBindingResult>.

			if (!typeof(ICustomRepresentationModel).GetTypeInfo().IsAssignableFrom(bindingContext.ModelType.GetTypeInfo()))
			{
				// Formatters are opt-in. This model either didn't specify [FromBody] or specified something
				// incompatible so let other binders run.
				return ModelBindingResult.NoResultAsync;
			}

			return BindModelCoreAsync(bindingContext);
		}

		private async Task<ModelBindingResult> BindModelCoreAsync(ModelBindingContext bindingContext)
		{
			if (bindingContext == null)
			{
				throw new ArgumentNullException(nameof(bindingContext));
			}

			var httpContext = bindingContext.OperationBindingContext.HttpContext;
			var model = (ICustomRepresentationModel)Activator.CreateInstance(bindingContext.ModelType);
			var modelBindingKey = bindingContext.ModelName;

			if (!model.CanRead(httpContext))
			{
				// TODO: Consider to add reference to Model documentation
				bindingContext.ModelState.AddModelError(modelBindingKey, "Imposible to parse request body, verify Content-Type header.");

				// This model binder is the only handler for ICustomRepresentationModel binding source and it cannot run
				// twice. Always tell the model binding system to skip other model binders and never to fall back i.e.
				// indicate a fatal error.
				return ModelBindingResult.Failed(modelBindingKey);
			}

			await model.SetContentAsync(httpContext.Request.Body);

			bindingContext.ModelState.SetModelValue(modelBindingKey, rawValue: model, attemptedValue: null);

			return ModelBindingResult.Success(modelBindingKey, model);
		}
	}
}
