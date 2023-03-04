using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public class SalaryData : INotifyPropertyChanged
    {
        private List<(Salary, Vacancy)> data;
        private Salary max;
        private Salary min;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SalaryData(List<Vacancy> vacancies)
        {
            var source = vacancies.Where(vac => vac.salary?.from != null && vac.salary.currency == "RUR").ToList();

            source.ForEach(vac =>
            {
                if (vac.salary.from == null)
                {
                    vac.salary.from = vac.salary.to;
                }
                else
                {
                    vac.salary.to = vac.salary.from;
                }
            });

            Data = source.Select(vacancy => (vacancy.salary, vacancy)).ToList();
            Data = Data.OrderBy(vac => vac.Item1.from).ToList();

            Max = source.OrderByDescending(vacancy => vacancy.salary.to).First().salary;
            Min = source.Where(vac => vac.salary?.from != null).OrderBy(vacancy => vacancy.salary.from).First().salary;

            SeriesCollection = new SeriesCollection();
            var quantaties = new int[6];
            var section = (double)Max.to / 6;
            Labels = new string[]
            {
                $"0 - {section:0}",
                $"{section:0} - {2 * section:0}",
                $"{2 * section:0} - {3 * section:0}",
                $"{3 * section :0} - {4 * section :0}",
                $"{4 * section :0} - {5 * section :0}",
                $"{5 * section :0} - {6 * section :0}",
            };

            foreach (var vac in Data)
            {
                var sectionNumber = Math.Ceiling((double)vac.Item1.from / section) - 1;
                quantaties[(int)sectionNumber]++;
            }

            SeriesCollection.Add(new ColumnSeries
            {
                Values = new ChartValues<int>(quantaties)
            });
        }

        public List<(Salary, Vacancy)> Data
        {
            get => data;
            private set
            {
                data = value;
                OnPropertyChanged();
            }
        }

        public Salary Max
        {
            get => max;
            set
            {
                max = value;
                OnPropertyChanged();
            }
        }

        public Salary Min
        {
            get => min;
            set
            {
                min = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection SeriesCollection { get; set; }

        public string[] Labels { get; set; }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
