using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
        private SkillsAnalyzer skillsAnalyzer;
        private SalaryVisualizer salaryAnalyzer;
        private Currency selectedCurrency;
        private int numOfLoadedVacancies;

        public MainViewModel()
        {
            apiClient = new ApiClient();
            currencyConverter.SetValue(70d, Currency.USD, Currency.RUR);
            currencyConverter.SetValue(431d, Currency.USD, Currency.KZT);
            currencyConverter.SetValue(0.17d, Currency.KZT, Currency.RUR);

            skillsAnalyzer = new SkillsAnalyzer();
            Vacancies = new ObservableCollection<Vacancy>();
            SalaryVisualizer = new SalaryVisualizer(currencyConverter);
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

        public SalaryVisualizer SalaryVisualizer
        {
            get => salaryAnalyzer;
            set
            {
                salaryAnalyzer = value;
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

            SalaryVisualizer.Clean();
            Vacancies.Clear();
            NumOfLoadedVacancies = 0;
            
            var itemsPerPage = 100;
            var hhResponse = await apiClient.GetVacancies(TextSearch, 0, itemsPerPage);
            var totalNumberOfPages = hhResponse.pages ?? 0;

            TotalNumberOfVacancies = hhResponse.found ?? 0;

            if (hhResponse == null || totalNumberOfPages == 0)
            {
                return;
            }

            foreach (var vacancy in hhResponse.items)
            {
                Vacancies.Add(vacancy);
                await SalaryVisualizer.VisualizeSalary(vacancy);
                NumOfLoadedVacancies++;
                await Task.Delay(10);
            }

            for (int i = 1; i < totalNumberOfPages; i++)
            {
                hhResponse = await apiClient.GetVacancies(TextSearch, i, itemsPerPage);

                foreach (var vacancy in hhResponse.items)
                {
                    Vacancies.Add(vacancy);
                    await SalaryVisualizer.VisualizeSalary(vacancy);
                    NumOfLoadedVacancies++;
                    await Task.Delay(10);
                }
            }

            Skills = await skillsAnalyzer.GetSkills(Vacancies);
        }

        private void OnPropertyChanged([CallerMemberName] string paramName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
        }
    }
}
