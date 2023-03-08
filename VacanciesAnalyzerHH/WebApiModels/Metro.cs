namespace VacanciesAnalyzerHH.Models
{
    public class Metro
    {
        public string station_name { get; set; }
        public string line_name { get; set; }
        public string station_id { get; set; }
        public string line_id { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
    }
}
