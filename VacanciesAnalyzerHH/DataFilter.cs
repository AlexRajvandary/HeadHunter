using System.ComponentModel;
using System.Runtime.CompilerServices;
using VacanciesAnalyzerHH.Models;

namespace VacanciesAnalyzerHH
{
    public class DataFilter
    {
        private string adress;
        private string companyName;
        private string contacts;
        private string name;
        private string published;
        private double? salaryFrom;
        private double? salaryTo;
        private string schedule;
        private bool isFilterActive;

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Adress
        {
            get => adress;
            set
            {
                if (adress != value)
                {
                    adress = value;
                    OnProperyChanged();
                }
            }
        }

        public string CompanyName
        {
            get => companyName;
            set
            {
                if (companyName != value)
                {
                    companyName = value;
                    OnProperyChanged();
                }
            }
        }

        public string Contacts
        {
            get => contacts;
            set
            {
                if (contacts != value)
                {
                    contacts = value;
                    OnProperyChanged();
                }
            }
        }

        public bool IsFilterActive
        {
            get => isFilterActive;
            set
            {
                if (isFilterActive != value)
                {
                    isFilterActive = value;
                    OnProperyChanged();

                    if (!value)
                    {
                        Clean();
                    }
                }
            }
        }

        public string Name
        {
            get => name;
            set
            {
                if (name != value)
                {
                    name = value;
                    OnProperyChanged();
                }
            }
        }

        public string Published
        {
            get => published;
            set
            {
                if (published != value)
                {
                    published = value;
                    OnProperyChanged();
                }
            }
        }

        public double? SalaryFrom
        {
            get => salaryFrom;
            set
            {
                if (salaryFrom != value)
                {
                    salaryFrom = value;
                    OnProperyChanged();
                }
            }
        }

        public double? SalaryTo
        {
            get => salaryTo;
            set
            {
                if (salaryTo != value)
                {
                    salaryTo = value;
                    OnProperyChanged();
                }
            }
        }

        public string Schedule
        {
            get => schedule;
            set
            {
                if (schedule != value)
                {
                    schedule = value;
                    OnProperyChanged();
                }
            }
        }

        public bool Filter(Vacancy? vacancy)
        {
            if (vacancy == null) return false;

            return CheckField(vacancy.Address?.ToString(), Adress)
                && CheckField(vacancy.Employer?.Name, CompanyName)
                && CheckField(vacancy.Name, Name)
                && CheckField(vacancy.Schedule?.Name, Schedule)
                && (vacancy.Salary != null ?
                        SalaryFrom == null || vacancy.Salary.VisibleFrom >= SalaryFrom
                        : SalaryFrom == null) && (vacancy.Salary != null 
                            ? SalaryTo == null || vacancy.Salary.VisibleTo <= SalaryTo
                            : SalaryFrom == null);
        }

        public void OnProperyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private static bool CheckField(string? field, string? filter)
        {
            return string.IsNullOrEmpty(field) || (string.IsNullOrEmpty(filter) || field.Contains(filter));
        }

        private void Clean()
        {
            Adress = null;
            CompanyName = null;
            Contacts = null;
            CompanyName = null;
            Schedule = null;
            SalaryFrom = null;
            SalaryTo = null;
            Published = null;
        }
    }
}
