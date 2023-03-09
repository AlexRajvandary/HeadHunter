using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Documents;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public class SearchEngine : INotifyPropertyChanged
    {
        private readonly ApiClient apiClient;
        private bool isSearchInProgress;
        private int numOfLoadedVacancies;
        private string textQuary;
        private int totalNumberOfVacancies;

        public SearchEngine()
        {
            apiClient = new ApiClient();
            SearchFilter = new SearchFilter();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsSearchInProcess
        {
            get => isSearchInProgress;
            set
            {
                if (isSearchInProgress != value)
                {
                    isSearchInProgress = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public SearchFilter SearchFilter { get; }

        public string TextQuary
        {
            get => textQuary;
            set
            {
                if (textQuary != value)
                {
                    textQuary = value;
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

        public void Cancel()
        {
            apiClient?.Cancel();
        }

        public async Task<List<Vacancy>> Search()
        {
            TotalNumberOfVacancies = 0;
            NumOfLoadedVacancies = 0;
            IsSearchInProcess = true;

            var itemsPerPages = SearchFilter.ItemsPerPage;
            var hhResponse = await apiClient.GetVacancies(TextQuary, 0, itemsPerPages);
            var totalNumberOfPages = hhResponse.pages ?? 0;
            TotalNumberOfVacancies = hhResponse.found ?? 0;

            if (totalNumberOfPages == 0)
            {
                IsSearchInProcess = false;
                return null;
            }

            var tasks = new List<Task<HHResponse>>();
            var result = new List<Vacancy>();

            for (var i = 1; i < totalNumberOfPages; i++)
            {
                tasks.Add(apiClient.GetVacancies(TextQuary, i, itemsPerPages));
            }

            var pagesOfVacancies = (await Task.WhenAll(tasks))?.Select(hhResponse => hhResponse.items).ToList();

            foreach (var vacancy in hhResponse.items)
            {
                result.Add(vacancy);
            }

            if (pagesOfVacancies == null)
            {
                return null;
            }
            else
            {
                foreach (var pageOfVacancies in pagesOfVacancies)
                {
                    if (pageOfVacancies == null)
                    {
                        continue;
                    }

                    foreach (var vacancy in pageOfVacancies)
                    {
                        result.Add(vacancy);
                    }
                }
            }

            IsSearchInProcess = false;
            return result;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
