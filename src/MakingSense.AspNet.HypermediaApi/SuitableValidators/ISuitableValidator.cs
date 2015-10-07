using System.Collections.Generic;
using MakingSense.AspNet.HypermediaApi.ExceptionHandling;
using MakingSense.AspNet.HypermediaApi.Linking;
using System.Threading.Tasks;
using MakingSense.AspNet.Abstractions;

namespace MakingSense.AspNet.HypermediaApi.SuitableValidators
{
	public interface ISuitableValidator
	{
		Task<Maybe<Problem>> ValidateAsync(IDictionary<string, TemplateParameter> values);
	}
}
