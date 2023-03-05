using System.Collections.Generic;

namespace VacanciesAnalyzerHH.Support_services
{
    public class CurrencyConverter
    {
        public Dictionary<(Currency from, Currency to), double> CurrencyPairsValues { get; } = new Dictionary<(Currency from, Currency to), double>();

        public bool TryConvert(double value,Currency from, Currency to, out double? result)
        {
            try
            {
                result = Convert(value, from, to);
            }
            catch
            {
                result = null;
            }
           
            return result != null;
        }

        public double? Convert(double value, Currency from, Currency to)
        {
            if(value < 0) return null;

            if (value == 0) return 0;

            if (from == to) return value;

            if (CurrencyPairsValues?.TryGetValue((from, to), out double price) ?? false)
            {
                return value * price;
            }
            else
            {
                return null;
            }
        }

        public void SetValue(double value, Currency from, Currency to)
        {
            if(value <= 0) return;

            if (from == to) return;

            if(!CurrencyPairsValues.TryAdd((from, to), value))
            {
                CurrencyPairsValues[(from, to)] = value;
            }

            if (!CurrencyPairsValues.TryAdd((to, from), 1 / value))
            {
                CurrencyPairsValues[(from, to)] = 1 / value;
            }
        }
    }
}
