using System.Globalization;
using System;
using System.Linq;

namespace VacanciesAnalyzerHH.SupportServices
{
    public static class StringUtils
    {
        public static string ToSnakeCase(this string str)
        {
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
        }

        public static string ToPascalCase(this string str)
        {
            var yourString = str.ToLower().Replace("_", " ");
            var info = CultureInfo.CurrentCulture.TextInfo;
            yourString = info.ToTitleCase(yourString).Replace(" ", string.Empty);
            return yourString;
        }
    }
}
