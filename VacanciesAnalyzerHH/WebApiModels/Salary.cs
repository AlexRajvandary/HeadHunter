using System.ComponentModel;
using System.Runtime.CompilerServices;
using VacanciesAnalyzerHH.Support_services;

namespace VacanciesAnalyzerHH.Models
{
    public class Salary : INotifyPropertyChanged
    {
        private int? from;
        private int? to;
        private string currency;
        private double? visibleFrom;
        private double? visibleTo;
        private string visibleCurrency;

        public Salary(int? from, int? to, string currency, bool? gross)
        {
            From = from;
            To = to;
            Currency = currency;
            Gross = gross;

            CompleteData();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public int? From
        {
            get => from;
            set
            {
                from = value;
                visibleFrom = from;
            }
        }

        public int? To
        {
            get => to;
            set
            {
                to = value;
                visibleTo = to;
            }
        }

        public string Currency
        {
            get => currency;
            set
            {
                currency = value;
                visibleCurrency = currency;
            }
        }

        public bool? Gross { get; set; }

        public double? VisibleFrom
        {
            get => visibleFrom;
            set
            {
                visibleFrom = value;
                OnPropertyChanged();
            }
        }

        public double? VisibleTo
        {
            get => visibleTo;
            set
            {
                visibleTo = value;
                OnPropertyChanged();
            }
        }

        public string VisibleCurrency
        {
            get => visibleCurrency;
            set
            {
                visibleCurrency = value;
                OnPropertyChanged();
            }
        }

        public void ConvertTo(Currency to, CurrencyConverter currencyConverter)
        {
            if (From == null) return;

            if (currencyConverter.TryConvert((double)From, CurrencyExtension.StringToCurrency(Currency), to, out var resultFrom))
            {
                VisibleFrom = resultFrom;
            }
            else
            {
                VisibleFrom = 0;
            }

            if (currencyConverter.TryConvert((double)To, CurrencyExtension.StringToCurrency(Currency), to, out var resultTo))
            {
                VisibleTo = resultTo;
            }
            else
            {
                VisibleTo = 0;
            }

            VisibleCurrency = to.CurrencyToString();
        }

        public override string ToString()
        {
            return To.HasValue ? $"{From} - {To} {Currency}" : $"{From} {Currency}";
        }

        private void CompleteData()
        {
            if (!IsDataIncompete())
            {
                return;
            }

            From ??= To;
            To ??= From;
        }

        private bool IsDataIncompete()
        {
            return From == null || To == null;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
