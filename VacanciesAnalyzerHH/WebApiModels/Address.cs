using System.Collections.Generic;

namespace VacanciesAnalyzerHH.Models
{
    public class Address
    {
        public string city { get; set; }
        public string street { get; set; }
        public string building { get; set; }
        public double? lat { get; set; }
        public double? lng { get; set; }
        public object description { get; set; }
        public string raw { get; set; }
        public Metro metro { get; set; }
        public List<MetroStation> metro_stations { get; set; }
        public string id { get; set; }

        public override string ToString()
        {
            return $"{building}, {city}, {street}";
        }
    }


}
