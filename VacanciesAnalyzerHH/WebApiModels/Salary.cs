using System.ComponentModel;
using System.Runtime.CompilerServices;
using VacanciesAnalyzerHH.SupportServices;

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
        private string visibleString;

        public Salary(int? from, int? to, string currency, bool? gross)
        {
            From = from;
            To = to;
            Currency = currency;
            Gross = gross;

            CompleteData();
            VisibleString = ToString();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public int? From
        {
            get => from;
            set
            {
                if (from != value)
                {
                    from = value;
                    visibleFrom = from;
                    OnPropertyChanged();
                }
            }
        }

        public int? To
        {
            get => to;
            set
            {
                if (to != value)
                {
                    to = value;
                    visibleTo = to;
                    OnPropertyChanged();
                }
            }
        }

        public string Currency
        {
            get => currency;
            set
            {
                if (currency != value)
                {
                    currency = value;
                    visibleCurrency = currency;
                    OnPropertyChanged();
                }
            }
        }

        public bool? Gross { get; set; }

        public double? VisibleFrom
        {
            get => visibleFrom;
            set
            {
                if (visibleFrom != value)
                {
                    visibleFrom = value;
                    OnPropertyChanged();
                }
            }
        }

        public double? VisibleTo
        {
            get => visibleTo;
            set
            {
                if (visibleTo != value)
                {
                    visibleTo = value;
                    OnPropertyChanged();
                }
            }
        }

        public string VisibleCurrency
        {
            get => visibleCurrency;
            set
            {
                if (visibleCurrency != value)
                {
                    visibleCurrency = value;
                    OnPropertyChanged();
                }
            }
        }

        public string VisibleString
        {
            get => visibleString;
            set
            {
                if (visibleString != value)
                {
                    visibleString = value;
                    OnPropertyChanged();
                }
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
            VisibleString = ToString();
        }

        public override string ToString()
        {
            return $"{VisibleFrom:N} — {VisibleTo:N} {VisibleCurrency}";
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
