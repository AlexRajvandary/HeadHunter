using Newtonsoft.Json;
using System;
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

            GetSkills();
        }

        public void GetSkills()
        {
            if (Vacancies.Count > 0)
            {
                var skills = Vacancies.Select(v => v.snippet.requirement);
                var t = new List<string>();
                var dict = new Dictionary<string, List<string>>();
                foreach (var skill in skills)
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

                Skills = new ObservableCollection<string>(t);
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
