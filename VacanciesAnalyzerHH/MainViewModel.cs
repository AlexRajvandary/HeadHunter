using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using VacanciesAnalyzerHH.Models;
using VacanciesAnalyzerHH.Support_services;

namespace VacanciesAnalyzerHH
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ApiClient apiClient;
        private CurrencyConverter currencyConverter = new CurrencyConverter();
        private string textSearch;
        private ObservableCollection<ObservableCollection<Vacancy>> vacancies;
        private ObservableCollection<Vacancy> currentPageOfVacancies;
        private int totalNumberOfVacancies;
        private Vacancy selectedVacancy;
        private int totalNumberOfPages;
        private int currentNumberOfPage;
        private IEnumerable<KeyValuePair<string, List<string>>> skills;
        private SalaryData salaryData;
        private Currency selectedCurrency;
        private int numOfLoadedVacancies;

        public MainViewModel()
        {
            apiClient = new ApiClient();
            currencyConverter.SetValue(70d, Currency.USD, Currency.RUR);
            currencyConverter.SetValue(431d, Currency.USD, Currency.KZT);
            currencyConverter.SetValue(0.17d, Currency.KZT, Currency.RUR);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public int NumOfLoadedVacancies
        {
            get => numOfLoadedVacancies;
            set
            {
                numOfLoadedVacancies = value;
                OnPropertyChanged();
            }
        }

        public Currency SelectedCurrency
        {
            get => selectedCurrency;
            set
            {
                selectedCurrency = value;
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<ObservableCollection<Vacancy>> PagesOfVacancies
        {
            get => vacancies;
            set
            {
                vacancies = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Vacancy> CurrentPageOfVacancies
        {
            get => currentPageOfVacancies;
            set
            {
                currentPageOfVacancies = value;
                OnPropertyChanged();
            }
        }

        public IEnumerable<KeyValuePair<string, List<string>>> Skills
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

        public SalaryData SalaryData
        {
            get => salaryData;
            set
            {
                salaryData = value;
                OnPropertyChanged();
            }
        }

        public void ConvertSalaries()
        {
            if (SelectedCurrency != null)
            {
                foreach (var page in PagesOfVacancies)
                {
                    foreach (var vacancy in page)
                    {
                        if (vacancy.salary == null) continue;
                        vacancy.salary.ConvertTo(SelectedCurrency, currencyConverter);
                    }
                }
            }
        }

        public async void Search()
        {
            if (string.IsNullOrWhiteSpace(TextSearch))
            {
                return;
            }

            NumOfLoadedVacancies = 0;

            var param = new Dictionary<string, string>();
            param.TryAdd($"text", TextSearch);
            param.TryAdd($"page", 0.ToString());
            param.TryAdd($"per_page", 50.ToString());
            var data = await (await apiClient.GetVacancies(param)).Content.ReadAsStringAsync();
            var deserializedData = JsonConvert.DeserializeObject<HHResponce>(data);
            PagesOfVacancies = new ObservableCollection<ObservableCollection<Vacancy>>();
            TotalNumberOfVacancies = deserializedData.found.Value;
            TotalNumberOfPages = deserializedData.pages.Value;

            var allVacancies = new List<Vacancy>();
            allVacancies.AddRange(deserializedData.items);

            PagesOfVacancies.Add(new ObservableCollection<Vacancy>(deserializedData.items));
            CurrentPageOfVacancies = PagesOfVacancies[0];

            for (int i = 1; i < TotalNumberOfPages; i++)
            {
                var p = new Dictionary<string, string>();
                p.TryAdd($"text", TextSearch);
                p.TryAdd($"page", i.ToString());
                p.TryAdd($"per_page", 50.ToString());
                var d = await (await apiClient.GetVacancies(param)).Content.ReadAsStringAsync();
                var t = JsonConvert.DeserializeObject<HHResponce>(data);

                PagesOfVacancies.Add(new ObservableCollection<Vacancy>(t.items));
                NumOfLoadedVacancies += t.items.Count;
                allVacancies.AddRange(t.items);
            }

            GetSkills();
            SalaryData = new SalaryData(allVacancies, currencyConverter);
        }

        public void GetSkills()
        {
            if (PagesOfVacancies.Count > 0)
            {
                var pagesOfSkills = PagesOfVacancies.Select(v => v.Select(v => v.snippet.requirement));

                var allSkills = new List<string>();

                foreach(var pageOfSkill in pagesOfSkills)
                {
                    allSkills.AddRange(pageOfSkill);
                }

                var t = new List<string>();
                var dict = new Dictionary<string, List<string>>();
                foreach (var skill in allSkills)
                {
                    if (skill == null)
                    {
                        continue;
                    }

                    var s = skill.ToLower().Split(new string[] { ";", "уверенное", "знание", "опыт", "уметь", "знать", ",", ". " }, options: System.StringSplitOptions.TrimEntries);
                    t.AddRange(s);
                }

                t.RemoveAll(w => string.IsNullOrWhiteSpace(w));
                var num = t.Count;
                for (int i = 0; i < t.Count; i++)
                {
                    if (dict.TryAdd(t[i], new List<string> { t[i] }))
                    {
                        for (int j = i + 1; j < t.Count; j++)
                        {
                            if (LevenshteinDistance(t[i], t[j]) < 5 || (t[i].Length >= t[j].Length ? t[i].Contains(t[j]) : t[j].Contains(t[i])))
                            {
                                if (t[i].Length <= 5 && !(t[i].Length >= t[j].Length ? t[i].Contains(t[j]) : t[j].Contains(t[i])))
                                {
                                    continue;
                                }

                                dict[t[i]].Add(t[j]);
                                t.RemoveAt(j);
                                j--;
                            }
                        }
                    }
                    else
                    {
                        continue;
                    }
                }
                var source = dict.OrderByDescending(d => d.Value.Count);
                Skills = source;
            }
        }

        public void GoToPage()
        {
            if(CurrentNumberOfPage > 0 && CurrentNumberOfPage <= TotalNumberOfPages)
            {
                CurrentPageOfVacancies = PagesOfVacancies[CurrentNumberOfPage - 1];
            }
        }

        public int LevenshteinDistance(string string1, string string2)
        {
            if (string1 == null) throw new ArgumentNullException("string1");
            if (string2 == null) throw new ArgumentNullException("string2");
            int diff;
            int[,] m = new int[string1.Length + 1, string2.Length + 1];

            for (int i = 0; i <= string1.Length; i++) { m[i, 0] = i; }
            for (int j = 0; j <= string2.Length; j++) { m[0, j] = j; }

            for (int i = 1; i <= string1.Length; i++)
            {
                for (int j = 1; j <= string2.Length; j++)
                {
                    diff = (string1[i - 1] == string2[j - 1]) ? 0 : 1;

                    m[i, j] = Math.Min(Math.Min(m[i - 1, j] + 1, m[i, j - 1] + 1), m[i - 1, j - 1] + diff);
                }
            }
            return m[string1.Length, string2.Length];
        }

        private void OnPropertyChanged([CallerMemberName] string paramName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
        }
    }
}
