using System.Text.Json;

namespace VacanciesAnalyzerHH.SupportServices
{
    public class PascalCaseNamingPolicy : JsonNamingPolicy
    {
        public static PascalCaseNamingPolicy Instance { get; } = new PascalCaseNamingPolicy();

        public override string ConvertName(string name)
        {
            return name.ToPascalCase();
        }
    }
}
