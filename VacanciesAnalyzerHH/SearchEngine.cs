using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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
                if(isSearchInProgress != value)
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

        public async IAsyncEnumerable<Vacancy> Search()
        {
            IsSearchInProcess = true;

            var itemsPerPages = SearchFilter.ItemsPerPage;
            var hhResponse = await apiClient.GetVacancies(TextQuary, 0, itemsPerPages);
            var totalNumberOfPages = hhResponse.pages ?? 0;
            TotalNumberOfVacancies = hhResponse.found ?? 0;

            if (totalNumberOfPages == 0)
            {
                yield return null;
            }
            var tasks = new List<Task<HHResponse>>();

            for (var i = 1; i < totalNumberOfPages; i++)
            {
                tasks.Add(apiClient.GetVacancies(TextQuary, i, itemsPerPages));
            }

            var vacancies = (await Task.WhenAll(tasks))?.Select(hhResponse => hhResponse.items);

            foreach (var item in hhResponse.items)
            {
                NumOfLoadedVacancies++;

                
                yield return item;
            }

            if (vacancies == null)
            {
                yield return null;
            }
            else
            {
                foreach (var vac in vacancies)
                {
                    if (vac == null)
                    {
                        continue;
                    }

                    foreach (var v in vac)
                    {
                        NumOfLoadedVacancies++;

                        if (NumOfLoadedVacancies % 10 == 0)
                        {
                            await Task.Delay(1);
                        }

                        yield return v;
                    }
                }
            }

            IsSearchInProcess = false;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
