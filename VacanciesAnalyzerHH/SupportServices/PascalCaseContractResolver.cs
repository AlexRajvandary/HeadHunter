using Newtonsoft.Json.Serialization;

namespace VacanciesAnalyzerHH.SupportServices
{
    public class PascalCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToPascalCase();
        }
    }
}
