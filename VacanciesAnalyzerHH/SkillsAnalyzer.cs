using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public class SkillsAnalyzer
    {
        public async Task<IEnumerable<KeyValuePair<string, List<string>>>> GetSkills(ICollection<Vacancy> Vacancies)
        {
            var allSkillsRaw = Vacancies.Where(vacancy => vacancy.snippet != null).Select(vacancy => vacancy.snippet.requirement);

            var skills = new List<string>();
            var dictionaryOfSkills = new Dictionary<string, List<string>>();

            foreach (var skill in allSkillsRaw)
            {
                if (skill == null) continue;

                var s = skill.ToLower().Split(new string[] { ";", "уверенное", "знание", "опыт", "уметь", "знать", ",", ". " }, options: System.StringSplitOptions.TrimEntries);
                skills.AddRange(s);
            }

            skills.RemoveAll(word => string.IsNullOrWhiteSpace(word));

            for (int i = 0; i < skills.Count; i++)
            {
                if(i % 500 == 0)
                {
                    await Task.Delay(10);
                }
               
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

            return dictionaryOfSkills.OrderByDescending(d => d.Value.Count);
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
    }
}
