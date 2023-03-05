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
        private double salaryStep = 10000;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SalaryData(List<Vacancy> vacancies)
        {
            Analyze(vacancies);
            Visualize();
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

        private void Analyze(List<Vacancy> vacancies)
        {
            var source = vacancies.Where(vac => vac.salary != null).ToList();

            Data = source.Select(vacancy => (vacancy.salary, vacancy))
                         .OrderBy(vac => vac.salary.From)
                         .ToList();

            Max = source.OrderByDescending(vacancy => vacancy.salary.To).First().salary;
            Min = source.OrderBy(vacancy => vacancy.salary.From).First().salary;
        }

        private void Visualize()
        {
            if (Data == null || Data.Count == 0 || Max == null || Min == null)
            {
                return;
            }

            SeriesCollection = new SeriesCollection();
            var numOfSections = (int)Math.Ceiling(Max.To.Value / salaryStep);

            Labels = new string[numOfSections];
            var quantaties = new int[numOfSections];

            for (int i = 0; i < numOfSections; i++)
            {
                Labels[i] = $"{i * salaryStep} - {(i + 1) * salaryStep}";
            }

            foreach (var vac in Data)
            {
                var sectionNumber = Math.Ceiling((double)vac.Item1.From / salaryStep) - 1;
                quantaties[(int)sectionNumber]++;
            }

            SeriesCollection.Add(new ColumnSeries
            {
                Values = new ChartValues<int>(quantaties)
            });
        }
    }
}
