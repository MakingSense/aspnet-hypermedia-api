using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNet.HypermediaApi.ApiMappers
{
	//TODO: make it async
	public interface IApiMapper<TIn, TOut>
	{
		void Fill(TIn input, TOut output);
	}
}
