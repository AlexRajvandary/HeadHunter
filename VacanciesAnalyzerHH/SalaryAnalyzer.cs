﻿using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using VacanciesAnalyzerHH.Models;
using VacanciesAnalyzerHH.Support_services;
using LiveCharts;
using LiveCharts.Wpf;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace VacanciesAnalyzerHH
{
    public class SalaryVisualizer : INotifyPropertyChanged
    {
        private CurrencyConverter currencyConverter;
        private List<(Salary, Vacancy)> data;
        private double? max;
        private double? min;
        private double salaryStep = 10000;

        public event PropertyChangedEventHandler? PropertyChanged;

        public SalaryVisualizer(CurrencyConverter currencyConverter)
        {
            this.currencyConverter = currencyConverter;
            SeriesCollection = new SeriesCollection();
            SeriesCollection.Add(new ColumnSeries
            {
                Values = new ChartValues<int>()
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

        public double? Max
        {
            get => max;
            set
            {
                max = value;
                OnPropertyChanged();
            }
        }

        public double? Min
        {
            get => min;
            set
            {
                min = value;
                OnPropertyChanged();
            }
        }

        public SeriesCollection SeriesCollection { get; set; }

        public ObservableCollection<string> Labels { get; set; }

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task VisualizeSalary(Vacancy vacancy)
        {
            if (vacancy == null) return;
            if (vacancy.salary == null) return;

            vacancy.salary.ConvertTo(Currency.RUR, currencyConverter);
            var section = (int)(vacancy.salary.VisibleFrom / salaryStep);

            if (SeriesCollection[0].Values.Count > section)
            {
                SeriesCollection[0].Values[section] = (int)SeriesCollection[0].Values[section] + 1;
            }
            else
            {
                var r = section - SeriesCollection[0].Values.Count + 1;
                for (var i = 0; i < r; i++)
                {
                    if (i < r - 1)
                    {
                        SeriesCollection[0].Values.Add(0);
                    }
                    else
                    {
                        SeriesCollection[0].Values.Add(1);
                    }

                    await Task.Delay(10);
                }
            }
        }
    }
}
