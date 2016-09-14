using Microsoft.Framework.Internal;
using System;
using System.Text.RegularExpressions;

namespace MakingSense.AspNetCore.Utilities
{
    public static class SlugHelper
    {
		// TODO: improve performance and add a new method to revert it
		public static string Slug(string text, string removePrefix = null, string removeSufix = null)
		{
			if (removePrefix != null)
			{
				if (text.StartsWith(removePrefix, StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(removePrefix.Length);
				}
			}

			if (removeSufix != null)
			{
				if (text.EndsWith(removeSufix, StringComparison.OrdinalIgnoreCase))
				{
					text = text.Substring(0, text.Length - removeSufix.Length);
				}
			}

			return string.Join("-", Regex.Split(text.Replace(",", ""), "(?<=[a-z])(?=[A-Z])")).ToLower();
        }

		public static string Slug([NotNull] Type type, string removePrefix = null, string removeSufix = null)
		{
			return Slug(type.Name, removePrefix, removeSufix);
		}
	}
}
