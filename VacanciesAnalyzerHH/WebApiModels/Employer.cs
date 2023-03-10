namespace VacanciesAnalyzerHH.Models
{
    public class Employer
    {
        public string? Id { get; set; }
        public string? Name { get; set; }
        public string? Url { get; set; }
        public string? AlternateUrl { get; set; }
        public LogoUrls? LogoUrls { get; set; }
        public string? VacanciesUrl { get; set; }
        public bool? Trusted { get; set; }
    }
}
