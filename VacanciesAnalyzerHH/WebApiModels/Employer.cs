namespace VacanciesAnalyzerHH.Models
{
    public class Employer
    {
        public string id { get; set; }
        public string name { get; set; }
        public string url { get; set; }
        public string alternate_url { get; set; }
        public LogoUrls logo_urls { get; set; }
        public string vacancies_url { get; set; }
        public bool? trusted { get; set; }
    }
}
