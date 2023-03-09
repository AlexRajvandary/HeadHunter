using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace VacanciesAnalyzerHH
{
    public partial class SkillsAnalyzer
    {
        public class Skill : INotifyPropertyChanged
        {
            private string val;
            private int occurances;

            public event PropertyChangedEventHandler? PropertyChanged;

            public string Value
            {
                get => val;
                set
                {
                    if (val != value)
                    {
                        val = value;
                        OnPropertyChanged();
                    }
                }
            }

            public int Occurances
            {
                get => occurances;
                set
                {
                    if (occurances != value)
                    {
                        occurances = value;
                        OnPropertyChanged();
                    }
                }
            }

            private void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}