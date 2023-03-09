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

        public ObservableCollection<Skill> Skills { get; set; } = new ObservableCollection<Skill>();

        public async Task AnalyzeSkills(Vacancy vacancy)
        {
            if (string.IsNullOrWhiteSpace(vacancy?.snippet?.requirement))
            {
                return;
            }

            var vacancySkills = GetSkills(vacancy.snippet.requirement).ToList();
            vacancySkills.RemoveAll(st => st == string.Empty);

            foreach (var skill in vacancySkills)
            {
                await CompareSkills(skill).ConfigureAwait(true);
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

        private async Task<(bool, string)> AreSkillsEqual(string skill, string key)
        {
            return await Task.Run(() =>
            {
                var isEqual = (LevenshteinDistance(skill, key) < 5 || (skill.Length >= key.Length ? skill.Contains(key) : key.Contains(skill))) && skill.Length <= 5 && !(skill?.Length >= key?.Length ? skill?.Contains(key) ?? false : key?.Contains(skill) ?? false);
                return (isEqual, key);
            }).ConfigureAwait(true);
        }

        private async Task CompareSkills(string skill)
        {
            var tasks = new List<Task<(bool, string)>>();

            if (Skills.Count == 0)
            {
                Skills.Add(new Skill() { Value = skill, Occurances = 1 });
                return;
            }

            foreach (var s in Skills)
            {
                tasks.Add(AreSkillsEqual(skill, s.Value));
            }

            var results = await Task.WhenAll(tasks).ConfigureAwait(true);
            var contains = results.Any(res => res.Item1 == true);

            if (contains)
            {
                var key = results.First(res => res.Item1 == true).Item2;
                Skills.First(skill => skill.Value == key).Occurances++;
            }
            else
            {
                Skills.Add(new Skill() { Occurances = 1, });
            }
        }

        private string[] GetSkills(string vacancyRequirements)
        {
            vacancyRequirements = vacancyRequirements.ToLower();
            return vacancyRequirements.Split(delimeters, options: System.StringSplitOptions.TrimEntries);
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