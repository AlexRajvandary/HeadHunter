using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using VacanciesAnalyzerHH.Models;
using VacanciesAnalyzerHH.SupportServices;
using static VacanciesAnalyzerHH.SkillsAnalyzer;

namespace VacanciesAnalyzerHH
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly CurrencyConverter currencyConverter;
        private int analyzedNumberOfSkills;
        private Currency selectedCurrency;
        private Vacancy selectedVacancy;
        private ObservableCollection<Skill> skills;
        private ObservableCollection<Vacancy> vacancies;
        private int totalNumberOfSkills;

        public MainViewModel()
        {
            currencyConverter = new CurrencyConverter();
            SkillsAnalyzer = new SkillsAnalyzer();
            SalaryVisualizer = new SalaryVisualizer(currencyConverter);
            SearchEngine = new SearchEngine();
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public int AnalyzedNumberOfSkills
        {
            get => analyzedNumberOfSkills;
            set
            {
                analyzedNumberOfSkills = value;
                OnPropertyChanged();
            }
        }

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

        public ObservableCollection<Skill> Skills
        {
            get => skills;
            set
            {
                skills = value;
                OnPropertyChanged();
            }
        } 

        public SkillsAnalyzer SkillsAnalyzer { get; private set; }

        public int TotalNumberOfSkills
        {
            get => totalNumberOfSkills;
            set
            {
                totalNumberOfSkills = value;
                OnPropertyChanged();
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

        public async Task AnalyzeSkills()
        {
            SkillsAnalyzer.IsActive = true;

            await Task.Run(async () =>
            {
                foreach (var vacancy in vacancies)
                {
                    await SkillsAnalyzer.AnalyzeSkills(vacancy);
                }
            });

            Skills = new ObservableCollection<Skill>(SkillsAnalyzer.Skills);
            TotalNumberOfSkills = Skills.Sum(sk => sk.Occurances);
            AnalyzedNumberOfSkills = Skills.Count;
            SkillsAnalyzer.IsActive = false;
        }

        public void Cancel()
        {
            SearchEngine?.Cancel();
            SkillsAnalyzer?.Cancel();
        }

        public void Clear()
        {
            AnalyzedNumberOfSkills = 0;
            TotalNumberOfSkills = 0;
            SalaryVisualizer.Clean();
            SkillsAnalyzer.Skills.Clear();
            Vacancies = null;
        }

        public void ConvertSalaries()
        {
            if (SelectedCurrency != Currency.Unknown)
            {
                foreach (var vacancy in Vacancies)
                {
                    if (vacancy.Salary == null) continue;
                    vacancy.Salary.ConvertTo(SelectedCurrency, currencyConverter);
                }
            }
        }

        public async Task Search()
        {
            Clear();

            Vacancies = new ObservableCollection<Vacancy>();
            var vacancies = await SearchEngine.Search();

            if (vacancies == null)
            {
                return;
            }

            foreach (var vacancy in vacancies)
            {
                if (vacancy != null)
                {
                    Vacancies.Add(vacancy);
                    SalaryVisualizer.VisualizeSalary(vacancy);
                }
                else
                {
                    break;
                }
            }
        }

        private void OnPropertyChanged([CallerMemberName] string paramName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paramName));
        }
    }
}
