using Newtonsoft.Json;

namespace VacanciesAnalyzerHH.Models
{
    public class LogoUrls
    {
        public string original { get; set; }

        [JsonProperty("90")]
        public string _90 { get; set; }

        [JsonProperty("240")]
        public string _240 { get; set; }
    }
}
