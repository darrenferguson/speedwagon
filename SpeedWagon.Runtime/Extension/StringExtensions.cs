using System.Globalization;
using System.Text.RegularExpressions;

namespace SpeedWagon.Runtime.Extension
{
    public static class StringExtensions
    {
        public static string ToUrlName(this string s)
        {
            if(string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            s = s.Replace("@", " at ");
            s = s.Replace("-", " ");
            s = new Regex(@"[^a-zA-Z0-9\\. ]").Replace(s, "");
            return s.Replace(" ", "-").ToLower();
        }

        public static string ToTitleCasedName(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            s = s.Replace("@", " at ");
            s = new Regex("[^a-zA-Z0-9\\. ]").Replace(s, "");
            s = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s);
            return s.Replace(" ", "-");
        }

    }
}
