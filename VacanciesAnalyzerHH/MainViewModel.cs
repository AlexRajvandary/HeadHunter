using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using VacanciesAnalyzerHH.Models;
using VacanciesAnalyzerHH.Support_services;

namespace VacanciesAnalyzerHH
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private ApiClient apiClient;
        private CurrencyConverter currencyConverter = new CurrencyConverter();
        private string textSearch;
        private ObservableCollection<Vacancy> vacancies;
        private int totalNumberOfVacancies;
        private Vacancy selectedVacancy;
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

            Vacancies = new ObservableCollection<Vacancy>();
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

        public ObservableCollection<Vacancy> Vacancies
        {
            get => vacancies;
            set
            {
                vacancies = value;
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
            if (SelectedCurrency != Currency.Unknown)
            {
                foreach (var vacancy in Vacancies)
                {
                    if (vacancy.salary == null) continue;
                    vacancy.salary.ConvertTo(SelectedCurrency, currencyConverter);
                }
            }
        }

        public async Task Search()
        {
            if (string.IsNullOrWhiteSpace(TextSearch))
            {
                return;
            }

            var itemsPerPage = 100;
            var hhResponse = await apiClient.GetVacancies(TextSearch, 0, itemsPerPage);
            var totalNumberOfPages = hhResponse.pages ?? 0;

            if (hhResponse == null)
            {
                return;
            }

            Vacancies.Clear();
            NumOfLoadedVacancies = 0;
            TotalNumberOfVacancies = hhResponse.found ?? 0;

            foreach (var vacancy in hhResponse.items)
            {
                Vacancies.Add(vacancy);
            }

            NumOfLoadedVacancies += hhResponse.items.Count;

            for (int i = 1; i < totalNumberOfPages; i++)
            {
                hhResponse = await apiClient.GetVacancies(TextSearch, i, itemsPerPage);

                foreach (var vacancy in hhResponse.items)
                {
                    Vacancies.Add(vacancy);
                    NumOfLoadedVacancies++;
                }
            }

            GetSkills();
            SalaryData = new SalaryData(Vacancies.ToList(), currencyConverter);
        }

        private void GetSkills()
        {
            var allSkillsRaw = Vacancies.Where(vacancy => vacancy.snippet != null).Select(vacancy => vacancy.snippet.requirement);

            var skills = new List<string>();
            var dictionaryOfSkills = new Dictionary<string, List<string>>();

            foreach (var skill in allSkillsRaw)
            {
                var s = skill.ToLower().Split(new string[] { ";", "уверенное", "знание", "опыт", "уметь", "знать", ",", ". " }, options: System.StringSplitOptions.TrimEntries);
                skills.AddRange(s);
            }

            skills.RemoveAll(word => string.IsNullOrWhiteSpace(word));
            
            for (int i = 0; i < skills.Count; i++)
            {
                if (dictionaryOfSkills.TryAdd(skills[i], new List<string> { skills[i] }))
                {
                    for (int j = i + 1; j < skills.Count; j++)
                    {
                        if (LevenshteinDistance(skills[i], skills[j]) < 5 || (skills[i].Length >= skills[j].Length ? skills[i].Contains(skills[j]) : skills[j].Contains(skills[i])))
                        {
                            if (skills[i].Length <= 5 && !(skills[i].Length >= skills[j].Length ? skills[i].Contains(skills[j]) : skills[j].Contains(skills[i])))
                            {
                                continue;
                            }

                            dictionaryOfSkills[skills[i]].Add(skills[j]);
                            skills.RemoveAt(j);
                            j--;
                        }
                    }
                }
                else
                {
                    continue;
                }
            }

            var source = dictionaryOfSkills.OrderByDescending(d => d.Value.Count);
            Skills = source;
        }

        private static int LevenshteinDistance(string string1, string string2)
        {
            if (string1 == null) throw new ArgumentNullException(nameof(string1));
            if (string2 == null) throw new ArgumentNullException(nameof(string2));
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
