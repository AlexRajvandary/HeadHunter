namespace VacanciesAnalyzerHH.Support_services
{
    public static class CurrencyExtension
    {
        public static string CurrencyToString(this Currency currency)
        {
            return currency switch
            {
                Currency.USD => "USD",
                Currency.RUR => "RUR",
                Currency.KZT => "KZT",
                _ => currency.ToString(),
            };
        }

        public static Currency StringToCurrency(string currencyStr)
        {
            if (string.IsNullOrWhiteSpace(currencyStr)) return Currency.Unknown;

            return currencyStr switch
            {
                "USD" => Currency.USD,
                "RUR" => Currency.RUR,
                "KZT" => Currency.KZT,
                _ => Currency.Unknown
            };
        }
    }
}
