using Newtonsoft.Json.Serialization;

namespace VacanciesAnalyzerHH.SupportServices
{
    public class SnakeCaseContractResolver : DefaultContractResolver
    {
        protected override string ResolvePropertyName(string propertyName)
        {
            return propertyName.ToSnakeCase();
        }
    }
}
