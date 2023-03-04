using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ApiClient apiClient;
        private string textSearch;
        private ObservableCollection<Vacancy> vacancies;
        private int totalNumberOfVacancies;
        private Vacancy selectedVacancy;
        private int totalNumberOfPages;
        private int currentNumberOfPage;
        private ObservableCollection<string> skills;
        private List<string> requirements;
        private List<string> responsibilities;

        public MainViewModel()
        {
            apiClient = new ApiClient();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string TextSearch
        {
            get => textSearch;
            set
            {
                if (textSearch != value)
                {
                    textSearch = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<Vacancy> Vacancies
        {
            get => vacancies;
            set
            {
                vacancies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<string> Skills
        {
            get => skills;
            set
            {
                skills = value;
                OnPropertyChanged();
            }
        }

        public Vacancy SelectedVacancy
        {
            get => selectedVacancy;
            set
            {
                selectedVacancy = value;
                OnPropertyChanged();
            }
        }

        public int TotalNumberOfVacancies
        {
            get => totalNumberOfVacancies;
            set
            {
                totalNumberOfVacancies = value;
                OnPropertyChanged();
            }
        }

        public int TotalNumberOfPages
        {
            get => totalNumberOfPages;
            set
            {
                totalNumberOfPages = value;
                OnPropertyChanged();
            }
        }

        public int CurrentNumberOfPage
        {
            get => currentNumberOfPage;
            set
            {
                currentNumberOfPage = value;
                OnPropertyChanged();
            }
        }

        public async void Search()
        {
            if (string.IsNullOrWhiteSpace(TextSearch))
            {
                return;
            }

            var param = new Dictionary<string, string>();
            param.TryAdd($"text", TextSearch);
            param.TryAdd($"page", CurrentNumberOfPage.ToString());
            param.TryAdd($"per_page", 50.ToString());
            var data = await (await apiClient.GetVacancies(param)).Content.ReadAsStringAsync();
            var deserializedData = JsonConvert.DeserializeObject<HHResponce>(data);
            Vacancies = new ObservableCollection<Vacancy>(deserializedData.items);
            TotalNumberOfVacancies = deserializedData.found.Value;
            TotalNumberOfPages = deserializedData.pages.Value;

            if (Vacancies.Count > 0)
            {
                var skills = Vacancies.Select(v => v.snippet.requirement);
                var t = new List<string>();
                foreach (var skill in skills)
                {
                    if (skill == null)
                    {
                        continue;
                    }

                    var s = skill.Split(new string[] { ";", "Знание", "," }, options: System.StringSplitOptions.RemoveEmptyEntries);
                    t.AddRange(s);
                }

                Skills = new ObservableCollection<string>(t);
            }
        }

        private void OnPropertyChanged([CallerMemberName] string paramName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
        }
    }
}
