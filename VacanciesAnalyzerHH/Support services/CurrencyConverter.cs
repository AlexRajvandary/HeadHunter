using System.Collections.Generic;

namespace VacanciesAnalyzerHH.Support_services
{
    public class CurrencyConverter
    {
        private Dictionary<(Currency from, Currency to), double> CurrencyPairsValues { get; } = new Dictionary<(Currency from, Currency to), double>();

        public CurrencyConverter()
        {
            SetValue(70d, Currency.USD, Currency.RUR);
            SetValue(431d, Currency.USD, Currency.KZT);
            SetValue(0.95d, Currency.USD, Currency.EUR);
            SetValue(0.84d, Currency.USD, Currency.GBP);
            SetValue(80.39d, Currency.EUR, Currency.RUR);
            SetValue(462.39d, Currency.EUR, Currency.KZT);
            SetValue(0.89d, Currency.EUR, Currency.GBP);
            SetValue(90.24d, Currency.RUR, Currency.GBP);
            SetValue(0.17d, Currency.KZT, Currency.RUR);
            SetValue(0.0019d, Currency.KZT, Currency.GBP);
        }

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
