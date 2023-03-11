using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public partial class SkillsAnalyzer : INotifyPropertyChanged, IDisposable
    {
        private CancellationTokenSource? cancellationTokenSource;
        private string[] delimeters = new string[] { ";", "уверенное", "знание", "опыт", "уметь", "знать", ",", ". " };
        private bool isActive;

        public event PropertyChangedEventHandler? PropertyChanged;

        public bool IsActive
        {
            get => isActive;
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged();
                }
            }
        }

        public List<Skill> Skills { get; set; } = new List<Skill>();

        public async Task AnalyzeSkills(Vacancy vacancy)
        {
            if (string.IsNullOrWhiteSpace(vacancy?.Snippet?.Requirement))
            {
                return;
            }

            var vacancySkills = GetSkills(vacancy.Snippet.Requirement).ToList();
            
            foreach (var skill in vacancySkills)
            {
                var resOfComparing = await CompareSkills(skill);

                if(resOfComparing.Item1)
                {
                    Skills.Add(new Skill() { Value = skill, Occurances = 1});
                }
                else
                {
                    Skills.First(s => s.Value == resOfComparing.Item2).Occurances++;
                }
            }
        }

        public void Cancel()
        {
            cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
            cancellationTokenSource?.Dispose();
            cancellationTokenSource = null;
        }

        private (bool, string, string) AreSkillsEqual(string skill, string key)
        {
            var isEqual = skill.Length > key.Length ? skill.Contains(key) : key.Contains(skill);
            return (isEqual, skill, key);
        }

        private async Task<(bool, string)> CompareSkills(string skillToCampare)
        {
            var tasks = new List<Task<(bool, string, string)>>();

            if (Skills.Count == 0)
            {
                return (true, string.Empty);
            }

            foreach (var skill in Skills)
            {
                tasks.Add(Task.Run(() => AreSkillsEqual(skillToCampare, skill.Value)));
            }

            var results = await Task.WhenAll(tasks);
            var isSkillNew = !results.Any(res => res.Item1 == true);
            var key = "";

            if (!isSkillNew)
            {
                key = results.First(res => res.Item1 == true).Item3;
            }
            return (isSkillNew, key);
        }

        private IEnumerable<string> GetSkills(string vacancyRequirements)
        {
            vacancyRequirements = vacancyRequirements.ToLower();
            return vacancyRequirements.Split(delimeters, StringSplitOptions.RemoveEmptyEntries).ToList().Where(str => !string.IsNullOrWhiteSpace(str));
        }

        private static int LevenshteinDistance(string string1, string string2)
        {
            if (string1 == null) return 0;
            if (string2 == null) return 0;
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

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}