﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VacanciesAnalyzerHH
{
    public class SearchFilter : INotifyPropertyChanged
    {
        private bool? isDegreeRequeired;
        private int itemsPerPage = 50;
        private int? salaryLevel;
        private int? requeiredYearsOfExperience;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool? IsDegreeRequeired
        {
            get => isDegreeRequeired;
            set
            {
                isDegreeRequeired = value;
                OnPropertyChanged();
            }
        }

        public int ItemsPerPage
        {
            get => itemsPerPage;
            set
            {
                itemsPerPage = value;
                OnPropertyChanged();
            }
        }

        public int? SalaryLevel
        {
            get => salaryLevel;
            set
            {
                salaryLevel = value;
                OnPropertyChanged();
            }
        }

        public int? RequeiredYearsOfExperience
        {
            get => requeiredYearsOfExperience;
            set
            {
                requeiredYearsOfExperience = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
