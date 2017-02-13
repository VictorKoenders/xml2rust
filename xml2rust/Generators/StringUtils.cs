using System.Linq;
using System.Text.RegularExpressions;

namespace xml2rust.Generators
{
	public static class StringUtils
	{
		public static readonly Regex WhitespaceRegex = new Regex("\\s{2,}", RegexOptions.Compiled);
		public static readonly Regex StartingNumericRegex = new Regex("^\\d+", RegexOptions.Compiled);
		public static readonly Regex NumericRegex = new Regex("\\d+", RegexOptions.Compiled);

		public static string ToSingleLine(this string str)
		{
			return WhitespaceRegex.Replace(str, " ").Trim();
		}

		public static bool IsValidFilename(this string str)
		{
			return !StartingNumericRegex.IsMatch(str);
		}

		public static string ToSnakeCase(this string str)
		{
			return str.ToLower();
		}

		public static string ToCamelCase(this string str)
		{
			string[] parts = str.Split('_');
			return string.Join("", parts.Select(p => char.ToUpper(p[0]) + p.Substring(1).ToLower()));
		}
	}
}