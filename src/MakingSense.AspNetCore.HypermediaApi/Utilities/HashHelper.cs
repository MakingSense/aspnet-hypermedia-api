using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace MakingSense.AspNetCore.Utilities
{
	public static class HashHelper
	{
		public static string CaseInsensitiveTrimmedMd5(string text, Encoding inputEncoding = null)
		{
			if (string.IsNullOrWhiteSpace(text))
			{
				return string.Empty;
			}

			inputEncoding = inputEncoding ?? Encoding.UTF8;
			text = text.Trim().ToLower();
			var inputBytes = inputEncoding.GetBytes(text);

			using (var md5 = MD5.Create())
			{
				var outputBytes = md5.ComputeHash(inputBytes);
				return string.Concat(outputBytes.Select(x => x.ToString("X2")));
			}
		}
	}
}
