using System.Collections.Generic;

namespace VacanciesAnalyzerHH.Models
{
    public class HHResponse
    {
        public List<Vacancy>? Items { get; set; }
        public int? Found { get; set; }
        public int? Pages { get; set; }
        public int? PerPage { get; set; }
        public int? Page { get; set; }
        public object? Clusters { get; set; }
        public object? Arguments { get; set; }
        public string? AlternateUrl { get; set; }
    }
}
