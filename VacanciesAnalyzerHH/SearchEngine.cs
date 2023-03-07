using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public class SearchEngine : INotifyPropertyChanged
    {
        private readonly ApiClient apiClient;
        private int numOfLoadedVacancies;
        private string textQuary;
        private int totalNumberOfVacancies;

        public SearchEngine()
        {
            apiClient = new ApiClient();
            SearchFilter = new SearchFilter();
        }

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
            var itemsPerPages = SearchFilter.ItemsPerPage;
            var hhResponse = await apiClient.GetVacancies(TextQuary, 0, itemsPerPages);
            var totalNumberOfPages = hhResponse.pages ?? 0;
            TotalNumberOfVacancies = hhResponse.found ?? 0;

            for (var i = 0; i < totalNumberOfPages; i++)
            {
                List<Vacancy> vacancies;

                if (i == 0)
                {
                    vacancies = hhResponse.items;
                }
                else
                {
                    vacancies = (await apiClient.GetVacancies(TextQuary, i, itemsPerPages)).items;
                }

                foreach (var vacancy in vacancies)
                {
                    NumOfLoadedVacancies++;
                    await Task.Delay(10);
                    yield return vacancy;
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
