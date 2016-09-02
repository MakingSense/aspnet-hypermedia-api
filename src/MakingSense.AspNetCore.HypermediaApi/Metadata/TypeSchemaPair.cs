using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MakingSense.AspNetCore.HypermediaApi.Metadata
{
	public class TypeSchemaPair
	{
		public readonly Type Type;
		public readonly SchemaAttribute SchemaAttribute;

		public TypeSchemaPair(Type type, SchemaAttribute schemaAttribute)
		{
			Type = type;
			SchemaAttribute = schemaAttribute;
		}
	}
}
