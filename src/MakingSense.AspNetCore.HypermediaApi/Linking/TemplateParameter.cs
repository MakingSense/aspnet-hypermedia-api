using System;
using System.Linq;
using System.Linq.Expressions;

namespace MakingSense.AspNetCore.HypermediaApi.Linking
{
	public class TemplateParameter
	{
		public string CustomText { get; private set; }
		public object ForcedValue { get; private set; }
		public bool ForceValue { get; private set; }

		public static TemplateParameter<T> Create<T>() =>
			new TemplateParameter<T>();

		public static TemplateParameter<T> Create<T>(string customText) =>
			new TemplateParameter<T>() { CustomText = customText };

		public static TemplateParameter<T> Force<T>(T value) =>
			new TemplateParameter<T>() { ForcedValue = value, ForceValue = true };

		public override string ToString()
		{
			return ForceValue ? (ForcedValue?.ToString() ?? "/null/") : "{" + (CustomText ?? "template") + "}";
        }
	}

	public class TemplateParameter<T> : TemplateParameter
	{
		public static implicit operator T(TemplateParameter<T> parameter) =>
			default(T);
	}
}
