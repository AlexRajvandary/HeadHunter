using System.Collections.Generic;

namespace VacanciesAnalyzerHH.Models
{
    public class Address
    {
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Building { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public object? Description { get; set; }
        public string? Raw { get; set; }
        public Metro? Metro { get; set; }
        public List<MetroStation>? MetroStations { get; set; }
        public string? Id { get; set; }

        public override string ToString()
        {
            return $"{Building}, {City}, {Street}";
        }
    }
}
