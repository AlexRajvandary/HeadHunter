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
        private readonly CurrencyConverter currencyConverter;

        private Currency selectedCurrency;
        private Vacancy selectedVacancy;
        private IEnumerable<KeyValuePair<string, List<string>>> skills;
        private ObservableCollection<Vacancy> vacancies;

        public MainViewModel()
        {
            currencyConverter = new CurrencyConverter();
            SkillsAnalyzer = new SkillsAnalyzer();
            Vacancies = new ObservableCollection<Vacancy>();
            SalaryVisualizer = new SalaryVisualizer(currencyConverter);
            SearchEngine = new SearchEngine();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public SalaryVisualizer SalaryVisualizer { get; }

        public SearchEngine SearchEngine { get; }

        public Currency SelectedCurrency
        {
            get => selectedCurrency;
            set
            {
                if (selectedCurrency != value)
                {
                    selectedCurrency = value;
                    OnPropertyChanged();
                }
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

        public IEnumerable<KeyValuePair<string, List<string>>> Skills
        {
            get => skills;
            set
            {
                skills = value;
                OnPropertyChanged();
            }
        }

        public SkillsAnalyzer SkillsAnalyzer { get; private set; }

        public ObservableCollection<Vacancy> Vacancies
        {
            get => vacancies;
            set
            {
                vacancies = value;
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
            SalaryVisualizer.Clean();
            Skills = null;
            Vacancies.Clear();

            await foreach (var vacancy in SearchEngine.Search())
            {
                if (vacancy != null)
                {
                    Vacancies.Add(vacancy);
                    await SalaryVisualizer.VisualizeSalary(vacancy);
                }
                else
                {
                    break;
                }
            }

            await Task.Run(() => { Skills = SkillsAnalyzer.GetSkills(Vacancies); });
        }

        private void OnPropertyChanged([CallerMemberName] string paramName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
        }
    }
}
