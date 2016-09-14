using System.Collections.Generic;
using MakingSense.AspNetCore.HypermediaApi.ExceptionHandling;
using MakingSense.AspNetCore.HypermediaApi.Linking;
using System.Threading.Tasks;
using MakingSense.AspNetCore.Abstractions;

namespace MakingSense.AspNetCore.HypermediaApi.SuitableValidators
{
	public interface ISuitableValidator
	{
		Task<Maybe<Problem>> ValidateAsync(IDictionary<string, TemplateParameter> values);
	}
}
