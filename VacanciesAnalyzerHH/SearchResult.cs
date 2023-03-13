using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public class SearchResult : INotifyPropertyChanged
    {
        private int totalNumberOfVacancies;
        private int numOfLoadedVacancies;

        public event PropertyChangedEventHandler? PropertyChanged;

        public int NumOfLoadedVacancies
        {
            get => numOfLoadedVacancies;
            set
            {
                if (numOfLoadedVacancies != value)
                {
                    numOfLoadedVacancies = value;
                    OnPropertyChanged();
                }
            }
        }

        public int TotalNumberOfVacancies
        {
            get => totalNumberOfVacancies;
            set
            {
                if (totalNumberOfVacancies != value)
                {
                    totalNumberOfVacancies = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Vacancy> Vacancies { get; set; }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
