using MakingSense.AspNetCore.Utilities;
using System;

namespace MakingSense.AspNetCore.HypermediaApi.Metadata
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
	public class SchemaAttribute : Attribute
	{
		public static string Path { get; set; } = "/schemas/";

		public string SchemaName { get; set; }

		public string SchemaFilePath => Path + SchemaName;

		public bool NoSchema => SchemaName == null;

		public SchemaAttribute(string schemaName)
		{
			SchemaName = schemaName;
		}

		// TODO: allow to infer schemaName by slugging annotated class name
		public SchemaAttribute(Type type, string removePrefix = null, string removeSufix = "Model")
			: this($"{SlugHelper.Slug(type, removePrefix, removeSufix)}.json")
		{
		}
	}

	public class NoSchemaAttribute : SchemaAttribute
	{
		public NoSchemaAttribute()
			: base(null)
		{ }
	}
}
