using MakingSense.AspNetCore.Utilities;
using System;
using System.Linq;
using System.Reflection;

namespace MakingSense.AspNetCore.HypermediaApi.Metadata
{
	public static class SchemaAttributeHelpers
	{
		public static SchemaAttribute GetSchemaAttribute(this TypeInfo typeInfo)
		{
			return typeInfo.GetCustomAttribute<SchemaAttribute>(false)
				?? typeInfo.GetCustomAttributes<SchemaAttribute>(true).FirstOrDefault();
		}

		public static SchemaAttribute GetSchemaAttribute(this Type type)
		{
			return GetSchemaAttribute(type.GetTypeInfo());
		}

	}
}
