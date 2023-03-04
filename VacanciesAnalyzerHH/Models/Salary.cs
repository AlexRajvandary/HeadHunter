namespace VacanciesAnalyzerHH.Models
{
    public class Salary
    {
        public int? from { get; set; }
        public int? to { get; set; }
        public string currency { get; set; }
        public bool? gross { get; set; }

        public override string ToString()
        {
            return to.HasValue ? $"{from} - {to} {currency}" : $"{from} {currency}";
        }
    }
}
