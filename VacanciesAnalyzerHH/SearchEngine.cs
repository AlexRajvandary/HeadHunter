using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public class SearchEngine : INotifyPropertyChanged
    {
        private readonly ApiClient apiClient;
        private bool isSearchInProgress;
        private string textQuary;
        private SearchResult searchResult;
      
        private bool isSearchCompleted;

        public SearchEngine()
        {
            apiClient = new ApiClient();
            SearchFilter = new SearchFilter();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsSearchCompleted
        {
            get => isSearchCompleted;
            set
            {
                if (isSearchCompleted != value)
                {
                    isSearchCompleted = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public SearchFilter SearchFilter { get; }

        public SearchResult SearchResult
        {
            get => searchResult;
            set
            {
                if(searchResult != value)
                {
                    searchResult = value;
                    OnPropertyChanged();
                }
            }
        }

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

        public void Cancel()
        {
            apiClient?.Cancel();
        }

        public async Task<List<Vacancy>> Search()
        { 
            IsSearchCompleted = false;
            IsSearchInProcess = true;
            SearchResult = new SearchResult();

            var itemsPerPages = SearchFilter.ItemsPerPage;
            var hhResponse = await apiClient.GetVacancies(TextQuary, 0, itemsPerPages);
            var totalNumberOfPages = hhResponse.Pages ?? 0;
            SearchResult.TotalNumberOfVacancies = hhResponse.Found ?? 0;

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

            var pagesOfVacancies = (await Task.WhenAll(tasks))?.Select(hhResponse => hhResponse.Items).ToList();

            foreach (var vacancy in hhResponse.Items)
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

            SearchResult.Vacancies = result;
            IsSearchInProcess = false;
            IsSearchCompleted = true;
            return result;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
