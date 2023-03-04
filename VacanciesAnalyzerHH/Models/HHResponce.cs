using System.Collections.Generic;

namespace VacanciesAnalyzerHH.Models
{
    public class HHResponce
    {
        public List<Vacancy> items { get; set; }
        public int? found { get; set; }
        public int? pages { get; set; }
        public int? per_page { get; set; }
        public int? page { get; set; }
        public object clusters { get; set; }
        public object arguments { get; set; }
        public string alternate_url { get; set; }
    }
}
