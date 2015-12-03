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
		public async Task<ModelBindingResult> BindModelAsync(ModelBindingContext bindingContext)
		{
			if (!bindingContext.IsTopLevelObject || !typeof(ICustomRepresentationModel).GetTypeInfo().IsAssignableFrom(bindingContext.ModelType.GetTypeInfo()))
			{
				// Binding Sources are opt-in. This model either didn't specify one or specified something
				// incompatible so let other binders run.
				return null;
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
				return new ModelBindingResult(modelBindingKey);
			}

			await model.SetContentAsync(httpContext.Request.Body);

			var valueProviderResult = new ValueProviderResult(rawValue: model);
			bindingContext.ModelState.SetModelValue(modelBindingKey, valueProviderResult);
			var validationNode = new ModelValidationNode(modelBindingKey, bindingContext.ModelMetadata, model)
			{
				ValidateAllProperties = true
			};

			return new ModelBindingResult(
				model,
				key: modelBindingKey,
				isModelSet: true,
				validationNode: validationNode);
		}
	}
}
